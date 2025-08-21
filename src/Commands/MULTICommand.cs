using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src.Commands;
public class MULTICommand : RedisCommand
{
    private readonly TransactionManager transactionManager;
    public override string Name => "MULTI";
    public MULTICommand(IServerContext serverContext) 
        : base(serverContext) 
    {
        transactionManager = TransactionManager.Instance;
    }

    public override string Execute(string[] arguments)
    {
        var transactionState = transactionManager.GetTransactionState();

        return "+OK\r\n";
    }
}
