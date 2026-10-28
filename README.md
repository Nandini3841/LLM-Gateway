# LLM-Gateway
Self-hosted LLM gateway in .NET 10: one OpenAI-compatible API in front of local models (Ollama). Auth with hashed API keys, Redis rate limits and token budgets, semantic cache (pgvector), routing with circuit-breaker fallback, SSE streaming, RabbitMQ/Hangfire workers, Prometheus + Grafana metrics.
