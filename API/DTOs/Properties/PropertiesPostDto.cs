using System.ComponentModel.DataAnnotations;

namespace API.DTOs.Properties
{
    public class PropertiesCreateDto
    {
        [Required]
        [MinLength(3)]
        public string Name {get; set;}
        [Required]
        [MinLength(3)]
        public string ShortDesc {get; set;}
        [Required]
        [MinLength(3)]
        public string Description {get; set;}
        [Required]
        [MinLength(3)]
        public string Address {get; set;}
        [Required]
        [MinLength(3)]
        public string Country {get; set;}
        [Required]
        [MinLength(3)]
        public string City {get; set;}
        [Required]
        public float Lng {get; set;}
        [Required]
        public float Lat {get; set;}
        [Required]
        public int BusinessId {get; set;}
        public int UserId {get; set;}
        [Required]
        public int PropertyTypeId {get; set;}
        public int TypeMode {get; set;}
        public string DataLocation {get; set;}
    }

    public class PropertiesUpdateDto
    {
        [Required]
        public int Id {get; set;}
        
        [Required]
        [MinLength(3)]
        public string Name {get; set;}
        [Required]
        [MinLength(3)]
        public string ShortDesc {get; set;}
        [Required]
        [MinLength(3)]
        public string Description {get; set;}
        [Required]
        [MinLength(5)]
        public string Address {get; set;}
        [Required]
        [MinLength(3)]
        public string Country {get; set;}
        [Required]
        [MinLength(3)]
        public string City {get; set;}
        [Required]
        public float Lng {get; set;}
        [Required]
        public float Lat {get; set;}
        [Required]
        public int PropertyTypeId {get; set;}
    }
}