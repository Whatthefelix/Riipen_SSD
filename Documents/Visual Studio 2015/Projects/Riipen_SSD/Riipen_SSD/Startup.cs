using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Riipen_SSD.Startup))]
namespace Riipen_SSD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
