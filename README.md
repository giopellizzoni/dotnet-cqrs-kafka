# dotnet-cqrs-kafka

A CQRS + Event Sourcing implementation for a social media post service, built with ASP.NET Core, Kafka, MongoDB, and SQL Server.

## Architecture

Two independent ASP.NET Core APIs communicate exclusively through Kafka:

- **Post.Cmd.Api** — Write side. Accepts commands, enforces business rules via `PostAggregate`, persists events to MongoDB, publishes them to Kafka.
- **Post.Query.Api** — Read side. Consumes Kafka events, projects them into a SQL Server read model, serves queries.

```
Client
  │
  ├─► POST.CMD.API ─► PostAggregate ─► MongoDB (event store) ─► Kafka
  │
  └─► POST.QUERY.API ◄─ Kafka ◄─ EventConsumer ─► SQL Server (read model)
```

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/) and Docker Compose
- MongoDB instance (default: `localhost:27017`)
- SQL Server instance (default: `localhost:1433`)

## 1. Infrastructure Setup

Create the Docker network (one-time):

```bash
docker network create dotnet-kafka-network
```

Start Kafka and related services:

```bash
docker-compose up -d
```

The docker-compose stack starts the following services:

| Service         | Host Port | Purpose                          |
|-----------------|-----------|----------------------------------|
| Kafka           | 9092      | Message broker                   |
| AKHQ            | 8080      | Kafka management UI              |
| Zookeeper       | —         | Kafka coordination (internal)    |
| Schema Registry | —         | Avro schema management (internal)|
| Kafka Connect   | —         | Connector framework (internal)   |

> MongoDB and SQL Server are **not** included in docker-compose. Run them separately or use local instances.

> **Important — Kafka hostname resolution:** Kafka advertises itself as `kafka:9092` (Docker-internal hostname). The .NET apps run on the host and use `localhost:9092` as the bootstrap address, but after the initial connection Kafka returns `kafka:9092` in its metadata — which the host cannot resolve. Add this entry to your `/etc/hosts` so the broker is reachable:
>
> ```
> 127.0.0.1 kafka
> ```

## 2. Configuration

### SQL Server password

The SQL Server password in `Post.Query.Api/appsettings.json` is set to `CHANGE_ME`. Override it before running:

```bash
# Option A: environment variable override (recommended)
export ConnectionStrings__SqlServer="Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=<your-password>;TrustServerCertificate=true"

# Option B: dotnet user-secrets (development only)
cd sm-post/Post.Query/Post.Query.Api
dotnet user-secrets set "ConnectionStrings:SqlServer" "Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=<your-password>;TrustServerCertificate=true"
```

### Kafka topic

Both APIs require the `KAFKA_TOPIC` environment variable at startup. The app will fail fast with a clear error if it is not set.

```bash
export KAFKA_TOPIC=SocialMediaPostEvents
```

## 3. Build

```bash
dotnet build sm-post/SM-Post.sln
```

## 4. Run

Open two terminals — one per API.

**Terminal 1 — Command API (write side, port 5033):**

```bash
export KAFKA_TOPIC=SocialMediaPostEvents
dotnet run --project sm-post/Post.Cmd/Post.Cmd.Api
```

**Terminal 2 — Query API (read side, port 5068):**

```bash
export KAFKA_TOPIC=SocialMediaPostEvents
export ConnectionStrings__SqlServer="Server=localhost,1433;Database=SocialMedia;User Id=SMUser;Password=<your-password>;TrustServerCertificate=true"
dotnet run --project sm-post/Post.Query/Post.Query.Api
```

The SQL Server database and tables are created automatically on first startup via `EnsureCreated()`.

## 5. Swagger UI

Swagger is available in the `Development` environment only:

- Command API: `http://localhost:5033/swagger`
- Query API: `http://localhost:5068/swagger`

## 6. API Endpoints

### Command API (`Post.Cmd.Api`)

| Method   | Route                               | Description                              |
|----------|-------------------------------------|------------------------------------------|
| `POST`   | `/api/v1/Posts`                     | Create a new post                        |
| `PUT`    | `/api/v1/Posts/editPost/{id}`       | Edit a post's message                    |
| `PUT`    | `/api/v1/Posts/likePost/{id}`       | Like a post                              |
| `DELETE` | `/api/v1/Posts/{id}`                | Delete a post                            |
| `PUT`    | `/api/v1/Comments/addComment/{id}`  | Add a comment to a post                  |
| `PUT`    | `/api/v1/Comments/editComment/{id}` | Edit a comment                           |
| `DELETE` | `/api/v1/Comments/{id}`             | Remove a comment (CommentId in body)     |
| `POST`   | `/api/v1/RestoreReadDb`             | Replay all events to rebuild read model  |

### Query API (`Post.Query.Api`)

| Method | Route                                      | Description                        |
|--------|--------------------------------------------|------------------------------------|
| `GET`  | `/api/v1/PostLookup`                       | List all posts                     |
| `GET`  | `/api/v1/PostLookup/byId/{id}`             | Get post by ID                     |
| `GET`  | `/api/v1/PostLookup/byAuthor/{author}`     | List posts by author               |
| `GET`  | `/api/v1/PostLookup/withLikes/{numberOfLikes}` | List posts with at least N likes |
| `GET`  | `/api/v1/PostLookup/withComments`          | List posts that have comments      |

## 7. Rebuilding the Read Model

If the SQL Server read model becomes inconsistent with the MongoDB event store, replay all events:

```bash
curl -X POST http://localhost:5033/api/v1/RestoreReadDb
```

This republishes every event from MongoDB to Kafka, which the Query API consumes to repopulate SQL Server from scratch.

## 8. Troubleshooting

| Symptom | Action |
|---------|--------|
| Query API returns stale or missing data | `POST /api/v1/RestoreReadDb` to replay events |
| Events not appearing in SQL Server | Check Query API logs for Kafka consumer errors |
| Aggregate state incorrect | Inspect the event stream in MongoDB (`socialmedia.eventStore`) |
| `KAFKA_TOPIC environment variable is not set` | Export the variable before starting either API |
| Connection refused on port 9092 | Ensure `docker-compose up -d` ran successfully; check `docker ps` |
| Broker not available / metadata error on port 9092 | Kafka advertises `kafka:9092` — add `127.0.0.1 kafka` to `/etc/hosts` |
