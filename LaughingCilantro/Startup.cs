using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LaughingCilantro.Startup))]
namespace LaughingCilantro
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
