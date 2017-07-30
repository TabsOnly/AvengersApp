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
        public string supername;

        public ResultsPage(string supername)
        { 
            InitializeComponent();
            this.supername = supername;
            ResultLabel.Text = supername;
        }

        private async void ICountAvengerRecord(object sender, EventArgs e)
        {
            var table = await IStatsManager.IAzureManagerInstance.IGetAvengerRecords();

            List<AvengerCount> countlist = table.GroupBy((g => g.name), (name, elements) => new AvengerCount
            {
                name = name,
                count = elements.Distinct().Count()
            }).ToList();

            await DisplayAlert("COUNTS", countlist[0].name + "'s count is " + countlist[0].count.ToString(), "Ok");
        }

    }
}