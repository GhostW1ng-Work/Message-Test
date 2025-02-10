using Microsoft.AspNetCore.SignalR;

namespace MessageTest
{
	public class MessageHub : Hub
	{
		private readonly ILogger<MessageHub> _logger;

		public MessageHub(ILogger<MessageHub> logger)
		{
			_logger = logger;
		}

		public async Task SendMessage(string text, int order)
		{
			var timestamp = DateTime.UtcNow;
			_logger.LogInformation("Отправка сообщения {Text} с номером {Order} в {Timestamp}", text,order,timestamp);

			try
			{
				await Clients.All.SendAsync("ReceiveMessage", text, timestamp, order);
				_logger.LogInformation("Сообщение успешно отправлено всем клиентам: {Text} с Номером {Order} в {Timestamp}", text, order, timestamp);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при отправке сообщения: {Text} с Номером {Order}", text, order);
			}
		}
	}
}
