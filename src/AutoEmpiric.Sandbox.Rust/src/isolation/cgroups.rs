use std::fs::{self, OpenOptions};
use std::io::{self, Write};
use std::path::{Path, PathBuf};

pub struct CgroupManager {
    base_path: PathBuf,
}

impl CgroupManager {
    pub fn new(cgroup_name: &str) -> io::Result<Self> {
        let path = Path::new("/sys/fs/cgroup").join(cgroup_name);
        
        if !path.exists() {
            fs::create_dir_all(&path)?;
        }
        
        Ok(Self { base_path: path })
    }

    pub fn set_memory_limit(&self, max_bytes: u64) -> io::Result<()> {
        let memory_max_path = self.base_path.join("memory.max");
        let mut file = OpenOptions::new().write(true).open(memory_max_path)?;
        write!(file, "{}", max_bytes)
    }

    pub fn set_cpu_limit(&self, quota: u64, period: u64) -> io::Result<()> {
        let cpu_max_path = self.base_path.join("cpu.max");
        let mut file = OpenOptions::new().write(true).open(cpu_max_path)?;
        write!(file, "{} {}", quota, period)
    }

    pub fn apply_to_pid(&self, pid: u32) -> io::Result<()> {
        let procs_path = self.base_path.join("cgroup.procs");
        let mut file = OpenOptions::new().write(true).open(procs_path)?;
        write!(file, "{}", pid)
    }

    pub fn remove(&self) -> io::Result<()> {
        if self.base_path.exists() {
            fs::remove_dir(&self.base_path)?;
        }
        Ok(())
    }
}

impl Drop for CgroupManager {
    fn drop(&mut self) {
        let _ = self.remove();
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.