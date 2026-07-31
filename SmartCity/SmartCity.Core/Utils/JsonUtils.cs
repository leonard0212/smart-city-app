using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;


namespace SmartCity.Core.Utils
{
    public static class JsonUtils
    {
        public static string SerializeObjectCamelCase(object obj)
        {
            var serializeSttings = new JsonSerializerSettings { ContractResolver = new CamelCasePropertyNamesContractResolver() };
            serializeSttings.Converters.Add(new StringEnumConverter());

            var serializedObject = JsonConvert.SerializeObject(obj, serializeSttings);

            return serializedObject;
        }

        public static T CloneObject<T>(T obj) where T : class
        {
            if (obj == null)
                return null;

            var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };
            var copy = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj), deserializeSettings);

            return copy;
        }

        public static string GetValueByKey(string json, string key)
        {
            if (string.IsNullOrEmpty(json))
                return null;

            if (string.IsNullOrEmpty(key))
                return null;

            try
            {
                var jContainer = (JContainer)JsonConvert.DeserializeObject(json);
                if (jContainer is JObject)
                    return GetValueByKeyInternal(jContainer as JObject, key);
            }
            catch
            {
                return null;
            }

            return null;
        }

        private static string GetValueByKeyInternal(JObject jObject, string key)
        {

            if (jObject == null)
                return null;

            foreach (var x in jObject)
            {
                string name = x.Key;
                JToken token = x.Value;

                if (token is JValue)
                {
                    var _value = token as JValue;
                    if (key != null && key.Trim().ToLower() == name.Trim().ToLower())
                        return _value.Value?.ToString();
                }

                if (token is JArray)
                {
                    var _array = token as JArray;
                    foreach (var _va in _array)
                        if (_va is JObject)
                            GetValueByKeyInternal(_va as JObject, key);
                }

                if (token is JObject)
                    GetValueByKeyInternal(token as JObject, key);
            }

            return null;

        }

        public static string RemoveKeysByName(string json, string[] keys, string replaceValue)
        {
            if (string.IsNullOrEmpty(json))
                return json;

            if (keys == null || keys.Length == 0)
                return json;

            try
            {
                var jContainer = (JContainer)JsonConvert.DeserializeObject(json);
                if (jContainer is JObject)
                    RemoveKeysByNameInternal(jContainer as JObject, keys, replaceValue);

                json = jContainer.ToString(Formatting.None);
            }
            catch
            {
                return json;
            }

            return json;
        }

        private static void RemoveKeysByNameInternal(JObject jObject, string[] keys, string replaceValue)
        {
            if (jObject == null)
                return;

            foreach (var x in jObject)
            {
                string name = x.Key;
                JToken token = x.Value;

                if (token is JArray)
                {
                    var _array = token as JArray;
                    foreach (var _va in _array)
                        if (_va is JObject)
                            RemoveKeysByNameInternal(_va as JObject, keys, replaceValue);
                }

                if (token is JValue)
                {
                    var _value = token as JValue;
                    if (keys != null && keys.Any(key => key.Trim().ToLower() == name.Trim().ToLower()))
                        _value.Value = replaceValue;
                }

                if (token is JObject)
                    RemoveKeysByNameInternal(token as JObject, keys, replaceValue);
            }
        }
    }
}
