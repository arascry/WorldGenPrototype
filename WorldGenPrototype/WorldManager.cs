using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace WorldGenPrototype
{
    class WorldManager
    {
        public int tileSize;
        public Vector2 worldSize;


        ContinentManager continentManager;

        IDictionary<Vector2, Vector2> pos;
        IDictionary<Vector2, double> heights;
        IDictionary<Vector2, Color> colors;
        Texture2D texture;
        Perlin map;

        private GraphicsDevice graphicsDevice;
        private SpriteBatch spriteBatch;
        private ContentManager content;

        private Random rand = new Random(121);

        public WorldManager(SpriteBatch spriteBatch, ContentManager content)
        {
            pos = new Dictionary<Vector2, Vector2>();
            heights = new Dictionary<Vector2, double>();
            colors = new Dictionary<Vector2, Color>();


            this.spriteBatch = spriteBatch;
            this.content = content;

            graphicsDevice = spriteBatch.GraphicsDevice;
            texture = this.content.Load<Texture2D>("pixel");
            tileSize = texture.Width;
        }

        public void CreateWorld(Vector2 worldSize)
        {
            List<Vector2> seedLocs = new List<Vector2>();

            int seed = 9;

            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    Vector2 location = new Vector2(i, j);
                    pos.Add(location, new Vector2(i * tileSize, j * tileSize));
                    heights.Add(location, 100);
                }
            }

            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    colors.Add(new Vector2(i, j), new Color((int)heights[new Vector2(i, j)], 0, 0));
                }
            }

            Voronoi tester = new Voronoi(5, seed, worldSize);
            tester.CreateVoronoi();

        }

        public class ContinentManager
        {
            public ContinentManager()
            {

            }
        }

        /*public void CreateWorld(Vector2 totalSize)
        {
            worldSize = totalSize;
            map = new Perlin();
            double seed = rand.Next(0, 1000);
            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {
                    Vector2 location = new Vector2(i, j);
                    double preHeight = map.OctavePerlin(((double)i + seed) / totalSize.X, ((double)j + seed) / worldSize.Y, 0, 8, .85) * 255;

                    if (preHeight > 130)
                        preHeight += 10;
                    else
                        preHeight -= (map.OctavePerlin(((double)i + seed + seed) / totalSize.X, ((double)j + seed + seed) / worldSize.Y, 0, 3, .3) * 50);

                    pos.Add(location, new Vector2(i * tileSize, j * tileSize));
                    heights.Add(location, preHeight);

                }
            }

            for (int i = 0; i < totalSize.X; i++)
                heights[new Vector2(i, 0)] = 0;

            for (int i = 0; i < totalSize.Y; i++)
                heights[new Vector2(0, i)] = 0;

            for (int i = 0; i < totalSize.X; i++)
                heights[new Vector2(i, totalSize.Y - 1)] = 0;

            for (int i = 0; i < totalSize.Y; i++)
                heights[new Vector2(totalSize.X - 1, i)] = 0;


            for (int i = 0; i < worldSize.X; i++)
            {
                for (int j = 0; j < worldSize.Y; j++)
                {

                    colors.Add(new Vector2(i, j), new Color((int)heights[new Vector2(i, j)], 0, 0));
                }
            }


            continentManager = new ContinentManager(heights);
            for (int i = 0; i < continentManager.retList.Count; i++)
            {
                colors[continentManager.retList.ElementAt(i)] = new Color(0, 100, 0);
            }
        }

        public class ContinentManager
        {
            public LinkedList<Vector2> retList;
            public List<LinkedList<Vector2>> continents;

            private List<Vector2> directions = new List<Vector2>()
            {
                new Vector2(0, -1),      //N
                new Vector2(1, -1),      //NE
                new Vector2(1, 0),       //E
                new Vector2(1, 1),       //SE
                new Vector2(0, 1),       //S
                new Vector2(-1, 1),      //SW
                new Vector2(-1, 0),      //W
                new Vector2(-1, -1)      //NW
            };

            IDictionary<Vector2, double> heights;

            private Vector2 prevVec;
            private int limit { get; set; }

            public ContinentManager(IDictionary<Vector2, double> heights)
            {
                this.heights = heights;
                LinkedList<Vector2> retList = CreateContinent(40);

            }

            LinkedList<Vector2> CreateContinent(int limit)
            {
                LinkedList<Vector2> continent = new LinkedList<Vector2>();
                Vector2 currentVec = new Vector2(1, 1);
                bool immobile = true;

                this.limit = limit;

                prevVec = new Vector2(0, 0);
                do
                {
                    if (continent.Count == limit)
                        break;


                    foreach (Vector2 dir in directions)
                    {

                        if (TryMove(currentVec, dir, continent))
                        {
                            currentVec = Move(currentVec, dir);
                            continent.AddLast(currentVec);
                            immobile = false;
                            break;
                        }

                        immobile = true;
                    }
                    
                    if (continent.Count != 0 && immobile)
                    {
                        //Retract
                        for (int i = continent.Count - 1; i >= 0; i--)
                        {
                            currentVec = continent.ElementAt(i);
                            foreach (Vector2 dir in directions)
                            {
                                //Console.WriteLine("Trying: {0}", currentVec + dir);
                                if (TryMove(currentVec, dir, continent))
                                {
                                    currentVec = Move(currentVec, dir);
                                    continent.AddLast(currentVec);
                                    i = -1;
                                    break;
                                }
                            }
                        }
                    }
                    else if(continent.Count == 0)
                    {
                        bool hitLand = false;
                        while (!hitLand)
                        {
                            if (!IsOcean(heights[currentVec]))
                                hitLand = true;

                            if (!heights[currentVec + new Vector2(1, 0)].Equals(null))
                            {
                                currentVec = Move(currentVec, new Vector2(1, 0));
                            }
                            else if (!heights[new Vector2(0, 1)].Equals(null))
                            {
                                prevVec = currentVec;
                                currentVec = new Vector2(0, 1);
                            }
                            else
                            {
                                return continent;
                            }
                        }

                        continent.AddLast(currentVec);
                    }
                    Console.WriteLine("Prev: {0} |=| Curr: {1}", prevVec.ToString(), currentVec.ToString());
                    Console.WriteLine("Value: {0}", continent.Count);
                    Console.WriteLine("Last: {0}", continent.Last.Value.ToString());
                    //Console.WriteLine("===");
                } while (!continent.First.Equals(currentVec));



                return continent;
            }

            bool IsOcean(double height)
            {
                if(height < 128)
                {
                    return true;
                }
                return false;
            }



            private Vector2 Move(Vector2 currentPos, Vector2 direction)
            {
                prevVec = currentPos;
                return currentPos + direction;
            }

            private bool TryMove(Vector2 currentPos, Vector2 direction, LinkedList<Vector2> continent)
            {
                if (!(currentPos + direction).Equals(prevVec)
                  && !IsOcean(heights[currentPos + direction])
                  && !continent.Contains(currentPos + direction))
                    return true;
                return false;
            }

        }*/


        public double ReturnHeight(Vector2 pos)
        {
            try
            {
                return heights[pos];
            }
            catch(Exception e)
            {
                Console.Write("Invalid Selection");
                return 0;
            }
        }

        public void Draw()
        {
            foreach(KeyValuePair<Vector2, Vector2> position in pos)
            {
                spriteBatch.Draw(texture, position.Value, colors[position.Key]);
            }
        }


    }
}
