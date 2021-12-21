using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class DoorLockEntity : BaseEntity<Guid>
    {
        [Required]
        public string DoorName { get; set; }

        /// <summary>
        /// 8 characters code
        /// </summary>
        [Required]
        public string DoorKeyCode { get; set; }

        [ForeignKey(nameof(Floor))]
        public int FloorId { get; set; }
        public FloorEntity Floor { get; set; }

        [Required]
        public double Longitude { get; set; }
        [Required]
        public double Latitude { get; set; }
    }
}
