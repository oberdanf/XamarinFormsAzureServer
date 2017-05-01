using System;
using Owin;

namespace XamFormsAzureServer
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
            try
            {
				app.MapSignalR();
            }
            catch (Exception ex)
            {
                app.Run(async ctx => await ctx.Response.WriteAsync(ex.ToString()));
            }
		}
	}
}
