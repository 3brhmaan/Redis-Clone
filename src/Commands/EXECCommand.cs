using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class EXECCommand : RedisCommand
{
    private readonly CommandRegistry commandRegistry;
    private readonly TransactionManager transactionManager;
    public override string Name => "EXEC";
    public EXECCommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager)
    {
        commandRegistry = CommandRegistry.Instance;
        transactionManager = TransactionManager.Instance;
    }

    public override string Execute(string[] arguments)
    {
        var transactionState = transactionManager.GetTransactionState();

        if (!transactionState.IsInTransaction)
        {
            return "-ERR EXEC without MULTI\r\n";
        }
        else
        {
            var queuedCommands = transactionState.GetQueuedCommands();

            var result = new StringBuilder();
            foreach (var queuedCommand in queuedCommands)
            {
                var command = commandRegistry.GetCommand(queuedCommand.name);
                result.Append(command.Execute(queuedCommand.args));
            }

            transactionState.EndTransaction();

            if (queuedCommands.Count == 0)
            {
                return "*0\r\n";
            }

            transactionManager.RemoveTransactionState();

            return result.ToString();
        }
    }
}
