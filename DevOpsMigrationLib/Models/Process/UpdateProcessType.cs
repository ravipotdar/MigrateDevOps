using System.Text.Json.Serialization;

namespace DevOpsMigrationLib.Models
{
    public class UpdateProcessType
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("IsDefault")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("IsEnabled")]
        public bool IsEnabled { get; set; } 
       
    }

}
