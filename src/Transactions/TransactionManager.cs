using System.Collections.Concurrent;

namespace codecrafters_redis.src.Transactions;
public class TransactionManager
{
    private static TransactionManager _instance;
    private static readonly object _lock = new();
    private readonly ThreadLocal<string> _currentClientId = new(); // it's value only visible to the current thread
    private readonly ConcurrentDictionary<string , TransactionState> _clientTransactions = new();

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

    public void CreateTransactionState(string clientId)
    {
        if(!_clientTransactions.ContainsKey(clientId))
        {
            _currentClientId.Value = clientId;
            _clientTransactions.TryAdd(_currentClientId.Value , new TransactionState());
        }
    }

    public TransactionState? GetTransactionState()
    {
        if (string.IsNullOrEmpty(_currentClientId.Value))
            return null;

        if (!_clientTransactions.ContainsKey(_currentClientId.Value))
            return null;

        return _clientTransactions[_currentClientId.Value];
    }

    public void RemoveTransactionState()
    {
        if (!string.IsNullOrEmpty(_currentClientId.Value))
        {
            _clientTransactions.TryRemove(
                _currentClientId.Value , out _
            );
        }
    }
}
