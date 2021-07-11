using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Parto.Connection.Abstractions
{
    public class ConnectionBlockBaseModel : IConnectionBlockBaseModel
    {
        public ConnectionBlockBaseModel(string path) => Path = path;

        [JsonConstructor]
        public ConnectionBlockBaseModel(string path, Guid? guid)
        {
            Path = path;
            Guid = guid;
        }

        public string Path { get; set; }
        public Guid? Guid { get; set; }
        public Dictionary<string, object> Parameters { get; set; } = new();
    }
}