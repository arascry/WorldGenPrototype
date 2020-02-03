using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WorldGenPrototype
{
    class Voronoi
    {
        public List<Vector2> borders { get; }

        private Vector2 worldSize;
        private List<Parabola> parabolas;
        private Random rand;


        public Voronoi(int amount, int seed, Vector2 worldSize)
        {
            this.worldSize = worldSize;

            parabolas = new List<Parabola>();
            rand = new Random(seed);

            for (int i = 0; i < amount; i++)
            {
                Vector2 parLoc = new Vector2(rand.Next((int)worldSize.X), rand.Next((int)worldSize.Y));
                parabolas.Add(new Parabola(parLoc));
            }
            parabolas.OrderBy(parabola => parabola.loc.X);
            foreach (Parabola parabola in parabolas)
                Console.WriteLine("Locs: {0}", parabola.loc.ToString());
        }

        public void CreateVoronoi()
        {
            List<Vector2> points = new List<Vector2>();

            int x = 0;
            int y = 0;

            while(x != worldSize.X + (worldSize.X / 2))
            {
                foreach (Parabola parabola in parabolas)
                {
                    if (y + parabola.loc.Y < worldSize.Y)
                    {
                        double parX = parabola.GetArcX(y, x);
                        if (parX >= 0 && parX < worldSize.X)
                            points.Add(new Vector2((float)parX, y));
                        else
                            continue;
                    }
                }

                foreach (Parabola parabola in parabolas)
                {
                    if (-y + parabola.loc.Y >= 0)
                    {
                        double parX = parabola.GetArcX(-y, x);
                        if (parX >= 0 && parX < worldSize.X)
                            points.Add(new Vector2((float)parX, -y));
                        else
                            continue;
                    }
                }



                y++;
                x++;
            }

        }

    }

    class Parabola
    {
        public Vector2 loc;

        private double adjLoc;
        private bool isDead;

        public Parabola(Vector2 loc)
        {
            this.loc = loc;
        }

        public double GetArcX(double y, double sweepX)
        {
            double arcX;
            adjLoc = (loc.X + sweepX) / 2;

            arcX = (Math.Pow(y, 2) + Math.Pow(loc.X, 2) - Math.Pow(sweepX, 2)) / (2 * (loc.X - sweepX));

            return arcX;
        }
    }
}
