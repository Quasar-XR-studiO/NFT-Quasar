using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

namespace Rarible
{
    [Serializable]
    public class RaribleItem
    {
        public string id = "";
        public string blockchain = "";
        public string contract = "";
        public int tokenId = 0;
        public List<Creator> creators = new List<Creator>();
        public List<object> owners = new List<object>(); //Owner
        public List<Royalty> royalties = new List<Royalty>();
        public int lazySupply = 0;
        public List<object> pending = new List<object>();
        public string mintedAt = "";
        public string lastUpdatedAt = "";
        public int supply = 0;
        public Meta meta = new Meta();
        public bool deleted = false;
        public BestSellOrder bestSellOrder = new BestSellOrder();
        public BestBidOrder bestBidOrder = new BestBidOrder();
        public List<object> auctions = new List<object>();
        public int totalStock = 0;
        public int sellers = 0;

        public RaribleItem()
        {
        }
    }

    [Serializable]
    public class Meta
    {
        public string name = "";
        public string description = "";
        public List<object> attributes = new List<object>();
        public List<Content> content = new List<Content>();
        public List<object> restrictions = new List<object>();
        public string raw = "";

        public Meta()
        {
        }
    }

    [Serializable]
    public class BestSellOrder
    {
        public string id = "";
        public double fill = 0d;
        public string platform = "";
        public string status = "";
        public string startedAt = "";
        public string endedAt = "";
        public double makeStock = 0d;
        public bool cancelled = false;
        public string createdAt = "";
        public string lastUpdatedAt = "";
        public double makePrice = 0d;
        public double takePrice = 0d;
        public double makePriceUsd = 0d;
        public double takePriceUsd = 0d;
        public List<PriceHistory> priceHistory = new List<PriceHistory>();
        public string maker = "";
        public string taker = "";
        public Make make = new Make();
        public Take take = new Take();
        public string salt = "";
        public string signature = "";
        public List<object> pending = new List<object>();
        public Data data = new Data();

        public BestSellOrder()
        {
        }
    }

    [Serializable]
    public class BestBidOrder
    {
        public string id = "";
        public double fill = 0d;
        public string platform = "";
        public string status = "";
        public string startedAt = "";
        public string endedAt = "";
        public double makeStock = 0d;
        public bool cancelled = false;
        public string createdAt = "";
        public string lastUpdatedAt = "";
        public double makePrice = 0d;
        public double takePrice = 0d;
        public double makePriceUsd = 0d;
        public double takePriceUsd = 0d;
        public List<PriceHistory> priceHistory = new List<PriceHistory>();
        public string maker = "";
        public string taker = "";
        public Make make = new Make();
        public Take take = new Take();
        public string salt = "";
        public string signature = "";
        public List<object> pending = new List<object>();
        public Data data = new Data();

        public BestBidOrder()
        {
        }
    }

    [Serializable]
    public class Make
    {
        public TypeItem type = new TypeItem();
        public string value = "";

        public Make()
        {
        }
    }

    [Serializable]
    public class Take
    {
        public TypeItem type = new TypeItem();
        public string value = "";

        public Take()
        {
        }
    }

    [Serializable]
    public class Data
    {
        //[JsonProperty("@type")]
        public string type = "";
        public string fee = "";
        public List<object> payouts = new List<object>();
        public List<OriginFee> originFees = new List<OriginFee>();

        public Data()
        {
        }
    }

    [Serializable]
    public class OriginFee
    {
        public string account = "";
        public int value = 0;

        public OriginFee()
        {
        }
    }

    [Serializable]
    public class Creator
    {
        public string account = "";
        public int value = 0;

        public Creator()
        {
        }
    }

    [Serializable]
    public class Royalty
    {
        public string account = "";
        public int value = 0;

        public Royalty()
        {
        }
    }

    [Serializable]
    public class Content
    {
        public enum TypeContent
        {
            UNKNOWN,
            IMAGE,
            GIF,
            VIDEO,
            AUDIO,
            MODEL,
            AR,
        }

        public enum Type3dModel
        {
            UNKNOWN,
            OBJ,
            FBX,
            FBX_BIN,
            FBX_ASCII,
            STL,
            GLB,
            GLTF,
        }

        public enum TypeRepresentation
        {
            UNKNOWN,
            SMALL,
            MEDIUM,
            BIG,
            PREVIEW,
            ORIGINAL
        }

        //[JsonProperty("@type")] 
        public string type = ""; //
        public string url = "";
        public string representation = "";
        public string mimeType = "";
        public int size = 0;
        public int width = 0;
        public int height = 0;

        public Content()
        {
        }

        public TypeContent GetType()
        {
            TypeContent typeContent = TypeContent.UNKNOWN;
            string lowerType = type.ToLower();
            switch (lowerType)
            {
                case ("image"):
                {
                    typeContent = TypeContent.IMAGE;
                    break;
                }
                case ("video"):
                {
                    typeContent = TypeContent.VIDEO;
                    break;
                }
                case ("audio"):
                {
                    typeContent = TypeContent.AUDIO;
                    break;
                }
                case ("3d"):
                {
                    typeContent = TypeContent.MODEL;
                    break;
                }
                case ("ar"):
                {
                    typeContent = TypeContent.AR;
                    break;
                }
                default:
                {
                    typeContent = TypeContent.UNKNOWN;
                    break;
                }
            }

            return typeContent;
        }

        public TypeRepresentation GetRepresentationType()
        {
            TypeRepresentation representationType = TypeRepresentation.UNKNOWN;
            string lowerType = representation.ToLower();
            switch (lowerType)
            {
                case ("small"):
                {
                    representationType = TypeRepresentation.SMALL;
                    break;
                }
                case ("medium"):
                {
                    representationType = TypeRepresentation.MEDIUM;
                    break;
                }
                case ("big"):
                {
                    representationType = TypeRepresentation.BIG;
                    break;
                }
                case ("original"):
                {
                    representationType = TypeRepresentation.ORIGINAL;
                    break;
                }
                case ("preview"):
                {
                    representationType = TypeRepresentation.PREVIEW;
                    break;
                }
                default:
                {
                    representationType = TypeRepresentation.UNKNOWN;
                    break;
                }
            }

            return representationType;
        }

        public Type3dModel Get3dModelType()
        {
            string lowerMimeType = mimeType.ToLower();
            Type3dModel type3dModel = Type3dModel.UNKNOWN;

            if (lowerMimeType.Contains("obj"))
            {
                return type3dModel = Type3dModel.OBJ;
            }

            if (lowerMimeType.Contains("fbx"))
            {
                return type3dModel = Type3dModel.FBX;
            }

            if (lowerMimeType.Contains("stl"))
            {
                return type3dModel = Type3dModel.STL;
            }

            if (lowerMimeType.Contains("glb"))
            {
                return type3dModel = Type3dModel.GLB;
            }

            if (lowerMimeType.Contains("gltf"))
            {
                return type3dModel = Type3dModel.GLTF;
            }

            return type3dModel;
        }
    }

    [Serializable]
    public class PriceHistory
    {
        public string date = "";
        public string makeValue = "";
        public string takeValue = "";

        public PriceHistory()
        {
        }
    }

    [Serializable]
    public class TypeItem
    {
        //[JsonProperty("@type")]
        public string type = "";
        public string contract = "";
        public string tokenId = "";

        public TypeItem()
        {
        }
    }
}