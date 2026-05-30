# DevAssistAI

## Overview

DevAssistAI is an AI-powered knowledge assistant built using ASP.NET Core, SQL Server, and Ollama. The application uses Retrieval-Augmented Generation (RAG) to provide accurate and context-aware responses from a custom knowledge base.

The system processes documents, generates embeddings, performs semantic search using cosine similarity, retrieves relevant information, and uses a Large Language Model (LLM) to generate responses.

---

## Features

* AI-powered chat using Ollama
* Chat session management
* Persistent chat history
* Document ingestion pipeline
* Document chunking
* Embedding generation
* Semantic search using cosine similarity
* Retrieval-Augmented Generation (RAG)
* Context-aware AI responses
* Repository Pattern implementation
* Global exception handling middleware
* Structured logging with ILogger
* SQL Server integration

---

## Tech Stack

### Backend

* ASP.NET Core (.NET 10)
* C#
* Entity Framework Core
* SQL Server

### AI Technologies

* Ollama
* Embeddings
* Semantic Search
* Retrieval-Augmented Generation (RAG)

### Design & Architecture

* Repository Pattern
* Dependency Injection
* Middleware
* Async/Await Programming
* Cancellation Tokens
* Service Layer Architecture

---

## How It Works

1. User submits a question.
2. The system generates an embedding for the query.
3. Relevant document chunks are retrieved using semantic similarity.
4. Retrieved context is injected into the prompt.
5. Ollama generates a response using the provided context.
6. Chat history is stored and used for future conversations.

---

## What I Learned

### ASP.NET Core

* Building REST APIs
* Dependency Injection
* Middleware
* Exception Handling
* Logging
* Service and Repository Layers

### Database

* Entity Framework Core
* SQL Server Integration
* Database Migrations
* Entity Relationships

### Generative AI

* Large Language Models (LLMs)
* Prompt Engineering
* Embeddings
* Vector Representations
* Cosine Similarity
* Semantic Search
* Document Chunking
* Retrieval-Augmented Generation (RAG)
* History-Aware Retrieval

### Software Engineering

* Clean Code Practices
* Separation of Concerns
* Scalable Service Design
* Production-Oriented Backend Development

---

## Future Improvements

* Vector Database Integration (Qdrant / Pinecone)
* Hybrid Search (Keyword + Vector Search)
* Re-ranking Pipeline
* AI Agents
* Tool Calling
* Model Context Protocol (MCP)
* Redis Caching
* Docker Deployment

---

## Project Status

Current Version: Production-Ready RAG Assistant

The project successfully demonstrates the implementation of modern Generative AI concepts including embeddings, semantic search, and Retrieval-Augmented Generation using a custom knowledge base.
