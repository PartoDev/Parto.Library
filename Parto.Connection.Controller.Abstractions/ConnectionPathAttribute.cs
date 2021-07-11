using System;

namespace Parto.Connection.Controller.Abstractions
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false)]
    public class ConnectionPathAttribute : Attribute
    {
        public ConnectionPathAttribute(string path) => Locations = path.Split("/");

        public string[] Locations { get; }
    }
}