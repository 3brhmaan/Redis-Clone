using System.Collections.Concurrent;

namespace codecrafters_redis.src.Transactions;
public class TransactionManager
{
    private static TransactionManager _instance;
    private static readonly object _lock = new();
    private readonly ThreadLocal<string> _currentClientId = new(); // it's value only visible to the current thread
    private readonly ConcurrentDictionary<string , TransactionState> _clientTransactions;

    private TransactionManager()
    {
        _clientTransactions = new ConcurrentDictionary<string , TransactionState>();
    }

    public static TransactionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        _instance = new TransactionManager();
                    }
                }
            }
            return _instance;
        }
    }

    public void SetCurrentClientId(string clientId)
    {
        _currentClientId.Value = clientId;
    }

    public TransactionState? GetTransactionState()
    {
        if (string.IsNullOrEmpty(_currentClientId.Value))
            return null;

        return _clientTransactions.GetOrAdd(
            _currentClientId.Value , new TransactionState()
        );
    }

    public void RemoveTransactionState()
    {
        if(!string.IsNullOrEmpty(_currentClientId.Value))
        {
            _clientTransactions.TryRemove(
                _currentClientId.Value , out _
            );
        }
    }
}
