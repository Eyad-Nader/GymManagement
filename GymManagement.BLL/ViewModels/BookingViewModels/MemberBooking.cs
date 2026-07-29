using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.BookingViewModels
{
    public class MemberBooking
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;

        public int SessionId { get; set; }
        public DateTime? BookingDate { get; set; }
    }
}
