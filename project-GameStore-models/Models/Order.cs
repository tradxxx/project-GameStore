using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_GameStore_models.Models
{
    public class Order
    {
        public Guid Id { get; set; }
        [Required]
        public virtual Client Client { get; set; }
        public DateTime Order_date { get; set; }
        public virtual ICollection<Game>? Games { get; set; }
    }
}
