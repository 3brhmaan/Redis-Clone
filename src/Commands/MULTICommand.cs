using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src.Commands;
public class MULTICommand : RedisCommand
{
    private readonly TransactionManager transactionManager;
    public override string Name => "MULTI";
    public MULTICommand(IRedisStorage storage , IKeyLockManager lockManager)
        : base(storage , lockManager)
    {
        transactionManager = TransactionManager.Instance;
    }

    public override string Execute(string[] arguments)
    {
        var transactionState = transactionManager.GetTransactionState();

        transactionState.StartTransaction();
        return "+OK\r\n";
    }
}
