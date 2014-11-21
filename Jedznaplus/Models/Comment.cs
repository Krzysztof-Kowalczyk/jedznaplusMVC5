using System;
using System.ComponentModel.DataAnnotations;

namespace Jedznaplus.Models
{
    public class Comment
    {
        public int Id {get; set; }
        public int RecipeId{get;set;}
        [Display(Name = "Nazwa użytkownika")]
        public string UserName{get;set;}
        [Display(Name = "Treść")]
        public string Content{get;set;}
        public DateTime CreateDate { get; set; }

    }
}