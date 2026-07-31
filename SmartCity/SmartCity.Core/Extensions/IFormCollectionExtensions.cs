using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Dynamic;

namespace SmartCity.Core.Extensions
{
    public static class IFormCollectionExtensions
    {
        private static List<string> _excludedKeys = new List<string>()
        {
            "__RequestVerificationToken",
            "Password",
            "ConfirmPassword"

        };

        public static string GetFormDataSerialized(this IFormCollection formCollection)
        {
            string data = JsonConvert.SerializeObject(formCollection.GetFormData());
            return data;
        }

        public static IDictionary<string, object> GetFormData(this IFormCollection formCollection)
        {
            dynamic obj = new ExpandoObject();
            var dict = obj as IDictionary<string, object>;
            foreach (var key in formCollection.Keys)
            {
                if (!_excludedKeys.Contains(key))
                    dict.Add(key, formCollection[key]);
            }

            return dict;
        }
    }
}
