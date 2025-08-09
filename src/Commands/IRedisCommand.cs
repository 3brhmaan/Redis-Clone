namespace codecrafters_redis.src.Commands;
public interface IRedisCommand
{
    string Execute(string[] arguments);
    string Name { get; }
}
