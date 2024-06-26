﻿// <copyright file="CitizenUnitUtils.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons;
    using ColossalFramework;
    using ColossalFramework.Math;
    using HarmonyLib;

    /// <summary>
    /// Utility class for dealing with CitizenUnits.
    /// </summary>
    [HarmonyPatch]
    internal static class CitizenUnitUtils
    {
        // Dekegates to private game methods.
        private static EnsureCitizenUnitsDelegate s_esuDelegate;
        private static ReleaseUnitImplementationDelegate s_ruiDelegate;

        /// <summary>
        /// Delegate to BuildingAI.EnsureCitizenUnits.
        /// </summary>
        /// <param name="instance">BuildingAI instance.</param>
        /// <param name="buildingID">ID of this building.</param>
        /// <param name="data">Building data.</param>
        /// <param name="homeCount">Building residential household count.</param>
        /// <param name="workCount">Building workplace count.</param>
        /// <param name="visitCount">Building vistor count.</param>
        /// <param name="studentCount">Building studetn count.</param>
        /// <param name="hotelCount">Building hotel capacity count.</param>
        private delegate void EnsureCitizenUnitsDelegate(BuildingAI instance, ushort buildingID, ref Building data, int homeCount, int workCount, int visitCount, int studentCount, int hotelCount);

        /// <summary>
        /// Delegate to CitizenManager.ReleaseUnitImplementation.
        /// </summary>
        /// <param name="instance">CitizenManager instance.</param>
        /// <param name="unit">CitizenUnit ID.</param>
        /// <param name="data">CitizenUnit data.</param>
        private delegate void ReleaseUnitImplementationDelegate(CitizenManager instance, uint unit, ref CitizenUnit data);

        /// <summary>
        /// Inititializes delegates.  Must be called prior to mod operation.
        /// </summary>
        internal static void InitializeDelegates()
        {
            Logging.KeyMessage("initializing delegates");

            s_esuDelegate = AccessTools.MethodDelegate<EnsureCitizenUnitsDelegate>(AccessTools.Method(typeof(BuildingAI), "EnsureCitizenUnits"));
            s_ruiDelegate = AccessTools.MethodDelegate<ReleaseUnitImplementationDelegate>(AccessTools.Method(typeof(CitizenManager), "ReleaseUnitImplementation"));
        }

        /// <summary>
        /// Delegate to CitizenManager.EnsureCitizenUnits to access private method of original instance.
        /// </summary>
        /// <param name="instance">BuildingAI instance.</param>
        /// <param name="buildingID">ID of this building.</param>
        /// <param name="data">Building data.</param>
        /// <param name="homeCount">Building residential household count.</param>
        /// <param name="workCount">Building workplace count.</param>
        /// <param name="visitCount">Building vistor count.</param>
        /// <param name="studentCount">Building student count.</param>
        /// <param name="hotelCount">Building hotel capacity count.</param>
        internal static void EnsureCitizenUnits(BuildingAI instance, ushort buildingID, ref Building data, int homeCount, int workCount, int visitCount, int studentCount, int hotelCount) =>
            s_esuDelegate(instance, buildingID, ref data, homeCount, workCount, visitCount, studentCount, hotelCount);

        /// <summary>
        /// Delegate to CitizenManager.ReleaseUnitImplementation to access private method of original instance.
        /// </summary>
        /// <param name="instance">CitizenManager instance.</param>
        /// <param name="unit">CitizenUnit ID.</param>
        /// <param name="data">CitizenUnit data.</param>
        internal static void ReleaseUnitImplementation(CitizenManager instance, uint unit, ref CitizenUnit data) =>
            s_ruiDelegate(instance, unit, ref data);

        /// <summary>
        /// Updates the CitizenUnits of already existing (placed/grown) building instances of the specified prefab, or all buildings of the specified service or subservice if prefab name is null.
        /// Called after updating a prefab's household/worker/visitor count, or when applying new default calculations, in order to apply changes to existing buildings.
        /// </summary>
        /// <param name="prefab">Target building prefab.</param>
        /// <param name="preserveOccupied">Set to true to preserve occupied residential households from removal, false otherwise.</param>
        internal static void UpdateCitizenUnits(BuildingInfo prefab, bool preserveOccupied)
        {
            // Don't do anything if we're not in-game, or if prefab is null.
            if (Singleton<ToolManager>.instance?.m_properties?.m_mode != ItemClass.Availability.Game || prefab == null)
            {
                return;
            }

            // Apply via SimulationManager action.
            bool thisPreserveOccupied = preserveOccupied;
            Singleton<SimulationManager>.instance.AddAction(() => RecalculateCitizenUnits(prefab, thisPreserveOccupied));
        }

        /// <summary>
        /// Removes CitizenUnits that are surplus to requirements from the specified building.
        /// </summary>
        /// <param name="building">Building reference.</param>
        /// <param name="homeCount">Number of households to apply.</param>
        /// <param name="workCount">Number of workplaces to apply.</param>
        /// <param name="visitCount">Number of visitplaces to apply.</param>
        /// <param name="studentCount">Number of student places to apply.</param>
        /// <param name="preserveOccupied">Preserve occupied residential households.</param>
        internal static void RemoveCitizenUnits(ref Building building, int homeCount, int workCount, int visitCount, int studentCount, bool preserveOccupied)
        {
            // Local references.
            CitizenManager citizenManager = Singleton<CitizenManager>.instance;
            CitizenUnit[] citizenUnits = citizenManager.m_units.m_buffer;

            // Set starting unit references.
            uint previousUnit = 0;
            uint currentUnit = building.m_citizenUnits;

            // Keep looping through all CitizenUnits in this building until the end.
            while (currentUnit != 0)
            {
                // Get reference to next unit and flags of this unit.
                CitizenUnit.Flags unitFlags = citizenUnits[currentUnit].m_flags;
                uint nextUnit = citizenUnits[currentUnit].m_nextUnit;

                // Status flag.
                bool removingFlag = false;

                // Is this a residential unit?
                if ((ushort)(unitFlags & CitizenUnit.Flags.Home) != 0)
                {
                    // Residential unit; are we still allocating homes, and if we're preserving occupied units, is it empty?
                    if (homeCount <= 0 && (!preserveOccupied || (citizenUnits[currentUnit].m_citizen0 + citizenUnits[currentUnit].m_citizen1 + citizenUnits[currentUnit].m_citizen2 + citizenUnits[currentUnit].m_citizen3 + citizenUnits[currentUnit].m_citizen4 == 0)))
                    {
                        // Already have the maximum number of households, therefore this workplace unit is surplus to requirements - remove it.
                        removingFlag = true;
                    }
                    else
                    {
                        // Still allocating - reduce unallocated homeCount by 1.
                        --homeCount;
                    }
                }

                // Is this a workplace unit?
                else if ((ushort)(unitFlags & CitizenUnit.Flags.Work) != 0)
                {
                    // Workplace unit; are we still allocating to workplaces?
                    if (workCount <= 0)
                    {
                        // Not allocating any more, therefore this workplace unit is surplus to requirements - remove it.
                        removingFlag = true;
                    }
                    else
                    {
                        // Still allocating - reduce unallocated workCount by 5.
                        workCount -= 5;
                    }
                }
                else if ((ushort)(unitFlags & CitizenUnit.Flags.Visit) != 0)
                {
                    // VisitPlace unit; are we still allocating to visitCount?
                    if (visitCount <= 0)
                    {
                        // Not allocating any more, therefore this workplace unit is surplus to requirements - remove it.
                        removingFlag = true;
                    }
                    else
                    {
                        // Still allocating - reduce unallocated visitCount by 5.
                        visitCount -= 5;
                    }
                }
                else if ((ushort)(unitFlags & CitizenUnit.Flags.Student) != 0)
                {
                    // Student unit; are we still allocating to students?
                    if (studentCount <= 0)
                    {
                        // Not allocating any more, therefore this workplace unit is surplus to requirements - remove it.
                        // Student buildings are set as workplace.
                        removingFlag = true;
                    }
                    else
                    {
                        // Still allocating - reduce unallocated visitCount by 5.
                        studentCount -= 5;
                    }
                }
                else
                {
                    // Invalid unit; remove it.
                    removingFlag = true;
                    Logging.Message("found unit ", currentUnit, " with no valid flags");
                }

                // Are we removing this unit?
                if (removingFlag)
                {
                    Logging.Message("removing unit ", currentUnit);

                    // Unlink this unit from building CitizenUnit list.
                    if (previousUnit != 0)
                    {
                        citizenUnits[previousUnit].m_nextUnit = nextUnit;
                    }
                    else
                    {
                        // No previous unit - unlink from building record.
                        building.m_citizenUnits = nextUnit;
                    }

                    // Release unit.
                    citizenUnits[currentUnit] = default;
                    ReleaseUnitImplementation(citizenManager, currentUnit, ref citizenUnits[currentUnit]);
                    citizenManager.m_unitCount = (int)(citizenManager.m_units.ItemCount() - 1);
                }
                else
                {
                    // Not removing - therefore previous unit reference needs to be updated.
                    previousUnit = currentUnit;
                }

                // Move on to next unit.
                currentUnit = nextUnit;
            }
        }

        /// <summary>
        /// Updates the CitizenUnits of already existing (placed/grown) building instances of the specified prefab, or all buildings of the specified service or subservice if prefab name is null.
        /// Called after updating a prefab's household/worker/visitor count, or when applying new default calculations, in order to apply changes to existing buildings.
        /// </summary>
        /// <param name="prefab">Target building prefab.</param>
        /// <param name="preserveOccupied">Set to true to preserve occupied residential households from removal, false otherwise.</param>
        private static void RecalculateCitizenUnits(BuildingInfo prefab, bool preserveOccupied)
        {
            // Local references.
            CitizenManager citizenManager = Singleton<CitizenManager>.instance;
            Building[] buildingBuffer = Singleton<BuildingManager>.instance?.m_buildings?.m_buffer;

            // Don't do anything if we couldn't get the building buffer.
            if (buildingBuffer == null)
            {
                return;
            }

            // Iterate through each building in the scene.
            for (ushort i = 0; i < buildingBuffer.Length; i++)
            {
                // Only interested in created buildings with private AI.
                if ((buildingBuffer[i].m_flags & Building.Flags.Created) != Building.Flags.None)
                {
                    // Residential building; check that either the supplied prefab name is null or it matches this building's prefab.
                    if (buildingBuffer[i].Info == prefab && prefab.m_buildingAI is PrivateBuildingAI privateAI)
                    {
                        // Got one!  Log initial status.
                        Logging.Message("Identified building ", i, " (", buildingBuffer[i].Info.name, ") with ", CountCitizenUnits(ref buildingBuffer[i]), " CitizenUnits");

                        // Recalculate home and visit counts.
                        ItemClass.Level buildingLevel = (ItemClass.Level)buildingBuffer[i].m_level;
                        byte buildingWidth = buildingBuffer[i].m_width;
                        byte buildingLength = buildingBuffer[i].m_length;
                        privateAI.CalculateWorkplaceCount(buildingLevel, new Randomizer(i), buildingWidth, buildingLength, out int level0, out int level1, out int level2, out int level3);
                        int workCount = level0 + level1 + level2 + level3;
                        int homeCount = privateAI.CalculateHomeCount(buildingLevel, new Randomizer(i), buildingWidth, buildingLength);
                        int visitCount = privateAI.CalculateVisitplaceCount(buildingLevel, new Randomizer(i), buildingWidth, buildingLength);

                        // Add CitizenUnits via EnsureCitizenUnits reverse patch..
                        EnsureCitizenUnits(privateAI, i, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[i], homeCount, workCount, visitCount, 0, 0);

                        // Remove any extra CitizenUnits.
                        RemoveCitizenUnits(ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[i], homeCount, workCount, visitCount, 0, preserveOccupied);

                        // Log changes.
                        Logging.Message("Reset CitizenUnits for building ", i, " (", prefab.name, ") with preserve occupied flag of ", preserveOccupied, "; building now has ", CountCitizenUnits(ref buildingBuffer[i]), " CitizenUnits, and total CitizenUnit count is now ", citizenManager.m_unitCount);
                    }
                }
            }
        }

        /// <summary>
        /// Counts the number of CitizenUnits attached to the given building.
        /// </summary>
        /// <param name="building">Building record.</param>
        /// <returns>CitizenUnit count.</returns>
        private static uint CountCitizenUnits(ref Building building)
        {
            uint unitCount = 0;

            // Local reference.
            CitizenUnit[] citizenUnts = Singleton<CitizenManager>.instance.m_units.m_buffer;

            // Follow m_nextUnit chain of linked CitizenUnits.
            uint currentUnit = building.m_citizenUnits;
            while (currentUnit != 0)
            {
                ++unitCount;
                currentUnit = citizenUnts[currentUnit].m_nextUnit;
            }

            return unitCount;
        }
    }
}
