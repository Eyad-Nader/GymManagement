using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Models
{
    public class Member : GymUser
    {
        // public DateTime joindate { get; set; } we have created at

        public string Photo { get; set; } = default!;

        public HealthRecord HealthRecord { get; set; } = default!;

        public ICollection<MemberShip> MemberShips { get; set; } = new List<MemberShip>();

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
