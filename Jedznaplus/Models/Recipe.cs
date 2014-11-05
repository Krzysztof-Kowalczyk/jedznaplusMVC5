using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Jedznaplus.Models
{
    public class Recipe
    {
        
        public long Id { get; set; }
      
        [Required(ErrorMessage="Pole Nazwa jest wymagane")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa potrawy musi się składać minimum z 2 znaków, nie może też być dłuższa niż 50 znaków")]
        [Display(Name = "Nazwa")]    
        public string Name { get; set; }

        [Required(ErrorMessage = "Lista składników jest wymagana")]
        [Display(Name = "Składniki")]
        public string Ingredients { get; set; }

        [Required(ErrorMessage = "Pole Sposób przygotowania jest wymagane")]
        [Display(Name = "Sposób przygotowania")]
        public string PreparationMethod { get; set; }
       
        [Display(Name = "Zdjęcie")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Pole Czas przygotowania jest wymagane")]
        [Display(Name = "Czas przygotowania")]
        [Range(1, 1000, ErrorMessage = "Czas przygotowania musi być z zakresu od 1 do 1000 minut")] 
        public int PreparationTime { get; set; }

        [Required(ErrorMessage = "Pole Porcje jest wymagane")]
        [Display(Name = "Porcje")]
        [Range(1, 100, ErrorMessage = "Musi być minimum jedna porcja, maksymalnie może być 100 porcji")] 
        public int Serves { get; set; }

        [Required(ErrorMessage = "Pole Kalorie jest wymagane")]
        [Display(Name = "Kalorie")]
        [Range(1, 9000, ErrorMessage = "Musi być powyżej 0 kalorii")] 
        public int Calories { get; set; }

        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

    }
}