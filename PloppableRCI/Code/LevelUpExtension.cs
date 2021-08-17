using ICities;
using ColossalFramework;


namespace PloppableRICO
{

    /// <summary>
    /// Adds RICO complaint overriding to game levelling up process.
    /// </summary>
    public class LevelUpExtension : LevelUpExtensionBase
    {

        /// <summary>
        /// Residential level up control - game extension method.
        /// Used to override 'low land value' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageEducation">Average education for the building (ignored)</param>
        /// <param name="landValue">Land value for the building (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        public override ResidentialLevelUp OnCalculateResidentialLevelUp(ResidentialLevelUp levelUp, int averageEducation, int landValue,
            ushort buildingID, Service service, SubService subService, Level currentLevel)
        {
            // Check if this building is RICO or not.
            bool isRICO = IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore low land value complaint' setting is set.
            if ((ModSettings.noValueOther && !isRICO) || (ModSettings.noValueRicoGrow && isRICO) || (ModSettings.noValueRicoPlop && IsRICOPloppable(buildingID)))
            {
                // It is - force land value complaint off.
                levelUp.landValueTooLow = false;
            }

            return levelUp;
        }


        /// <summary>
        /// Commercial level up control - game extension method.
        /// Used to override 'low land value' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageWealth">Building average wealth (ignored)</param>
        /// <param name="landValue">Land value for the building (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        /// <returns></returns>
        public override CommercialLevelUp OnCalculateCommercialLevelUp(CommercialLevelUp levelUp, int averageWealth, int landValue,
            ushort buildingID, Service service, SubService subService, Level currentLevel)
        {
            // Check if this building is RICO or not.
            bool isRICO = IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore low land value complaint' setting is set.
            if ((ModSettings.noValueOther && !isRICO) || (ModSettings.noValueRicoGrow && isRICO) || (ModSettings.noValueRicoPlop && IsRICOPloppable(buildingID)))
            {
                // It is - force land value complaint off.
                levelUp.landValueTooLow = false;
            }

            return levelUp;
        }


        /// <summary>
        /// Industrial level up control - game extension method.
        /// Used to override 'too few services' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageEducation">Average education for the building (ignored)</param>
        /// <param name="serviceScore">Building service score (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        /// <returns></returns>
        public override IndustrialLevelUp OnCalculateIndustrialLevelUp(IndustrialLevelUp levelUp, int averageEducation, int serviceScore,
            ushort buildingID, Service service, SubService subService, Level currentLevel)
        {
            // Check if this building is RICO or not.
            bool isRICO = IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore too few services complaint' setting is set.
            if ((ModSettings.noServicesOther && !isRICO) || (ModSettings.noServicesRicoGrow && isRICO) || (ModSettings.noServicesRicoPlop && IsRICOPloppable(buildingID)))
            {
                // It is - force too few services complaint off.
                levelUp.tooFewServices = false;
            }

            return levelUp;

        }


        /// <summary>
        /// Office level up control - game extension method.
        /// Used to override 'too few services' complaint as appropriate.
        /// </summary>
        /// <param name="levelUp">Original upgrade struct (target level and progress)</param>
        /// <param name="averageEducation">Average education for the building (ignored)</param>
        /// <param name="serviceScore">Building service score (ignored)</param>
        /// <param name="buildingID">Building instance ID</param>
        /// <param name="service">Building service (ignored)</param>
        /// <param name="subService">Building subservice (ignored)</param>
        /// <param name="currentLevel">Existing building level (ignored)</param>
        /// <returns>Revised target level (level and progress)</returns>
        /// <returns></returns>
        public override OfficeLevelUp OnCalculateOfficeLevelUp(OfficeLevelUp levelUp, int averageEducation, int serviceScore, ushort buildingID,
            Service service, SubService subService, Level currentLevel)
        {
            // Check if this building is RICO or not.
            bool isRICO = IsRICOBuilding(buildingID);

            // Check if the relevant 'ignore too few services complaint' setting is set.
            if ((ModSettings.noServicesOther && !isRICO) || (ModSettings.noServicesRicoGrow && isRICO) || (ModSettings.noServicesRicoPlop && IsRICOPloppable(buildingID)))
            {
                // It is - force too few services complaint off.
                levelUp.tooFewServices = false;
            }

            return levelUp;
        }


        /// <summary>
        /// Checks to see whether or not the specified building is a Ploppable RICO building.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this is a Ploppable RICO building, false otherwise</returns>
        private bool IsRICOBuilding(ushort buildingID) => RICOUtils.IsRICOAI(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.GetAI() as PrivateBuildingAI);


        /// <summary>
        /// Checks to see whether or not the specified building is a Ploppable RICO non-growable building.
        /// </summary>
        /// <param name="buildingID">Building instance ID</param>
        /// <returns>True if this is a Ploppable RICO non-growable building, false otherwise</returns>
        private bool IsRICOPloppable(ushort buildingID) => RICOUtils.IsRICOPloppableAI(Singleton<BuildingManager>.instance.m_buildings.m_buffer[buildingID].Info.GetAI() as PrivateBuildingAI);
    }
}