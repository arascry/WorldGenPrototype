using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace WorldGenPrototype
{
    class EventManager
    {

    }

    public class WorldClickEventArgs : EventArgs
    {
        public Vector2 clickPos { get; set; }
    }
}
