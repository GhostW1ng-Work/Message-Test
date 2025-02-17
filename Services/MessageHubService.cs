using MessageTest.Hubs;
using MessageTest.Services.Interfaces;
using Microsoft.AspNetCore.SignalR;

namespace MessageTest.Services
{
	public class MessageHubService : IMessageHubService
	{
		private readonly IHubContext<MessageHub> _hubContext;

		public MessageHubService(IHubContext<MessageHub> hubContext)
		{
			_hubContext = hubContext;
		}

		public async Task SendMessageAsync(string text, long order, DateTime timestamp)
		{
			await _hubContext.Clients.All.SendAsync("ReceiveMessage", text, order, timestamp);
		}
	}
}
