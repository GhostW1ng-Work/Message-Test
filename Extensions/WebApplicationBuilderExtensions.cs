using MessageTest.Data;
using MessageTest.Repositories;
using MessageTest.Repositories.Interfaces;
using MessageTest.Services;
using MessageTest.Services.Interfaces;
using Microsoft.OpenApi.Models;
using Npgsql;

namespace MessageTest.Extensions
{
	public static class WebApplicationBuilderExtensions
	{
		public static WebApplicationBuilder AddSwaggerConfiguration(this WebApplicationBuilder builder)
		{
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo
				{
					Title = "Message API",
					Version = "v1",
					Description = "API для отправки и получения сообщений"
				});
			});

			return builder;
		}

		public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
		{
			builder.Services.AddSingleton(new DbConnectionFactory(builder.Configuration));

			builder.Services.AddScoped<IMessageService, MessageService>();
			builder.Services.AddScoped<IMessageHubService, MessageHubService>();
			builder.Services.AddScoped<IMessageRepository, MessageRepository>();

			return builder;
		}

		public static WebApplicationBuilder AddCorsConfiguration(this WebApplicationBuilder builder)
		{
			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAll",
					policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
			});


			return builder;
		}
	}
}
