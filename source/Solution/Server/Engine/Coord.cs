using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt.Tags;

namespace EnigmaMM.Engine
{
    class Coord
    {
        private int mX;
        private int mY;
        private int mZ;

        public Coord()
        {
            mX = 0;
            mY = 0;
            mZ = 0;
        }

        public Coord(int x, int y, int z)
        {
            mX = x;
            mY = y;
            mZ = z;
        }

        public Coord(EMMServerMessage message)
        {
            Coord coord;
            if (Coord.TryParse(message, out coord))
            {
                mX = coord.X;
                mY = coord.Y;
                mZ = coord.Z;
            }
            else
            {
                throw new ArgumentException("Invalid message passed");
            }
        }

        public Coord(NbtTag x, NbtTag y, NbtTag z)
        {
            mX = (int)Math.Floor(((NbtDouble)x).Value);
            mY = (int)Math.Floor(((NbtDouble)y).Value);
            mZ = (int)Math.Floor(((NbtDouble)z).Value);
        }

        public int X
        { 
            get { return mX; }
            set { mX = value; }
        }

        public int Y
        {
            get { return mY; }
            set { mY = value; }
        }

        public int Z
        {
            get { return mZ; }
            set { mZ = value; }
        }

        public static bool TryParse(EMMServerMessage message, out Coord coord)
        {
            coord = new Coord();
            double x = 0;
            double y = 0;
            double z = 0;
            try
            {
                double.TryParse(message.Data["LocX"], out x);
                double.TryParse(message.Data["LocY"], out y);
                double.TryParse(message.Data["LocZ"], out z);
            }
            catch
            {
                return false;
            }
            coord.X = (int)Math.Floor(x);
            coord.Y = (int)Math.Floor(y);
            coord.Z = (int)Math.Floor(z);
            return true;
        }

    }
}
