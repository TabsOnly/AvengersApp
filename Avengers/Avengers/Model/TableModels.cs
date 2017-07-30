using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Avengers.Model
{
    public class AvengerRecordTable
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "supername")]
        public string supername { get; set; }
    }

    public class AvengerIdTable

    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "personId")]
        public string personId { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "supername")]
        public string supername { get; set; }
    }

    public class AvengerCount
    {
        public string name { get; set; }
        public int count { get; set; }
    }

}
