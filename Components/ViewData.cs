namespace myapp.Components;

public class ViewData
{
    private readonly Dictionary<string, object> _data = new();

    public T? GetValue<T>(string key)
    {
        return _data.TryGetValue(key, out var value) && value is T typedValue ? typedValue : default;
    }

    public void SetValue<T>(string key, T value)
    {
        _data[key] = value;
    }
}