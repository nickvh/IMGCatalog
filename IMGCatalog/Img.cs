using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Net.Http;
using System.Net;
using System.IO;
using MongoDB.Driver;
using System.IO.Compression;

namespace IMGCatalog
{
    class Img
    {
        //private static Root<CatalogObject> vho = new IMGCatalog.Root<CatalogObject>();
        //private static Dictionary<string, CatalogObject> VHO = new Dictionary<string, CatalogObject>();
        static MongoDB.Driver.MongoClient client = new MongoDB.Driver.MongoClient("mongodb://127.0.0.1:27017");
        static MongoDB.Driver.IMongoDatabase database = client.GetDatabase("IMGCatalog");
        static MongoDB.Driver.IMongoCollection<CatalogObject> collection = database.GetCollection<CatalogObject>("content");


        static void Main(string[] args)
        {
            database.DropCollectionAsync("content");
            CreateTasksAsync().Wait();
        }


        private static async Task CreateTasksAsync()
        {
            //Cleanup old and start fresh
            //?? why not just update with new by comparing?
            //it may be quicker to just insert than read file and DB and compare
            //Todo analyze this
            var catalogs = new List<Catalog>();
            var vhos = new List<Task<bool>>();
            for(int j=1;j <=15; j++)
            {
                catalogs.Add (new Catalog(j));
                vhos.Add (catalogs.Last().AddUniqueFromVHOAsync(collection));
            }
            await Task.WhenAll(vhos);
            //Todo, cant grab non IMG since its invalid JSON, bug filed
            //AddUniqueFromVHOAsync(0);
        }
    }

}



