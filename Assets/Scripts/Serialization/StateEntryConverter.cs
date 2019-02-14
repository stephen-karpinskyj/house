using Newtonsoft.Json;
using System;

// TODO: Try again using JsonConverter<T> when package is upgraded
public class StateEntryConverter<T> : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return true;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        return new StateEntry<T>((T)reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        StateEntry<T> entry = (StateEntry<T>)value;
        writer.WriteValue(entry.Value);
    }
}
