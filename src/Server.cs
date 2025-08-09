using codecrafters_redis.src;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;


var redisServer = new RedisServer(
    new RedisCommandHandler(new RedisStorage() , new KeyLockManager()) , 
    6379
);

redisServer.Start();
