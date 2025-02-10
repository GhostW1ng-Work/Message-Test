using MessageTest;
using MessageTest.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:80");

// ��������� �����������
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
		Description = "API ��� �������� � ��������� ���������"
	});
});
builder.Services.AddSignalR();
builder.Services.AddLogging();
builder.Services.AddSingleton<IDataBaseService, DataBaseService>();

// ��������� CORS ��� ��������� AllowAnyOrigin � AllowCredentials
builder.Services.AddCors(options =>
{
	options.AddPolicy("CorsPolicy", policy =>
	{
		policy.WithOrigins("http://localhost:3000", "http://messagetest-frontend-1")
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

// ������������� CORS �� ��������
app.UseCors("CorsPolicy");
app.UseRouting();
app.UseAuthorization();

app.MapControllers();
app.MapHub<MessageHub>("/messageHub");

app.Run();
