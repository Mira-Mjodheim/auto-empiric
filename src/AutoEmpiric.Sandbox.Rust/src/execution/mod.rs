pub mod runner;

use std::io::{Error, ErrorKind, Result};
use std::process::{Command, Output, Stdio};

pub struct ExecutionContext {
    binary_path: String,
    arguments: Vec<String>,
}

impl ExecutionContext {
    pub fn new(binary_path: String, arguments: Vec<String>) -> Self {
        Self {
            binary_path,
            arguments,
        }
    }

    pub fn run(&self) -> Result<Output> {
        let mut command = Command::new(&self.binary_path);
        command.args(&self.arguments);
        command.stdout(Stdio::piped());
        command.stderr(Stdio::piped());

        let child = command.spawn()?;
        let output = child.wait_with_output()?;

        if output.status.success() {
            Ok(output)
        } else {
            Err(Error::new(
                ErrorKind::Other,
                format!("Execution failed with exit code: {:?}", output.status.code()),
            ))
        }
    }
}
