using System;
using SpatialMap_SparseSpatialMap;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ZBoom.Common.SpatialMap
{
    public class PropGroupItemController : MonoBehaviour
    {
        [SerializeField] public TextMeshProUGUI Title;
        [SerializeField] public GameObject Grid;
        
        private void Awake()
        {
        }
    }
}