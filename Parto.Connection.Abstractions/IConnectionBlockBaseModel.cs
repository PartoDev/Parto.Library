using System;
using System.Collections.Generic;

namespace Parto.Connection.Abstractions
{
    public interface IConnectionBlockBaseModel
    {
        string Path { get; set; }
        public Guid? Guid { get; set; }
        Dictionary<string, object> Parameters { get; set; }
    }
}