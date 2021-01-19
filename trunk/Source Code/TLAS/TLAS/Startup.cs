using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(TLAS.Startup))]
namespace TLAS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
