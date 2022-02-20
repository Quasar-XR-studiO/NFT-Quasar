using System;
using System.Collections.Generic;

namespace ZBoom.Common.SpatialMap
{
    [Serializable]
    public class SpatialMapMeshData
    {
        public string shortUrlMesh = "";
        public string fullUrlMesh = "";
        public string nameMesh = "";

        public float[] position = new float[3];
        public float[] rotation = new float[4];
        public float[] eulerRotation = new float[3];
        public float[] scale = new float[3];
        
        public SpatialMapMeshData()
        {
        }
    }
}