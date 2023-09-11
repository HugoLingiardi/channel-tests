using Worker.With.Channels;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {
        services.AddHostedService<SimpleWorker>();
    })
    .Build();

host.Run();