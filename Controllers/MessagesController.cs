using MessageTest.Dtos;
using MessageTest.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Npgsql;
using System.Text.Json;

namespace MessageTest.Controllers
{
	[Route("api/messages")]
	[ApiController]
	public class MessagesController : ControllerBase
	{
		private readonly IDataBaseService _dataBaseService;
		private readonly ILogger<MessagesController> _logger;
		private readonly IHubContext<MessageHub> _hubContext;
		public MessagesController(IDataBaseService dataBaseService, 
			ILogger<MessagesController> logger,
			IHubContext<MessageHub> hubContext)
		{
			_dataBaseService = dataBaseService;
			_logger = logger;
			_hubContext = hubContext;
		}

		[HttpPost("send")]
		public async Task<IActionResult> SendMessage([FromBody] MessageDto message)
		{
			_logger.LogInformation("Отправка сообщения: {Message}", JsonSerializer.Serialize(message));

			if (!ModelState.IsValid)
			{
				var errors = string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage));
				_logger.LogError("Некорректные данные: {Errors}", errors);
				return BadRequest(ModelState);
			}

			_logger.LogInformation("Получено сообщение: {Text} с Номером {Order} и временем {Timestamp}",
				message.Text, message.Order, message.Timestamp);

			try
			{
				await _dataBaseService.SaveMessageAsync(message.Text, message.Order, DateTime.UtcNow);
				_logger.LogInformation("Сообщение успешно сохранено: {Message}", message.Text);

				await _hubContext.Clients.All.SendAsync("ReceiveMessage", message.Text, message.Order, DateTime.UtcNow);
				return Ok();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при сохранении сообщения");
				return StatusCode(500, "Ошибка сервера");
			}
		}

		[HttpGet("history")]
		public async Task<IActionResult> GetHistory([FromQuery] DateTimeOffset from, [FromQuery] DateTimeOffset to)
		{
			_logger.LogInformation("Запрос на получение сообщений от {From} до {To}", from, to);

			try
			{
				var messages = await _dataBaseService.GetMessageAsync(from, to);
				_logger.LogInformation("Найдено {Count} сообщений", messages.Count);
				return Ok(messages);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Ошибка при получении сообщений от {From} до {To}", from, to);
				return StatusCode(500, "Ошибка сервера");
			}
		}
	}
}
