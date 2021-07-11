using System.Collections.Generic;
using Parto.Connection.Abstractions;

namespace Parto.Connection.Controller.Abstractions
{
    public interface IConnectionControllerRequestContext
    {
        IConnectionBlockModel Block { get; }
        List<object?> MethodParameters { get; }
    }
}