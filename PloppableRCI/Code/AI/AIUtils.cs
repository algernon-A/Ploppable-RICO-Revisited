using ColossalFramework;
using ColossalFramework.Math;


namespace PloppableRICO
{
    internal static class AIUtils
    {

        /// <summary>
        /// Sets the building flags of the selected building to be 'Ploppable RICO Friendly', to ensure the persistence and operation of Ploppable RICO buildings.
        /// Called for Ploppable RICO buildings before and after each simulation step to force the relevant flags.
        /// For our purposes, this is simpler and more robust than the alternative of having to do some messy work with the SimulationStep code.
        /// </summary>
        /// <param name="buildingData">Building instance data</param>
        internal static void SetBuildingFlags(ref Building buildingData)
        {
            // Force reset timers to zero.
            buildingData.m_garbageBuffer = 100;
            buildingData.m_majorProblemTimer = 0;
            buildingData.m_levelUpProgress = 0;

            // Clear key flags.
            buildingData.m_flags &= ~Building.Flags.ZonesUpdated;
            buildingData.m_flags &= ~Building.Flags.Abandoned;
            buildingData.m_flags &= ~Building.Flags.Demolishing;

            // Make sure building isn't 'turned off' (otherwise this could be an issue with coverted parks, monuments, etc. that were previously turned off). 
            buildingData.m_problems &= ~Notification.Problem1.TurnedOff;
        }


        /// <summary>
        /// Calculates the construction cost of a workplace, depending on current settings (overrides or default). 
        /// </summary>
        /// <param name="thisAI">AI reference to calculate for</param>
        /// <returns>Final construction cost</returns>
        internal static int WorkplaceConstructionCost(PrivateBuildingAI thisAI, int fixedCost)
        {
            int baseCost;

            // Local references.
            BuildingInfo thisInfo = thisAI.m_info;
            ItemClass.Level thisLevel = thisInfo.GetClassLevel();

            // Are we overriding cost?
            if (ModSettings.overrideCost)
            {
                // Yes - calculate based on workplaces by level multiplied by appropriate cost-per-job setting.
                thisAI.CalculateWorkplaceCount(thisLevel, new Randomizer(), thisInfo.GetWidth(), thisInfo.GetLength(), out int jobs0, out int jobs1, out int jobs2, out int jobs3);
                baseCost = (ModSettings.costPerJob0 * jobs0) + (ModSettings.costPerJob1 * jobs1) + (ModSettings.costPerJob2 * jobs2) + (ModSettings.costPerJob3 * jobs3);
            }
            else
            {
                // No - just use the base cost provided.
                baseCost = fixedCost;
            }

            // Multiply base cost by 100 before feeding to EconomyManager for nomalization to game conditions prior to return.
            baseCost *= 100;
            Singleton<EconomyManager>.instance.m_EconomyWrapper.OnGetConstructionCost(ref baseCost, thisInfo.GetService(), thisInfo.GetSubService(), thisLevel);
            return baseCost;
        }
    }
}
