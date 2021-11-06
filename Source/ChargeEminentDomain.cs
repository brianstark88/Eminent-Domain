using ICities;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.Plugins;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EminentDomain.Source
{
    public class ChargeEminentDomain : BuildingExtensionBase
    {
        public override void OnBuildingReleased(ushort buildingId)
        {
            try
            {
                BuildingManager buildingManager = Singleton<BuildingManager>.instance;
                Building building = buildingManager.m_buildings.m_buffer[buildingId];
                BuildingInfo buildingInfo = building.Info;
                DistrictManager districtManager = Singleton<DistrictManager>.instance;
                District district = districtManager.m_districts.m_buffer[districtManager.GetDistrict(building.m_position)];
                int eminentDomain = 0;


                if (!(buildingInfo.m_class.m_service == ItemClass.Service.Office || buildingInfo.m_class.m_service == ItemClass.Service.Residential || buildingInfo.m_class.m_service == ItemClass.Service.Commercial || buildingInfo.m_class.m_service == ItemClass.Service.Industrial))
                    return;

                //if (IsCollapsedAbandoned(buildingId))
                //{
                //    Debug.Log("Building Collapsed or Abandoned: Eminent Domain Not Charged.");
                //    return;
                //}


                if (!IsZoned(building, buildingInfo))
                {
                    eminentDomain = CalculateEminentDomain(buildingId);
                    Charge(eminentDomain, buildingInfo.m_class, "Zone check failed", district, building);
                    return;
                }

                if (!(IsCorrectDistrict(buildingInfo, district)))
                {
                    eminentDomain = CalculateEminentDomain(buildingId);
                    Charge(eminentDomain, buildingInfo.m_class, "Invalid District", district, building);
                    return;
                }

                if (BulldozeToolActive())
                {
                    eminentDomain = CalculateEminentDomain(buildingId);
                    Charge(eminentDomain, buildingInfo.m_class, "Bulldozed", district, building);
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:OnBuildingReleased -> Exception: " + e.Message + " " + e.StackTrace);
            }
            

        }

        // Cases:
        // 1. Collapsed / Abandoned
        // 2. Dezoned 
        // 3. Wrong District Policy 
        // 4. Bulldozed

        private void Charge(int amount, ItemClass m_class, string message, District district, Building building)
        {
            try
            {
                if (amount != 0)
                {


                    var dLandValue = district.GetLandValue();
                    var hasHighRiseBan = district.IsPolicySet(DistrictPolicies.Policies.HighriseBan);

                    Debug.Log("Eminent Domain -------------------- ");
                    Debug.Log("Building Name : " + building.Info.name);
                    Debug.Log("Eminent Domain Reason: " + message);
                    Debug.Log("Service : " + building.Info.m_class.m_service.ToString());
                    Debug.Log("SubService : " + building.Info.m_class.m_subService.ToString());
                    Debug.Log("Land Value : " + dLandValue.ToString());
                    Debug.Log("Cells: " + (building.m_length * building.m_width).ToString());
                    Debug.Log("Has Highrise Ban: " + hasHighRiseBan.ToString());
                    Debug.Log("Eminent Domain: " + (-1 * amount).ToString());
                    Debug.Log("");



                    Singleton<EconomyManager>.instance.AddResource(EconomyManager.Resource.RefundAmount, amount, m_class.m_service, m_class.m_subService, ItemClass.Level.None, district.m_taxationPolicies);
                }
                    
            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:Charge -> Exception: " + e.Message + " " + e.StackTrace);
            }
        }


        public bool IsCollapsedAbandoned(ushort buildingId)
        {


            try
            {
                CollapsedAbandonedBuildings _buildingIdsGameObject = GameObject.Find("EDCollapsedAndAbandoned").GetComponent<CollapsedAbandonedBuildings>();

                var buildingList = _buildingIdsGameObject.buildingList;

                DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, buildingList.Count().ToString());

                if (buildingList.Contains(buildingId))
                {
                    buildingList.Remove(buildingId);
                    return true;
                }
                return false;

            }


            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:IsCollapsedAbandoned -> Exception: " + e.Message + " " + e.StackTrace);
                return false;
            }           

        }

        public bool IsZoned(Building building, BuildingInfo buildingInfo)
        {
            try
            {
                return (building.CheckZoning(buildingInfo.m_class.GetZone(), buildingInfo.m_class.GetSecondaryZone(), false)); ;
            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:IsZoned -> Exception: " + e.Message + " " + e.StackTrace);
            }

            return true;
        }

        public bool IsCorrectDistrict(BuildingInfo buildingInfo, District district)
        {

            try
            {

                var specialDistricts = new List<ItemClass.SubService>();
                specialDistricts.Add(ItemClass.SubService.ResidentialLowEco);
                specialDistricts.Add(ItemClass.SubService.IndustrialFarming);
                specialDistricts.Add(ItemClass.SubService.IndustrialForestry);
                specialDistricts.Add(ItemClass.SubService.IndustrialOil);
                specialDistricts.Add(ItemClass.SubService.IndustrialOre);
                specialDistricts.Add(ItemClass.SubService.CommercialLeisure);
                specialDistricts.Add(ItemClass.SubService.CommercialTourist);
                specialDistricts.Add(ItemClass.SubService.ResidentialHighEco);
                specialDistricts.Add(ItemClass.SubService.OfficeHightech);

                ItemClass.Service service = buildingInfo.m_class.m_service;
                ItemClass.SubService subService = buildingInfo.m_class.m_subService;

                if (!(specialDistricts.Contains(subService)))
                    return true;

                switch (subService)
                {
                    case ItemClass.SubService.ResidentialLowEco:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Selfsufficient;
                    case ItemClass.SubService.IndustrialFarming:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Farming;
                    case ItemClass.SubService.IndustrialForestry:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Forest;
                    case ItemClass.SubService.IndustrialOil:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Oil;
                    case ItemClass.SubService.IndustrialOre:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Ore;
                    case ItemClass.SubService.CommercialLeisure:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Leisure;
                    case ItemClass.SubService.CommercialTourist:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Tourist;
                    case ItemClass.SubService.ResidentialHighEco:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Selfsufficient;
                    case ItemClass.SubService.OfficeHightech:
                        return district.m_specializationPolicies == DistrictPolicies.Specialization.Hightech;
                }
                return true;
            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:IsCorrectDistrict -> Exception: " + e.Message + " " + e.StackTrace);
            }
            return true;

        }

        

        public bool BulldozeToolActive()
        {
            try
            {
                BulldozeTool bulldozeTool = Singleton<BulldozeTool>.instance;
                return bulldozeTool.enabled;
            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:BulldozeToolActive -> Exception: " + e.Message + " " + e.StackTrace);
            }
            return false;
            
        }




        public static int CalculateEminentDomain(ushort buildingId)
        {
            try
            {
                BuildingManager buildingManager = Singleton<BuildingManager>.instance;
                Building building = buildingManager.m_buildings.m_buffer[buildingId];
                BuildingInfo buildingInfo = building.Info;
                ItemClass.Service buildingService = buildingInfo.GetService();
                ItemClass.SubService subService = buildingInfo.GetSubService();
                DistrictManager districtManager = Singleton<DistrictManager>.instance;
                District district = districtManager.m_districts.m_buffer[districtManager.GetDistrict(building.m_position)];

                int eminentDomain = 0;

                // District land value * 64 m^2 * length * width
                int landValue = district.GetLandValue() * 64 * buildingInfo.m_cellLength * buildingInfo.m_cellWidth;



                //DebugOutputPanel.AddMessage(PluginManager.MessageType.Message, "AutoRemove :" + buildingInfo.m_autoRemove);

                if (buildingService == ItemClass.Service.Office || buildingService == ItemClass.Service.Residential || buildingService == ItemClass.Service.Commercial || buildingService == ItemClass.Service.Industrial)// && building.CheckZoning(ItemClass.Zone.ResidentialLow, ItemClass.Zone.ResidentialHigh, false) == false)
                {
                    if (buildingService != ItemClass.Service.Office && (subService == ItemClass.SubService.ResidentialLow ||
                        subService == ItemClass.SubService.ResidentialLowEco ||
                        subService == ItemClass.SubService.CommercialLow ||
                        subService == ItemClass.SubService.CommercialEco ||
                        subService == ItemClass.SubService.IndustrialFarming ||
                        subService == ItemClass.SubService.IndustrialForestry ||
                        subService == ItemClass.SubService.IndustrialGeneric ||
                        subService == ItemClass.SubService.IndustrialOil ||
                        subService == ItemClass.SubService.IndustrialOre))
                    {
                        eminentDomain = -1 * landValue;
                    }
                    else if (
                            buildingService != ItemClass.Service.Office ||
                            subService == ItemClass.SubService.CommercialHigh ||
                            subService == ItemClass.SubService.CommercialLeisure ||
                            subService == ItemClass.SubService.CommercialTourist ||
                            subService == ItemClass.SubService.ResidentialHigh ||
                            subService == ItemClass.SubService.ResidentialHighEco ||
                            subService == ItemClass.SubService.OfficeGeneric ||
                            subService == ItemClass.SubService.OfficeHightech)
                    {


                        if (district.IsPolicySet(DistrictPolicies.Policies.HighriseBan))
                        {
                            eminentDomain = (int)(-1.5 * landValue);
                        }
                        else
                        {
                            eminentDomain = -2 * landValue;
                        }
                    }
                }


                return eminentDomain * 100;
            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: ChargeEminentDomain:CalculateEminentDomain -> Exception: " + e.Message + " " + e.StackTrace);
            }

            return 0;
            
        }
       
    }
}
