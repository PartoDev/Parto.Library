using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Parto.Connection.Controller.Abstractions;

namespace Parto.Connection.Server.Test
{
    [ConnectionPath("test")]
    public class TestController : ConnectionController
    {
        private readonly ILogger<TestController> logger;

        public TestController(ILogger<TestController> logger) => this.logger = logger;

        [ConnectionPath("is")]
        public ValueTask<bool> Test1Async(int i, CancellationToken cancellationToken) =>
            ValueTask.FromResult(i % 2 == 0);
        [ConnectionPath("nis")]
        public ValueTask<bool> Test2Async(CancellationToken cancellationToken) =>
            ValueTask.FromResult(true);

        [ConnectionPath("error")]
        public void Error() => logger.LogError("error");
    }
}