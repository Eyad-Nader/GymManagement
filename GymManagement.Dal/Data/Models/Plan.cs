using System.Data;

namespace GymManagement.DAL.Data.Models
{
    public class Plan : BaseEntity
    {

        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int Duration { get; set; }
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = false;

        public ICollection<MemberShip> MemberShips { get; set; } = new List<MemberShip>();

    }
}
