use std::env;
use std::io::{Read, Write};
use std::net::{TcpListener, TcpStream};
use std::process::{Command, Stdio};
use std::thread;

fn handle_connection(mut stream: TcpStream) {
    let mut buffer = [0; 2048];
    if let Ok(bytes_read) = stream.read(&mut buffer) {
        if bytes_read == 0 {
            return;
        }

        let request = String::from_utf8_lossy(&buffer[..bytes_read]);
        let command = request.trim();

        let response = match command {
            "INIT" => execute_lifecycle_command("SANDBOX_INIT_CMD", "true"),
            "START" => execute_lifecycle_command("SANDBOX_START_CMD", "true"),
            "STOP" => execute_lifecycle_command("SANDBOX_STOP_CMD", "true"),
            "CLEANUP" => execute_lifecycle_command("SANDBOX_CLEANUP_CMD", "true"),
            "STATUS" => "STATUS: OK\n".to_string(),
            _ => format!("ERROR: UNKNOWN COMMAND '{}'\n", command),
        };

        let _ = stream.write_all(response.as_bytes());
    }
}

fn execute_lifecycle_command(env_var: &str, default_cmd: &str) -> String {
    let cmd_string = env::var(env_var).unwrap_or_else(|_| default_cmd.to_string());
    
    match Command::new("sh")
        .arg("-c")
        .arg(&cmd_string)
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .output() 
    {
        Ok(output) => {
            if output.status.success() {
                format!("SUCCESS: {}\n", String::from_utf8_lossy(&output.stdout).trim())
            } else {
                format!("FAILURE: {}\n", String::from_utf8_lossy(&output.stderr).trim())
            }
        }
        Err(err) => {
            format!("SYSTEM_ERROR: {}\n", err)
        }
    }
}

fn main() {
    let host = env::var("DAEMON_HOST").unwrap_or_else(|_| "127.0.0.1".to_string());
    let port = env::var("DAEMON_PORT").unwrap_or_else(|_| "9000".to_string());
    let bind_address = format!("{}:{}", host, port);

    match TcpListener::bind(&bind_address) {
        Ok(listener) => {
            for stream in listener.incoming() {
                match stream {
                    Ok(valid_stream) => {
                        thread::spawn(move || {
                            handle_connection(valid_stream);
                        });
                    }
                    Err(_) => continue,
                }
            }
        }
        Err(e) => {
            eprintln!("FATAL: Cannot bind to address {}: {}", bind_address, e);
            std::process::exit(1);
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.