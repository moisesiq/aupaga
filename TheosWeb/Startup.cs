using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TheosWeb.Startup))]
namespace TheosWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
