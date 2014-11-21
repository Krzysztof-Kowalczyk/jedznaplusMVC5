using System.Data.Entity;

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

        public System.Data.Entity.DbSet<Jedznaplus.Models.NewestRecipesViewModel> NewestRecipesViewModels { get; set; }

         /*  static DatabaseModel()
           {
             Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DatabaseModel>());
           }*/

    }
}