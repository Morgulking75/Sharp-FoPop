using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sharp_FoPop
{
    public class Bitfield
    {
        public int bitnum;
        public string[] credits;
        public string freespins;
        public int seconds;
        public string bins;
        public bool progressive;
        public int position;

        public Bitfield(int bit, int pos)
        {
            bitnum = bit;
            position = pos;
            seconds = 4;
            credits = new string[5];
            freespins = "0";
            progressive = false;
        }

        public void popCredits(int loc, string val)
        {
            credits[loc] = val;
        }

        public void popSpins( string val)
        {
            freespins = val;
            calcTime();
        }

        private void calcTime()
        {
            if (freespins.Equals("5")) seconds = 26;
            else if (freespins.Equals("10")) seconds = 28;
            else if (freespins.Equals("20")) seconds = 36;
        }
    }
}
