﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project_GameStore_models.Models
{
    public class Game : IEntity
    {
        public Guid Id { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [StringLength(500)]
        public string? Description { get; set; }
        [JsonIgnore] public virtual Category? Category { get; set; }
        [JsonIgnore] public virtual ICollection<Entity_Game>? Keys_Game { get; set; }
    }
}