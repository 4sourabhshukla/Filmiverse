using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Filmiverse.Startup))]
namespace Filmiverse
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
