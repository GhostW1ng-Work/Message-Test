using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MessageTest.Models.Dtos
{
	public class MessageDto
	{
		[Required]
		public string Text { get; set; }
		[Required]
		[JsonPropertyName("order_number")]
		public long Order { get; set; }
		[Required]
		[JsonPropertyName("timestamp")]
		public DateTime Timestamp { get; set; }
	}
}
