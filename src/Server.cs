using codecrafters_redis.src;


var redisServer = new RedisServer(6379);

try
{
    redisServer.Start();
}
catch (Exception ex)
{
    Console.WriteLine($"Server error: {ex.Message}");
}