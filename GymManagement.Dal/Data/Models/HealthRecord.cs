using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Models
{
    public class HealthRecord :BaseEntity
    {
        public decimal Weight { get; set; } = default!;  
        public decimal Height { get; set; } = default!;
        public string? Note { get; set; } 
        public string Bloodtype { get; set; } = default!;

        // lastupdated = updatedat

        public int MemberId { get; set; }
        public Member Member { get; set; } = default!;



    }
}
