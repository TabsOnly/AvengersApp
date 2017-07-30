using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Avengers.Model
{
    /*  Models matching Easy Table Schema*/

    // Model to record avenger look-alike
    public class AvengerRecordTable
    {
        [JsonProperty(PropertyName = "Id")]
        public string ID { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string name { get; set; }

        [JsonProperty(PropertyName = "supername")]
        public string supername { get; set; }
    }

    // Model for accessing table which stores personids,names
    // and superhero names for all Avengers
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

    // Model for storing a count of how many lookalikes 
    // are record for a specific avenger name (super hero name)
    public class IAvengerCount
    {
        public string name { get; set; }
        public int count { get; set; }
    }

}
