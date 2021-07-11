using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionControllerContext : IConnectionControllerContext
    {
        public ConnectionControllerContext(IConnectionContext connectionContext,
            IConnectionControllerRequestContext requestContext,
            IConnectionControllerResponseContext responseContext,
            ConnectionLocation connectionLocation)
        {
            ConnectionContext = connectionContext;
            Request = requestContext;
            Response = responseContext;
            ConnectionLocation = connectionLocation;
        }

        public IConnectionContext ConnectionContext { get; }
        public IConnectionControllerRequestContext Request { get; }
        public IConnectionControllerResponseContext Response { get; }
        public ConnectionLocation ConnectionLocation { get; }
    }
}