using Newtonsoft.Json;

namespace DevOpsMigrationLib.Helpers
{
    public class CustomJsonHelper
    {
        public static T GetDeserializedJson<T>(string strJson) where T : class
        {
            return JsonConvert.DeserializeObject<T>(strJson);
        }
    }
}
