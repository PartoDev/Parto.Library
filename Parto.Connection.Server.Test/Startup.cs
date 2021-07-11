using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Parto.Connection.Abstractions;
using Parto.Connection.Controller;
using Parto.Connection.Manager;

namespace Parto.Connection.Server.Test
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddConnectionManager();
            services.AddConnectionController(options =>
            {
                options.ReceiveBlockNullErrorPath = "test/error";
                options.ReceiveControllerNullErrorPath = "test/error";
                options.ReceiveLocationNUllErrorPath = "test/error";
            });
            services.AddConnectionServer(ConfigureConnection);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseWebSockets();
            app.UseConnectionWebSocketListener("/test");

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("/", async context => { await context.Response.WriteAsync("Hello World!"); });
            });
        }

        public void ConfigureConnection(IConnectionMiddlewareBuilder builder)
        {
            builder.UseConnectionManager();
            builder.UseEndPointConnectionController();
            
        }
    }
}