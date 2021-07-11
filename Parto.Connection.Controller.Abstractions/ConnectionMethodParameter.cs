using System;

namespace Parto.Connection.Controller.Abstractions
{
    public class ConnectionMethodParameter
    {
        public ConnectionMethodParameter(string name,
            ConnectionMethodParameterType parameterType,
            Type type,
            object? @default)
        {
            Name = name;
            ParameterType = parameterType;
            Type = type;
            Default = @default;
        }

        public string Name { get; }
        public ConnectionMethodParameterType ParameterType { get; }
        public Type Type { get; }
        public object? Default { get; }
    }
}