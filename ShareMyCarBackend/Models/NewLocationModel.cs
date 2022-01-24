using System.ComponentModel.DataAnnotations;

namespace ShareMyCarBackend.Models
{
    public class NewLocationModel
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Address { get; set; }
        [Required]
        public string ZipCode { get; set; }
        [Required]
        public string City { get; set; }

    }
}
