using MessageTest.Dtos;

namespace MessageTest.Services
{
	public interface IDataBaseService
	{
		Task SaveMessageAsync(string text, long order, DateTime timestamp);
		Task<List<MessageDto>> GetMessageAsync(DateTimeOffset from, DateTimeOffset to);
	}
}
