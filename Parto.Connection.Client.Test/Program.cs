using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Parto.Connection.Abstractions;
using Parto.Connection.Controller;
using Parto.Connection.Manager;

namespace Parto.Connection.Client.Test
{
    internal class Program
    {
        private static async Task Main()
        {
            ServiceCollection serviceDescriptors = new();
            Configure(serviceDescriptors);
            var serviceProvider = serviceDescriptors.BuildServiceProvider();
            var client = serviceProvider.GetRequiredService<IConnectionClient>();
            await client.ConnectToWebSocketAsync(new("ws://localhost:5000/test")).ConfigureAwait(false);
        }

        private static void Configure(IServiceCollection services)
        {
            services.AddConnectionManager();
            services.AddConnectionController(options =>
            {
                options.ReceiveBlockNullErrorPath = "/test/error";
                options.ReceiveControllerNullErrorPath = "/test/error";
                options.ReceiveLocationNUllErrorPath = "/test/error";
            });
            services.AddConnectionClient(ConfigureConnection);
        }

        private static void ConfigureConnection(IConnectionMiddlewareBuilder builder)
        {
            builder.UseConnectionManager();
            object? block = null;
            builder.Use(next => async (context, cancellationToken) =>
            {
                var res = await context.TransferBlockAsync(new ConnectionBlockModel("test/nis"), cancellationToken)
                    .ConfigureAwait(false);
                block = res;
                Console.WriteLine(res?.GetBody() ?? "res is null");
                await next(context, cancellationToken);
            });
            builder.UseEndPointConnectionController();
        }
    }
}