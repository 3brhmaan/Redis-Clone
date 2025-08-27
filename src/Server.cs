using codecrafters_redis.src;
using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;
using codecrafters_redis.src.PubSub;


void RegisterCommands(IServerContext context)
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
}


var configuration = new RedisServerConfiguration(args);

if (!string.IsNullOrEmpty(configuration.RDBdire))
{
    RdbFileHandler.EnsureFileExist(configuration.RDBdire, configuration.RDBdbfilename);
}

var storage = new RedisStorage();
var lockManager = new KeyLockManager();
var commandContainer = new CommandContainer();
var subscriptionManager = new SubscriptionManager();
var commandExecutor = new CommandExecutor(commandContainer, subscriptionManager);


var serverContext = new ServerContext(
    configuration ,
    storage ,
    lockManager,
    commandContainer,
    commandExecutor,
    subscriptionManager
);

RegisterCommands(serverContext);

var server = new RedisServer(serverContext);

server.Start();
