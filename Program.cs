using postgresql_worker;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        
        var configuration =  hostContext.Configuration;
        services.Configure<PostgreSQLConfiguration>(configuration.GetSection(nameof(PostgreSQLConfiguration)));
        services.AddHostedService<UpdateWorker>();
        services.AddHostedService<ReadWorker>();
        services.AddHostedService<InsertWorker>();
    }).Build();
await host.RunAsync();