using System;
using AllArt.Solana.Nft;
using Rarible;

namespace ZBoom.Common.SpatialMap
{
    [Serializable]
    public class SpatialMapItemData
    {
        public string id = "";
        public string name = "";

        public bool isNft = false;
        //public RaribleItem nft = new RaribleItem();
        public RaribleItem raribleNft = null;
        public Nft solanaNft = null;
        
        public float[] position = new float[3];
        public float[] rotation = new float[4];
        public float[] eulerRotation = new float[3];
        public float[] scale = new float[3];

        public float positionX = 0f;
        public float positionY = 0f;
        public float positionZ = 0f;

        public float rotationX = 0f;
        public float rotationY = 0f;
        public float rotationZ = 0f;

        public float scaleX = 0f;
        public float scaleY = 0f;
        public float scaleZ = 0f;

        public SpatialMapItemData()
        {
        }
    }
}