using MessageTest.Data;
using MessageTest.Models.Dtos;
using MessageTest.Models.Entities;
using MessageTest.Repositories.Interfaces;
using Npgsql;

namespace MessageTest.Repositories
{
	public class MessageRepository : IMessageRepository
	{
		private readonly DbConnectionFactory _connectionFactory;

		public MessageRepository(DbConnectionFactory connectionFactory)
		{
			_connectionFactory = connectionFactory;
		}

		public async Task<List<Message>> GetMessagesAsync(DateTimeOffset from, DateTimeOffset to)
		{
			var messages = new List<Message>();

			using var connection = _connectionFactory.CreatePostgreSqlConnection();
			var query = "SELECT text, order_number, " +
				"timestamp FROM messages WHERE timestamp BETWEEN @from and @to";

			await using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("from", from);
			command.Parameters.AddWithValue("to", to);

			await connection.OpenAsync();

			await using var reader = await command.ExecuteReaderAsync();
			while (await reader.ReadAsync())
			{
				messages.Add(new Message
				{
					Text = reader.GetString(0),
					Order = reader.GetInt64(1),
					Timestamp = reader.GetDateTime(2),
				});
			}

			return messages;
		}

		public async Task SaveMessageAsync(string text, long order, DateTime timestamp)
		{
			using var connection = _connectionFactory.CreatePostgreSqlConnection();
			var query = "INSERT INTO messages (text, order_number, timestamp) VALUES (@text, @order,@timestamp)";

			using var command = new NpgsqlCommand(query, connection);
			command.Parameters.AddWithValue("text", text);
			command.Parameters.AddWithValue("order", order);
			command.Parameters.AddWithValue("timestamp", timestamp);

			await connection.OpenAsync();
			await command.ExecuteNonQueryAsync();
		}
	}
}