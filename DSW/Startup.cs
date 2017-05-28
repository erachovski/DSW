using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(DSW.Startup))]
namespace DSW
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
