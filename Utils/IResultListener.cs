using System;

namespace ZBoom.Common.SpatialMap
{
    public class IResultListener<T>
    {
        public Action<T, string> OnSuccess;

        public Action<string> OnError;

        public Action OnFinish;
    }
}