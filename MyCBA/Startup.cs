using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyCBA.Startup))]
namespace MyCBA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
