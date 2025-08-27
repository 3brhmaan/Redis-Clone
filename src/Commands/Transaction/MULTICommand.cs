using codecrafters_redis.src.Commands.Base;
using codecrafters_redis.src.Concurrency.Transactions;
using codecrafters_redis.src.Core;

namespace codecrafters_redis.src.Commands.Transaction;
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
