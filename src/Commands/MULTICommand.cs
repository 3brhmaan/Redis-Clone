using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src.Commands;
public class MULTICommand : RedisCommand
{
    private readonly TransactionState state;
    public override string Name => "MULTI";
    public MULTICommand(IRedisStorage storage , IKeyLockManager lockManager)
    : base(storage , lockManager)
    {
        state = TransactionState.Instance;
    }

    public override string Execute(string[] arguments)
    {
        state.StartTransaction();
        return "+OK\r\n";
    }
}
