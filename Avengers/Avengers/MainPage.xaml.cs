using Plugin.Media;
using Plugin.Media.Abstractions;
using System;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;
using Avengers.Model;
using System.Collections.Generic;
using System.Text;

namespace Avengers
{
    public partial class MainPage : ContentPage
    {  
        // Field which captures the faceId of a detected face
        // for use to compare
        public string detectedFaceId;

        public MainPage()
        {
            InitializeComponent();
        }

        private async void ILoadCamera(object sender, EventArgs e)
        {
            // Method which loads the camera and uses detects face in captured image

            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
            {
                await DisplayAlert("No Camera", ":( No camera available.", "OK");
                return;
            }

            MediaFile file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
            {
                PhotoSize = PhotoSize.Medium,
                Directory = "Sample",
                Name = $"{DateTime.UtcNow}.jpg"
            });

            if (file == null)
                return;

            // Start loading
            loadingcircle.IsRunning = true;

            // Only reveal analyse button if the face detection 
            // returned true
            if(await IDetectFaceRequest(file))
            {
                analyseButton.IsVisible = true;
            }

            // stop loading
            loadingcircle.IsRunning = false;    

            // get rid of image
            file.Dispose();

            return;
        }

        static byte[] IGetImageAsByteArray(MediaFile file)
        {
            // converts image into its binary representation 
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task<bool> IDetectFaceRequest(MediaFile file)
        {
            // Method that uses api to detect face in image

            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "edc7bafc8b4c4f9492f6e0ce69fc0c08");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

            HttpResponseMessage response;

            // Converts image to binary for request body
            byte[] byteData = IGetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {
                // Adds content type header
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    // Gets response as a string
                    string responseString = await response.Content.ReadAsStringAsync();

                    // If no face is detected, API returns empty array
                    if (responseString == "[]")
                    {
                        await DisplayAlert("ERROR", "Couldn't discern a face, please try again", "OK");
                        return false;
                    }

                    // Deserialises from JSON to a model
                    List<IDetectionModel> responseModel = JsonConvert.DeserializeObject<List<IDetectionModel>>(responseString);
                    
                    // The detected faceid is stored in class field
                    detectedFaceId = responseModel[0].faceId;

                    // returns no errors
                    return true;
                }

                // Just incase subscription-key runs out etc
                await DisplayAlert("ERROR", "There's been an please try again", "OK");

                return false;

            }
        }

        async Task<string> ICompareFaceRequest(string faceid)
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "edc7bafc8b4c4f9492f6e0ce69fc0c08");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/identify";

            HttpResponseMessage response;

            // Creating a new request body model based passed in faceid
            IDentifyRequestBodyModel data = new IDentifyRequestBodyModel();

            // Makes sure face is compared to avengers faces
            data.personGroupId = "avengers";
            data.faceIds.Add(faceid);

            // Serialising into a JSON object
            string json = JsonConvert.SerializeObject(data);

            // Encoding Request JSON object into binary

            byte[] byteData = Encoding.UTF8.GetBytes(json);

            using (var content = new ByteArrayContent(byteData))
            {
                // adding content type header
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    List<IDentifyResponseModel> responseModel = JsonConvert.DeserializeObject<List<IDentifyResponseModel>>(responseString);

                    // Get the personid of the most likely avenger face
                    // Output from API is sorted so that avenger with most confidence
                    // is at the front (index 0)
                    string personid = responseModel[0].candidates[0].personId;

                    // Add this to the table the table of records
                    return await IAddToRecordTable(personid);
                }

                return null;
            }

        }

        private async Task<string> IAddToRecordTable(string personid)
        {
         // Adds a record of avenger based on personid

         // Gets relevant fields to make a record
         string newRecordName = await IEasyTableManager.IAzureManagerInstance.IGetAvengerName(personid);
         string newSuperName = await IEasyTableManager.IAzureManagerInstance.IGetAvengerSuperName(personid);

        // Creates a new record
         AvengerRecordTable newrecord = new AvengerRecordTable { name = newRecordName, supername = newSuperName };
        
        // Pushes new record into easytable
         await IEasyTableManager.IAzureManagerInstance.IPostAvengerRecord(newrecord);

        // Returns the Super Hero name of newly added avenger
         return newSuperName;

        }

        private async void INavigateResultsPage(object sender, EventArgs e)
        {
            // Gets the avenger look alike and navigates to Results Page

            // start loading
            loadingcircle.IsRunning = true;

            // gets the avenger look alike
            string supername = await ICompareFaceRequest(detectedFaceId);

            // stop loading
            loadingcircle.IsRunning = false;

            // pushs on new results page and passes in the avenger look alike
            await Navigation.PushAsync(new ResultsPage(supername));
        }
    }
}


