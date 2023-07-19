using System.Text.Json.Serialization;

namespace DevOpsMigrationLib.Models
{
    public class NewProcessType
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("parentProcessTypeId")]
        public string? ParentProcessTypeId { get; set; } = string.Empty;

        [JsonPropertyName("referenceName")]
        public object? ReferenceName { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }

}
