
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Collections.Generic;

namespace Avengers.Model
{
    public class IDetectionModel
    {
        //[JsonProperty(PropertyName = "faceId")]
        public string faceId { get; set; }

        //[JsonProperty(PropertyName = "faceRectangle")]
        public faceRectangle faceRectangle { get; set; }
    }

    public class faceRectangle
    {
        //[JsonProperty(PropertyName = "top")]
        public int top { get; set; }

        //[JsonProperty(PropertyName = "left")]
        public int left { get; set; }

        //[JsonProperty(PropertyName = "width")]
        public int width { get; set; }

        //[JsonProperty(PropertyName = "height")]
        public int height { get; set; }
    }

    public class IDentifyRequestBodyModel
    {
        [JsonProperty(PropertyName = "personGroupId")]
        public string personGroupId { get; set; }

        [JsonProperty(PropertyName = "faceIds")]
        public List<string> faceIds { get; set; }
        
        [JsonProperty(PropertyName = "confidenceThreshold")]
         public float confidenceThreshold { get; set; }

        public IDentifyRequestBodyModel() { 
            this.faceIds = new List<string>();
            this.confidenceThreshold = 0;
        }
    }

    public class Candidate
    {
        public string personId { get; set; }
        public double confidence { get; set; }
    }

    public class IDentifyModel
    {
        public string faceId { get; set; }
        public List<Candidate> candidates { get; set; }
    }
}

