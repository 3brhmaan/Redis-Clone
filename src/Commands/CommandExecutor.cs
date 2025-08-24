using codecrafters_redis.src.Core;
using codecrafters_redis.src.Parsing;
using codecrafters_redis.src.Transactions;

namespace codecrafters_redis.src.Commands;


public class CommandExecutor
{
    private readonly CommandContainer container;
    private readonly TransactionManager transactionManager;

    public CommandExecutor(CommandContainer container)
    {
        transactionManager = TransactionManager.Instance;
        this.container = container;
    }
    public string Execute(string strCommand , string clientId)
    {
        //Console.WriteLine($"Parsing: {request}");
        var commandParts = RedisProtocolParser.Parse(strCommand);

        var commandName = commandParts[0];
        var arguments = commandParts.Skip(1).ToArray();


        if (commandName == "MULTI")
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
            var command = container.GetCommand(commandName);

            return command.Execute(arguments);
        }
    }
    private static bool IsTransactionControlCommand(string commandName)
    {
        var upperCommand = commandName.ToUpper();
        return upperCommand is "MULTI" or "EXEC" or "DISCARD";
    }
}