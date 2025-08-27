using System.Collections.Concurrent;
using System.Net.Sockets;

namespace codecrafters_redis.src.PubSub;
public class SubscriptionManager
{
    private readonly ConcurrentDictionary<string , HashSet<string>> _clientChannels = new();
    private readonly ConcurrentDictionary<string , HashSet<Socket>> _channelSockets = new();
    private readonly ThreadLocal<Socket> _currentClient = new ThreadLocal<Socket>();
    private readonly ThreadLocal<bool> _isInSubcsribeMode = new ThreadLocal<bool>();
    private readonly object _lock = new object();

    public bool IsInSubscribeMode => _isInSubcsribeMode.Value;
    public HashSet<Socket> GetChannelSockts(string channel)
    {
        if (_channelSockets.TryGetValue(channel , out var sockts))
            return sockts;
        else
            return new HashSet<Socket> { };
    }
    public void SetCurrentClient(Socket client)
    {
        _currentClient.Value = client;
    }
    public int Subscribe(string channel)
    {
        _isInSubcsribeMode.Value = true;
        string clientId = _currentClient.Value.RemoteEndPoint?.ToString()!;

        lock (_lock)
        {
            if (!_clientChannels.ContainsKey(clientId))
            {
                _clientChannels[clientId] = new HashSet<string>();
            }

            if (!_channelSockets.ContainsKey(channel))
            {
                _channelSockets[channel] = new HashSet<Socket>();
            }

            _channelSockets[channel].Add(_currentClient.Value);
            _clientChannels[clientId].Add(channel);

            return _clientChannels[clientId].Count;
        }
    }
    public int Unsubscribe(string channel)
    {
        string clientId = _currentClient.Value.RemoteEndPoint?.ToString()!;

        lock (_lock)
        {
            if (_clientChannels.ContainsKey(clientId) && _clientChannels[clientId].Contains(channel))
                _clientChannels[clientId].Remove(channel);

            if(_channelSockets.ContainsKey(channel) && _channelSockets[channel].Contains(_currentClient.Value))
                _channelSockets[channel].Remove(_currentClient.Value);
        }

        return _clientChannels[clientId].Count;
    }
}
