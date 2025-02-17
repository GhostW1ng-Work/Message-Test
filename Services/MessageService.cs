using MessageTest.Models.Dtos;
using MessageTest.Repositories.Interfaces;
using MessageTest.Services.Interfaces;

namespace MessageTest.Services
{
	public class MessageService : IMessageService
	{
		private readonly ILogger<MessageService> _logger;
		private readonly IMessageRepository _messageRepository;
		private readonly IMessageHubService _messageHubService;

		public MessageService(
			ILogger<MessageService> logger,
			IMessageRepository messageRepository,
			IMessageHubService messageHubService)
		{
			_logger = logger;
			_messageRepository = messageRepository;
			_messageHubService = messageHubService;
		}

		public async Task<List<MessageDto>> GetMessagesAsync(DateTimeOffset from, DateTimeOffset to)
		{
			var result = new List<MessageDto>();
			_logger.LogInformation($"Запрос сообщений от {from} до {to}");

			try
			{
				_logger.LogInformation($"Попытка подключения к базе данных для получения сообщений...");

				var messages = await _messageRepository.GetMessagesAsync( from, to );

				_logger.LogInformation($"Найдено {messages.Count} сообщений в базе данных");

				result = messages.Select(x => 
				new MessageDto()
				{
					Text = x.Text,
					Order = x.Order,
					Timestamp = x.Timestamp
				}).ToList();
			}
			catch(Exception exception) 
			{
				_logger.LogError(exception, "Ошибка получения сообщений из базы данных");
				throw;
			}

			return result;
		}

		public async Task SaveMessageAsync(string text, long order, DateTime timestamp)
		{
			try
			{
				await _messageRepository.SaveMessageAsync(text, order, timestamp);

				_logger.LogInformation($"Сообщение с номером {order} успешно сохранено");
			}
			catch(Exception exception)
			{
				_logger.LogError($"Не удалось сохранить сообщение с номером {order}");
				throw;
			}
		}

		public async Task SendMessageAsync(MessageDto message)
		{
			_logger.LogInformation($"Получено сообщение: {message.Text} с номером {message.Order} и временем {message.Timestamp}");

			try
			{
				await SaveMessageAsync(message.Text, message.Order, DateTime.UtcNow);
				await _messageHubService.SendMessageAsync(message.Text, message.Order, DateTime.UtcNow);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Ошибка при отправке сообщения");
			}
		}
	}
}
