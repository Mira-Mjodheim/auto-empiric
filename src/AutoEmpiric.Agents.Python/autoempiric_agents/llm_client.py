import os
import time
from enum import Enum
from typing import Dict, Any, Optional

class LLMProvider(Enum):
    OPENAI = "openai"
    ANTHROPIC = "anthropic"

class LLMClient:
    def __init__(self, provider: LLMProvider = LLMProvider.OPENAI, max_retries: int = 3, retry_delay: int = 2):
        self.provider = provider
        self.max_retries = max_retries
        self.retry_delay = retry_delay

        if self.provider == LLMProvider.OPENAI:
            import openai
            self.api_key = os.environ.get("OPENAI_API_KEY", "")
            if not self.api_key:
                raise ValueError("OPENAI_API_KEY environment variable is not set")
            self.client = openai.Client(api_key=self.api_key)
            self.model = os.environ.get("OPENAI_MODEL", "gpt-4-turbo")
        elif self.provider == LLMProvider.ANTHROPIC:
            import anthropic
            self.api_key = os.environ.get("ANTHROPIC_API_KEY", "")
            if not self.api_key:
                raise ValueError("ANTHROPIC_API_KEY environment variable is not set")
            self.client = anthropic.Anthropic(api_key=self.api_key)
            self.model = os.environ.get("ANTHROPIC_MODEL", "claude-3-opus-20240229")
        else:
            raise ValueError("Unsupported LLM provider")

    def generate_text(self, prompt: str, system_prompt: Optional[str] = None, temperature: float = 0.7) -> str:
        retries = 0
        while retries <= self.max_retries:
            try:
                if self.provider == LLMProvider.OPENAI:
                    return self._generate_openai(prompt, system_prompt, temperature)
                elif self.provider == LLMProvider.ANTHROPIC:
                    return self._generate_anthropic(prompt, system_prompt, temperature)
            except Exception as e:
                retries += 1
                if retries > self.max_retries:
                    raise RuntimeError(f"Max retries reached. Last error: {str(e)}")
                time.sleep(self.retry_delay * retries)
        return ""

    def _generate_openai(self, prompt: str, system_prompt: Optional[str], temperature: float) -> str:
        messages = []
        if system_prompt:
            messages.append({"role": "system", "content": system_prompt})
        messages.append({"role": "user", "content": prompt})

        response = self.client.chat.completions.create(
            model=self.model,
            messages=messages,
            temperature=temperature
        )
        if response.choices and len(response.choices) > 0:
            return response.choices[0].message.content or ""
        return ""

    def _generate_anthropic(self, prompt: str, system_prompt: Optional[str], temperature: float) -> str:
        kwargs: Dict[str, Any] = {
            "model": self.model,
            "max_tokens": 4096,
            "messages": [{"role": "user", "content": prompt}],
            "temperature": temperature
        }
        if system_prompt:
            kwargs["system"] = system_prompt

        response = self.client.messages.create(**kwargs)
        if response.content and len(response.content) > 0:
            return response.content[0].text
        return ""
