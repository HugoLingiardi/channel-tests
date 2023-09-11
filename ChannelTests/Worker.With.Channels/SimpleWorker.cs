using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR.Client;

namespace Worker.With.Channels;

public class SimpleWorker : BackgroundService
{
    private readonly ILogger<SimpleWorker> _logger;
    private readonly HubConnection _hubConnection;
    private readonly Channel<Data> _channelData;
    
    public SimpleWorker(ILogger<SimpleWorker> logger)
    {
        _logger = logger;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5215/hub/simple-hub")
            .WithAutomaticReconnect(new []{ TimeSpan.FromMilliseconds(250), TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(750) })
            .AddJsonProtocol()
            .Build();
        
        _channelData = Channel.CreateBounded<Data>(new BoundedChannelOptions(1_000)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = true,
        });
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _ = StartHubListening(stoppingToken);
        _ = StartChannelProcess(stoppingToken);
        
        return Task.CompletedTask;
    }

    private async Task StartChannelProcess(CancellationToken cancellationToken)
    {
        await Parallel.ForEachAsync(_channelData.Reader.ReadAllAsync(cancellationToken), cancellationToken, async (data, token) =>
        {
            await Task.Delay(Random.Shared.Next(250, 1000), token);

            await _hubConnection.SendAsync("pong", data, token);
            
            _logger.LogInformation("Data sent {Data}", data.Id);
        });
    }

    private async Task StartHubListening(CancellationToken cancellationToken)
    {

        _hubConnection.On<Data>("Ping", data =>
        {
            _logger.LogInformation("Data received {Data}", data.Id);

            _ = _channelData.Writer.WriteAsync(data, cancellationToken).AsTask();
        });
        
        await _hubConnection.StartAsync(cancellationToken);
        
        _logger.LogInformation("Connected to hub");
    }
}