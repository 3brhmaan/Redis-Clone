using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Concurrency.Transactions;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.Transaction;
public class DISCARDCommand : RedisCommand
{
    private readonly TransactionManager transactionManager;
    public override string Name => "DISCARD";
    public DISCARDCommand(IServerContext serverContext) 
        : base(serverContext)
    {
        transactionManager = TransactionManager.Instance;
    }

    public override string Execute(string[] arguments)
    {
        var transactionState = transactionManager.GetTransactionState();

        if(transactionState is not null )
        {
            transactionManager.RemoveTransactionState();

            return "+OK\r\n";
        }
        else
        {
            return "-ERR DISCARD without MULTI\r\n";
        }
    }
}
