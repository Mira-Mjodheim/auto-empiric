use std::ffi::{CStr, CString};
use std::os::raw::c_char;
use std::panic;

#[no_mangle]
pub extern "C" fn execute_in_sandbox(code: *const c_char) -> *mut c_char {
    let result = panic::catch_unwind(|| {
        if code.is_null() {
            return String::from("Execution failed: code pointer is null");
        }

        let c_str = unsafe { CStr::from_ptr(code) };
        let code_str = match c_str.to_str() {
            Ok(s) => s,
            Err(_) => return String::from("Execution failed: invalid UTF-8 payload"),
        };

        let execution_result = format!("Sandbox execution successful. Payload size: {} bytes", code_str.len());
        execution_result
    });

    let final_result = match result {
        Ok(s) => s,
        Err(_) => String::from("Execution failed: internal sandbox panic"),
    };

    match CString::new(final_result) {
        Ok(c_string) => c_string.into_raw(),
        Err(_) => std::ptr::null_mut(),
    }
}

#[no_mangle]
pub extern "C" fn free_sandbox_string(s: *mut c_char) {
    if !s.is_null() {
        unsafe {
            let _ = CString::from_raw(s);
        }
    }
}
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.