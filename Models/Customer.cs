using MeetingRoomApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MeetingRoomAPI.Models
{
    public class Customer
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
