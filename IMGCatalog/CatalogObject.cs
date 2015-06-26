using System;
using System.Collections;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace IMGCatalog
{
    [Serializable]
    public class CatalogObject 
    {
        [BsonId]
        public string AltCode { get; set; }
        public string AssetID { get; set; }
        public string VHOID { get; set; }
        public string Provider { get; set; }
        public string ProviderAssetID { get; set; }
        public string ProviderID { get; set; }
        public string TMSID { get; set; }
        public string Title { get; set; }
        public string BriefTitle { get; set; }
        public string LicensingStartDate { get; set; }
        public string LicensingEndDate { get; set; }
        public string LicensingStartDateLocal { get; set; }
        public string LicensingEndDateLocal { get; set; }
        public string CAST { get; set; }
        public string CastDisplay { get; set; }
        public string Directors { get; set; }
        public string Producers { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
        public string Genre { get; set; }
        public string length { get; set; }
        public string Rating { get; set; }
        public string ReleaseYear { get; set; }
        public string EpisodeID { get; set; }
        public string EpisodeName { get; set; }
        public string SeasonID { get; set; }
        public string SeriesID { get; set; }
        public bool CC { get; set; }
        public bool IsHD { get; set; }
        public string AudioFormat { get; set; }
        public string VideoFormat { get; set; }
        public string ShowType { get; set; }
        public string Language { get; set; }
        public bool IsAdult { get; set; }
        public bool Is3D { get; set; }
        public bool IsFlexView { get; set; }
        public bool IsFlexViewTV { get; set; }
        public bool IsSVOD { get; set; }
        public bool IsFree { get; set; }
        public bool AP_IMG { get; set; }
        public string OrigAirDate { get; set; }
        public string RentPrice { get; set; }
        public string PurchasePrice { get; set; }
        public string RentStart { get; set; }
        public string RentEnd { get; set; }
        public string PurchaseStart { get; set; }
        public string PurchaseEnd { get; set; }
        public string HDSDLink { get; set; }

        public IEnumerator<CatalogObject> GetEnumerator()
        {
            throw new NotImplementedException();
        }


    }
}
