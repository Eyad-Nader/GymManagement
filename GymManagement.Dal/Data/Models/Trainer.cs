using GymManagement.DAL.Data.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.DAL.Data.Models
{
    public class Trainer : GymUser
    {
        public Specialization Specialization { get; set; } = default!;
        
        public ICollection<Session> Sessions { get; set; } = new List<Session>();
    }
}
