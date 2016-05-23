using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WeBind.Startup))]
namespace WeBind
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
