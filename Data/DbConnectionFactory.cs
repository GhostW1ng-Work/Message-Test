using Npgsql;

namespace MessageTest.Data
{
	public class DbConnectionFactory
	{
		private const string CONNECTION_STRING_NAME = "PostgresConnection";
		private readonly string _connectionString;

		public DbConnectionFactory(IConfiguration configuration)
		{
			_connectionString = configuration.GetConnectionString(CONNECTION_STRING_NAME);
		}

		public NpgsqlConnection CreatePostgreSqlConnection()
		{
			return new NpgsqlConnection(_connectionString);
		}
	}
}
