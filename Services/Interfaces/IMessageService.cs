using MessageTest.Models.Dtos;

namespace MessageTest.Services.Interfaces
{
	public interface IMessageService
	{
		public Task SendMessageAsync(MessageDto message);
		public Task SaveMessageAsync(string text, long order, DateTime timestamp);
		public Task<List<MessageDto>> GetMessagesAsync(DateTimeOffset from, DateTimeOffset to);
	}
}
