using System.Collections.Generic;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace EminentDomain.Source
{
    public class CollapsedAbandonedBuildings : MonoBehaviour
    {
        public List<ushort> buildingList { get; set; }

        private void Awake()
        {
            buildingList = new List<ushort>();
        }

       
    
    }
}
