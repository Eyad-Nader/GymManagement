using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.ViewModels.MemberShipViewModels
{
    public class CreateMemberShipViewModel
    {
        [Required]
        public int MemberId { get; set; }

        [Required]
        public int PlanId { get; set; }
    }
}
