use std::io::{self, Read};
use std::process::{Command, Output, Stdio};
use std::sync::mpsc;
use std::thread;
use std::time::Duration;

#[derive(Debug, Clone)]
pub struct ExecutionResult {
    pub exit_code: Option<i32>,
    pub stdout: String,
    pub stderr: String,
}

pub struct ProcessRunner {
    executable: String,
    arguments: Vec<String>,
    working_directory: Option<String>,
    timeout: Duration,
}

impl ProcessRunner {
    pub fn new(executable: String, arguments: Vec<String>) -> Self {
        Self {
            executable,
            arguments,
            working_directory: None,
            timeout: Duration::from_secs(30),
        }
    }

    pub fn set_working_directory(&mut self, dir: String) {
        self.working_directory = Some(dir);
    }

    pub fn set_timeout(&mut self, timeout: Duration) {
        self.timeout = timeout;
    }

    pub fn run(&self) -> io::Result<ExecutionResult> {
        let mut command = Command::new(&self.executable);
        command.args(&self.arguments);

        if let Some(ref dir) = self.working_directory {
            command.current_dir(dir);
        }

        command.stdout(Stdio::piped());
        command.stderr(Stdio::piped());

        let mut child = command.spawn()?;
        let mut stdout_stream = child.stdout.take().expect("Failed to capture standard output");
        let mut stderr_stream = child.stderr.take().expect("Failed to capture standard error");
        let (sender, receiver) = mpsc::channel();

        thread::spawn(move || {
            let mut stdout_buffer = String::new();
            let mut stderr_buffer = String::new();

            let out_thread = thread::spawn(move || {
                let _ = stdout_stream.read_to_string(&mut stdout_buffer);
                stdout_buffer
            });

            let err_thread = thread::spawn(move || {
                let _ = stderr_stream.read_to_string(&mut stderr_buffer);
                stderr_buffer
            });

            let exit_code = match child.wait() {
                Ok(status) => status.code(),
                Err(_) => None,
            };

            let stdout_res = out_thread.join().unwrap_or_default();
            let stderr_res = err_thread.join().unwrap_or_default();

            let _ = sender.send(ExecutionResult {
                exit_code,
                stdout: stdout_res,
                stderr: stderr_res,
            });
        });

        match receiver.recv_timeout(self.timeout) {
            Ok(res) => Ok(res),
            Err(mpsc::RecvTimeoutError::Timeout) => Ok(ExecutionResult {
                exit_code: None,
                stdout: String::new(),
                stderr: String::from("Process execution timed out."),
            }),
            Err(mpsc::RecvTimeoutError::Disconnected) => Err(io::Error::new(
                io::ErrorKind::Other,
                "Internal runner error: channel disconnected",
            )),
        }
    }
}

pub fn execute_in_sandbox(command: &mut Command, timeout: Duration) -> io::Result<Output> {
    command.stdout(Stdio::piped());
    command.stderr(Stdio::piped());

    let child = command.spawn()?;
    let child_id = child.id();
    let (sender, receiver) = mpsc::channel();

    thread::spawn(move || {
        let result = child.wait_with_output();
        let _ = sender.send(result);
    });

    match receiver.recv_timeout(timeout) {
        Ok(result) => result,
        Err(mpsc::RecvTimeoutError::Timeout) => {
            let _ = Command::new("kill").arg("-9").arg(child_id.to_string()).status();
            Err(io::Error::new(io::ErrorKind::TimedOut, "Process execution timed out."))
        }
        Err(mpsc::RecvTimeoutError::Disconnected) => Err(io::Error::new(
            io::ErrorKind::Other,
            "Internal runner error: channel disconnected",
        )),
    }
}
