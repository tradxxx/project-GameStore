using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_GameStore_models.Models
{
    public class Entity_Game:IEntity
    {
        public Guid Id { get; set; }
        [Required]
        public virtual Game Game {get; set;}
        public string? Key { get; set;}
        [Required]
        public virtual Platform Platform { get; set;}
    }
}
