# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Overview

A Social Media Posts Management System implementing CQRS + Event Sourcing with Kafka. Two independent ASP.NET Core 8 microservices communicate exclusively through Kafka events — the Command side writes events to MongoDB; the Query side reads a denormalized SQL Server database updated by a Kafka consumer.

## Build & Run

```bash
# Build entire solution
dotnet build sm-post/SM-Post.sln

# Run Command API (write side)
dotnet run --project sm-post/Post.Cmd/Post.Cmd.Api

# Run Query API (read side)
dotnet run --project sm-post/Post.Query/Post.Query.Api
```

## Infrastructure

```bash
# Create network (one-time)
docker network create dotnet-kafka-network

# Start all infrastructure
docker-compose up -d
```

Services: Kafka (9092), Zookeeper (2181), MongoDB (27017), SQL Server (1433), Schema Registry (8085), Kafka Connect (8083), AKHQ UI (8080).

## Architecture

### CQRS + Event Sourcing Flow

**Command side (write):**
HTTP → Controller → MediatR Command → CommandHandler → `PostAggregate` → raises domain event → `EventStore` persists to MongoDB → `EventProducer` publishes to Kafka

**Query side (read):**
`ConsumerHostedService` (background) → `EventConsumer` polls Kafka → routes to `IEventHandler<T>` → updates SQL Server via EF Core

The two sides are decoupled — eventual consistency is by design. The `POST /api/v1/RestoreReadDb` endpoint on the Command API replays all stored events to rebuild the read model.

### Key Projects

| Project | Role |
|---|---|
| `cqrs-es/CQRS.Core` | Shared framework: `AggregateRoot`, `BaseEvent`, `BaseCommand`, `BaseQuery`, interfaces |
| `sm-post/Post.Common` | Shared domain events (`PostCreatedEvent`, `CommentAddedEvent`, etc.) and DTOs |
| `Post.Cmd.Domain` | `PostAggregate` — the only aggregate root |
| `Post.Cmd.Infrastructure` | `EventStore` (MongoDB), `EventProducer` (Kafka), `EventSourcingHandler` |
| `Post.Query.Infrastructure` | `EventConsumer` (Kafka), EF Core SQL Server, event handlers updating read model |

### Domain Events (Post.Common)

`PostCreatedEvent`, `MessageUpdatedEvent`, `PostLikedEvent`, `PostRemovedEvent`, `CommentAddedEvent`, `CommentUpdatedEvent`, `CommentRemovedEvent`

All extend `BaseEvent` from `CQRS.Core` which carries `Id`, `Type`, and `Version`.

### Adding a New Command

1. Add event to `Post.Common`
2. Add command record to `Post.Cmd.Application/Commands/`
3. Add method on `PostAggregate` that raises the event
4. Add `Apply(YourEvent)` on `PostAggregate` to update aggregate state
5. Add command handler in `Post.Cmd.Application/Handlers/`
6. Add endpoint to relevant controller
7. Add event handler in `Post.Query.Infrastructure/Handlers/` to update the read model
8. Register the event handler in `EventConsumer`

## Configuration

**Command side** (`Post.Cmd.Api/appsettings.json`): MongoDB connection + Kafka `ProducerConfig`
**Query side** (`Post.Query.Api/appsettings.json`): SQL Server connection string + Kafka `ConsumerConfig`

Runtime env var required on Command side: `KAFKA_TOPIC` (Kafka topic name for events).

Consumer config: `GroupId: "SM_Consumer"`, `EnableAutoCommit: false`, `AutoOffsetReset: Earliest`.
