using System;
using System.Collections.Generic;
using Rarible;

namespace ZBoom.Common.SpatialMap
{
    [Serializable]
    public class SpatialMapData
    {
        public string id = "";
        public string name = "";
        public string createdDate = "";
        public string updatedDate = "";
        public string urlPreview = "";
        public SpatialMapMeshData mesh = new SpatialMapMeshData();

        public List<SpatialMapItemData> mapItems = new List<SpatialMapItemData>();

        public SpatialMapData()
        {
        }
    }
}