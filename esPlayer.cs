using System;
using System.Collections.Generic;
using TShockAPI;

namespace TestPlugin //Using parts of Essentials' code
{
    public class esPlayer
    {
        public int Index { get; set; }
        public double ptTime { get; set; }
        public bool ptDay { get; set; }

        public esPlayer(int index)
        {
            this.Index = index;
            this.ptTime = -1.0;
            this.ptDay = true;
        }
    }
}
