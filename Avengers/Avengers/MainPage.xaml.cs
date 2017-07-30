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
using System.Linq;

namespace Avengers
{
    public partial class MainPage : ContentPage
    {  
        public string detectedFaceId;

        public MainPage()
        {
            InitializeComponent();
            //this.isLoading = false;
        }

        private async void ILoadCamera(object sender, EventArgs e)
        {
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

            loadingcircle.IsVisible = true;

            if(await IDetectFaceRequest(file))
            {
                analyseButton.IsVisible = true;
            }
            
            loadingcircle.IsVisible = false;    

            file.Dispose();

            return;
        }

        static byte[] IGetImageAsByteArray(MediaFile file)
        {
            var stream = file.GetStream();
            BinaryReader binaryReader = new BinaryReader(stream);
            return binaryReader.ReadBytes((int)stream.Length);
        }

        async Task<bool> IDetectFaceRequest(MediaFile file)
        {
            var client = new HttpClient();

            // Request headers
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "edc7bafc8b4c4f9492f6e0ce69fc0c08");

            var uri = "https://westcentralus.api.cognitive.microsoft.com/face/v1.0/detect";

            HttpResponseMessage response;

            // Request body
            byte[] byteData = IGetImageAsByteArray(file);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    if (responseString == "[]")
                    {
                        await DisplayAlert("ERROR", "Couldn't discern a face, please try again", "OK");
                        return false;
                    }

                    List<IDetectionModel> responseModel = JsonConvert.DeserializeObject<List<IDetectionModel>>(responseString);

                    // await ICompareFaceRequest(responseModel[0].faceId);
                    detectedFaceId = responseModel[0].faceId;
                    return true;
                }
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

            IDentifyRequestBodyModel data = new IDentifyRequestBodyModel();

            data.personGroupId = "avengers";
            data.faceIds.Add(faceid);

            string json = JsonConvert.SerializeObject(data);

            byte[] byteData = Encoding.UTF8.GetBytes(json);

            using (var content = new ByteArrayContent(byteData))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

                response = await client.PostAsync(uri, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseString = await response.Content.ReadAsStringAsync();

                    List<IDentifyModel> responseModel = JsonConvert.DeserializeObject<List<IDentifyModel>>(responseString);

                    string personid = responseModel[0].candidates[0].personId;


                    return await AddToRecordTable(personid);
                }

                return null;
            }

        }

        private async Task<string> AddToRecordTable(string personid)
        {
         string newRecordName = await IStatsManager.IAzureManagerInstance.IGetAvengerName(personid);
         string newSuperName = await IStatsManager.IAzureManagerInstance.IGetAvengerSuperName(personid);

         AvengerRecordTable newrecord = new AvengerRecordTable { name = newRecordName, supername = newSuperName };

         await IStatsManager.IAzureManagerInstance.IPostAvengerRecord(newrecord);

         return newSuperName;

        }

        private async void INavigateResultsPage(object sender, EventArgs e)
        {
            loadingcircle.IsVisible = true;
            string supername =  await ICompareFaceRequest(detectedFaceId);
            loadingcircle.IsVisible = false;
            await Navigation.PushAsync(new ResultsPage(supername));
        }
    }
}


