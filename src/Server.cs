using codecrafters_redis.src;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

var configuration = RedisServerConfiguration.ParseArguments(args);
var storage = new RedisStorage();
var lockManager = new KeyLockManager();

var serverContext = new ServerContext(
    configuration ,
    storage ,
    lockManager
);

var commandHandler = new RedisRequestProcessor(serverContext);

var server = new RedisServer(commandHandler , configuration);

server.Start();
