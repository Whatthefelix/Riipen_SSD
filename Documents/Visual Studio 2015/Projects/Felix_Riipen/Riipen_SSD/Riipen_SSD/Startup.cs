using Microsoft.Owin;
using Owin;
using Riipen_SSD.App_Start;

[assembly: OwinStartupAttribute(typeof(Riipen_SSD.Startup))]
namespace Riipen_SSD
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var container = SimpleInjectorInitializer.Initialize(app);
            ConfigureAuth(app, container);
        }
    }
}
