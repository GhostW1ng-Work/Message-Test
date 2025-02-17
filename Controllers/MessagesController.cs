using MessageTest.Attributes;
using MessageTest.Models.Dtos;
using MessageTest.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace MessageTest.Controllers
{
	[Route("api/messages")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly IMessageService _messageService;
		private readonly ILogger<MessagesController> _logger;

		public MessagesController(IMessageService messageService, ILogger<MessagesController> logger)
		{
			_messageService = messageService;
			_logger = logger;
		}

		[HttpPost("[action]")]
		[ValidateModel()]
		public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
		{
			_logger.LogInformation($"Получен запрос SendMessage: {message.Text}\n{message.Order}\n{message.Timestamp}");
			try
			{
				await _messageService.SendMessageAsync(message);
				return Ok();
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, "Ошибка при отправке сообщения");
				return StatusCode(500, "Ошибка сервера");
			}
		}

		[HttpGet("[action]")]
		[ValidateModel]
		public async Task<IActionResult> GetHistory([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
		{
			try
			{
				var messages = await _messageService.GetMessagesAsync(from, to);
				return Ok(messages);
			}
			catch (Exception exception)
			{
				_logger.LogError(exception, $"Ошибка получения сообщений от {from} до {to}");
				return StatusCode(500, "Ошибка сервера");
			} 
		}
	}
}
