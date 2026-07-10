using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagement.BLL.ViewModels.PlanViewModels
{
    public class EditedPlanViewModel
    {
        public string Name { get; set; } = default!;

        [StringLength(200, ErrorMessage = "Description can't exceed 200 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, 99999999, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
    }
}
