using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller.Abstractions
{
    public interface IConnectionControllerResponseContext
    {
        ConnectionBlockModel<object> Block { get; }
    }
}