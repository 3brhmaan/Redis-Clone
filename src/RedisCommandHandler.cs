using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src;


public class RedisCommandHandler
{
    private readonly CommandRegistry commandRegistry;
    private readonly IRedisStorage storage;
    private readonly IKeyLockManager lockManager;
    private readonly TransactionManager transactionManager;

    public RedisCommandHandler(
            IRedisStorage storage ,
            IKeyLockManager lockManager
    )
    {
        this.storage = storage;
        this.lockManager = lockManager;

        transactionManager = TransactionManager.Instance;
        commandRegistry = CommandRegistry.Instance;

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        commandRegistry.Register(() => new BLPOPCommand(storage , lockManager));
        commandRegistry.Register(() => new ECHOCommand(storage , lockManager));
        commandRegistry.Register(() => new GETCommand(storage , lockManager));
        commandRegistry.Register(() => new LLENCommand(storage , lockManager));
        commandRegistry.Register(() => new LPOPCommand(storage , lockManager));
        commandRegistry.Register(() => new LPUSHCommand(storage , lockManager));
        commandRegistry.Register(() => new LRANGECommand(storage , lockManager));
        commandRegistry.Register(() => new PINGCommand(storage , lockManager));
        commandRegistry.Register(() => new RPUSHCommand(storage , lockManager));
        commandRegistry.Register(() => new SETCommand(storage , lockManager));
        commandRegistry.Register(() => new TYPECommand(storage , lockManager));
        commandRegistry.Register(() => new XADDCommand(storage , lockManager));
        commandRegistry.Register(() => new XRANGECommand(storage , lockManager));
        commandRegistry.Register(() => new XREADCommand(storage , lockManager));
        commandRegistry.Register(() => new INCRCommand(storage , lockManager));
        commandRegistry.Register(() => new MULTICommand(storage , lockManager));
        commandRegistry.Register(() => new EXECCommand(storage , lockManager));
    }
    public string ParseRedisCommand(string request)
    {
        var transactionState = transactionManager.GetTransactionState();

        var commandParts = RespParser.Parse(request);

        var commandName = commandParts[0];
        var arguments = commandParts.Skip(1).ToArray();

        if (transactionState is not null && transactionState.IsInTransaction && commandName != "EXEC")
        {
            transactionState.QueueCommand(commandName , arguments);
            return "+QUEUED\r\n";
        }
        else
        {
            var command = commandRegistry.GetCommand(commandName);

            return command.Execute(arguments);
        }
    }
}