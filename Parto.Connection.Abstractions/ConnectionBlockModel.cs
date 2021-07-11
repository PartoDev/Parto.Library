using System;
using System.Buffers;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Parto.Connection.Abstractions
{
    public class ConnectionBlockModel : ConnectionBlockBaseModel, IConnectionBlockModel
    {
        public ConnectionBlockModel(string path) : this(path, default)
        {
        }

        public ConnectionBlockModel(string path, JsonElement? body = default) : this(path, default, body)
        {
        }

        [JsonConstructor]
        public ConnectionBlockModel(string path, Guid? guid = default, JsonElement? body = default) :
            base(path, guid) => Body = body;

        public JsonElement? Body { get; set; }
        public object? GetBody() => Body;

        public object? GetBody(Type type)
        {
            if (Body == null) return default;

            ArrayBufferWriter<byte> bufferWriter = new();
            using (Utf8JsonWriter writer = new(bufferWriter)) Body?.WriteTo(writer);

            return JsonSerializer.Deserialize(bufferWriter.WrittenSpan,
                type,
                IConnectionBlockModel.JsonSerializerOptions);
        }

        public async ValueTask SerializeAsync(Stream stream, CancellationToken cancellationToken = default) =>
            await JsonSerializer
                .SerializeAsync(stream, this, IConnectionBlockModel.JsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);

        public static async ValueTask<ConnectionBlockModel?> DeserializeAsync(Stream stream,
            CancellationToken cancellationToken = default, bool exception = false)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<ConnectionBlockModel>(stream,
                    IConnectionBlockModel.JsonSerializerOptions,
                    cancellationToken);
            }
            catch
            {
                if (exception)
                    throw;
                return null;
            }
        }
    }

    public class ConnectionBlockModel<TBody> : ConnectionBlockBaseModel, IConnectionBlockModel
    {
        public ConnectionBlockModel(string path) : this(path, default)
        {
        }

        public ConnectionBlockModel(string path, TBody? body = default) : this(path, default, body)
        {
        }

        [JsonConstructor]
        public ConnectionBlockModel(string path, Guid? guid = default, TBody? body = default) : base(path, guid) =>
            Body = body;

        public TBody? Body { get; set; }
        public object? GetBody(Type type) => typeof(TBody) == type ? Body : default;

        public object? GetBody() => Body;

        public async ValueTask SerializeAsync(Stream stream, CancellationToken cancellationToken = default) =>
            await JsonSerializer
                .SerializeAsync(stream, this, IConnectionBlockModel.JsonSerializerOptions, cancellationToken)
                .ConfigureAwait(false);

        public static async ValueTask<ConnectionBlockModel<TBody>?> DeserializeAsync(Stream stream,
            CancellationToken cancellationToken = default, bool exception = false)
        {
            try
            {
                return await JsonSerializer.DeserializeAsync<ConnectionBlockModel<TBody>>(stream,
                    IConnectionBlockModel.JsonSerializerOptions,
                    cancellationToken);
            }
            catch
            {
                if (exception)
                    throw;
                return null;
            }
        }
    }
}