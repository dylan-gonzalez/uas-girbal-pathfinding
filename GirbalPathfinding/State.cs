using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Policy;
using System.Text;

namespace GirbalPathfinding
{
    public class State
    {
        public int x;
        public int y;
        public int z;

        public int droneRadius = 2;

        public int time { get; set; }

        //neighbours
        public List<PointWithTime> predecessors { get; set; }
        public List<PointWithTime> staticPredecessors { get; set; }

        public double g { get; set; }
        public double h { get; set; }
        public int frontier_time { get; set; }
        public State parent { get; set; }
        public int comparisonFhat { get; set; }

        public State()
        {
            x = -1;
            x = -1;
            time = 0;
            predecessors = new List<PointWithTime>();
            staticPredecessors = new List<PointWithTime>();
            comparisonFhat = 0;
        }

        public State(int x, int y, int z, double cost_s, double h_s, int time) : this()
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.g = cost_s;
            this.h = h_s;
            this.time = time;
        }

        public State(State state) : this(state.x, state.y, state.z, state.g, state.h, state.time)
        {
            this.parent = state.parent;
            this.predecessors = state.predecessors;
        }

        public double fhat()
        {
            return g + h;
            //return f_static();
        }

        public double f_static()
        {
            return g + h;
        }

        /// <summary>
        /// Sets the properties of this state instance and checks validity
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <param name="time"></param>
        /// <returns>True if state is valid</returns>
        public bool setProperties(int x, int y, int z, int time, int boardw, int boardh, int boardz)
        {
            setProperties(x, y, z, time);

            if (this.x < 0 || this.x > boardw || this.y < 0 || this.y > boardh || this.z < 0||this.z > boardz)
            {
                return false;
            }
            return true;
        }

        public void setProperties(int x, int y, int z, int time)
        {
            this.x = x;
            this.y = y;
            this.z = z;
            this.time = time;
        }

        public void setProperties(PointWithTime point)
        {
            setProperties(point.x, point.y, point.z, point.time);
        }

        public void setProperties(State state)
        {
            setProperties(state.x, state.y, state.z, state.time);
        }

        /// <summary>
        /// Checks to see if the state is a static obstacle
        /// </summary>
        /// <param name="staticObstacles">List of static obstacles</param>
        /// <returns>True if state is a static obstacle</returns>
        public bool checkIfStaticObstacle(List<StaticObstacle> staticObstacles)
        {


            for (int i = 0; i < staticObstacles.Count; i++)
            {
                var obs = staticObstacles[i];

                if ((this.x - Globals.droneRadius) <= obs.x && obs.x <= (this.x + Globals.droneRadius) && (this.y - Globals.droneRadius) <= obs.y && obs.y <= (this.y + Globals.droneRadius)&& (this.z - Globals.droneRadius) <= obs.z && obs.z <= (this.z + Globals.droneRadius))
                {
                    return true;
                }
            }

            return false;

            //return staticObstacles.Exists((obs) => obs.x == this.x && obs.y == this.y);
        }

        public bool checkIfConstraint(List<(State, int)> constraints, int agentIndex)
        {
            //if (constraints.Count > 0)
            //{
            //    Trace.WriteLine("checkIfConstraint: " + constraints[0].Item1.x.ToString() + " " + constraints[0].Item1.y.ToString() + " " + constraints[0].Item1.time.ToString() + " " + constraints[0].Item2.ToString());
            //    Trace.WriteLine("checkIfConstraint [state]: " + this.x.ToString() + " " + this.y.ToString() + " " + this.time.ToString() + " " + agentIndex.ToString());

            //}

            //var listIndex = constraints.FindIndex((constraint) => constraint.Item1.x == this.x && constraint.Item1.y == this.y && constraint.Item1.time == this.time && constraint.Item2 == agentIndex);

            //if (listIndex != -1)
            //{
            //        Trace.WriteLine("Constraint found: " + this.x.ToString() + " " + this.y.ToString() + " " + this.time.ToString() + " " + agentIndex.ToString());
            //        return true;
            //}


            if (constraints.Exists((constraint) => constraint.Item1.x == this.x && constraint.Item1.y == this.y && constraint.Item1.time == this.time && constraint.Item2 == agentIndex))
            {
                //Trace.WriteLine("Constraint found: " + this.x.ToString() + " " + this.y.ToString() + " " + this.time.ToString() + " " + agentIndex.ToString());
                return true;
            }

            return false;

            //return constraints.Exists((constraint) => constraint.Item1.x == this.x && constraint.Item1.x == this.y && constraint.Item1.time == this.time && constraint.Item2 == agentIndex );
        }

        public float distanceTo(State input)
        {
            return MathF.Sqrt(MathF.Pow(x - input.x, 2) + MathF.Pow(y - input.z, 2)+MathF.Pow(z-input.z,2));
        }

        public float distanceTo(int x, int y, int z)
        {
            return MathF.Sqrt(MathF.Pow(this.x - x, 2) + MathF.Pow(this.y - y, 2) + MathF.Pow(this.z - z, 2));
        }

        public void generateComparisonFhat()
        {
            try
            {
                comparisonFhat = (int)(fhat());
            }
            catch (OverflowException)
            {
                comparisonFhat = int.MaxValue;
            }
            if (comparisonFhat == int.MinValue)
                comparisonFhat = int.MaxValue;
            //Debug.WriteLine("{0}, {1}, {2}, {3}, {4}", cost_s, cost_d, h_s, h_d, h_err);
        }

        public bool positionEqual(State state, bool includeRadius)
        {
            if (includeRadius)
            {
                if ((state.x - Globals.droneRadius <= x && x <= state.x + Globals.droneRadius) && (state.y - Globals.droneRadius <= y && y <= state.y + Globals.droneRadius) && (state.z - Globals.droneRadius <= z && z <= state.z + Globals.droneRadius))
                {

                    return true;
                }

                return false;

            } else
            {

                return x == state.x && y == state.y && z == state.z;
            }
        }

        public bool Equals(State state, bool includeRadius)
        {
            if (includeRadius)
            {
                if ((state.x - Globals.droneRadius <= x && x <= state.x + Globals.droneRadius) && (state.y - Globals.droneRadius <= y && y <= state.y + Globals.droneRadius) && (state.z - Globals.droneRadius <= z && z <= state.z + Globals.droneRadius) && state.time == time)
                {

                    return true;
                }

                return false;
            } else
            {
                return x == state.x && y == state.y && z == state.z && time == state.time;
            }
        }


        public static bool operator ==(State a, State b)
        {
            return a.x == b.x && a.y == b.y && a.z == b.z && a.time == b.time;
        }

        public static bool operator !=(State a, State b)
        {
            return !(a == b);
        }
    }
}
