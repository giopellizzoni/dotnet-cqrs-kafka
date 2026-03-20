# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

The solution file is at `sm-post/SM-Post.sln`.

```bash
# Build
dotnet build sm-post/SM-Post.sln

# Run Command API
dotnet run --project sm-post/Post.Cmd/Post.Cmd.Api

# Run Query API
dotnet run --project sm-post/Post.Query/Post.Query.Api

# Infrastructure (Kafka, MongoDB, SQL Server, AKHQ)
# The docker-compose network must exist first:
docker network create dotnet-kafka-network
docker-compose up -d
```

There are no automated tests in this repository.

## Architecture

This is a **CQRS + Event Sourcing** implementation for a social media post service, split into two independent ASP.NET Core APIs that communicate via Kafka.

### Data flow

1. Client sends a command to **Post.Cmd.Api**
2. MediatR dispatches it to a command handler in `Post.Cmd.Application`
3. The handler uses `IEventSourcingHandler<PostAggregate>` to load/save the aggregate
4. `EventStore` persists events to **MongoDB** and publishes them to **Kafka**
5. **Post.Query.Api** runs a background `ConsumerHostedService` that consumes Kafka events
6. `EventConsumer` routes each event by type to the appropriate `IEventHandler<T>`
7. Event handlers update the **SQL Server** read model via Entity Framework Core

### Projects

- **`cqrs-es/CQRS.Core`** — Framework-level abstractions: `AggregateRoot`, `BaseEvent`, `IEventStore`, `IEventProducer`, `IEventConsumer`, guard clause extensions. Referenced by all other projects.
- **`sm-post/Post.Common`** — Shared domain events (`PostCreatedEvent`, `CommentAddedEvent`, etc.) and `BaseResponse` DTO. Both the Cmd and Query sides depend on this.
- **`sm-post/Post.Cmd/`** — Write side:
  - `Post.Cmd.Domain` — `PostAggregate` (the only aggregate; enforces business rules and raises events)
  - `Post.Cmd.Application` — MediatR command handlers, registers all DI via `ApplicationModule`
  - `Post.Cmd.Infrastructure` — MongoDB event store repository, Kafka `EventProducer`, `EventSourcingHandler`
  - `Post.Cmd.Api` — Controllers for posts, comments, and a `RestoreReadDb` endpoint
- **`sm-post/Post.Query/`** — Read side:
  - `Post.Query.Domain` — `PostEntity`/`CommentEntity` (EF Core entities), repository interfaces
  - `Post.Query.Application` — MediatR query handlers
  - `Post.Query.Infrastructure` — EF Core `DatabaseContext` (SQL Server), Kafka `EventConsumer`, per-event `IEventHandler<T>` implementations
  - `Post.Query.Api` — `PostLookupController` for all read queries

### Key design decisions

- **`AggregateRoot.Apply(object event)`** uses a `switch` expression on event type — when adding a new event, you must add a case here in `PostAggregate` as well as register the BSON class map in `InfrastructureModule` (Cmd side).
- **Optimistic concurrency** is enforced in `EventStore.SaveEventsAsync` by comparing `expectedVersion` against the last persisted event version.
- **Read model is rebuilt** via `POST /api/v1/RestoreReadDb` — this replays all events from MongoDB through Kafka to repopulate SQL Server.
- **Kafka topic** is read from the `KAFKA_TOPIC` environment variable at runtime (not from `appsettings.json`).
- The docker-compose network (`dotnet-kafka-network`) is declared `external: true` and must be created manually before running `docker-compose up`.

### Configuration

**Post.Cmd.Api** (`appsettings.json`):

- `MongoDbConfig` — connection string, database (`socialmedia`), collection (`eventStore`)
- `ProducerConfig.BootstrapServers` — Kafka broker address

**Post.Query.Api** (`appsettings.json`):

- `ConnectionStrings.SqlServer` — EF Core SQL Server connection
- `ConsumerConfig` — Kafka consumer settings (`GroupId: SM_Consumer`, manual commit)

### Infrastructure services (docker-compose)

| Service    | Port  | Purpose             |
| ---------- | ----- | ------------------- |
| Kafka      | 9092  | Message broker      |
| AKHQ       | 8080  | Kafka management UI |
| MongoDB    | 27017 | Event store (cmd)   |
| SQL Server | 1433  | Read model (query)  |

## Development Guidelines

- Follow existing CQRS + Event Sourcing patterns strictly — do not introduce CRUD shortcuts.
- Never write directly to the read database from the command side.
- All state changes must go through events.
- Do not bypass aggregates or EventSourcingHandler.
- Prefer small, focused changes — avoid large refactors unless explicitly requested.

## Adding New Features

When implementing a new feature:

1. Create/extend the Aggregate in `Post.Cmd.Domain`
2. Add new events in `Post.Common.Events`
3. Update `AggregateRoot.Apply()` and `PostAggregate`
4. Register BSON class map in `Post.Cmd.Infrastructure`
5. Implement command + handler (MediatR)
6. Ensure events are persisted and published
7. Implement corresponding event handlers in `Post.Query.Infrastructure`
8. Update read model (EF Core)
9. Add query handler if needed

Never skip event creation — events are the source of truth.

## Event Sourcing Rules

- Events are immutable — never modify existing event contracts
- Always version events if structure changes
- Every event must be handled on the Query side
- Missing event handlers will cause read model inconsistency

## Code Conventions

- Use MediatR for all commands and queries
- Use constructor injection (no service locator)
- Keep controllers thin — business logic belongs in handlers or aggregates
- Prefer explicit types over `var` when clarity is important
- Use async/await consistently

## Kafka Usage

- All events must be published to the configured topic
- Do not introduce additional topics unless explicitly required
- Consumers must be idempotent
- Handle deserialization failures gracefully

## Data Responsibilities

- MongoDB = source of truth (event store)
- SQL Server = read model only
- Never treat SQL Server as authoritative data

## Troubleshooting

- If data is inconsistent → rebuild read model via `/api/v1/RestoreReadDb`
- If events are not processed → check Kafka consumer logs
- If aggregate state is wrong → inspect event stream in MongoDB

## What NOT to do

- Do not introduce direct database writes in command handlers
- Do not skip event publishing
- Do not couple Query side with Command side
- Do not reuse EF entities in the command domain

## Claude Behavior

- Prefer explaining trade-offs when suggesting architectural changes
- When unsure, ask before making breaking changes
- Highlight risks when modifying event contracts
