using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jedznaplus.Models
{
    public class Recipe
    {
        public int Id { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastEditDate { get; set; }

        public string LastEditorName { get; set; }

        [Required(ErrorMessage = "Pole Nazwa jest wymagane")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa potrawy musi się składać minimum z 2 znaków, nie może też być dłuższa niż 50 znaków")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Display(Name = "Sposób przygotowania")]
        public string PreparationMethod { get; set; }

        [Display(Name = "Zdjęcie")]
        public string ImageUrl { get; set; }

        [Required(ErrorMessage = "Pole Czas przygotowania jest wymagane")]
        [Range(1, 1000, ErrorMessage = "Czas przygotowania musi być z zakresu od 1 do 1000 minut")]
        [Display(Name = "Czas przygotowania")]        
        public int PreparationTime { get; set; }

        [Required(ErrorMessage = "Pole Porcje jest wymagane")]
        [Range(1, 100, ErrorMessage = "Musi być minimum jedna porcja, maksymalnie może być 100 porcji")]
        [Display(Name = "Porcje")]       
        public int Serves { get; set; }

        [Required(ErrorMessage = "Pole Poziom trudności jest wymagane")]
        [Display(Name = "Poziom trdności")]
        public string Difficulty { get; set; }

        [Required(ErrorMessage = "Pole Kalorie jest wymagane")]
        [Range(1, 9000, ErrorMessage = "Musi być powyżej 0 kalorii")]
        [Display(Name = "Kalorie")]        
        public int Calories { get; set; }

        [Display(Name = "Nazwa uzytkownika")]
        public string UserName { get; set; }

        public string Votes { get; set; }

        public virtual List<Comment> Comments { get; set; }

        [Display(Name = "Lista składników")]
        public virtual List<Ingredient> Ingredients { get; set; }

        [Display(Name = "Danie wegetariańskie")]
        public bool Vegetarian { get; set; }

        public Recipe()
        {
            Votes = "";
        }

    }
}