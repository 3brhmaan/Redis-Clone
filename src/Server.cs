using codecrafters_redis.src;
using codecrafters_redis.src.Commands;
using codecrafters_redis.src.Core;
using codecrafters_redis.src.Data.Storage;
using codecrafters_redis.src.Locking;


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
}


var configuration = RedisServerConfiguration.ParseArguments(args);
var storage = new RedisStorage();
var lockManager = new KeyLockManager();
var commandContainer = new CommandContainer();
var commandExecutor = new CommandExecutor(commandContainer);


var serverContext = new ServerContext(
    configuration ,
    storage ,
    lockManager,
    commandContainer,
    commandExecutor
);

RegisterCommands(serverContext);

var server = new RedisServer(serverContext);

server.Start();
