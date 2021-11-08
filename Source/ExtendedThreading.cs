using ColossalFramework;
using ICities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EminentDomain.Source
{
    public class ExtendedThreading : ThreadingExtensionBase
    {

        private bool _running;
        private int _cachedInterval;
        private float _timer;
        private bool _intervalPassed;
        private int _interval = 1;
        private SimulationManager _simulationManager;

        public override void OnCreated(IThreading threading)
        {
            try
            {
                _simulationManager = Singleton<SimulationManager>.instance;

            }
            catch (Exception e)
            {
                Debug.Log("[Bulldoze It!] Threading:OnCreated -> Exception: " + e.Message);
            }
        }

        public override void OnUpdate(float realTimeDelta, float simulationTimeDelta)
        {



            CollapsedAbandonedBuildings _buildingIdsGameObject = GameObject.Find("collapsedBuildingsObject").GetComponent<CollapsedAbandonedBuildings>();


            BuildingManager _buildingManager = Singleton<BuildingManager>.instance;
            Building _building;

            try
            {
                if (!_running)
                {
                    switch (_interval)
                    {
                        case 1:
                            _intervalPassed = _simulationManager.m_currentGameTime.Day != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Day;
                            break;
                        case 2:
                            _intervalPassed = _simulationManager.m_currentGameTime.Month != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Month;
                            break;
                        case 3:
                            _intervalPassed = _simulationManager.m_currentGameTime.Year != _cachedInterval ? true : false;
                            _cachedInterval = _simulationManager.m_currentGameTime.Year;
                            break;
                        case 4:
                            _timer += realTimeDelta;
                            if (_timer > 5f)
                            {
                                _timer = _timer - 5f;
                                _intervalPassed = true;
                            }
                            break;
                        case 5:
                            _timer += realTimeDelta;
                            if (_timer > 10f)
                            {
                                _timer = _timer - 10f;
                                _intervalPassed = true;
                            }
                            break;
                        case 6:
                            _timer += realTimeDelta;
                            if (_timer > 30f)
                            {
                                _timer = _timer - 30f;
                                _intervalPassed = true;
                            }
                            break;
                        default:
                            break;
                    }
                }

                if (_intervalPassed)
                {
                    _running = true;

                    _intervalPassed = false;

                    for (ushort i = 0; i < _buildingManager.m_buildings.m_buffer.Length; i++)
                    {
                        if (!_buildingIdsGameObject.buildingList.Contains(i))
                        {
                            _building = _buildingManager.m_buildings.m_buffer[i];

                            if (IsRICOBuilding(_building))
                            {
                                if ((_building.m_flags & Building.Flags.Historical) != Building.Flags.None)
                                {
                                    continue;
                                }

                                if ((_building.m_flags & Building.Flags.Abandoned) != Building.Flags.None)
                                {
                                    _buildingIdsGameObject.buildingList.Add(i);
                                }
                                else if ((_building.m_flags & Building.Flags.BurnedDown) != Building.Flags.None || (_building.m_flags & Building.Flags.Collapsed) != Building.Flags.None)
                                {
                                    if ((_building.m_problems & Notification.Problem.Fire) != Notification.Problem.None)
                                    {
                                        _buildingIdsGameObject.buildingList.Add(i);
                                    }
                                    else if (((_building.m_problems & Notification.Problem.StructureDamaged) != Notification.Problem.None || (_building.m_problems & Notification.Problem.StructureVisited) != Notification.Problem.None))
                                    {
                                        _buildingIdsGameObject.buildingList.Add(i);
                                    }
                                }
                                else if ((_building.m_flags & Building.Flags.Flooded) != Building.Flags.None)
                                {
                                    _buildingIdsGameObject.buildingList.Add(i);
                                }
                            }
                        }
                        
                    }
                }
                _running = false;
            }
            catch (Exception e)
            {
                Debug.Log("[Eminent Domain] Threading:OnUpdate -> Exception: " + e.Message);
                _running = false;
            }

        }
        private bool IsRICOBuilding(Building building)
            {
                bool isRICO = false;

                switch (building.Info.m_class.GetZone())
                {
                    case ItemClass.Zone.ResidentialHigh:
                    case ItemClass.Zone.ResidentialLow:
                    case ItemClass.Zone.Industrial:
                    case ItemClass.Zone.CommercialHigh:
                    case ItemClass.Zone.CommercialLow:
                    case ItemClass.Zone.Office:
                        isRICO = true;
                        break;
                    default:
                        isRICO = false;
                        break;
                }

                return isRICO;
            }

            
        }
    }
