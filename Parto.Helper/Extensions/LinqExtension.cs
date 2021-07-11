using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Parto.Helper.Extensions
{
    public static class LinqExtension
    {
        public static void Set<TKey, TValue>(this IDictionary<TKey, TValue> keyValuePairs, TKey key, TValue value)
        {
            if (!keyValuePairs.TryAdd(key, value))
                keyValuePairs[key ?? throw new ArgumentNullException(nameof(key))] = value;
        }

        public static bool TryGet<TType>(
            this IEnumerable<TType> types,
            Func<TType, bool> func,
            out TType? dataStoreRecord)
        {
            dataStoreRecord = default;
            if (types.FirstOrDefault(func) is not { } data)
                return false;

            dataStoreRecord = data;
            return true;
        }

        public static void Set<TType>(
            this IList<TType> types,
            Func<TType, bool> func,
            TType type)
        {
            for (var index = 0; index < types.Count; index++)
            {
                var get = types[index];
                if (!func(get))
                    continue;

                types[index] = type;
                break;
            }
        }

        public static bool Edit<TType>(
            this IEnumerable<TType> types,
            Func<TType, bool> func,
            Action<TType?> action)
        {
            if (types.TryGet(func, out var dataStoreRecord) is { } result)
                action.Invoke(dataStoreRecord);

            return result;
        }

        public static void AddOrEdit<TType>(
            this ICollection<TType> types,
            Func<TType, bool> func,
            Action<TType?> update,
            TType add)
        {
            if (!types.Edit(func, update))
                types.Add(add);
        }

        public static void AddOrEdit<TType>(
            this ICollection<TType> types,
            Func<TType, bool> func,
            Action<TType?> update)
            where TType : new()
        {
            if (types.Edit(func, update))
                return;

            TType add = new();
            update(add);
            types.Add(add);
        }

        public static async ValueTask<IEnumerable> AsEnumerable<TType>(
            this IAsyncEnumerable<TType> asyncEnumerable)
        {
            ConcurrentQueue<TType> list = new();
            await foreach (var type in asyncEnumerable)
                list.Enqueue(type);

            return list;
        }

        public static async IAsyncEnumerable<TModel> SelectAsync<TType, TModel>(this IEnumerable<TType> types,
            Func<TType, ValueTask<TModel>> func)
        {
            foreach (var i in types)
                yield return await func.Invoke(i)
                    .ConfigureAwait(false);
        }
    }
}