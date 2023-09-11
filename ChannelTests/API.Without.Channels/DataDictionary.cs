using System.Collections.Concurrent;

namespace API.Without.Channels;

public sealed class DataDictionary : ConcurrentDictionary<int, DataWrapper>
{
    public DataDictionary(IEnumerable<KeyValuePair<int, DataWrapper>> data) : base(data)
    {
    }
}