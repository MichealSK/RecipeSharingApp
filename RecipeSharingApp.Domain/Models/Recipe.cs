using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeSharingApp.Domain.Models
{
    public class Recipe : BaseEntity
    {
        public string? Name { get; set; }
        public string? ImageUrl { get; set; }
        public ICollection<string>? Ingredients { get; set; }
        public ICollection<string>? Instructions { get; set; }
        public ICollection<string>? Tags { get; set; }
        public int PrepTime { get; set; }
        public double Rating { get; set; }
        public int Pins { get; set; }
    }
}
