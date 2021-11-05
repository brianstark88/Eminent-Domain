namespace ExtendedBuildings
{
    using ColossalFramework;
    using ColossalFramework.UI;
    using EminentDomain.Source;
    using System;
    using System.Reflection;
    using UnityEngine;

    public class BuildingInfoWindow5 : UIPanel
    {
        const float vertPadding = 26;
      
        public ZonedBuildingWorldInfoPanel baseBuildingWindow;
        FieldInfo baseSub;
        UILabel eminentDomainLabel;
        UILabel landAreaLabel;

        ushort selectedBuilding;
        bool showDescription = true;

        public override void Awake()
        {
            eminentDomainLabel = AddUIComponent<UILabel>();
            landAreaLabel = AddUIComponent<UILabel>();
            base.Awake();

        }



        public override void Start()
        {
            base.Start();

            backgroundSprite = "MenuPanel2";
            opacity = 0.8f;
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            SetupControls();
        }

        public void SetupControls()
        {
            base.Start();
            float y = 10;

            //SetLabel(EminentDomainLabel, "Happiness");
            eminentDomainLabel.textScale = 0.65f;
            eminentDomainLabel.wordWrap = true;
            eminentDomainLabel.autoSize = false;
            eminentDomainLabel.width = this.size.x;
            eminentDomainLabel.wordWrap = true;
            eminentDomainLabel.autoHeight = true;
            eminentDomainLabel.anchor = (UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right);
            
            y += vertPadding;
            landAreaLabel.textScale = 0.65f;
            landAreaLabel.wordWrap = true;
            landAreaLabel.autoSize = false;
            landAreaLabel.width = this.size.x;
            landAreaLabel.wordWrap = true;
            landAreaLabel.autoHeight = true;
            landAreaLabel.anchor = (UIAnchorStyle.Top | UIAnchorStyle.Left | UIAnchorStyle.Right);

            height = y;

        }

        private void SetPos(UILabel title, float x, float y, bool visible)
        {
            
            title.relativePosition = new Vector3(x, y);
            
        }

        public override void Update()
        {
            //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "Here");
            var instanceId = GetParentInstanceId();
            if (instanceId.Type == InstanceType.Building && instanceId.Building != 0)
            {
                ushort building = instanceId.Building;
                if (this.baseBuildingWindow != null && this.enabled && isVisible && Singleton<BuildingManager>.exists && ((Singleton<SimulationManager>.instance.m_currentFrameIndex & 15u) == 15u || selectedBuilding != building))
                {
                    BuildingManager instance = Singleton<BuildingManager>.instance;
                    this.UpdateBuildingInfo(building, instance.m_buildings.m_buffer[(int)building]);
                    selectedBuilding = building;
                }
            }

            

            base.Update();
        }

        private void UpdateBuildingInfo(ushort buildingId, Building building)
        {
            
            
            //var zone = info.m_class.GetZone();
            //Building building = Singleton<BuildingManager>.instance.m_buildings.m_buffer[(int)buildingId];
            ushort[] array;
            int num;
            Singleton<ImmaterialResourceManager>.instance.CheckLocalResources(building.m_position, out array, out num);

            var x = 14f;
            float y = 1f;
            SetPos(eminentDomainLabel, x, y, true);
            y += vertPadding;
            SetPos(landAreaLabel, x, y, true);


            if (this.baseBuildingWindow != null)
            {
                if (showDescription)
                {
                    

                    landAreaLabel.text = "Land Area: " + String.Format("{0:n0}", building.Info.m_cellLength * building.Info.m_cellWidth * 64) + "m2";
                    landAreaLabel.Show();
                    landAreaLabel.relativePosition = new Vector3(x, y);
                    y += eminentDomainLabel.height + 10;

                    eminentDomainLabel.text = @"Eminent Domain: " + String.Format("{0:n0}", -1 * ChargeEminentDomain.CalculateEminentDomain(buildingId));
                    eminentDomainLabel.Show();
                    eminentDomainLabel.relativePosition = new Vector3(x, y);
                    
                }
            }
            height = y + 20;

        }


        private InstanceID GetParentInstanceId()
        {
            if (baseSub == null)
            {
                baseSub = this.baseBuildingWindow.GetType().GetField("m_InstanceID", BindingFlags.NonPublic | BindingFlags.Instance);
            }
            return (InstanceID)baseSub.GetValue(this.baseBuildingWindow);
        }

    }
}
