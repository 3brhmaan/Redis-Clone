using System.Collections.Concurrent;

namespace codecrafters_redis.src.PubSub;
public class SubscriptionManager
{
    private readonly ConcurrentDictionary<string , HashSet<string>> _clientChannels = new();
    private readonly ThreadLocal<string> _currentClientId = new ThreadLocal<string>();
    private readonly object _lock = new object();

    public void SetCurrentClient(string clientId)
    {
        _currentClientId.Value = clientId;
    }

    public int Subscribe(string channel)
    {
        string clientId = _currentClientId.Value;
        if (string.IsNullOrEmpty(clientId))
            throw new InvalidOperationException("No client context set");

        lock (_lock)
        {
            if (!_clientChannels.ContainsKey(clientId))
            {
                _clientChannels[clientId] = new HashSet<string>();
            }

            _clientChannels[clientId].Add(channel);
            return _clientChannels[clientId].Count;
        }
    }
}
