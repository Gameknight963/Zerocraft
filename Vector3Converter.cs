using Newtonsoft.Json;
using System;
using UnityEngine;

namespace mszcubemod
{
    public class Vector3Converter : JsonConverter<Vector3>
    {
        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            float[] arr = serializer.Deserialize<float[]>(reader);
            return new Vector3(arr[0], arr[1], arr[2]);
        }

        public override void WriteJson(JsonWriter writer, Vector3 value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, new float[] { value.x, value.y, value.z });
        }
    }
}
