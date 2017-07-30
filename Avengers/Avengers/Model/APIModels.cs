
using System.Collections.Generic;

namespace Avengers.Model
{
         /*  Models for interacting with face API*/

   // Response body from Detect Face Method from faceAPI
    public class IDetectionModel
    {
        public string faceId { get; set; }
        public faceRectangle faceRectangle { get; set; }
    }

    // Part of response body from Detect Face Method from faceAPI
    public class faceRectangle
    {
        public int top { get; set; }
        public int left { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    // Request body for Identify Face Method from faceAPI
    public class IDentifyRequestBodyModel
    {
        public string personGroupId { get; set; }
        public List<string> faceIds { get; set; }
        public float confidenceThreshold { get; set; }

        public IDentifyRequestBodyModel() { 
            this.faceIds = new List<string>();
            this.confidenceThreshold = 0;
        }
    }

    // Reponse body for Identify Face Method from faceAPI
    public class IDentifyResponseModel
    {
        public string faceId { get; set; }
        public List<Candidate> candidates { get; set; }
    }

    // Part of reponse body for Identify Face Method from faceAPI
    public class Candidate
    {
        public string personId { get; set; }
        public double confidence { get; set; }
    }


}

