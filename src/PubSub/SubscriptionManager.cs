using System.Collections.Concurrent;

namespace codecrafters_redis.src.PubSub;
public class SubscriptionManager
{
    private readonly ConcurrentDictionary<string , HashSet<string>> _clientChannels = new();
    private readonly ThreadLocal<string> _currentClientId = new ThreadLocal<string>();
    private readonly ThreadLocal<bool> _isInSubcsribeMode = new ThreadLocal<bool>();
    private readonly object _lock = new object();

    public bool IsInSubscribeMode => _isInSubcsribeMode.Value;
    public void SetCurrentClient(string clientId)
    {
        _currentClientId.Value = clientId;
    }
    public int Subscribe(string channel)
    {
        _isInSubcsribeMode.Value = true;
        string clientId = _currentClientId.Value;

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
