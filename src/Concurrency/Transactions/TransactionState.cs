namespace codecrafters_redis.src.Concurrency.Transactions;
public class TransactionState
{
    private List<(string name, string[] args)> CommandQueue { get; set; } = new();

    public void QueueCommand(string command , string[] args)
    {
        CommandQueue.Add((command, args));
    }

    public List<(string name, string[] args)> GetQueuedCommands()
    {
        return new List<(string, string[])>(CommandQueue);
    }
}
