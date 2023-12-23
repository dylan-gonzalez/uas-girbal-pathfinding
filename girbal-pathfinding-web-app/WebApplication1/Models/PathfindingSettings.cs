using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class PathfindingSettings
    {
        //[JsonProperty(Required = Required.Always)]
        [JsonPropertyName("agents")]
        public List<Agent> agents { get; set; }
        //public Agent[] agents { get; set; }

        public static bool CheckIfValid(PathfindingSettings s)
        {
            if (s == null) return false;

            for (int i = 0; i < s.agents.Count; i++)
            //for (int i = 0; i < s.agents.Length; i++)
            {
                if (s.agents[i].startState.x < 0 || s.agents[i].startState.y < 0) return false;

                if (s.agents[i].startState.positionEqual(s.agents[i].goalState, false)) return false;
            }

            return true;
        }
    }
}