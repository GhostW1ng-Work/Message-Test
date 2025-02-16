using MessageTest;
using MessageTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Npgsql;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
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
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo
	{
		Title = "Message API",
		Version = "v1",
		Description = "API для отправки и получения сообщений"
	});
});

builder.Services.AddSignalR();
builder.Services.AddLogging();
builder.Services.AddSingleton<IDataBaseService, DataBaseService>();

builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", policy =>
	{
		policy.WithOrigins("http://localhost:3000", "http://message-test-frontend-1")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}


app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");

app.Run();
