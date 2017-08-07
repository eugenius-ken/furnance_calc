using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FurnanceCalculator.Startup))]
namespace FurnanceCalculator
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
