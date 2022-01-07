using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class BuildingEntity : BaseEntity<int>
    {
        [Required]
        public string Name { get; set; }
        public string Address { get; set; }
        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
    }
}
