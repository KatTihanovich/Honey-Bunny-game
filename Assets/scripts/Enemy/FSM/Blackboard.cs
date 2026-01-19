using System.Collections.Generic;

public class Blackboard
{
    private readonly Dictionary<string, object> _data = new();

    public void Set<T>(string key, T value)
    {
        _data[key] = value;
    }

    public bool TryGet<T>(string key, out T value)
    {
        if (_data.TryGetValue(key, out var obj) && obj is T cast)
        {
            value = cast;
            return true;
        }

        value = default;
        return false;
    }

    public T GetOrDefault<T>(string key, T defaultValue = default)
    {
        return TryGet(key, out T v) ? v : defaultValue;
    }

    public bool Has(string key) => _data.ContainsKey(key);
}
