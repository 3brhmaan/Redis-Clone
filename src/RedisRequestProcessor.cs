using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Parsing;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src;


public class RedisRequestProcessor
{
    private readonly CommandFactory commandRegistry;
    private readonly IServerContext _serverContext;
    private readonly TransactionManager transactionManager;

    public RedisRequestProcessor(IServerContext serverContext)
    {
        _serverContext = serverContext;
        transactionManager = TransactionManager.Instance;
        commandRegistry = CommandFactory.Instance;

        RegisterCommands();
    }

    private void RegisterCommands()
    {
        commandRegistry.Register(() => new BLPOPCommand(_serverContext));
        commandRegistry.Register(() => new ECHOCommand(_serverContext));
        commandRegistry.Register(() => new GETCommand(_serverContext));
        commandRegistry.Register(() => new LLENCommand(_serverContext));
        commandRegistry.Register(() => new LPOPCommand(_serverContext));
        commandRegistry.Register(() => new LPUSHCommand(_serverContext));
        commandRegistry.Register(() => new LRANGECommand(_serverContext));
        commandRegistry.Register(() => new PINGCommand(_serverContext));
        commandRegistry.Register(() => new RPUSHCommand(_serverContext));
        commandRegistry.Register(() => new SETCommand(_serverContext));
        commandRegistry.Register(() => new TYPECommand(_serverContext));
        commandRegistry.Register(() => new XADDCommand(_serverContext));
        commandRegistry.Register(() => new XRANGECommand(_serverContext));
        commandRegistry.Register(() => new XREADCommand(_serverContext));
        commandRegistry.Register(() => new INCRCommand(_serverContext));
        commandRegistry.Register(() => new MULTICommand(_serverContext));
        commandRegistry.Register(() => new EXECCommand(_serverContext));
        commandRegistry.Register(() => new DISCARDCommand(_serverContext));
        commandRegistry.Register(() => new INFOCommand(_serverContext));
        commandRegistry.Register(() => new REPLCONFCommand(_serverContext));
        commandRegistry.Register(() => new PSYNCCommand(_serverContext));
        commandRegistry.Register(() => new WAITCommand(_serverContext));
    }
    public string ParseRedisCommand(string request, string clientId)
    {
        //Console.WriteLine($"Parsing: {request}");
        var commandParts = RedisProtocolParser.Parse(request);

        var commandName = commandParts[0];
        var arguments = commandParts.Skip(1).ToArray();


        if(commandName == "MULTI")
        {
            transactionManager.CreateTransactionState(clientId);
        }

        var transactionState = transactionManager.GetTransactionState();

        if (transactionState is not null && !IsTransactionControlCommand(commandName))
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

    private static bool IsTransactionControlCommand(string commandName)
    {
        var upperCommand = commandName.ToUpper();
        return upperCommand is "MULTI" or "EXEC" or "DISCARD";
    }
}