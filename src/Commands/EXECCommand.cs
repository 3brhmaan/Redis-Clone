using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;
using System.Text;

namespace codecrafters_redis.src.Commands;
public class EXECCommand : RedisCommand
{
    private readonly CommandFactory commandRegistry;
    private readonly TransactionManager transactionManager;
    public override string Name => "EXEC";
    public EXECCommand(IServerContext serverContext) 
        : base(serverContext) 
    {
        commandRegistry = CommandFactory.Instance;
        transactionManager = TransactionManager.Instance;
    }

    public override string Execute(string[] arguments)
    {
        var transactionState = transactionManager.GetTransactionState();

        if (transactionState is null)
        {
            return "-ERR EXEC without MULTI\r\n";
        }
        else
        {
            var queuedCommands = transactionState.GetQueuedCommands();

            var result = new StringBuilder();
            result.Append($"*{queuedCommands.Count}\r\n");
            foreach (var queuedCommand in queuedCommands)
            {
                var command = commandRegistry.GetCommand(queuedCommand.name);
                result.Append(command.Execute(queuedCommand.args));
            }

            transactionManager.RemoveTransactionState();

            return result.ToString();
        }
    }
}
