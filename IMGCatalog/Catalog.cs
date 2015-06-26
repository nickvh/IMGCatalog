using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;
using MongoDB.Driver;
using System.IO.Compression;

namespace IMGCatalog
{
    class Catalog
    {
        static bool singleScreen = true;
        static bool multiScreen = true;
        static bool cache = false;
        private int vhoNumber;
        public Catalog (int vho)
        { vhoNumber = vho; }
        /// <summary>
        /// For a given VHO #, add any new unique AltCodes into our member vho
        /// When completed a VHO, cache this in the DB
        /// </summary>
        /// <param name="vhoNumber"></param>
        public async Task<bool> AddUniqueFromVHOAsync(MongoDB.Driver.IMongoCollection<CatalogObject> collection)
        {
            
            Dictionary<string, CatalogObject> vho = new Dictionary<string, CatalogObject>();

            string path = GetTemplate(vhoNumber, "d:\\img\\img\\", new DateTime(2015, 6, 17));
            if (path != string.Empty)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Processing VHO : " + vhoNumber);
                Console.ForegroundColor = ConsoleColor.White;
                try
                {
                    String tempFile = Decompress(path);
                    using (System.IO.StreamReader file = new System.IO.StreamReader(tempFile, Encoding.UTF8))
                    {
                        string line;
                        while ((line = await file.ReadLineAsync()) != null)
                        {
                            CatalogObject json = JsonConvert.DeserializeObject<CatalogObject>(line);

                            if ((json.AltCode == string.Empty))//blank AltCode, prob single screen
                            {
                                if (singleScreen)
                                {
                                    //Use the PID PAID since the AltCode is missing
                                    json.AltCode = json.ProviderID + ":" + json.ProviderAssetID.Substring(4);
                                    AddMedia(vho, json);
                                }
                            }
                            else if (multiScreen)
                            {
                                AddMedia(vho, json);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception caught opening file :" + ex.Message);
                }
            }
            try
            {
                //and Upsert the vho!
                await collection.InsertManyAsync(vho.Values);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
            return true;
        }
        public Dictionary<string, CatalogObject> AddMedia(Dictionary<string, CatalogObject> vho, CatalogObject json)
        {
            CatalogObject r = new CatalogObject();
            var found = vho.TryGetValue(json.AltCode, out r);
            if (found)
            {
                //check to see if its an exact copy, if it is then skip
                if (r.GetHashCode() != json.GetHashCode())
                {
                    if (json.IsHD)//only add values/updates from the HD version
                    {
                        vho[json.AltCode] = json;
                        Compare(r, json);
                    }
                    else
                    {
                        //need to Add Avails data for nonHD here which requires making the private fields into a list of...
                    }
                }
            }
            else
            {
                //Not this is unusual and an error
                //So possible outcome here is we have same content but first four chars dont match, but content is same
                //var dict = vho.Where(kvp => json.ProviderAssetID.Substring(4)).SelectMany(kvp => kvp.Value);
                //var dict = vho.SelectMany(m => m).Where(k => vho.Keys.Contains(json.ProviderAssetID.Substring(4)));
                var selectValues = (from keyValuePair in vho
                                    where keyValuePair.Key.Contains(json.ProviderAssetID.Substring(4))
                                    select keyValuePair.Value).ToList();
                if (selectValues.Count > 1)
                    Console.WriteLine(vhoNumber + " : " + json.AltCode);
                vho[json.AltCode] = json;
            }
            return vho;
        }
        /// <summary>
        /// getTemplate returns a file name convention 
        /// for a specific vho on a spcecific date
        /// </summary>
        /// <param name="vho"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        public string GetTemplate(int vho, string path, DateTime date)
        {
            string front = "VHO" + vho + "_VODFULL_";
            if (vho == 0)
            {
                front = "VHOALL_NONIMG_";//get non IMG catalog
            }
            string dt = date.Year + date.Month.ToString("d2") + date.Day.ToString("d2");
            string filePattern = front + dt + "*.gz";            try
            {

                string[] files = System.IO.Directory.GetFiles(path, filePattern, System.IO.SearchOption.TopDirectoryOnly);
                 return files.Length > 0 ? files[0] : string.Empty;
           }
            catch(Exception ex)
            {
                Console.WriteLine("exception on file pattern construction " + vho + " : :" + path);
            }
            return string.Empty;
        }
        public void Compare(CatalogObject source, CatalogObject current)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(vhoNumber + " : " + "AltCode : " + source.AltCode + "(" + source.AltCode.Length + ")");
            Console.ForegroundColor = ConsoleColor.White;
            List<Variance> rt = source.DetailedCompare(current);
        }

        public string Decompress(string fileName)
        {
            using (FileStream fInStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
            {
                using (GZipStream zipStream = new GZipStream(fInStream, CompressionMode.Decompress))
                {
                    using (FileStream fOutStream = new FileStream(fileName + ".tmp", FileMode.Create, FileAccess.Write))
                    {
                        byte[] tempBytes = new byte[4096];
                        int i;
                        while ((i = zipStream.Read(tempBytes, 0, tempBytes.Length)) != 0)
                        {
                            fOutStream.Write(tempBytes, 0, i);
                        }
                    }
                }
            }
            return fileName + ".tmp";
        }

    }
}
