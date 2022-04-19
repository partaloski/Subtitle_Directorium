using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Subtitle_Directorium.Startup))]
namespace Subtitle_Directorium
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
