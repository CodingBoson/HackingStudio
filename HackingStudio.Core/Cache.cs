using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace HackingStudio.Core;

public static class Cache
{
    private static readonly ConcurrentDictionary<object, CachedValue> _cache = [];

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Clear()
    {
        _cache.Clear();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void Remove(object key)
    {
        _cache.TryRemove(key, out _);
    }

    private readonly struct CachedValue(object value, TimeSpan lastTime)
    {
        public object Value { get; } = value;

        public TimeSpan TimeCreated { get; } = lastTime;
    }

    #region Sync

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T GetValue<T>(Func<T> getter, TimeSpan duration, bool shortCircuit = false)
    {
        return GetValue(getter, getter, duration, shortCircuit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static T GetValue<T>(object key, Func<T> getter, TimeSpan duration, bool shortCircuit = false)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(getter, nameof(getter));

        if (shortCircuit) {
            return getter();
        }

        if (!_cache.TryGetValue(key, out var cachedValue)) {
            var value = getter();

            cachedValue = new CachedValue(value!, DateTime.Now.TimeOfDay);

            _cache.TryAdd(key, cachedValue);

            return value;
        }

        var now = DateTime.Now;

        if (now.TimeOfDay - cachedValue.TimeCreated >= duration) {
            var value = getter();

            _cache[key] = new CachedValue(value!, DateTime.Now.TimeOfDay);

            return value;
        }

        return (T)cachedValue.Value;
    }

    #endregion Sync

    #region Async

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Task<T> GetValueAsync<T>(Func<Task<T>> getter, TimeSpan duration, bool shortCircuit = false)
    {
        return GetValueAsync(getter, getter, duration, shortCircuit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static async Task<T> GetValueAsync<T>(object key, Func<Task<T>> getter, TimeSpan duration, bool shortCircuit = false)
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(getter, nameof(getter));

        if (shortCircuit) {
            return await getter();
        }

        if (!_cache.TryGetValue(key, out var cachedValue)) {
            var value = await getter();

            cachedValue = new CachedValue(value!, DateTime.Now.TimeOfDay);

            _cache.TryAdd(key, cachedValue);

            return value;
        }

        var now = DateTime.Now;

        if (now.TimeOfDay - cachedValue.TimeCreated > duration) {
            var value = await getter();

            _cache[key] = new CachedValue(value!, DateTime.Now.TimeOfDay);

            return value;
        }

        return (T)cachedValue.Value;
    }

    #endregion Async
}
