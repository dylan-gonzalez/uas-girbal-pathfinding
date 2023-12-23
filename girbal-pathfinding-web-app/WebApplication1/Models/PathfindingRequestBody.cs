using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;


namespace WebApplication1.Models
{
    public class PathfindingRequestBody
    {
        //[JsonProperty(Required = Required.Always, Item = PreserveReferencesHandling.All)]
        //[JsonProperty(Required = Required.Always, ItemReferenceLoopHandling = ReferenceLoopHandling.Ignore)]
        [JsonPropertyName("agents")]
        public List<Agent> agents { get; set; }

        //public Agent[] agents { get; set; }

        //[JsonProperty(Required = Required.Always)]
        [JsonPropertyName("map")]
        public int[][] map { get; set; } 

        public PathfindingSettings ToSettings()
        {
            return new PathfindingSettings()
            {
                agents = agents,
            };
        }
    }
}