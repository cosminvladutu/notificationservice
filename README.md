# Notification Service (Demo)

This repository contains a demo implementation of a notification service built on Azure Functions (.NET 10) using an outbox-style flow and Durable Functions for orchestration. It is intended to be **an inspiration and starting point** for others who want to implement a similar approach.

> **Disclaimer**: This is **not production-ready**. It is far from clean, intentionally simplified, and missing important pieces such as tests, strong separation into dedicated projects, and hardened operational concerns.

## What it does (high-level)

- Ingests notification requests from Azure Service Bus.
- Saves raw requests to Cosmos DB.
- Fans out per-channel/per-recipient work items.
- Sends notifications via SMS (Twilio) and Email (ACS).
- Tracks status (Started, InProgress, Pending, Done, Failed) and error details.
- Uses embedded templates for emails and SMS.

## Architecture overview

- **Domain layer**: Rich domain models with validation and state transitions.
- **Application layer**: Use-case services; all business logic lives here.
- **Persistence**: Cosmos DB models + mappers + repository.
- **Functions**: Thin triggers, activities, and orchestrations.
- **Templates**: Embedded resources loaded in memory at startup.

## Health checks

I also wanted to try out a **custom health check implementation** even though it is not the main subject of this service. The repository includes simple health checks for Cosmos DB and Service Bus using custom `IHealthCheck` implementations and extension methods.

## What’s missing / intentionally simplified

- **No tests** (unit, integration, or performance).
- **Everything sits in one Functions project** — it should be split into separate projects (Domain, Application, Infrastructure, Functions, etc.).
- **Limited configuration validation and security hardening**.
- **No full observability or operational tooling** beyond basic logging.

## Purpose

This is a **demo implementation** meant to show a possible structure and flow for an outbox-based notification service. It’s intended to **inspire** and help others build their own solution rather than be used as-is.
