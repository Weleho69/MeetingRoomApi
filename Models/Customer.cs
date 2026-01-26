using MeetingRoomApi.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MeetingRoomAPI.Models
{
    public class Customer
    {

        [Key]
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        public string Name { get; set; }

    }
}
