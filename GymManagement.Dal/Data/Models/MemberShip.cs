using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Models
{
    public class MemberShip : BaseEntity
    {
        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;
        public int PlanId { get; set; }
        public Plan Plan { get; set; } = default!;

        public DateTime EndDate { get; set; }

        public string Status  => EndDate > DateTime.Now  ? "Active" : "Expired";

        public bool IsActive => EndDate > DateTime.Now;

    }
}
