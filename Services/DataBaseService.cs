using MessageTest.Dtos;
using Npgsql;

namespace MessageTest.Services
{
	public class DataBaseService : IDataBaseService
	{
		private const string CONNECTION_STRING_NAME = "PostgresConnection";
		private readonly string _connectionString;
		private readonly ILogger<DataBaseService> _logger;

		public DataBaseService(IConfiguration configuration, ILogger<DataBaseService> logger)
		{
			_connectionString = configuration.GetConnectionString(CONNECTION_STRING_NAME);
			_logger = logger;
		}

		public async Task<List<MessageDto>> GetMessageAsync(DateTimeOffset from, DateTimeOffset to)
		{
			var messages = new List<MessageDto>();
			_logger.LogInformation("Запрос сообщений от {From} до {To}", from, to);

			try
			{
				_logger.LogInformation("Попытка подключения к базе данных для сохранения сообщения...");
				await using var connection = new NpgsqlConnection(_connectionString);
				await connection.OpenAsync();
				_logger.LogInformation("Соединение с базой данных установлено.");

				await using var command = new NpgsqlCommand("SELECT text, order_number, " +
					"timestamp FROM messages WHERE timestamp BETWEEN @from and @to", connection);
				command.Parameters.AddWithValue("from", from);
				command.Parameters.AddWithValue("to", to);
				await using var reader = await command.ExecuteReaderAsync();
				while (await reader.ReadAsync())
				{
					messages.Add(new MessageDto
					{
						Text = reader.GetString(0),
						Order = reader.GetInt64(1),
						Timestamp = reader.GetDateTime(2),
					});
				}
				_logger.LogInformation("Найдено {Count} сообщений в базе данных", messages.Count);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error retrieving messages from database");
				throw;
			}
			return messages;
		}

		public async Task SaveMessageAsync(string text, long order, DateTime timestamp)
		{
			try
			{
				await using var connection = new NpgsqlConnection(_connectionString);
				await connection.OpenAsync();

				await using var command = new NpgsqlCommand(
					"INSERT INTO messages (text, order_number, timestamp) VALUES (@text, @order,@timestamp)", connection);
				command.Parameters.AddWithValue("text", text);
				command.Parameters.AddWithValue("order", order);
				command.Parameters.AddWithValue("timestamp", timestamp);
				await command.ExecuteNonQueryAsync();
				_logger.LogInformation("Сообщение с Номером {Order} успешно сохранено в базе данных", order);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Error saving message to database");
				throw;
			}
		}
	}
}
