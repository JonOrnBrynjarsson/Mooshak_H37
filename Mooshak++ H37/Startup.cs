using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Mooshak___H37.Startup))]
namespace Mooshak___H37
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
