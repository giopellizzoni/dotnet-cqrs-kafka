# dotnet-cqrs-kafka

A CQRS + Event Sourcing implementation for a social media post service built with .NET 8, Kafka, MongoDB, and SQL Server.

## Architecture

Two independent ASP.NET Core APIs that communicate exclusively through Kafka:

- **Post.Cmd.Api** — Write side. Accepts commands, enforces business rules through `PostAggregate`, persists events to MongoDB, publishes them to Kafka.
- **Post.Query.Api** — Read side. Consumes Kafka events via a background `ConsumerHostedService`, projects them into a SQL Server read model, serves queries.

```
Client
  │
  ├─► Post.Cmd.Api ─► PostAggregate ─► MongoDB (event store) ─► Kafka
  │
  └─► Post.Query.Api ◄── Kafka ◄── ConsumerHostedService ─► SQL Server (read model)
```

### Project structure

| Project | Layer | Responsibility |
|---------|-------|----------------|
| `cqrs-es/CQRS.Core` | Framework | `AggregateRoot`, `BaseEvent`, `IEventStore`, `IEventProducer`, `IEventConsumer`, guard clause extensions |
| `sm-post/Post.Common` | Shared | Domain events (`PostCreatedEvent`, `CommentAddedEvent`, etc.) and `BaseResponse` DTO |
| `Post.Cmd.Domain` | Write / Domain | `PostAggregate` — enforces all business rules, raises events |
| `Post.Cmd.Application` | Write / App | MediatR command handlers |
| `Post.Cmd.Infrastructure` | Write / Infra | MongoDB event store, Kafka `EventProducer`, `EventSourcingHandler` |
| `Post.Cmd.Api` | Write / API | Controllers, exception middleware |
| `Post.Query.Domain` | Read / Domain | `PostEntity`, `CommentEntity` (EF Core), repository interfaces |
| `Post.Query.Application` | Read / App | MediatR query handlers |
| `Post.Query.Infrastructure` | Read / Infra | EF Core `DatabaseContext` (SQL Server), Kafka `EventConsumer`, per-event `IEventHandler<T>` |
| `Post.Query.Api` | Read / API | `PostLookupController`, exception middleware |

### Domain events

All state changes flow through these immutable events published to Kafka:

| Event | Trigger |
|-------|---------|
| `PostCreatedEvent` | New post created |
| `MessageUpdatedEvent` | Post message edited |
| `PostLikedEvent` | Post liked |
| `PostRemovedEvent` | Post deleted |
| `CommentAddedEvent` | Comment added to a post |
| `CommentUpdatedEvent` | Comment edited |
| `CommentRemovedEvent` | Comment removed |

### Business rules

Enforced in `PostAggregate` before any event is raised:

- A post must be active to be liked, commented on, or have its comments edited/removed.
- Only the original author can delete a post.
- Only the comment author can edit or remove their own comment.
- Optimistic concurrency is checked in `EventStore.SaveEventsAsync` — concurrent writes to the same aggregate throw `ConcurrencyException`.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) and Docker Compose
- MongoDB running on `localhost:27017`
- SQL Server running on `localhost:1433`

---

## 1. Kafka infrastructure

Create the external Docker network (one-time):

```bash
docker network create dotnet-kafka-network
```

Start all services:

```bash
docker-compose up -d
```

| Service    | Host Port | Purpose             |
|------------|-----------|---------------------|
| Kafka      | 9092      | Message broker      |
| AKHQ       | 8080      | Kafka management UI |
| MongoDB    | 27017     | Event store (cmd)   |
| SQL Server | 1433      | Read model (query)  |

### Kafka hostname resolution

Kafka advertises itself as `kafka:29092` for inter-broker communication (the Docker-internal hostname). The .NET apps run on the host and bootstrap via `localhost:9092`, but Kafka can return `kafka:29092` in its broker metadata — which the host cannot resolve by default.

Add this entry to `/etc/hosts`:

```
127.0.0.1  kafka
```

---

## 2. Configuration

### Post.Cmd.Api — `appsettings.json`

```json
"MongoDbConfig": {
  "ConnectionString": "mongodb://localhost:27017",
  "DatabaseName": "socialmedia",
  "Collection": "eventStore"
},
"ProducerConfig": {
  "BootstrapServers": "localhost:9092"
}
```

### Post.Query.Api — `appsettings.json`

```json
"ConnectionStrings": {
  "SqlServer": "Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=CHANGE_ME;TrustServerCertificate=true"
},
"ConsumerConfig": {
  "GroupId": "SM_Consumer",
  "BootstrapServers": "localhost:9092",
  "EnableAutoCommit": false,
  "AutoOffsetReset": "Earliest",
  "AllowAutoCreateTopics": true
}
```

Override the SQL Server password before running (do not commit credentials):

```bash
# Recommended: environment variable
export ConnectionStrings__SqlServer="Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=<your-password>;TrustServerCertificate=true"

# Alternative: dotnet user-secrets
cd sm-post/Post.Query/Post.Query.Api
dotnet user-secrets set "ConnectionStrings:SqlServer" "Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=<your-password>;TrustServerCertificate=true"
```

### Kafka topic

Both APIs read the topic name from the `KAFKA_TOPIC` environment variable at startup. The app throws and exits immediately if it is not set.

```bash
export KAFKA_TOPIC=SocialMediaPostEvents
```

---

## 3. Build

```bash
dotnet build sm-post/SM-Post.sln
```

---

## 4. Run

Open two terminals.

**Terminal 1 — Command API (port 5033):**

```bash
export KAFKA_TOPIC=SocialMediaPostEvents
dotnet run --project sm-post/Post.Cmd/Post.Cmd.Api
```

**Terminal 2 — Query API (port 5068):**

```bash
export KAFKA_TOPIC=SocialMediaPostEvents
export ConnectionStrings__SqlServer="Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=<your-password>;TrustServerCertificate=true"
dotnet run --project sm-post/Post.Query/Post.Query.Api
```

The SQL Server database and tables (`Posts`, `Comments`) are created automatically on first startup via `EnsureCreated()`.

---

## 5. Swagger UI

Available in the `Development` environment only:

- Command API: `http://localhost:5033/swagger`
- Query API: `http://localhost:5068/swagger`

---

## 6. API reference

### Command API — `Post.Cmd.Api` (port 5033)

| Method   | Route                               | Request body fields              |
|----------|-------------------------------------|----------------------------------|
| `POST`   | `/api/v1/Posts`                     | `Author`, `Message`              |
| `PUT`    | `/api/v1/Posts/editPost/{id}`       | `Message`                        |
| `PUT`    | `/api/v1/Posts/likePost/{id}`       | —                                |
| `DELETE` | `/api/v1/Posts/{id}`                | `Username`                       |
| `PUT`    | `/api/v1/Comments/addComment/{id}`  | `Comment`, `Username`            |
| `PUT`    | `/api/v1/Comments/editComment/{id}` | `CommentId`, `Comment`, `Username` |
| `DELETE` | `/api/v1/Comments/{id}`             | `CommentId`, `Username`          |
| `POST`   | `/api/v1/RestoreReadDb`             | —                                |

> `{id}` in comment routes is the **post ID**. `CommentId` is passed in the request body.

### Query API — `Post.Query.Api` (port 5068)

| Method | Route                                       | Description                      |
|--------|---------------------------------------------|----------------------------------|
| `GET`  | `/api/v1/PostLookup`                        | All posts                        |
| `GET`  | `/api/v1/PostLookup/byId/{id}`              | Post by ID                       |
| `GET`  | `/api/v1/PostLookup/byAuthor/{author}`      | Posts by author                  |
| `GET`  | `/api/v1/PostLookup/withComments`           | Posts that have comments         |
| `GET`  | `/api/v1/PostLookup/withLikes/{numberOfLikes}` | Posts with at least N likes   |

---

## 7. Rebuilding the read model

If SQL Server becomes inconsistent with the MongoDB event store, replay all events:

```bash
curl -X POST http://localhost:5033/api/v1/RestoreReadDb
```

This replays every event stream from MongoDB through Kafka. The Query API consumes them and repopulates SQL Server from scratch.

---

## 8. Troubleshooting

| Symptom | Action |
|---------|--------|
| Query API returns stale or missing data | `POST /api/v1/RestoreReadDb` |
| Events not appearing in SQL Server | Check Query API logs for Kafka consumer errors |
| Aggregate state is wrong | Inspect the event stream in MongoDB (`socialmedia.eventStore`) |
| `KAFKA_TOPIC environment variable is not set` | Export `KAFKA_TOPIC` before starting either API |
| Connection refused on port 9092 | Run `docker-compose up -d`; verify with `docker ps` |
| Broker not available after connecting on 9092 | Add `127.0.0.1 kafka` to `/etc/hosts` — Kafka advertises its internal hostname |
| 403 / business rule error | Check aggregate rules: inactive post, wrong author, wrong comment owner |
