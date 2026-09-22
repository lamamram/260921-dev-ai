ARG OLLAMA_VERSION=0.34.2
FROM ollama/ollama:${OLLAMA_VERSION}

LABEL author="mlamamra@dawan.fr"
LABEL created="2025-06-11"
LABEL stack="ollama"
LABEL stack.version="0.34.2"

RUN useradd -m -U ollama
RUN apt-get update && apt-get install -y --no-install-recommends \
    git \
    curl \
    wget \
    ca-certificates \
    && rm -rf /var/lib/apt/lists/*

USER ollama
WORKDIR /home/ollama

RUN mkdir -p /home/ollama/.ollama
