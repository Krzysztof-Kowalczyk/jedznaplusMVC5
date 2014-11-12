using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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

    }
}