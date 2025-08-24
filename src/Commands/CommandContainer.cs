namespace codecrafters_redis.src.Commands;
public class CommandContainer
{
    private readonly Dictionary<string , Func<IRedisCommand>> commands = new();

    public void Register<T>(Func<T> factory) where T : IRedisCommand
    {
        var command = factory();
        commands[command.Name.ToUpper()] = () => factory();
    }
    public IRedisCommand? GetCommand(string name)
    {
        return commands.TryGetValue(name.ToUpper() , out var factory)
            ? factory()
            : null;
    }
}
