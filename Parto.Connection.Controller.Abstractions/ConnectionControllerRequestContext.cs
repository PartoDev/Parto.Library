using System.Collections.Generic;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionControllerRequestContext : IConnectionControllerRequestContext
    {
        public ConnectionControllerRequestContext(IConnectionBlockModel requestBlock) => Block = requestBlock;

        public IConnectionBlockModel Block { get; }
        public List<object?> MethodParameters { get; } = new();
    }
}