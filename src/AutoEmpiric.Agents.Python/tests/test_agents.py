from autoempiric_agents.base_agent import BaseAgent, ValidationResult
from autoempiric_agents.confidence_calibrator import ConfidenceCalibrator


class ReasoningAgentFixture(BaseAgent):
    def generate_hypothesis(self, problem_description: str) -> str:
        return f"hypothesis:{problem_description}"

    def search_for_proofs(self, hypothesis: str) -> dict:
        return {"hypothesis": hypothesis, "verified": True}

    def evaluate_proofs(self, proofs: dict) -> ValidationResult:
        return ValidationResult(is_valid=proofs["verified"], score=0.95, message="Verified")

    def adapt_hypothesis(self, hypothesis: str, validation_result: ValidationResult) -> str:
        return f"{hypothesis}:adapted"


def test_confidence_calibrator_initialization():
    calibrator = ConfidenceCalibrator()

    assert calibrator.get_current_confidence() == 1.0


def test_confidence_calibration_decreases_on_syntax_error():
    calibrator = ConfidenceCalibrator()

    score = calibrator.evaluate({"success": False, "error_message": "SyntaxError"})

    assert score == 0.7


def test_confidence_calibration_increases_on_success_without_exceeding_one():
    calibrator = ConfidenceCalibrator(initial_confidence=0.95)

    score = calibrator.evaluate({"success": True})

    assert score == 1.0


def test_base_agent_reasoning_loop_success():
    agent = ReasoningAgentFixture(agent_id="validator-agent")

    result = agent.run_reasoning_loop("Validate the following data")

    assert result["status"] == "success"
    assert result["steps"] == 1
    assert result["history"][0]["score"] == 0.95
