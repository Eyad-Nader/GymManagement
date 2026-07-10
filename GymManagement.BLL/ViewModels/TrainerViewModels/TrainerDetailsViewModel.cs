using GymManagement.DAL.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.TrainerViewModels
{
    public class TrainerDetailsViewModel : TrainerViewModel
    {
        public string DateOfBirth { get; set; } = default!;

        public string Address { get; set; } = default!;
    }
}
