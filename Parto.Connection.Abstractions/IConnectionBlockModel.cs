using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public interface IConnectionBlockModel : IConnectionBlockBaseModel
    {
        public static readonly JsonSerializerOptions JsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            IgnoreNullValues = true,
            DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
            IncludeFields = false
        };

        public TBody? GetBody<TBody>() => GetBody(typeof(TBody)) is TBody model ? model : default;
        public bool HasBody => GetBody() != null;
        public object? GetBody(Type type);
        public object? GetBody();
        ValueTask SerializeAsync(Stream stream, CancellationToken cancellationToken = default);
    }
}