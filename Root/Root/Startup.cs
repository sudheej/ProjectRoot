using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Root.Startup))]
namespace Root
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
