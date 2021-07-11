namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionController : IConnectionController
    {
        public IConnectionControllerContext Context { get; set; } = default!;
    }
}