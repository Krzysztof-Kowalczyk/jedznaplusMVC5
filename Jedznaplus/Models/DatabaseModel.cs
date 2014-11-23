using System.Data.Entity;
using Jedznaplus.Models.ViewModels;

namespace Jedznaplus.Models
{
    public class DatabaseModel : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<VoteLog> VoteLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Ingredient> Ingredient { get; set; }

        public DatabaseModel()
            : base("DefaultConnection")
        {
        }

        public DbSet<NewestRecipesViewModel> NewestRecipesViewModels { get; set; }

         /*  static DatabaseModel()
           {
             Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DatabaseModel>());
           }*/

    }
}