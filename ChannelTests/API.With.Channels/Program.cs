using API.With.Channels;
using Microsoft.AspNetCore.SignalR;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddSignalR()
    .AddJsonProtocol();

builder.Services.AddSingleton<DataDictionary>(_ =>
{
    var data = Enumerable.Range(1, 10).Select(i => 
        new KeyValuePair<int, DataWrapper>(i, new DataWrapper {Data = new Data() {Id = i, Message = $"{i}"}}));
    
    return new DataDictionary(data);
});

var app = builder.Build();


app.MapPost("/api/send-and-wait", async (ILogger<Program> logger, DataDictionary dataDictionary, IHubContext<SimpleHub, ISimpleClientHub> hubContext) =>
{
    foreach (var item in dataDictionary.Values)
    {
        item.ReceivedAt = null;
        
        await hubContext.Clients.All.Ping(item.Data!);

        item.SentAt = DateTime.Now;
        
        logger.LogInformation("Data sent {Data}", item.Data!.Id);
    }

    while (!dataDictionary.Values.All(x => x.ReceivedAt is not null))
    {
        await Task.Delay(10);
    }
    
    return Results.Ok(new { message = "all processed" });
});

app.MapHub<SimpleHub>("/hub/simple-hub");

app.Run();