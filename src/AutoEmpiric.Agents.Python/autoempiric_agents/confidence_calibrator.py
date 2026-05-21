import logging
from typing import Dict, Any, List

logger = logging.getLogger(__name__)

class ConfidenceCalibrator:
    def __init__(self, initial_confidence: float = 1.0):
        self.current_confidence = initial_confidence
        self.history: List[Dict[str, Any]] = []

    def evaluate(self, sandbox_output: Dict[str, Any]) -> float:
        success = sandbox_output.get("success", False)
        error_message = sandbox_output.get("error_message", "")
        
        penalty = 0.0
        bonus = 0.0

        if success:
            bonus += 0.1
        else:
            penalty += 0.2
            if error_message:
                if "SyntaxError" in error_message or "IndentationError" in error_message:
                    penalty += 0.1
                elif "TypeError" in error_message or "ValueError" in error_message:
                    penalty += 0.05
                    
        self.current_confidence = self.current_confidence - penalty + bonus
        self.current_confidence = max(0.0, min(1.0, self.current_confidence))
        
        self.history.append({
            "sandbox_output": sandbox_output,
            "adjusted_confidence": self.current_confidence
        })
        
        return self.current_confidence

    def get_current_confidence(self) -> float:
        return self.current_confidence
        
    def reset(self, initial_confidence: float = 1.0) -> None:
        self.current_confidence = initial_confidence
        self.history.clear()
