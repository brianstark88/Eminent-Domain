using ColossalFramework.UI;
using ICities;
using System;
using UnityEngine;

namespace EminentDomain.Source
{
    public class Loading : LoadingExtensionBase
    {

        private GameObject _gameObject;


        public override void OnLevelLoaded(LoadMode mode)
        {
            try
            {

                UILabel objectOfType = UnityEngine.Object.FindObjectOfType<UILabel>();
                if (objectOfType != null)
                {
                    _gameObject = new GameObject("CollapsedAbandonedBuildings");
                    _gameObject.transform.parent = objectOfType.transform;
                    _gameObject.AddComponent<CollapsedAbandonedBuildings>();
                }
            }
            catch (Exception e)
            {
                Debug.Log("[Bulldoze It!] Loading:OnLevelLoaded -> Exception: " + e.Message);
            }
        }



    }
}
