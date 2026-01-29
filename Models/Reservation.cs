using MeetingRoomAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace MeetingRoomApi.Models;

public class Reservation
{
    [JsonPropertyName("id")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    [BindNever]
    [SwaggerSchema(ReadOnly = true)]
    public Guid Id { get; set; } = Guid.NewGuid();

    public Guid MeetingRoomId { get; set; }

    [JsonIgnore]
    public MeetingRoom? MeetingRoom { get; set; }

    public DateTime StartUtc { get; set; }
    public DateTime EndUtc { get; set; }

    [JsonIgnore]
    public Guid CustomerId { get; set; }

    [JsonIgnore]
    public Customer? Customer { get; set; }
}