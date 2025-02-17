namespace MessageTest.Services.Interfaces
{
	public interface IMessageHubService
	{
		public Task SendMessageAsync(string text, long order, DateTime timestamp);
	}
}
