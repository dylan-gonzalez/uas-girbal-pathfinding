using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace GirbalPathfinding
{
    public partial class Form1 : Form
    {
        private MapRenderer mapRenderer;

        private ExecutiveController executiveController;

        private const int mapWidth = 100;
        private const int mapHeight = 50;
        private const int mapDepth = 20;
        private const int noOfAgents = 4;


        private List<State> startStates = new List<State>()
        {
            new State() { x = 10, y = 8},
            new State() {x = 72, y = 8},
            new State() {x = 40, y = 30 },
            new State() {x = 20, y = 5 }

        };

        private List<State> goalStates = new List<State>()
        {
            new State() { x = 72, y= 8},
            new State() { x = 10, y = 8},
            new State() { x = 20, y = 30},
            new State() {x = 40, y = 30 }

        };

        private bool loopMovementFlag = false;

        //OBSTICLE VALUES
        public int obsticleMethods = 2; //step size for obsicle gernerator
        public int obsticleChance = 95; //% chance a space may be empty
        public int obsticleRectangle = 50; //% chance a space will not contain a rectange if an obsticle is declared
        public int obsticleCircle = 30;  //% chance a space will not contain a circle if an obsticle is declared


        //SETUP SYSTEM
        public Form1()
        {
            InitializeComponent();
            //Change here to required algorithm
            executiveController = new ExecutiveController(300, noOfAgents);
            executiveController.initialisePlanner<AStar>();

            mapRenderer = new MapRenderer();
            //this.WindowState = FormWindowState.Maximized;
            var rand = new Random();

            //SET START AND END GOAL. Make sure these are set AFTER the obstacles
            for (int i = 0; i < noOfAgents; i++)
            {
                executiveController.map.setGoal(goalStates[i], i); //i = plannerIndex in Map class
                executiveController.map.startStates.Add(startStates[i]);
            }

            //Trace.WriteLine("No. agents: ", )


            //for (int i = 1; i < mapWidth - 1; i++)
            //{
            //    for (int j = 0; j < mapHeight; j++)
            //    {
            //        if (rand.Next() % 30 == 1)
            //        {
            //            var newObstacle = new State() { x = i, y = j };
            //            var isObstacleInvalid = false;

            //            for (int k = 0; k < noOfAgents; k++)
            //            {
            //                //check if new obstacle collides with start/goal state
            //                if (newObstacle.positionEqual(executiveController.map.startStates[k], true) || newObstacle.positionEqual(executiveController.map.goalStates[k], true))
            //                {
            //                    isObstacleInvalid = true;
            //                    break;
            //                }
            //            }

            //            if (!isObstacleInvalid)
            //            {
            //                if (rand.Next() % 5 == 1)
            //                {
            //                    //executiveController.staticObstacles.Add(new StaticObstacle(i, j));
            //                    bottomCornerRectangleObsicle(i, j, 5, 1);
            //                }
            //                else if (rand.Next() % 40 == 1)
            //                {
            //                    centreCircleObsicle(i, j, 4);//centre
            //                }
            //                else
            //                {
            //                    rightAngleTriangleCornerObsicle(i, j, 4, 4, 1);
            //                }
            //            }
            //        }
            //    }
            //}

            //obsticle generation methods start here 
            int[,] obsticleBase = new int[mapWidth, mapHeight];

            for (int i = 1; i < mapWidth - 1; i+= obsticleMethods)
            {
                for (int j = 0; j < mapHeight; j+= obsticleMethods)
                {
                    if (rand.Next() % obsticleChance == 1)
                    {
                        //var newObstacle = new State() { x = i, y = j };
                        var isObstacleInvalid = false;

                        if (!isObstacleInvalid)//can be removed in final build
                        {  
                            if (rand.Next() % obsticleRectangle == 1) {
                                //executiveController.staticObstacles.Add(new StaticObstacle(i, j));
                                bottomCornerRectangleObsicle(i,j,3,2,obsticleBase,1);
                            }
                            else if (rand.Next() % obsticleCircle == 1)
                            {
                                centreCircleObsicle(i,j,4,obsticleBase);//centre
                            }
                            else
                            {
                                rightAngleTriangleCornerObsicle(i,j,2,2,1, obsticleBase);
                            }
                       }
                        //for (int k = 0; k < noOfAgents; k++)
                        //{
                        //    //check if new obstacle collides with start/goal state
                        //    if (newObstacle.positionEqual(executiveController.map.startStates[k], true) || newObstacle.positionEqual(executiveController.map.goalStates[k], true))
                        //    {
                        //        bottomCornerRectangleObsicle(i-1, j-1, 3, 3, obsticleBase, 0);
                        //    }
                        //}
                    }
                }
            }
            
            //new obsticle application and checking method 
            checkObsitcles(obsticleBase);
            applyObsicles(obsticleBase);

           
            




            executiveController.setMapDimensions(mapWidth, mapHeight, mapDepth);

            mapRenderer.render(executiveController, this, mainPictureBox);

          

            //Events
            menuStrip1.Items.Add(toolStripMenuItem4);
            //Draw button
            toolStripMenuItem3.Click += ToolStripMenuItem3_Click;
            //start planning button
            toolStripMenuItem4.Click += ToolStripMenuItem4_Click;
            toolStripMenuItem5.Click += ToolStripMenuItem5_Click;
        }

        //*******************************************************
        //*******************************************************
        //*******************************************************
        //*******************************************************

        //"draw + move" button
        private void ToolStripMenuItem5_Click(object sender, EventArgs e)
        {
            //Trace.WriteLine("ITEM 5");
            executiveController.moveAlongPath();
            executiveController.planPath();
            //CBS.checkForConstraints() ?

            mapRenderer.render(executiveController, this, mainPictureBox);
        }

        //"Start planning" button
        private void ToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("ITEM 4");
            loopMovementFlag = !loopMovementFlag;
            if (loopMovementFlag)
            {
                startMoving();
            }
        }

        private void startMoving()
        {
            new Thread(() =>
            {
                Thread.CurrentThread.IsBackground = true;
                executiveController.planPath();
                mapRenderer.render(executiveController, this, mainPictureBox);
                executiveController.moveAlongPath();
                //Thread.Sleep(50);

                if (loopMovementFlag)
                {
                    startMoving();
                }
            }).Start();
        }

        //"draw" button
        private void ToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            Trace.WriteLine("ITEM 3");
            executiveController.planPath();

            mapRenderer.render(executiveController, this, mainPictureBox);
            //executiveController.moveAlongPath();
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            if (!loopMovementFlag)
                mapRenderer.render(executiveController, this, mainPictureBox);

            //Take out the trash cause we're leaving holes in memory
            GC.Collect();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }

        //obsticle geration methods
        // these methods consider up and right to be positive

        //bottom left method
        private void bottomCornerRectangleObsicle(int posRectangle1, int posRectangle2, int width, int hieght, int[,] arrayIn,int filler)
        {
            for (int i = posRectangle1; i <= posRectangle1 + width; i++)
            {
                for (int j = posRectangle2; j < posRectangle2 + hieght; j++)
                { //select all points in area
                    //executiveController.staticObstacles.Add(new StaticObstacle(i, j));
                    if (i < mapWidth && j < mapHeight && filler ==1)
                    {
                        arrayIn[i, j] = 1;
                    }
                    if (i < mapWidth && i >= 0 && j < mapHeight && j >= 0 && filler != 1)
                    {
                        arrayIn[i, j] = 0;
                    }
                }
            }
        }

        //create circle from centre
        //used x^2+y^2=r^2
        private void centreCircleObsicle(int posCircle1,int posCircle2, int radius, int[,] arrayIn)
        {
            int square = (int) Math.Pow(radius, 2);
            for (int i = -radius; i <= radius; i++)
            { // 
                int k = (int) Math.Round(Math.Sqrt(square - Math.Pow(i, 2))); //determine height of collumn
                for (int j = -k; j <= k; j++)
                {
                    //executiveController.staticObstacles.Add(new StaticObstacle(posCircle1 + i, posCircle2 + j));
                    if (posCircle1 + i < mapWidth && posCircle1 +i >=0 && posCircle2 + j < mapHeight && posCircle2+j >=0)
                    {
                        arrayIn[posCircle1 + i, posCircle2 + j] = 1;
                    }
                }
            }
        }

        //1 is bottom left corner, 2 is bottom right corner, 3 is top right corner, 4 is top left corner
        private void rightAngleTriangleCornerObsicle(int corner1,int corner2,int width, int height,int edgeQuad, int[,] arrayIn)
        {
            float gradiant = height / width;
            if (edgeQuad == 1)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height - Math.Round(i * gradiant); j++)
                    {
                        if (corner1 + i < mapWidth && corner2 + j < mapHeight) { 
                            //executiveController.staticObstacles.Add(new StaticObstacle(corner1 + i, corner2 + j));
                            arrayIn[corner1 + i, corner2 + j] = 1;
                            }
                    }
                }
            }
            if (edgeQuad == 2)
            {
                for (int i = 0; i > -width; i--)
                {
                    for (int j = 0; j < height + Math.Round(i * gradiant); j++)
                    {
                        if (corner1 + i < mapWidth && corner2 + j < mapHeight)
                        {
                            //executiveController.staticObstacles.Add(new StaticObstacle(corner1 + i, corner2 + j));
                            arrayIn[corner1 + i, corner2 + j] = 1;
                        }
                    }
                }
            }
            if (edgeQuad == 3)
            {
                for (int i = 0; i > -width; i--)
                {
                    for (int j = 0; j > -height - Math.Round(i * gradiant); j--)
                    {
                        if (corner1 + i < mapWidth && corner2 + j < mapHeight)
                        {
                            //executiveController.staticObstacles.Add(new StaticObstacle(corner1 + i, corner2 + j));
                            arrayIn[corner1 + i, corner2 + j] = 1;
                        }
                    }
                }
            }
            if (edgeQuad == 4)
            {
                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j > -height + Math.Round(i * gradiant); j--)
                    {
                        if (corner1 + i < mapWidth && corner2 + j < mapHeight)
                        {
                            //executiveController.staticObstacles.Add(new StaticObstacle(corner1 + i, corner2 + j));
                            arrayIn[corner1 + i, corner2 + j] = 1;
                        }
                    }
                }
            }
        }
        //checks and removes obsticles around startstates and goal states
        private void checkObsitcles(int[,] arrayIn)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    var newObstacle = new State() { x = i, y = j };
                    for (int k = 0; k < noOfAgents; k++)
                    {
                        //check if new obstacle collides with start/goal state
                        if (newObstacle.positionEqual(executiveController.map.startStates[k], true) || newObstacle.positionEqual(executiveController.map.goalStates[k], true))
                        {
                            bottomCornerRectangleObsicle(i - Globals.droneRadius, j - Globals.droneRadius, 1+2*Globals.droneRadius, 1+2*Globals.droneRadius, arrayIn, 0);
                        }
                    }
                }
            }
        }
        //converts array into obsicles
        private void applyObsicles(int[,] arrayIn)
        {
            for (int i = 0; i < mapWidth; i++)
            {
                for (int j = 0; j < mapHeight; j++)
                {
                    if (arrayIn[i, j] != 0)
                    {
                        Random random = new Random();
                   
                        executiveController.staticObstacles.Add(new StaticObstacle(i, j, random.Next(0,mapDepth+1)));
                    }
                }
            }
        }

        private void mainPictureBox_Click(object sender, EventArgs e)
        {

        }
    }
}
