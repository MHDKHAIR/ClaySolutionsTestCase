using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class FloorEntity : BaseEntity<int>
    {
        public int Index { get; set; }

        [ForeignKey(nameof(Building))]
        public int BuildingId { get; set; }
        public BuildingEntity Building { get; set; }

    }
}
