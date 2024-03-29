﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Jedznaplus.Models
{
    public class ExcludeRecipe
    {
        [Display(Name = "Bez składników alergicznych")]
        public bool NotAlergic { get; set; }
        [Display(Name = "Wegetariańskie")]
        public bool Vegetarians { get; set; }
        [Display(Name = "Lista wykluczonych składników")]
        public List<string> ExcludeIngredients { get; set; }
        [Display(Name = "Lista pożądanych składników")]
        public List<string> WantedIngredients { get; set; }
        [Display(Name = "Maksymalny czas przygotowywania")]
        public int MaxTime { get; set; }
    }
}