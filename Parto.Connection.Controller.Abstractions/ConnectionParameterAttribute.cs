using System;

namespace Parto.Connection.Controller.Abstractions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class ConnectionParameterAttribute : Attribute
    {
        public ConnectionParameterAttribute(string? name) => Name = name;

        public string? Name { get; }
    }
}