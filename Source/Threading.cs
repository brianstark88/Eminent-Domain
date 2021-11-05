using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EminentDomain.Source
{
    public class Threading : ThreadingExtensionBase
    {
        private List<ushort> _buildingIds;
        private BuildingManager _buildingManager;
        private Building _building;

        public override void OnCreated(IThreading threading)
        {

            try
            {
                _buildingIds = new List<ushort>();
                _buildingManager = Singleton<BuildingManager>.instance;


            }
            catch (Exception e)
            {
                Debug.Log("Eminent Domain: Threading:OnCreated -> Exception: " + e.Message + " " + e.StackTrace);
            }

        }


        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {

            try
            {
                for (ushort i = 0; i < _buildingManager.m_buildings.m_buffer.Length; i++)
                {
                    _building = _buildingManager.m_buildings.m_buffer[i];

                    if (IsRICOBuilding(_building))
                    {
                        if ((_building.m_flags & Building.Flags.Abandoned) != Building.Flags.None)
                        {
                            _buildingIds.Add(i);
                        }
                        else if ((_building.m_flags & Building.Flags.BurnedDown) != Building.Flags.None || (_building.m_flags & Building.Flags.Collapsed) != Building.Flags.None)
                        {

                            if ((_building.m_problems & Notification.Problem.Fire) != Notification.Problem.None)
                            {
                                _buildingIds.Add(i);
                            }
                            else if (((_building.m_problems & Notification.Problem.StructureDamaged) != Notification.Problem.None || (_building.m_problems & Notification.Problem.StructureVisited) != Notification.Problem.None))
                            {
                                _buildingIds.Add(i);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.Log("EminentDomain: Threading:OnUpdate -> Exception: " + e.Message + " " + e.StackTrace);
            }



        }

        private bool IsRICOBuilding(Building building)
        {

            try
            {
                bool isRICO = false;

                switch (building.Info.m_class.m_service)
                {
                    case ItemClass.Service.Residential:
                    case ItemClass.Service.Commercial:
                    case ItemClass.Service.Industrial:
                    case ItemClass.Service.Office:
                        isRICO = true;
                        break;
                    default:
                        isRICO = false;
                        break;
                }

                return isRICO;


            }
            catch (Exception e)
            {
                Debug.Log("Eminent Domain: Threading:IsRICOBuilding -> Exception: " + e.Message + " " + e.StackTrace);
            }
            return false;

        }

       
    }
}
