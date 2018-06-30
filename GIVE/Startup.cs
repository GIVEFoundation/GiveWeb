using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GIVE.Startup))]
namespace GIVE
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
