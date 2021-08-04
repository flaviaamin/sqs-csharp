using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ReCall2.Models
{
    public class Recall
    {
        string type = "RECALL";
        public string Type
        {
            get
            {
                return (type);
            }
        }
        public string Environment { get; set; }
        public string Brand { get; set; }
        public string CallNumber { get; set; }
        public string CampaignId { get; set; }
        public string DefaultLanguage { get; set; }
        public string PreferredLanguage
        {
            get
            {
                return DefaultLanguage;
            }
        }
        public string EventDate { get; set; }
        public Translation Translations { get; set; }

        [JsonIgnore]
        public string SQSId { get; set; }

        [JsonIgnore]
        public string Title { get; set; }
        [JsonIgnore]
        public string ThankYouMessage { get; set; }
        [JsonIgnore]
        public string Text { get; set; }

        public List<string> CustomerChannels
        { 
            get
            {
                return new List<string>() { "CONNECTED_APP", "VEHICLE"};
            }
        }
       
        public string Market { get; set; }
        public List<string> Vins { get; set; }
        public string VisibleFromDate { get; set; }
        public string VisibleToDate { get; set; }
        public int Version { get; set; }
        public List<string> ConnectedAppActionButtons { get; set; }
    }
   
}
