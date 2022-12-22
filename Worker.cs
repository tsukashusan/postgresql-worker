using Npgsql;
using Microsoft.Extensions.Options;

namespace postgresql_worker;


public abstract class Worker : BackgroundService
{
    private readonly string Host;
    private readonly string User;
    private readonly string DBname;
    private readonly string Password;
    private readonly ushort Port;
    private readonly ushort MinPoolSize;
    private readonly ushort MaxPoolSize;
    private readonly ushort ConnectionLifeTime;
    protected readonly ushort ReadDelayTime;
    protected readonly ushort UpdateDelayTime;
    protected readonly IHostApplicationLifetime _hostApplicationLifetime;
    protected readonly ILogger<Worker>? _logger;
    private readonly PostgreSQLConfiguration _postgreSQLConfiguration;
    private readonly NpgsqlConnectionStringBuilder _connectionStringBuilder;
    public Worker(IHostApplicationLifetime hostApplicationLifetime, ILogger<Worker>? logger, IOptions<PostgreSQLConfiguration> options)
    {
        _hostApplicationLifetime = hostApplicationLifetime;
        _logger = logger;
        _postgreSQLConfiguration = options.Value;

        Host = _postgreSQLConfiguration.Host;
        User = _postgreSQLConfiguration.User;
        DBname = _postgreSQLConfiguration.DBname;
        Password = _postgreSQLConfiguration.Password;
        Port = _postgreSQLConfiguration.Port;
        ReadDelayTime = _postgreSQLConfiguration.ReadDelayTime;
        UpdateDelayTime = _postgreSQLConfiguration.UpdateDelayTime;
        MinPoolSize = _postgreSQLConfiguration.MinPoolSize;
        MaxPoolSize = _postgreSQLConfiguration.MaxPoolSize;
        ConnectionLifeTime = _postgreSQLConfiguration.ConnectionLifeTime;
        _connectionStringBuilder = new NpgsqlConnectionStringBuilder($"Server={Host};Username={User};Database={DBname};Port={Port};Password={Password};SSLMode=Prefer;MinPoolSize={MinPoolSize};MaxPoolSize={MaxPoolSize};ConnectionLifeTime={ConnectionLifeTime}");
    }
    protected abstract override Task ExecuteAsync(CancellationToken stoppingToken);

    protected async Task create_and_insert()
    {
        // Build connection string using parameters from portal
        //
        NpgsqlConnection? conn = null;
        try
        {
            using (conn = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                _logger.LogInformation("Opening connection");
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand("DROP TABLE IF EXISTS inventory", conn))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Finished dropping table (if existed)");

                }

                using (var command = new NpgsqlCommand("CREATE TABLE inventory(id serial PRIMARY KEY, name VARCHAR(50), quantity INTEGER)", conn))
                {
                    await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Finished creating table");
                }

                using (var command = new NpgsqlCommand("INSERT INTO inventory (name, quantity) VALUES (@n1, @q1), (@n2, @q2), (@n3, @q3)", conn))
                {
                    command.Parameters.AddWithValue("n1", "banana");
                    command.Parameters.AddWithValue("q1", 150);
                    command.Parameters.AddWithValue("n2", "orange");
                    command.Parameters.AddWithValue("q2", 154);
                    command.Parameters.AddWithValue("n3", "apple");
                    command.Parameters.AddWithValue("q3", 100);

                    int nRows = await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Number of rows inserted={0}", nRows);
                }
                await conn.CloseAsync();
            }

            _logger.LogInformation("Exit!");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            if(conn != null) NpgsqlConnection.ClearPool(conn);
            throw e;
        }
    }

    protected async Task read()
    {
        // Build connection string using parameters from portal
        //
        NpgsqlConnection? conn = null;
        try
        {
            using (conn = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                _logger.LogInformation("Opening connection");
                await conn.OpenAsync();


                using (var command = new NpgsqlCommand("SELECT * FROM inventory", conn))
                {

                    var reader = await command.ExecuteReaderAsync();
                    while (await reader.ReadAsync())
                    {
                        _logger.LogInformation("Reading from table=({0}, {1}, {2})", reader.GetInt32(0).ToString(), reader.GetString(1), reader.GetInt32(2).ToString());
                    }
                    await reader.CloseAsync();
                }
                await conn.CloseAsync();
            }
            _logger.LogInformation("Exit");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            if(conn != null) NpgsqlConnection.ClearPool(conn);
            throw e;

        }
        //Console.WriteLine("Press RETURN to exit");
        ////Console.ReadLine();
    }

    protected async Task update()
    {
        // Build connection string using parameters from portal
        //
        NpgsqlConnection? conn = null;
        try
        {
            using (conn = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                _logger.LogInformation("Opening connection");
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand("UPDATE inventory SET quantity = @q WHERE name = @n", conn))
                {
                    command.Parameters.AddWithValue("n", "banana");
                    command.Parameters.AddWithValue("q", 200);
                    int nRows = await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Number of rows updated={0}", nRows);
                }
                await conn.CloseAsync();
            }
            _logger.LogInformation("update complete");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            if(conn != null) NpgsqlConnection.ClearPool(conn);
            throw e;

        }
    }
    protected async Task delete()
    {
        // Build connection string using parameters from portal
        //
        NpgsqlConnection? conn = null;
        try
        {
            using (conn = new NpgsqlConnection(_connectionStringBuilder.ConnectionString))
            {
                _logger.LogInformation("Opening connection");
                await conn.OpenAsync();

                using (var command = new NpgsqlCommand("DELETE FROM inventory WHERE name = @n", conn))
                {
                    command.Parameters.AddWithValue("n", "orange");

                    int nRows = await command.ExecuteNonQueryAsync();
                    _logger.LogInformation("Number of rows deleted={0}", nRows);
                }
                await conn.CloseAsync();
            }

            _logger.LogInformation("Exit");
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            if(conn != null) NpgsqlConnection.ClearPool(conn);
            throw e;

        }
    }
}
