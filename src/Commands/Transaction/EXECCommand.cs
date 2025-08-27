using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Commands.Container;
using codecrafters_redis.src.Concurrency.Transactions;
using codecrafters_redis.src.Core;
using System.Text;

namespace codecrafters_redis.src.Concurrency.Concurrency.Commands.Transaction;
public class EXECCommand : RedisCommand
{
    private readonly CommandContainer commandContainer;
    private readonly TransactionManager transactionManager;
    public override string Name => "EXEC";
    public EXECCommand(IServerContext serverContext) 
        : base(serverContext) 
    {
        commandContainer = serverContext.CommandContainer;
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
                var command = commandContainer.GetCommand(queuedCommand.name);
                result.Append(command.Execute(queuedCommand.args));
            }

            transactionManager.RemoveTransactionState();

            return result.ToString();
        }
    }
}
