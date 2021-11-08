using ColossalFramework.UI;
using EminentDomain.Source;
using ICities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ExtendedBuildings
{
    public class ExtendedLoading : LoadingExtensionBase
    {
        static GameObject buildingWindowGameObject;
        BuildingInfoWindow5 buildingWindow;
        private LoadMode _mode;
        private GameObject _gameObject;

        //public class ExtendedLoadingException : Exception
        //{
        //    public ExtendedLoadingException(string message) : base(message) { }
        //}



        private static IEnumerable<UIPanel> GetUIPanelInstances()
        {
            return UIView.library.m_DynamicPanels
                .Select(p => p.instance).OfType<UIPanel>(); 
        }
    
        //private static string[] GetUIPanelNames() => GetUIPanelInstances().Select(p => p.name).ToArray();
        private UIPanel GetPanel(string name)
        {
            return GetUIPanelInstances().FirstOrDefault(p => p.name == name);
        }

        public override void OnCreated(ILoading loading)
        {
            BuildingManager objectOfType = UnityEngine.Object.FindObjectOfType<BuildingManager>();
            Debug.Log("objectOfType is null: " + (objectOfType == null).ToString());

            if (objectOfType != null)
            {
                _gameObject = new GameObject("collapsedBuildingsObject");
                _gameObject.transform.parent = objectOfType.transform;
                _gameObject.AddComponent<CollapsedAbandonedBuildings>();
                _gameObject.SetActive(true);
               

            }
            var gameObject = GameObject.Find("collapsedBuildingsObject").GetComponent<CollapsedAbandonedBuildings>();
            Debug.Log("_gameObject list length: " + gameObject.buildingList.Count().ToString());

            base.OnCreated(loading);
        }

        public override void OnLevelLoaded(LoadMode mode)
        {
            if (mode != LoadMode.LoadGame && mode != LoadMode.NewGame)
                return;

            _mode = mode;

            buildingWindowGameObject = new GameObject("buildingWindowObject");

            var buildingInfo = UIView.Find<UIPanel>("(Library) ZonedBuildingWorldInfoPanel");
            
            this.buildingWindow = buildingWindowGameObject.AddComponent<BuildingInfoWindow5>();
            this.buildingWindow.transform.parent = buildingInfo.transform;
            this.buildingWindow.size = new Vector3(buildingInfo.size.x, buildingInfo.size.y);
            this.buildingWindow.baseBuildingWindow = buildingInfo.gameObject.transform.GetComponentInChildren<ZonedBuildingWorldInfoPanel>();
            this.buildingWindow.position = new Vector3(0, 12);
            buildingInfo.eventVisibilityChanged += buildingInfo_eventVisibilityChanged;

            var serviceBuildingInfo = GetPanel("(Library) CityServiceWorldInfoPanel");//UIView.Find<UIPanel>("(Library) CityServiceWorldInfoPanel");
            

            


        }

        public override void OnLevelUnloading()
        {
            if (_mode != LoadMode.LoadGame && _mode != LoadMode.NewGame)
                return;

            if (buildingWindow != null)
            {
                if (this.buildingWindow.parent != null)
                {
                    this.buildingWindow.parent.eventVisibilityChanged -= buildingInfo_eventVisibilityChanged;
                }
            }

            if (buildingWindowGameObject != null)
            {
                GameObject.Destroy(buildingWindowGameObject);
            }
        }

        void buildingInfo_eventVisibilityChanged(UIComponent component, bool value)
        {
            this.buildingWindow.isEnabled = value;
            if (value)
            {
                this.buildingWindow.Show();
            }
            else
            {
                this.buildingWindow.Hide();
            }
        }

    }
}
