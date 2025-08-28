[![progress-banner](https://backend.codecrafters.io/progress/redis/85947751-78a5-4020-9fa2-959745c2341a)](https://app.codecrafters.io/users/codecrafters-bot?r=2qF)    
> **Built following the [CodeCrafters "Build Your Own Redis" Challenge](https://codecrafters.io/challenges/redis)**    
  
# 🚀 Redis Clone 

A feature-rich Redis server implementation built in C# that supports the Redis protocol, replication, pub/sub messaging, transactions, and multiple data structures. 
This project was developed following CodeCrafters' Redis implementation challenge and test cases

## 📋 Table of Contents

- [🎯 Project Overview](#-project-overview)
- [🚀 Features](#-features)
- [🏗️ Architecture](#️-architecture)
- [⚙️ Installation & Setup](#️-installation--setup)
- [💻 Usage Examples](#-usage-examples)
- [🔧 Configuration](#-configuration)
- [📁 Project Structure](#-project-structure)
- [🧪 Testing](#-testing)
- [🤝 Contributing](#-contributing)
- [📄 License](#-license)

## 🎯 Project Overview

This Redis clone implements a multi-threaded TCP server that provides Redis-compatible functionality with support for multiple data types, master-slave replication, pub/sub messaging, and transaction processing. 
The server follows Redis protocol specifications and supports concurrent client connections with thread-safe operations.

### Key Capabilities
- ✅ Redis Protocol Compatible
- ✅ Multi-threaded Client Handling
- ✅ Master-Slave Replication
- ✅ Pub/Sub Messaging System
- ✅ ACID Transactions
- ✅ Stream Data Processing
- ✅ RDB Persistence Support

## 🚀 Features

### 📝 String Operations
- `GET` - Retrieve string values
- `SET` - Store string values with optional expiration
- `INCR` - Increment numeric string values
- `ECHO` - Echo back messages
- `PING` - Connection testing

### 📋 List Operations
- `LPUSH` / `RPUSH` - Add elements to lists
- `LPOP` - Remove elements from lists
- `LLEN` - Get list length
- `LRANGE` - Get list range
- `BLPOP` - Blocking list pop operations

### 🔢 Sorted Set Operations
- `ZADD` - Add members with scores
- `ZRANK` - Get member rank
- `ZRANGE` - Get range of members
- `ZCARD` - Get sorted set cardinality
- `ZSCORE` - Get member score
- `ZREM` - Remove members

### 🌊 Stream Operations
- `XADD` - Add entries to streams
- `XREAD` - Read from streams with blocking support
- `XRANGE` - Get stream range

### 🔄 Replication Features
- **Master-Slave Replication** with automatic synchronization
- **PSYNC** protocol support for initial sync
- **REPLCONF** for replica configuration
- **Command Propagation** to maintain consistency
- **RDB File Transfer** for initial synchronization

### 📢 Pub/Sub System
- `PUBLISH` - Publish messages to channels
- `SUBSCRIBE` / `UNSUBSCRIBE` - Channel subscription management
- **Multi-client Broadcasting**
- **Thread-safe Channel Management**

### 💳 Transaction Support
- `MULTI` - Start transaction blocks
- `EXEC` - Execute queued commands
- `DISCARD` - Cancel transactions
- **Command Queuing** with atomic execution

### 🔧 Server Management
- `INFO` - Server information and statistics
- `CONFIG` - Configuration management
- `KEYS` - Key pattern matching
- `TYPE` - Get key data types
- `WAIT` - Wait for replica acknowledgments

## 🏗️ Architecture

### High-Level System Design

The Redis clone follows a layered architecture with clear separation of concerns:

```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   TCP Clients   │───▶│   RedisServer    │───▶│ CommandExecutor │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                                │                        │
                                ▼                        ▼
                       ┌──────────────────┐    ┌─────────────────┐
                       │ MasterManager/   │    │ CommandContainer│
                       │ ReplicaManager   │    └─────────────────┘
                       └──────────────────┘             │
                                                        ▼
                                               ┌─────────────────┐
                                               │  RedisStorage   │
                                               └─────────────────┘
```

### Key Components

- **RedisServer**: TCP connection handler and client management
- **CommandExecutor**: Redis protocol parsing and command routing
- **ServerContext**: Dependency injection container
- **MasterManager**: Handles replication to slaves
- **ReplicaManager**: Manages slave synchronization
- **SubscriptionManager**: Pub/sub channel management

### Design Patterns
- **Command Pattern**: Each Redis command is a separate class
- **Dependency Injection**: Centralized service management
- **Factory Pattern**: Command instantiation via CommandContainer
- **Singleton Pattern**: Transaction and subscription managers
- **Observer Pattern**: Pub/sub message broadcasting

## ⚙️ Installation & Setup

### Prerequisites
- **.NET 6.0 or higher**
- **C# 10.0 support**
- **Windows/Linux/macOS compatible**

### Build Instructions

```bash
# Clone the repository
git clone https://github.com/3brhmaan/Redis-Clone.git
cd Redis-Clone

# Build the project
dotnet build

# Run the server
dotnet run
```

### Configuration Options

The server supports various command-line arguments:

```bash
# Basic server startup
dotnet run

# Custom port
dotnet run --port 6380

# Slave mode with master connection
dotnet run --replicaof "127.0.0.1 6379"

# RDB persistence
dotnet run --dir "/path/to/data" --dbfilename "dump.rdb"
```

## 💻 Usage Examples

### Starting the Server

```bash
# Start master server on default port 6379
dotnet run

# Start slave server connecting to master
dotnet run --port 6380 --replicaof "127.0.0.1 6379"
```

### Basic Operations

```bash
# Connect with redis-cli or any Redis client
redis-cli -p 6379

# String operations
SET mykey "Hello World"
GET mykey
INCR counter

# List operations
LPUSH mylist "item1" "item2"
LRANGE mylist 0 -1

# Sorted set operations
ZADD myset 1 "member1" 2 "member2"
ZRANGE myset 0 -1

# Pub/Sub
SUBSCRIBE channel1
PUBLISH channel1 "Hello subscribers!"

# Transactions
MULTI
SET key1 "value1"
SET key2 "value2"
EXEC
```

## 🔧 Configuration

### Command-Line Arguments

| Argument | Description | Example |
|----------|-------------|---------|
| `--port` | Server port | `--port 6380` |
| `--replicaof` | Master server for replication | `--replicaof "127.0.0.1 6379"` |
| `--dir` | RDB file directory | `--dir "/data"` |
| `--dbfilename` | RDB filename | `--dbfilename "redis.rdb"` |

### Replication Setup

**Master Configuration:**
```bash
dotnet run --port 6379
```

**Slave Configuration:**
```bash
dotnet run --port 6380 --replicaof "127.0.0.1 6379"
```

The slave automatically performs the replication handshake and synchronizes with the master.

## 📁 Project Structure

```
codecrafters-redis/
├── 📄 Program.cs                      # Entry point
│
├── 📁 Core/                          # 🏗️ Foundation layer
│   ├── IServerContext.cs             # Dependency injection container
│   ├── ServerContext.cs              
│   ├── RedisServerConfiguration.cs   # Server configuration
│   └── ReplicationMode.cs            # Master/Slave enum
│
├── 📁 Storage/                       # 💾 Data persistence
│   ├── IRedisStorage.cs              # Storage interface
│   ├── RedisStorage.cs               # In-memory storage
│   ├── RdbFileHandler.cs             # RDB file operations
│   └── Values/                       # Redis data types
│       ├── RedisString.cs, RedisList.cs, RedisStream.cs, etc.
│
├── 📁 Commands/                      # 🎯 Redis commands
│   ├── Base/                         # Command infrastructure
│   ├── Container/                    # Command registration & execution
│   ├── String/                       # GET, SET, INCR, ECHO
│   ├── List/                         # LPUSH, LPOP, BLPOP, LRANGE, etc.
│   ├── Stream/                       # XADD, XREAD, XRANGE
│   ├── SortedSet/                    # ZADD, ZRANK, ZRANGE, etc.
│   ├── Transaction/                  # MULTI, EXEC, DISCARD
│   ├── PubSub/                       # SUBSCRIBE, PUBLISH, UNSUBSCRIBE
│   ├── Replication/                  # PSYNC, REPLCONF, WAIT
│   └── Server/                       # PING, INFO, CONFIG, KEYS, TYPE
│
├── 📁 Network/                       # 🌐 Communication layer
│   ├── RedisServer.cs                # TCP server
│   ├── MasterManager.cs              # Master replication logic
│   ├── ReplicaManager.cs             # Slave replication logic
│   └── Parsing/
│       └── RedisProtocolParser.cs    # RESP protocol parser
│
├── 📁 Concurrency/                   # 🔒 Thread safety
│   ├── Locking/                      # Key-based locking
│   └── Transactions/                 # Transaction state management
│
└── 📁 Features/                      # ⚡ Advanced features
    └── PubSub/
        └── SubscriptionManager.cs    # Pub/Sub channel management
```

### 🎯 Key Components
- **Core**: Configuration and dependency injection
- **Storage**: In-memory data structures and RDB persistence  
- **Commands**: 35+ Redis commands organized by data type
- **Network**: TCP server with master-slave replication
- **Concurrency**: Thread-safe operations and transactions

## 🧪 Testing

The implementation was developed following CodeCrafters' test cases and Redis protocol specifications. The server has been tested for:

- ✅ Redis protocol compliance
- ✅ Multi-client concurrent access
- ✅ Replication synchronization
- ✅ Transaction atomicity
- ✅ Pub/Sub message delivery
- ✅ Data type operations



## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Implement your changes following the existing patterns
4. Add new commands by extending `RedisCommand` base class
5. Register new commands in `Server.cs`
6. Test thoroughly with Redis clients

## 🙏 Acknowledgments

Special thanks to **CodeCrafters** for providing the Redis implementation challenge, test cases, and guidance that made this project possible. Their structured approach and comprehensive test suite were instrumental in building a Redis-compatible server.

- 🎓 **CodeCrafters Redis Challenge:** https://codecrafters.io/challenges/redis
- 📚 **Redis Protocol Specification:** Used for ensuring compatibility
- 🧪 **Test Cases:** CodeCrafters' test suite validated the implementation

