using System.ComponentModel.DataAnnotations;

namespace Jedznaplus.Models
{
    public class Ingredient
    {
        public int Id { get; set; }

        [Display(Name = "Nazwa")]
        [Required]
        public string Name { get; set; }

        [Display(Name = "Jednostka")]
        [Required]
        public string UnitName { get; set; }

        [Display(Name = "Ilość")]
        [Required]
        [Range(0.001, double.MaxValue, ErrorMessage = "Ilość musi być więlsza od 0")]
        public double Quantity { get; set; }
    }
}