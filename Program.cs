using MessageTest.Extensions;
using MessageTest.Hubs;
using Npgsql;
using Serilog;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

var builder = WebApplication.CreateBuilder(args)
	.AddSwaggerConfiguration()
	.AddApplicationServices()
	.AddCorsConfiguration();

var configuration = builder.Configuration;
var connectionString = configuration.GetConnectionString("PostgresConnection");

EnsureDatabaseSetup(connectionString);

builder.WebHost.UseUrls("http://0.0.0.0:80");


void EnsureDatabaseSetup(string connectionString)
{
	using var connection = new NpgsqlConnection(connectionString);
	connection.Open();

	using var command = new NpgsqlCommand(@"
	CREATE TABLE IF NOT EXISTS messages (
		id SERIAL PRIMARY KEY,
		text TEXT NOT NULL,
		order_number BIGINT NOT NULL,
		timestamp TIMESTAMP NOT NULL
);", connection);
	command.ExecuteNonQuery();
}

Log.Logger = new LoggerConfiguration()
	.WriteTo.Console()
	.WriteTo.File("logs/app.log", rollingInterval: RollingInterval.Day)
	.CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSignalR();
builder.Services.AddLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");

app.Run();
