using Newtonsoft.Json;
using System.Collections.Generic;

namespace eology.SharedLibs.ExternalApis.SERanking
{
    public class SER_SerpApiRequestItem
    {
        public int engine_id { get; set; }
        public List<string> query { get; set; }
        public string region_name { get; set; }
    }

    public class SER_AccountBalance
    {
        public string Currency { get; set; }
        public float Value { get; set; }
        public SER_AccountBalance()
        {
            Currency = "";
        }
    }

    public class SER_SerpQueueResultItem
    {
        [JsonProperty("query")]
        public string Query { get; set; }

        [JsonProperty("task_id")]
        public int TaskId { get; set; }
    }


    public class SER_SERPResult
    {
        [JsonProperty("results")]
        public List<SER_SERPResultItem> results { get; set; }
    }


    public class SER_SERPResultItem
    {
        [JsonProperty("position")]
        public string Position { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("snippet")]
        public string Snippet { get; set; }

        [JsonProperty("cache_url")]
        public string CacheUrl { get; set; }
    }
}

