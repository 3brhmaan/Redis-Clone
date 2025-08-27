namespace codecrafters_redis.src.Commands.Base;
public interface IRedisCommand
{
    string Execute(string[] arguments);
    string Name { get; }
}
