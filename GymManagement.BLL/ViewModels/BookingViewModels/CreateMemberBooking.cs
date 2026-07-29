using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace GymManagement.BLL.ViewModels.BookingViewModels
{
    public class CreateMemberBooking
    {
        [Required(ErrorMessage = "Member is required")]
        public int? Id { get; set; }
        public int SessionId { get; set; }
    }
}
