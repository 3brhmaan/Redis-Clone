namespace codecrafters_redis.src.Transactions;
public class TransactionState
{
    private static TransactionState instance;
    public bool IsInTransaction { get; private set;} = false;
    private List<(string name, string[] args)> CommandQueue { get; set; } = new();
    public static TransactionState Instance
    {
        get
        {
            if(instance == null)
                instance = new TransactionState();

            return instance;
        }
    }

    public void StartTransaction()
    {
        IsInTransaction = true;
        CommandQueue.Clear();
    }

    public void EndTransaction()
    {
        IsInTransaction = false;
        CommandQueue.Clear();
    }

    public void QueueCommand(string command , string[] args)
    {
        CommandQueue.Add((command, args));
    }

    public List<(string name, string[] args)> GetQueuedCommands()
    {
        return new List<(string, string[])>(CommandQueue);
    }
}
