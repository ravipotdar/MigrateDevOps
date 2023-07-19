using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DevOpsMigrationLib.Models
{
    public class ProcessTypeResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("value")]
        public List<ProcessType> value { get; set; } = new List<ProcessType>();
    }

    public class ProcessType
    {
        [JsonPropertyName("typeId")] 
        public string TypeId { get; set; } = string.Empty;

        [JsonPropertyName("referenceName")]
        public object? ReferenceName { get; set; }

        [JsonPropertyName("name")] 
        public string Name { get; set; } = string.Empty;    

        [JsonPropertyName("description")] 
        public string Description { get; set; } = string.Empty;

        [JsonPropertyName("parentProcessTypeId")]
        public string ParentProcessTypeId { get; set; } =  string.Empty;

        [JsonPropertyName("isEnabled")]
        public bool IsEnabled { get; set; }

        [JsonPropertyName("isDefault")]
        public bool IsDefault { get; set; }

        [JsonPropertyName("customizationType")]
        public string CustomizationType { get; set; } = string.Empty;
    }

}
