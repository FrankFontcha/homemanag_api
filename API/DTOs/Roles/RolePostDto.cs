
using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Roles
{
    public class CreateRolesDto
    {
        [Required]
        public string Title {get; set;}

        public string Description {get; set;}

        [Required]
        public int BusinessId {get; set;}
    }

    public class UpdateRolesDto
    {
        [Required]
        public int Id {get; set;}

        [Required]
        public string Title {get; set;}

        public string Description {get; set;}
        
        [Required]
        public int BusinessId {get; set;}

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    public class DeleteRolesDto
    {
        [Required]
        public int Id {get; set;}

    }
}