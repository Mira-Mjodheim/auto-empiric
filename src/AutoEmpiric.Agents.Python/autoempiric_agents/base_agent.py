import abc
import os
import logging
from typing import Any, Dict, List

logging.basicConfig(level=logging.INFO)
logger = logging.getLogger(__name__)

class ValidationResult:
    def __init__(self, is_valid: bool, score: float, message: str):
        self.is_valid = is_valid
        self.score = score
        self.message = message

class BaseAgent(abc.ABC):
    def __init__(self, agent_id: str):
        self.agent_id = agent_id
        self.max_reasoning_steps = int(os.environ.get("MAX_REASONING_STEPS", "5"))
        self.history: List[Dict[str, Any]] = []

    @abc.abstractmethod
    def generate_hypothesis(self, problem_description: str) -> str:
        pass

    @abc.abstractmethod
    def search_for_proofs(self, hypothesis: str) -> Any:
        pass

    @abc.abstractmethod
    def evaluate_proofs(self, proofs: Any) -> ValidationResult:
        pass

    @abc.abstractmethod
    def adapt_hypothesis(self, hypothesis: str, validation_result: ValidationResult) -> str:
        pass

    def run_reasoning_loop(self, problem_description: str) -> Dict[str, Any]:
        logger.info("Agent %s starting reasoning loop for problem.", self.agent_id)
        
        current_hypothesis = self.generate_hypothesis(problem_description)
        step = 0
        
        while step < self.max_reasoning_steps:
            step += 1
            logger.info("Step %d: Evaluating hypothesis.", step)
            
            try:
                proofs = self.search_for_proofs(current_hypothesis)
                validation_result = self.evaluate_proofs(proofs)
                
                self.history.append({
                    "step": step,
                    "hypothesis": current_hypothesis,
                    "is_valid": validation_result.is_valid,
                    "score": validation_result.score,
                    "message": validation_result.message
                })
                
                if validation_result.is_valid and validation_result.score >= 0.9:
                    logger.info("Valid proof found at step %d with score %f.", step, validation_result.score)
                    return {
                        "status": "success",
                        "steps": step,
                        "history": self.history
                    }
                
                current_hypothesis = self.adapt_hypothesis(current_hypothesis, validation_result)
            except Exception as e:
                logger.error("Error during reasoning step: %s", e)
                break
        
        return {
            "status": "failure",
            "steps": step,
            "history": self.history
        }
