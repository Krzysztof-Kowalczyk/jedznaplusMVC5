using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Jedznaplus.Startup))]
namespace Jedznaplus
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
