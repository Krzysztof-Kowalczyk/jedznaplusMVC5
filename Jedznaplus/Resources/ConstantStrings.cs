using System.Web.Mvc;

namespace Jedznaplus.Resources
{
    public static class ConstantStrings
    {
        public const string DefaultUserAvatar = "~/Resources/Images/Users/defaultavatar.png";
        public const string DefaultRecipePhoto = "~/Resources/Images/Recipes/noPhoto.png";
        public const string RecipePhotosPath = "~/Resources/Images/Recipes/";
        public const string UserAvatarsPath = "~/Resources/Images/Users/";
        public const string LogsPath = "~/Resources/Logs/";

        public static SelectList UnitNameList = new SelectList(new[]
                {
                    "sztuka", "gram", "dekagram", "kilogram", "mililitr", "litr", "opakowanie","kostka", "plaster", "szklanka",
                    "łyżka", "łyżeczka","ząbek", "szczypta"
                });

        public static SelectList Difficulties = new SelectList(new[] { "Łatwy", "Średni", "Trudny", "Bardzo Trudny" });
    }
}