use autoempiric_sandbox::execution::runner;
use autoempiric_sandbox::isolation::cgroups;
use std::process::Command;
use std::time::Duration;

#[test]
fn test_sandbox_cgroup_creation() {
    let result = cgroups::setup_cgroup("test_env_cgroup", 256, 1);
    if let Ok(cgroup_path) = result {
        assert!(std::path::Path::new(&cgroup_path).exists());
        let _ = cgroups::cleanup_cgroup("test_env_cgroup");
    }
}

#[test]
fn test_sandbox_execution_success() {
    let mut command = Command::new("echo");
    command.arg("test_output");
    
    let result = runner::execute_in_sandbox(&mut command, Duration::from_secs(2));
    assert!(result.is_ok());
    
    let output = result.unwrap();
    assert!(output.status.success());
    assert_eq!(String::from_utf8_lossy(&output.stdout).trim(), "test_output");
}

#[test]
fn test_sandbox_execution_timeout() {
    let mut command = Command::new("sleep");
    command.arg("2");
    
    let result = runner::execute_in_sandbox(&mut command, Duration::from_secs(1));
    assert!(result.is_err());
}
