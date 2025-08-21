using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src.Commands;
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
