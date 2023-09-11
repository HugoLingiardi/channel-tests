using Microsoft.AspNetCore.SignalR.Client;

namespace Worker.Without.Channels;

public class SimpleWorker : BackgroundService
{
    private readonly ILogger<SimpleWorker> _logger;
    private readonly HubConnection _hubConnection;
    
    public SimpleWorker(ILogger<SimpleWorker> logger)
    {
        _logger = logger;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5075/hub/simple-hub")
            .WithAutomaticReconnect(new []{ TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(750) })
            .AddJsonProtocol()
            .Build();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = StartHubListening(stoppingToken);
        
        return Task.CompletedTask;
    }

    private async Task StartHubListening(CancellationToken cancellationToken)
    {

        _hubConnection.On<Data>("Ping", async data =>
        {
            _logger.LogInformation("Data received {Data}", data.Id);

            await Task.Delay(Random.Shared.Next(250, 1000), cancellationToken);

            await _hubConnection.SendAsync("pong", data, cancellationToken);

            _logger.LogInformation("Data sent {Data}", data.Id);

        });
        
        await _hubConnection.StartAsync(cancellationToken);
        
        _logger.LogInformation("Connected to hub");
    }
}