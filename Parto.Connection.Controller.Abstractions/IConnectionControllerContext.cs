using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller.Abstractions
{
    public interface IConnectionControllerContext
    {
        IConnectionContext ConnectionContext { get; }
        IConnectionControllerRequestContext Request { get; }
        IConnectionControllerResponseContext Response { get; }
        ConnectionLocation ConnectionLocation { get; }
    }
}