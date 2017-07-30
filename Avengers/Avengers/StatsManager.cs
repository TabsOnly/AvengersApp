using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Avengers.Model;
using System.Linq;

namespace Avengers
{
    public class IStatsManager
    {
        private static IStatsManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<AvengerRecordTable> avengerRecordTable;
        private IMobileServiceTable<AvengerIdTable> avengerIdTable;

        private IStatsManager()
        {
            this.client = new MobileServiceClient("https://avenger.azurewebsites.net");
            this.avengerRecordTable = this.client.GetTable<AvengerRecordTable>();
            this.avengerIdTable = this.client.GetTable<AvengerIdTable>();
        }

        public MobileServiceClient IAzureClient
        {
            get { return client; }
        }

        public static IStatsManager IAzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IStatsManager();
                }
                return instance;
            }
        }

        public async Task<List<AvengerRecordTable>> IGetAvengerRecords()
        {
            return await this.avengerRecordTable.ToListAsync();
        }

        public async Task<string> IGetAvengerName(string personid)
        {
            var table=  await this.avengerIdTable.ToListAsync();

            AvengerIdTable entry = table.Find(x => x.personId == personid);

            return entry.name;
        }

        public async Task<string> IGetAvengerSuperName(string personid)
        {
            var table = await this.avengerIdTable.ToListAsync();

            AvengerIdTable entry = table.Find(x => x.personId == personid);

            return entry.supername;
        }

        public async Task IPostAvengerRecord(AvengerRecordTable avengerRecordTable)
        {
            await this.avengerRecordTable.InsertAsync(avengerRecordTable);
        }

    }
}