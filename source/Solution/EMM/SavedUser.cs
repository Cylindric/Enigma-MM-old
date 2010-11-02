using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibNbt.Tags;

namespace EnigmaMM
{
    class SavedUser
    {
        private string mUsername;
        private double mPosX;
        private double mPosY;
        private double mPosZ;

        public void LoadData(string filename)
        {
            // The username isn't in the file, it's just in the filename
            mUsername = Path.GetFileNameWithoutExtension(filename);

            // Extract the position data from the NBT file for the player
            LibNbt.NbtFile nbtFile = new LibNbt.NbtFile();
            nbtFile.LoadFile(filename);
            NbtCompound root = nbtFile.RootTag;
            NbtList pos = (NbtList)root["Pos"];
            NbtDouble coordx = (NbtDouble)pos[0];
            NbtDouble coordy = (NbtDouble)pos[1];
            NbtDouble coordz = (NbtDouble)pos[2];

            mPosX = coordx.Value;
            mPosY = coordy.Value;
            mPosZ = coordz.Value;
        }

    }
}
