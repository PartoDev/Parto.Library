using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Parto.Helper.Extensions
{
    public static class HttpClientExtension
    {
        public static async ValueTask<HttpResponseMessage> RequestAsync(
            this HttpClient httpClient,
            Uri uri,
            HttpMethod method,
            Action<HttpRequestMessage>? options = null) => await httpClient.RequestAsync(x =>
        {
            x.RequestUri = uri;
            x.Method = method;
            options?.Invoke(x);
        });

        // ReSharper disable once MemberCanBePrivate.Global
        public static async ValueTask<HttpResponseMessage> RequestAsync(
            this HttpClient httpClient,
            string uri,
            HttpMethod method,
            Action<HttpRequestMessage>? options = null) => await httpClient.RequestAsync(x =>
        {
            x.RequestUri = new(uri);
            x.Method = method;
            options?.Invoke(x);
        });

        public static async ValueTask<HttpResponseMessage> RequestAsync(
            this HttpClient httpClient,
            Uri uri,
            Action<HttpRequestMessage>? options = null) => await httpClient.RequestAsync(x =>
        {
            x.RequestUri = uri;
            x.Method = HttpMethod.Get;
            options?.Invoke(x);
        });

        // ReSharper disable once MemberCanBePrivate.Global
        public static async ValueTask<HttpResponseMessage> RequestAsync(
            this HttpClient httpClient,
            string uri,
            Action<HttpRequestMessage>? options = null) => await httpClient.RequestAsync(x =>
        {
            x.RequestUri = new(uri);
            x.Method = HttpMethod.Get;
            options?.Invoke(x);
        });

        // ReSharper disable once MemberCanBePrivate.Global
        public static async ValueTask<HttpResponseMessage> RequestAsync(
            this HttpClient httpClient,
            Action<HttpRequestMessage>? options = null)
        {
            using HttpRequestMessage request = new();
            options?.Invoke(request);
            return await httpClient.SendAsync(request)
                .ConfigureAwait(false);
        }

        public static async ValueTask SetJsonModelAsync<TModel>(this HttpRequestMessage httpRequestMessage,
            TModel model,
            JsonSerializerOptions? jsonSerializerOptions = null)
        {
            await using MemoryStream memory = new();
            await JsonSerializer.SerializeAsync(memory, model, jsonSerializerOptions)
                .ConfigureAwait(false);
            httpRequestMessage.Content = new StreamContent(memory);
            httpRequestMessage.Content.Headers.ContentType = new("application/json");
        }

        public static async ValueTask<TModel?> GetJsonModelAsync<TModel>(this HttpContent httpContent,
            JsonSerializerOptions? jsonSerializerOptions = null)
        {
            await using Stream stream = await httpContent.ReadAsStreamAsync()
                .ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<TModel>(stream, jsonSerializerOptions)
                .ConfigureAwait(false);
        }
    }
}