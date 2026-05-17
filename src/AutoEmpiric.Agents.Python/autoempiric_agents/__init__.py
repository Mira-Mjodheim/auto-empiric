"""
AutoEmpiric Agents - Python package initialization.
"""

import os
import sys

__version__ = "1.0.0"

def initialize_agent_environment() -> None:
    """Initializes the environment for Python agents."""
    if "AUTOEMPIRIC_HOME" not in os.environ:
        os.environ["AUTOEMPIRIC_HOME"] = os.path.dirname(os.path.abspath(__file__))

initialize_agent_environment()

__all__ = ["__version__", "initialize_agent_environment"]
