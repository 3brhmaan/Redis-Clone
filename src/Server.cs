using codecrafters_redis.src;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;

int port = 6379;
if(args.Length > 0)
{
    port = int.Parse(args[1]);
}

var redisServer = new RedisServer(
    new RedisCommandHandler(new RedisStorage() , new KeyLockManager()) , 
    port
);

redisServer.Start();
