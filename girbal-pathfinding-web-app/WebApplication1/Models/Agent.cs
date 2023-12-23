using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Models
{
    public class Agent
    {
        public State startState { get; set; }
        public State goalState { get; set; }
        public int id { get; set; }

        public List<State> path { get; set; }

        //new
        //public Agent(State startState, State goalState, List<State> path, int id)
        public Agent()
        {
            this.startState = startState;
            this.goalState = goalState;
            this.id = id;
            this.path = path;
        }

        public Agent(List<State> path, int id)
        {
            this.id = id;
            this.path = path;
            //this.startState = null;
            //this.goalState = null;
        }
    }
}
