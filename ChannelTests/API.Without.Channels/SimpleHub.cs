using Microsoft.AspNetCore.SignalR;

namespace API.Without.Channels;

public class SimpleHub : Hub<ISimpleClientHub>
{
    private readonly ILogger<SimpleHub> _logger;
    private readonly DataDictionary _dataDictionary;

    public SimpleHub(ILogger<SimpleHub> logger, DataDictionary dataDictionary)
    {
        _logger = logger;
        _dataDictionary = dataDictionary;
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogInformation("Client connected {Client}", Context.ConnectionId);
        
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogInformation("Client disconnected {Client}", Context.ConnectionId);

        return base.OnDisconnectedAsync(exception);
    }

    public Task Pong(Data data)
    {
        var item = _dataDictionary[data.Id];
        
        item.ReceivedAt = DateTime.Now;

        return Task.CompletedTask;
    }
}