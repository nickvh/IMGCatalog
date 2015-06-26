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
    class Variance
    {
        public string Prop { get; set; }
        public object valA { get; set; }
        public object valB { get; set; }
    }
    //within our objects, find the field differences (ignoring some in toIgnore)
    static class extensions
    {
        public static List<Variance> DetailedCompare<T>(this T val1, T val2)
        {
            string toIgnore = "<ProviderID>k__BackingField, <ProviderAssetID>k__BackingField,<Provider>k__BackingField, <IsHD>k__BackingField, <AssetID>k__BackingField, <VHOID>k__BackingField";
            List<Variance> variances = new List<Variance>();
            FieldInfo[] fi = val1.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (FieldInfo f in fi)
            {
                Variance v = new Variance();
                v.Prop = f.Name;
                v.valA = f.GetValue(val1);
                v.valB = f.GetValue(val2);
                if (!v.valA.Equals(v.valB))
                {
                    if (!toIgnore.Contains(f.Name))
                    {
                        variances.Add(v);
                        Console.WriteLine("Mismatch : " + f.Name + " A : " + v.valA + " B : " + v.valB);
                    }
                }

            }
            return variances;
        }
    }
}
