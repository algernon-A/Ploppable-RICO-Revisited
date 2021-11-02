using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;


namespace PloppableRICO
{
    /// <summary>
    /// Harmony patches to override OnCalculateResidentialLevelUp and OnCommercialResidentialUp to force disabling of 'land value too low' complaints for all buildings.
    /// </summary>
    [HarmonyPatch]
    public static class LandValueComplaintPatches
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
        /// <param name="landValueTooLow">If a 'land value too low' complaint has been calculated</param>
        /// <param name="buildingID">Building instance ID</param>
        public static void Postfix(ref bool landValueTooLow, ushort buildingID)
        {
            // Check if this building is RICO or not.
            bool isRICO = RICOUtils.IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore low land value complaint' setting is set.
            if ((ModSettings.noValueOther && !isRICO) || (ModSettings.noValueRicoGrow && isRICO) || (ModSettings.noValueRicoPlop && RICOUtils.IsRICOPloppable(buildingID)))
            {
                // It is - force land value complaint off.
                landValueTooLow = false;
            }
        }
    }


    /// <summary>
    /// Harmony patches to override OnCalculateResidentialLevelUp and OnCommercialResidentialUp to force disabling of 'land value too low' complaints for all buildings.
    /// </summary>
    [HarmonyPatch]
    public static class TooFewServicesComplaintPatches
    {
        /// <summary>
        /// Determines list of target methods to patch - in this case, LevelUpWrapper methods OnCalculateOfficeLevelUp and OnCalculateIndustrialLevelUp.
        /// </summary>
        /// <returns>List of target methods to patch</returns>
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateOfficeLevelUp));
            yield return AccessTools.Method(typeof(LevelUpWrapper), nameof(LevelUpWrapper.OnCalculateIndustrialLevelUp));
        }


        /// <summary>
        /// Harmony Postfix patch to force 'too few services' complaints off for all buildings.
        /// </summary>
        /// <param name="tooFewServices">If a 'too few services' complaint has been calculated</param>
        /// <param name="buildingID">Building instance ID</param>
        public static void Postfix(ref bool tooFewServices, ushort buildingID)
        {
            // Check if this building is RICO or not.
            bool isRICO = RICOUtils.IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore too few services complaint' setting is set.
            if ((ModSettings.noServicesOther && !isRICO) || (ModSettings.noServicesRicoGrow && isRICO) || (ModSettings.noServicesRicoPlop && RICOUtils.IsRICOPloppable(buildingID)))
            {
                // It is - force too few services complaint off.
                tooFewServices = false;
            }
        }
    }
}
