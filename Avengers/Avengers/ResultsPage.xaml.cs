using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avengers.Model;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Avengers
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ResultsPage : ContentPage
    {
        // Which look-alike the person matched to
        public string superName;

        public ResultsPage(string supername)
        { 
            InitializeComponent();
            superName = supername;

            // Displays to the user their lookalike
            ResultLabel.Text = supername;
        }

        private async void ICountAvengerRecord(object sender, EventArgs e)
        {
            // start loading
            loadingcircle.IsRunning = true;

            // store table of avenger records in a variable
            var table = await IEasyTableManager.IAzureManagerInstance.IGetAvengerRecords();

            List<IAvengerCount> countlist = table .GroupBy((g => g.supername),(supername, elements)
                                                              => new IAvengerCount
                                                                    {
                                                                        name = supername,
                                                                        count = elements.Distinct().Count()
                                                                    }).ToList();

            // stop loading
            loadingcircle.IsRunning = false;

            // set returned count list as the source for ListView
            CountList.ItemsSource = countlist;
        }

    }
}