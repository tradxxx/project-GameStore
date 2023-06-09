﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project_GameStore_models.Models
{
    public class Client:IEntity
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string Email { get; set; }
        [JsonIgnore]
        [Required]
        public string Password { get; set; }
        [Required]
        [StringLength(50)]
        public string Login { get; set; }
        public Role Role { get; set; }
        public virtual ICollection<Order>? Orders { get; set; }
    }
}
