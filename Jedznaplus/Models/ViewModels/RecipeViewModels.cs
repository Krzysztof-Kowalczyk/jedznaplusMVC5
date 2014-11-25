using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Jedznaplus.Models.ViewModels
{
    public class RecipeEditViewModels
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Pole Nazwa jest wymagane")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa potrawy musi się składać minimum z 2 znaków, nie może też być dłuższa niż 50 znaków")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

        [Required]
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

        [Required(ErrorMessage = "Pole Poziom trudności jest wymagane")]
        [Display(Name = "Poziom trdności")]
        public string Difficulty { get; set; }

        [Required(ErrorMessage = "Pole Kalorie jest wymagane")]
        [Display(Name = "Kalorie")]
        [Range(1, 9000, ErrorMessage = "Musi być powyżej 0 kalorii")]
        public int Calories { get; set; }

        [Display(Name = "Lista składników")]
        [Required]
        public virtual List<Ingredient> Ingredients { get; set; }

        [Display(Name = "Danie wegetariańskie")]
        public bool Vegetarian { get; set; }

        public SelectList UnitNameList { get; set; }
        public SelectList Difficulties { get; set; }

        public RecipeEditViewModels()
        {
            UnitNameList = new SelectList(new[] { "litr", "mililitr", "kilogram", "dekagram", "gram", "sztuka", "plaster", "opakowanie", "łyżka", "łyżeczka", "szklanka", "szczypta" });
            Difficulties = new SelectList(new[] { "Łatwy", "Średni", "Trudny", "Bardzo Trudny" });
        }


    }

    public class CreateRecipeFirstPhaseViewModel
    {
        [Required(ErrorMessage = "Pole Nazwa jest wymagane")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwa potrawy musi się składać minimum z 2 znaków, nie może też być dłuższa niż 50 znaków")]
        [Display(Name = "Nazwa")]
        public string Name { get; set; }

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

        [Required(ErrorMessage = "Pole Poziom trudności jest wymagane")]
        [Display(Name = "Poziom trdności")]
        public string Difficulty { get; set; }

        [Required(ErrorMessage = "Pole Kalorie jest wymagane")]
        [Display(Name = "Kalorie")]
        [Range(1, 9000, ErrorMessage = "Musi być powyżej 0 kalorii")]
        public int Calories { get; set; }

        [Display(Name = "Danie wegetariańskie")]
        public bool Vegetarian { get; set; }

        public SelectList Difficulties { get; set; }  
    }

    public class CreateRecipeSecondPhaseViewModel:CreateRecipeFirstPhaseViewModel
    {
        [Display(Name = "Sposób przygotowania")]
        public string PreparationMethod { get; set; }
        
        [Display(Name = "Lista składników")]
        [Required]
        
        public virtual List<Ingredient> Ingredients { get; set; }
        
        public SelectList UnitNameList { get; set; }
        public CreateRecipeSecondPhaseViewModel() :base()
        {
            UnitNameList = new SelectList(new[] { "litr", "mililitr", "kilogram", "dekagram", "gram", "sztuka", "plaster", "opakowanie", "łyżka", "łyżeczka", "szklanka", "szczypta" });
        }
    }

    public class NewestRecipesViewModel
    {
        public int Id { get; set; }
        public string Name { get; set;}
        public string ImageUrl { get; set;}

    }

    public class BestRatedRecipesViewModel : NewestRecipesViewModel
    {

    }




}