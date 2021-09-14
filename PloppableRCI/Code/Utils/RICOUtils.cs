namespace PloppableRICO
{
    /// <summary>
    /// Internal static RICO utilities class.
    /// </summary>
    internal static class RICOUtils
    {
        /// <summary>
        /// Checks if a builing instance has a Ploppable RICO custom AI.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this building has a Ploppable RICO custom AI, false otherwise</returns>
        internal static bool IsRICOAI(PrivateBuildingAI prefabAI) => prefabAI != null && (prefabAI is GrowableResidentialAI || prefabAI is GrowableCommercialAI || prefabAI is GrowableIndustrialAI || prefabAI is GrowableOfficeAI || prefabAI is GrowableExtractorAI);


        /// <summary>
        /// Checks if a builing instance has a Ploppable RICO custom non-growable AI.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this building has a Ploppable RICO non-growable custom AI, false otherwise</returns>
        internal static bool IsRICOPloppableAI(PrivateBuildingAI prefabAI) => prefabAI != null && (prefabAI is PloppableResidentialAI || prefabAI is PloppableCommercialAI || prefabAI is PloppableIndustrialAI || prefabAI is PloppableOfficeAI || prefabAI is PloppableExtractorAI);


        /// <summary>
        /// Returns the currently applied RICO settings (RICO building) for the provided BuilingData instance.
        /// </summary>
        /// <param name="buildingData">BuildingData record</param>
        /// <returns>Currently active RICO building setting (null if none)</returns>
        internal static RICOBuilding CurrentRICOSetting(BuildingData buildingData)
        {
            if (buildingData.hasLocal)
            {
                return buildingData.local;
            }
            else if (buildingData.hasAuthor)
            {
                return buildingData.author;
            }
            else if (buildingData.hasMod)
            {
                return buildingData.mod;
            }

            return null;
        }
    }
}
