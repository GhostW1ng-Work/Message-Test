using MessageTest.Models.Entities;

namespace MessageTest.Repositories.Interfaces
{
	public interface IMessageRepository
	{
		public Task SaveMessageAsync(string text, long order, DateTime timestamp);
		public Task<List<Message>> GetMessagesAsync(DateTimeOffset from, DateTimeOffset to);
	}
}
