import pytest
import os
from unittest.mock import MagicMock
from autoempiric_agents.base_agent import BaseAgent
from autoempiric_agents.confidence_calibrator import ConfidenceCalibrator
from autoempiric_agents.llm_client import LLMClient

def test_confidence_calibrator_initialization():
    calibrator = ConfidenceCalibrator()
    assert calibrator is not None

def test_confidence_calibration_score():
    calibrator = ConfidenceCalibrator()
    calibrator.calculate_confidence = MagicMock(return_value=0.88)
    score = calibrator.calculate_confidence("Task completed with expected output.")
    assert score == 0.88

def test_llm_client_initialization():
    os.environ["API_KEY_ENV"] = "test_environment_key"
    client = LLMClient()
    client.generate_response = MagicMock(return_value="Simulated response")
    response = client.generate_response("Test prompt")
    assert response == "Simulated response"

def test_base_agent_creation():
    client = LLMClient()
    agent = BaseAgent(name="ValidatorAgent", llm_client=client)
    assert agent.name == "ValidatorAgent"
    assert agent.llm_client == client

def test_base_agent_execution_flow():
    client = LLMClient()
    client.generate_response = MagicMock(return_value="Processed validation")
    
    agent = BaseAgent(name="ValidatorAgent", llm_client=client)
    agent.execute_task = MagicMock(return_value="Processed validation")
    
    result = agent.execute_task("Validate the following data")
    assert result == "Processed validation"
    agent.execute_task.assert_called_once_with("Validate the following data")
[WARNING] --raw-output is enabled. Model output is not sanitized and may contain harmful ANSI sequences (e.g. for phishing or command injection). Use --accept-raw-output-risk to suppress this warning.