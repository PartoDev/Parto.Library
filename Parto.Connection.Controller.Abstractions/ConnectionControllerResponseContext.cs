using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionControllerResponseContext : IConnectionControllerResponseContext
    {
        public ConnectionControllerResponseContext(ConnectionBlockModel<object> responseBlock) => Block = responseBlock;

        public ConnectionBlockModel<object> Block { get; }
    }
}