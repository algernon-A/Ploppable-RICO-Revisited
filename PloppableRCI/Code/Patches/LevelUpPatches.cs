using System.Collections.Generic;
using System.Reflection;
using ColossalFramework;
using HarmonyLib;


namespace PloppableRICO.Code.Patches
{
    /// <summary>
    /// Harmony patches to override OnCalculateResidentialLevelUp and OnCommercialResidentialUp to force disabling of 'land value too low' complaints for all buildings.
    /// </summary>
    [HarmonyPatch]
    public static class LevelUpPatches
    {
        /// <summary>
        /// Determines list of target methods to patch - in this case, LevelUpWrapper methods OnCalculateResidentialLevelUp and OnCommercialResidentialUp.
        /// </summary>
        /// <returns>List of target methods to patch</returns>
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateResidentialLevelUp));
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateCommercialLevelUp));
        }


        /// <summary>
        /// Harmony Postfix patch to force 'land value too low' complaints off for all buildings.
        /// </summary>
        /// <param name="landValueTooLow"></param>
        public static void Postfix(ref bool landValueTooLow, ushort buildingID)
        {
            // Check if this building is RICO or not.
            bool isRICO = IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore low land value complaint' setting is set.
            if ((ModSettings.noValueOther && !isRICO) || (ModSettings.noValueRicoGrow && isRICO) || (ModSettings.noValueRicoPlop && IsRICOPloppable(buildingID)))
            {
                // It is - force land value complaint off.
                landValueTooLow = false;
            }

        }

        /// <summary>
        /// Checks to see whether or not the specified building is a Ploppable RICO building.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this is a Ploppable RICO building, false otherwise</returns>
        private static bool IsRICOBuilding(ushort buildingID) => RICOUtils.IsRICOAI(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.GetAI() as PrivateBuildingAI);


        /// <summary>
        /// Checks to see whether or not the specified building is a Ploppable RICO non-growable building.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this is a Ploppable RICO non-growable building, false otherwise</returns>
        private static bool IsRICOPloppable(ushort buildingID) => RICOUtils.IsRICOPloppableAI(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.GetAI() as PrivateBuildingAI);
    }
}
