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
            // buildingData.m_garbageBuffer = 100;
            buildingData.m_majorProblemTimer = 0;
            buildingData.m_levelUpProgress = 0;

            // Clear key flags.
            buildingData.m_flags &= ~Building.Flags.ZonesUpdated;
            buildingData.m_flags &= ~Building.Flags.Abandoned;
            buildingData.m_flags &= ~Building.Flags.Demolishing;

            // Make sure building isn't 'turned off' (otherwise this could be an issue with coverted parks, monuments, etc. that were previously turned off).
            buildingData.m_problems &= ~Notification.Problem.TurnedOff;

            if (ModSettings.noValueRico)
            {
                // Disable the "Low land value" complaint
                buildingData.m_problems &= ~Notification.Problem.LandValueLow;
            }

            if (ModSettings.noServicesRico)
            {
                // Disable the "Too few services" complaint
                buildingData.m_problems &= ~Notification.Problem.TooFewServices;
            }
        }
    }
}
