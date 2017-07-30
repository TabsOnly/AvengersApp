using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.MobileServices;
using Avengers.Model;
using System.Linq;

namespace Avengers
{
    public class IEasyTableManager
    {
        /*  This class deals with accessing Easy Tables stored on Azure*/

        private static IEasyTableManager instance;
        private MobileServiceClient client;
        private IMobileServiceTable<AvengerRecordTable> avengerRecordTable;
        private IMobileServiceTable<AvengerIdTable> avengerIdTable;

        private IEasyTableManager()
        {
            // initialises client to point to endpoint where relevant easy tables are stored
            // and grabs tables
            this.client = new MobileServiceClient("https://avenger.azurewebsites.net");
            this.avengerRecordTable = this.client.GetTable<AvengerRecordTable>();
            this.avengerIdTable = this.client.GetTable<AvengerIdTable>();
        }

        public MobileServiceClient IAzureClient
        {
            get { return client; }
        }

        public static IEasyTableManager IAzureManagerInstance
        {
            get
            {
                if (instance == null)
                {
                    instance = new IEasyTableManager();
                }
                return instance;
            }
        }

        public async Task<List<AvengerRecordTable>> IGetAvengerRecords()
        {
            // returns table of records
            return await this.avengerRecordTable.ToListAsync();
        }

        public async Task<string> IGetAvengerName(string personid)
        {
            // returns name of an avenger based on personId passed in
            // searchs AvengerIdTable to find name
            var table =  await this.avengerIdTable.ToListAsync();

            AvengerIdTable entry = table.Find(x => x.personId == personid);

            return entry.name;
        }

        public async Task<string> IGetAvengerSuperName(string personid)
        {
            // returns super hero name of an avenger based on personId passed in
            // searchs AvengerIdTable to find super hero name
            var table = await this.avengerIdTable.ToListAsync();

            AvengerIdTable entry = table.Find(x => x.personId == personid);

            return entry.supername;
        }

        public async Task IPostAvengerRecord(AvengerRecordTable avengerRecordTable)
        {
            // publish a new entry in the record table
            await this.avengerRecordTable.InsertAsync(avengerRecordTable);
        }

    }
}