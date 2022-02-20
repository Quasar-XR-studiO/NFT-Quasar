using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ZBoom.Common.SpatialMap
{
    public class FileHelper
    {
        public static string PATH_DOWNLOAD = "Downloads/Maps";
        public static string EXT = "stl";

        public static string GetDownloadedMapPath()
        {
            return $"{Application.persistentDataPath}/{PATH_DOWNLOAD}";
        }

        public static string GetLocalFilePath(string name)
        {
            return $"{Application.persistentDataPath}/{PATH_DOWNLOAD}/{name}";
        }
    }
}