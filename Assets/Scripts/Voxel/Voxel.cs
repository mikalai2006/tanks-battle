using System;
using UnityEngine;

namespace Mikalai2006.Voxel
{
    [Serializable]
    public struct Voxel
    {
        public byte ID;
        public Color color;
        public int IndexSubMesh;

        public bool isSolid
        {
            get
            {
                return ID != 0;
            }
        }
    }
}