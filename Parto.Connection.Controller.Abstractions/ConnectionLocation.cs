using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;

namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionLocation
    {
        public ConcurrentDictionary<ConnectionStepType, LinkedList<IConnectionStep>?> Steps { get; } = new();
        public Type? ControllerType { get; set; }

        public List<ConnectionMethodParameter> Parameters { get; } = new();
        public MethodInfo? Method { get; set; }
        public Dictionary<string, ConnectionLocation> Locations { get; } = new();
    }
}