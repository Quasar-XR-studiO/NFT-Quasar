using System;
using System.Collections.Generic;

namespace Rarible
{
    [Serializable]
    public class RaribleCollection
    {

        public List<RaribleItem> items = new List<RaribleItem>();
        
        public RaribleCollection()
        {
            
        }
    }
}