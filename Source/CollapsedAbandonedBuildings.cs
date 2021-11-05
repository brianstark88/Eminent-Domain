using System.Collections.Generic;
using ColossalFramework.UI;
using System;
using UnityEngine;

namespace EminentDomain.Source
{
    public class CollapsedAbandonedBuildings : MonoBehaviour
    {
        public List<ushort> buildingList;

        private void Awake()
        {
            buildingList = new List<ushort>();
        }
    }
}
