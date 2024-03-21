using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(humber_http_5226_collaborative_project.Startup))]
namespace humber_http_5226_collaborative_project
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
