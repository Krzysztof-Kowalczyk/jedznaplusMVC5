using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Jedznaplus.Models
{
    public class DatabaseModel : DbContext
    {
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<VoteLog> VoteLogs { get; set; }
        public DbSet<Comment> Comments { get; set; }

        public DatabaseModel()
            : base("DefaultConnection")
        {
        }

           static DatabaseModel()
           {
             Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DatabaseModel>());
           }

    }
}