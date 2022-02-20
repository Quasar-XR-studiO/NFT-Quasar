using System;
using System.Collections.Generic;

namespace ZBoom.Common.SpatialMap
{
    [Serializable]
    public class SpatialMapContainer
    {
        public List<SpatialMapData> spatialMaps = new List<SpatialMapData>();

        public SpatialMapContainer()
        {
        }
    }
}