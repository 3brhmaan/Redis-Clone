using codecrafters_redis.src.Commands.Container;
using codecrafters_redis.src.Commands.List;
using codecrafters_redis.src.Commands.PubSub;
using codecrafters_redis.src.Commands.Replication;
using codecrafters_redis.src.Commands.Server;
using codecrafters_redis.src.Commands.SortedSet;
using codecrafters_redis.src.Commands.Stream;
using codecrafters_redis.src.Commands.String;
using codecrafters_redis.src.Commands.Transaction;
using codecrafters_redis.src.Concurrency.Concurrency.Commands.Transaction;
using codecrafters_redis.src.Concurrency.Locking;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Network;
using codecrafters_redis.src.PubSub;
using codecrafters_redis.src.Storage;


internal class Program
{
    private static void RegisterCommands(IServerContext context)
    {
        context.CommandContainer.Register(() => new BLPOPCommand(context));
        context.CommandContainer.Register(() => new ECHOCommand(context));
        context.CommandContainer.Register(() => new GETCommand(context));
        context.CommandContainer.Register(() => new LLENCommand(context));
        context.CommandContainer.Register(() => new LPOPCommand(context));
        context.CommandContainer.Register(() => new LPUSHCommand(context));
        context.CommandContainer.Register(() => new LRANGECommand(context));
        context.CommandContainer.Register(() => new PINGCommand(context));
        context.CommandContainer.Register(() => new RPUSHCommand(context));
        context.CommandContainer.Register(() => new SETCommand(context));
        context.CommandContainer.Register(() => new TYPECommand(context));
        context.CommandContainer.Register(() => new XADDCommand(context));
        context.CommandContainer.Register(() => new XRANGECommand(context));
        context.CommandContainer.Register(() => new XREADCommand(context));
        context.CommandContainer.Register(() => new INCRCommand(context));
        context.CommandContainer.Register(() => new MULTICommand(context));
        context.CommandContainer.Register(() => new EXECCommand(context));
        context.CommandContainer.Register(() => new DISCARDCommand(context));
        context.CommandContainer.Register(() => new INFOCommand(context));
        context.CommandContainer.Register(() => new REPLCONFCommand(context));
        context.CommandContainer.Register(() => new PSYNCCommand(context));
        context.CommandContainer.Register(() => new WAITCommand(context));
        context.CommandContainer.Register(() => new CONFIGCommand(context));
        context.CommandContainer.Register(() => new KEYSCommand(context));
        context.CommandContainer.Register(() => new SUBSCRIBECommand(context));
        context.CommandContainer.Register(() => new PUBLISHCommand(context));
        context.CommandContainer.Register(() => new UNSUBSCRIBECommand(context));
        context.CommandContainer.Register(() => new ZADDCommand(context));
        context.CommandContainer.Register(() => new ZRANKCommand(context));
        context.CommandContainer.Register(() => new ZRANGECommand(context));
        context.CommandContainer.Register(() => new ZCARDCommand(context));
        context.CommandContainer.Register(() => new ZSCORECommand(context));
        context.CommandContainer.Register(() => new ZREMCommand(context));
    }
    private static void Main(string[] args)
    {
        var configuration = new RedisServerConfiguration(args);

        if (!string.IsNullOrEmpty(configuration.RDBdire))
        {
            RdbFileHandler.EnsureFileExist(configuration.RDBdire , configuration.RDBdbfilename);
        }

        var storage = new RedisStorage();
        var lockManager = new KeyLockManager();
        var commandContainer = new CommandContainer();
        var subscriptionManager = new SubscriptionManager();
        var commandExecutor = new CommandExecutor(commandContainer , subscriptionManager);


        var serverContext = new ServerContext(
            configuration ,
            storage ,
            lockManager ,
            commandContainer ,
            commandExecutor ,
            subscriptionManager
        );

        RegisterCommands(serverContext);

        var server = new RedisServer(serverContext);

        server.Start();
    }
}