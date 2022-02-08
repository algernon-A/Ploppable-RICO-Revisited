using System;
using ColossalFramework.Math;
using ColossalFramework;
using UnityEngine;


namespace PloppableRICO
{
    /// <summary>
    /// Taken from Realistic Population Revisited.  Original by Whitefang Greytail.
    /// Applies exact household and workplace totals, moving citizens if necessary.
    /// </summary>
    internal class RealisticCitizenUnits
    {
        private static readonly CitizenManager citizenManager = Singleton<CitizenManager>.instance;
        private static readonly CitizenUnit[] citizenUnitArray = Singleton<CitizenManager>.instance.m_units.m_buffer;
        private static readonly Citizen[] citizenArray = Singleton<CitizenManager>.instance.m_citizens.m_buffer;


        internal static void EnsureCitizenUnits(ref PrivateBuildingAI __instance, ushort buildingID, ref Building data, int homeCount, int workCount, int visitCount, int studentCount)
        {
            int totalWorkCount = (workCount + 4) / 5;
            int totalHomeCount = homeCount;

            int[] workersRequired = new int[] { 0, 0, 0, 0 };


            if ((data.m_flags & (Building.Flags.Abandoned | Building.Flags.BurnedDown)) == Building.Flags.None)
            {
                Citizen.Wealth wealthLevel = Citizen.GetWealthLevel(__instance.m_info.m_class.m_level);
                uint num = 0u;
                uint num2 = data.m_citizenUnits;
                while (num2 != 0u)
                {
                    CitizenUnit.Flags flags = citizenUnitArray[(int)((UIntPtr)num2)].m_flags;
                    if ((ushort)(flags & CitizenUnit.Flags.Home) != 0)
                    {
                        citizenUnitArray[(int)((UIntPtr)num2)].SetWealthLevel(wealthLevel);
                        homeCount--;
                    }
                    if ((ushort)(flags & CitizenUnit.Flags.Work) != 0)
                    {
                        workCount -= 5;
                        for (int i = 0; i < 5; i++)
                        {
                            uint citizen = citizenUnitArray[(int)((UIntPtr)num2)].GetCitizen(i);
                            if (citizen != 0u)
                            {
                                // Tick off education to see what is there
                                workersRequired[(int)citizenArray[(int)((UIntPtr)citizen)].EducationLevel]--;
                            }
                        }
                    }
                    if ((ushort)(flags & CitizenUnit.Flags.Visit) != 0)
                    {
                        visitCount -= 5;
                    }
                    if ((ushort)(flags & CitizenUnit.Flags.Student) != 0)
                    {
                        studentCount -= 5;
                    }
                    num = num2;
                    num2 = citizenUnitArray[(int)((UIntPtr)num2)].m_nextUnit;
                } // end while
                /*
                                homeCount = Mathf.Max(0, homeCount);
                                workCount = Mathf.Max(0, workCount);
                 */
                visitCount = Mathf.Max(0, visitCount);
                studentCount = Mathf.Max(0, studentCount);

                if (homeCount > 0 || workCount > 0 || visitCount > 0 || studentCount > 0)
                {
                    if (citizenManager.CreateUnits(out uint num4, ref Singleton<SimulationManager>.instance.m_randomizer, buildingID, 0, homeCount, workCount, visitCount, 0, studentCount))
                    {
                        if (num != 0u)
                        {
                            citizenUnitArray[(int)((UIntPtr)num)].m_nextUnit = num4;
                        }
                        else
                        {
                            data.m_citizenUnits = num4;
                        }
                    }
                }


                // Stop incoming offers to get HandleWorkers() to start fresh
                TransferManager.TransferOffer offer = default;
                offer.Building = buildingID;
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.Worker0, offer);
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.Worker1, offer);
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.Worker2, offer);
                Singleton<TransferManager>.instance.RemoveIncomingOffer(TransferManager.TransferReason.Worker3, offer);

                ItemClass.Level level = ((PrivateBuildingAI)data.Info.GetAI()).m_info.m_class.m_level;
                ((PrivateBuildingAI)data.Info.GetAI()).CalculateWorkplaceCount(level, new Randomizer((int)buildingID), data.Width, data.Length,
                                                                               out int worker0, out int worker1, out int worker2, out int worker3);

                // Update the workers required once figuring out how many are needed by the new building
                workersRequired[0] += worker0;
                workersRequired[1] += worker1;
                workersRequired[2] += worker2;
                workersRequired[3] += worker3;

                if (workCount < 0)
                {
                    RemoveWorkerBuilding(ref data, totalWorkCount);
                }
                else if (homeCount < 0)
                {
                    RemoveHouseHold(ref data, totalHomeCount);
                }
                /*
                    if (visitCount < 0)
                    {
                        RemoveVisitorsBuilding(buildingID, ref data, totalVisitCount);
                    }
                */
                PromoteWorkers(ref data, ref workersRequired);
                // Do nothing for students

            } // end if good building
        } // end EnsureCitizenUnits

        /// <summary>
        /// Send this unit away to empty to requirements
        /// EmptyBuilding
        /// </summary>
        /// <param name="data"></param>
        /// <param name="citizenNumber"></param>
        private static void RemoveWorkerBuilding(ref Building data, int workerUnits)
        {
            int loopCounter = 0;
            uint previousUnit = data.m_citizenUnits;
            uint currentUnit = data.m_citizenUnits;


            while (currentUnit != 0u)
            {
                // If this unit matches what we one, send the citizens away or remove citzens
                uint nextUnit = citizenUnitArray[currentUnit].m_nextUnit;
                bool removeCurrentUnit = false;

                // Only think about removing if it matches the flag
                if ((ushort)(CitizenUnit.Flags.Work & citizenUnitArray[currentUnit].m_flags) != 0)
                {
                    if (workerUnits > 0)
                    {
                        // Don't remove the unit, we'll remove excess afterwards
                        workerUnits--;
                    }
                    else
                    {
                        // Send unit away like empty building
                        for (int i = 0; i < 5; i++)
                        {
                            uint citizen = citizenUnitArray[(int)((UIntPtr)currentUnit)].GetCitizen(i);
                            if (citizen != 0u)
                            {
                                // Do not shift back where possible. There's enough staff turnover that the spaces aren't worth the intensive checking
                                citizenManager.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_workBuilding = 0;
                            }  // end citizen
                        } // end for
                        removeCurrentUnit = true;
                    } // end if
                } // Flag match

                // Don't need to worry about trying to remove the initial citizen unit. 
                // This should always exist and other code will always force at least one.
                if (removeCurrentUnit)
                {
                    // Link previous unit to next unit and release the item
                    citizenUnitArray[previousUnit].m_nextUnit = nextUnit;

                    citizenUnitArray[currentUnit] = default;
                    citizenManager.m_units.ReleaseItem(currentUnit);
                    // Previous unit number has not changed
                }
                else
                {
                    // Current unit is not to be removed, proceed to next
                    previousUnit = currentUnit;
                }
                currentUnit = nextUnit;

                if (++loopCounter > 524288)
                {
                    currentUnit = 0u; // Bail out loop
                }
            } // end while
        } // end RemoveWorkerBuilding


        /// <summary>
        /// Promote the workers to fit the education bill better.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="workersRequired"></param>
        /// <param name="instance"></param>
        /// <param name="citizenUnitArray"></param>
        /// <param name="citizenArray"></param>
        private static void PromoteWorkers(ref Building data, ref int[] workersRequired)
        {
            if (workersRequired[0] == 0 && workersRequired[1] == 0 && workersRequired[2] == 0 && workersRequired[3] == 0)
            {
                // We are okay with employees, or it's residential. Return
                return;
            }

            // Crime and garbage are reset
            data.m_crimeBuffer = 0;
            data.m_garbageBuffer = 0;

            int loopCounter = 0;
            uint currentUnit = data.m_citizenUnits;
            while (currentUnit != 0u)
            {
                // If this unit matches what we one, send the citizens away or remove citzens
                uint nextUnit = citizenUnitArray[currentUnit].m_nextUnit;

                // Only think about removing if it matches the flag
                if ((ushort)(CitizenUnit.Flags.Work & citizenUnitArray[currentUnit].m_flags) != 0)
                {
                    // Send unit away like empty building
                    for (int i = 0; i < 5; i++)
                    {
                        uint citizen = citizenUnitArray[(int)((UIntPtr)currentUnit)].GetCitizen(i);
                        if (citizen != 0u)
                        {
                            // Do not shift back where possible. There should be enough staff turnover that the spaces aren't worth the intensive checking.

                            // Get education level. Perform checks
                            Citizen cit = citizenArray[(int)((UIntPtr)citizen)];
                            int education = (int)cit.EducationLevel;

                            // -ve workersRequired means excess workers. Ignoring three schools
                            // Checks if the citizen should be promoted or fire
                            // Remove excess 0, 1, 2. However, give 20 - 50 % change to go up an education level. Don't touch lvl 3 educated (they'll disappear fast given the chance)
                            if ((cit.EducationLevel != Citizen.Education.ThreeSchools) && (workersRequired[education] < 0 && workersRequired[education + 1] > 0))
                            {
                                // Need to be above 50 to be promoted. However, each level is harder to get to, effectively (50, 65, 80)
                                int number = Singleton<SimulationManager>.instance.m_randomizer.Int32(0, 100) - (education * 15);
                                if (number > 50)
                                {
                                    if (cit.EducationLevel == Citizen.Education.Uneducated)
                                    {
                                        cit.Education1 = true;
                                        workersRequired[0]++;
                                        workersRequired[1]--;
                                    }
                                    else if (cit.EducationLevel == Citizen.Education.OneSchool)
                                    {
                                        cit.Education2 = true;
                                        workersRequired[1]++;
                                        workersRequired[2]--;
                                    }
                                    else if (cit.EducationLevel == Citizen.Education.TwoSchools)
                                    {
                                        cit.Education3 = true;
                                        workersRequired[2]++;
                                        workersRequired[3]--;
                                    }
                                }
                                else
                                {
                                    workersRequired[education]++;
                                    citizenManager.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_workBuilding = 0;
                                    RemoveFromCitizenUnit(currentUnit, i);
                                }
                            }
                            else if (workersRequired[education] < 0)
                            {
                                workersRequired[education]++;
                                citizenManager.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_workBuilding = 0;
                                RemoveFromCitizenUnit(currentUnit, i);
                            } // end if
                        }  // end citizen
                    } // end for
                } // Flag match

                currentUnit = nextUnit;

                if (++loopCounter > 524288)
                {
                    currentUnit = 0u; // Bail out loop
                }
            } // end while
        } // end PromoteWorkers


        /// <summary>
        /// 
        /// </summary>
        /// <param name="citizenUnitArray"></param>
        /// <param name="currentUnit"></param>
        /// <param name="i"></param>
        private static void RemoveFromCitizenUnit(uint currentUnit, int i)
        {
            switch (i)
            {
                case 0:
                    citizenUnitArray[(int)((UIntPtr)currentUnit)].m_citizen0 = 0u;
                    break;
                case 1:
                    citizenUnitArray[(int)((UIntPtr)currentUnit)].m_citizen1 = 0u;
                    break;
                case 2:
                    citizenUnitArray[(int)((UIntPtr)currentUnit)].m_citizen2 = 0u;
                    break;
                case 3:
                    citizenUnitArray[(int)((UIntPtr)currentUnit)].m_citizen3 = 0u;
                    break;
                case 4:
                    citizenUnitArray[(int)((UIntPtr)currentUnit)].m_citizen4 = 0u;
                    break;
            }
        }


        /// <summary>
        /// Send this unit away to empty to requirements
        /// EmptyBuilding
        /// </summary>
        /// <param name="data"></param>
        /// <param name="citizenNumber"></param>
        private static void RemoveHouseHold(ref Building data, int maxHomes)
        {
            CitizenManager instance = Singleton<CitizenManager>.instance;
            CitizenUnit[] citizenUnitArray = instance.m_units.m_buffer;

            int loopCounter = 0;
            uint previousUnit = data.m_citizenUnits;
            uint currentUnit = data.m_citizenUnits;

            while (currentUnit != 0u)
            {
                // If this unit matches what we one, send the citizens away or remove citzens
                uint nextUnit = citizenUnitArray[currentUnit].m_nextUnit;
                bool removeCurrentUnit = false;

                // Only think about removing if it matches the flag
                if ((ushort)(CitizenUnit.Flags.Home & citizenUnitArray[currentUnit].m_flags) != 0)
                {
                    if (maxHomes > 0)
                    {
                        maxHomes--;
                    }
                    else
                    {
                        // Remove excess citizens
                        for (int i = 0; i < 5; i++)
                        {
                            // CommonBuildingAI.RemovePeople() -> CitizenManager.ReleaseUnitImplementation()
                            uint citizen = citizenUnitArray[(int)((UIntPtr)currentUnit)].GetCitizen(i);
                            citizenManager.m_citizens.m_buffer[(int)((UIntPtr)citizen)].m_homeBuilding = 0;
                        } // end for
                        removeCurrentUnit = true;
                    } // end if - above count
                } // Flag match

                // Don't need to worry about trying to remove the initial citizen unit. 
                // This should always exist and other code will always force at least one.
                if (removeCurrentUnit)
                {
                    // Link previous unit to next unit and release the item
                    citizenUnitArray[previousUnit].m_nextUnit = nextUnit;

                    citizenUnitArray[currentUnit] = default;
                    instance.m_units.ReleaseItem(currentUnit);
                    // Previous unit number has not changed
                }
                else
                {
                    // Current unit is not to be removed, proceed to next
                    previousUnit = currentUnit;
                }
                currentUnit = nextUnit;

                if (++loopCounter > 524288)
                {
                    currentUnit = 0u; // Bail out loop
                }
            } // end while
        } // end RemoveHouseHold
    } // end AI_Building
}
