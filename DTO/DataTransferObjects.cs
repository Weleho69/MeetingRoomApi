using MeetingRoomApi.Models;
using MeetingRoomAPI.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MeetingRoomAPI.DTO
{
   
    public class ReservationResponse
    {
        [JsonPropertyName("id")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        [BindNever]
        [SwaggerSchema(ReadOnly = true)]
        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public Guid MeetingRoomId { get; set; }

        [Required, EmailAddress]
        public string CustomerEmail { get; set; } = string.Empty;

        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
    }

    public class ReservationWithCustomer
    {
        public Guid Id { get; set; }
        public DateTime StartUtc { get; set; }
        public DateTime EndUtc { get; set; }
        public CustomerInfo Customer { get; set; } = new CustomerInfo();
        public RoomInfo Room { get; set; } = new RoomInfo();
    }

    public class CustomerInfo
    {
        public string email { get; set; } = string.Empty;
        public string name { get; set; } = string.Empty;
        public string? phone { get; set; }
    }

    public class RoomInfo
    {
        public Guid id { get; set; }
        public string name { get; set; } = string.Empty;
        public int capacity { get; set; }
    }
}
