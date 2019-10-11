using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DH_Blog.Startup))]
namespace DH_Blog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
