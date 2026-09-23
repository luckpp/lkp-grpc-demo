# lkp-grpc-demo

A small .NET gRPC demo built around an EV (electric vehicle) scenario. It demonstrates two of the core gRPC communication patterns:

- **Unary RPC** — `GetCarStatus`: the client sends a car ID and receives the current EV status.
- **Server Streaming RPC** — `GetCarLocationRealTime`: the client sends a car ID and the server streams simulated location updates.

Demo car IDs: `ev1`, `ev2`, `ev3`.

---

## Presentation Structure

### 1. Protocol Buffers (Protobuf)

Protobuf is the interface definition language (IDL) and serialization format used by gRPC.

- **Define messages & services** in a single `.proto` contract shared by client and server.
- **Intuitive message definition** — the schema is compact and easy to read.
- **Generates code** — the compiler produces strongly-typed C# classes for messages and service stubs.
- **Efficient serialization/deserialization** — data is encoded as a compact **binary** format (not text).

**Protobuf vs JSON**

| Aspect | Protobuf | JSON |
| --- | --- | --- |
| Payload size | Smaller (binary) | Larger (text) |
| Parsing cost | Low | CPU intensive |
| Speed | Faster | Slower |
| Schema | Strongly typed, enforced | Loosely typed |

- Smaller payloads and lower parsing cost make Protobuf **faster** overall.
- **Better for mobile and microcontrollers**, where bandwidth, CPU, and battery are constrained.

### 2. HTTP/2 (the transport used by gRPC)

Helpful visualizations:
- https://yurukusa.github.io/http2-visualizer/
- https://aprelium.com/abyssws/articles/multiplexing-in-http2.html

Key characteristics:
- **One shared TCP connection** for many requests.
- **Multiplexing** — client and server can send multiple messages in parallel over the same TCP connection.
- **Supports server push**.
- **Headers & body compressed to binary** data.
- **TLS required by default**.

**HTTP/1.1 (for comparison)**
- Powers most of the web.
- Typically **opens a TCP connection per request**.
- **Does not compress headers** (plaintext).
- Follows a strict **request/response** model only.

### 3. Practical Steps

**Install the NuGet packages**
- **Grpc** — metapackage that pulls in all required dependencies.
- **Grpc.Tools** — the compiler for `.proto` files (generates C# code at build time).
- **Google.Protobuf** — runtime library that represents messages as C# objects and handles serialization/deserialization.

**Update the `.csproj` files**
1. Unload the project.
2. Add the Protobuf item group so the compiler generates the C# classes:

```xml
<ItemGroup>
  <Protobuf Include="../*.proto" OutputDir="%(RelativePath)Models/"></Protobuf>
</ItemGroup>
```

3. Reload the project and build — the generated classes appear under `Models/`.

---
