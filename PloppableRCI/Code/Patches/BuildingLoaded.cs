using System;
using System.Runtime.CompilerServices;
using ColossalFramework;
using ColossalFramework.Math;
using HarmonyLib;


/// <summary>
/// Patch for PrivateBuildingAI.BuildingLoaded to catch Ploppable RICO buildings and override their initialisation with Ploppable RICO assets.
/// This is used to catch Ploppable RICO buildings created from PrivateBuilding (growable) originals; normally, their original stats would initialise before Ploppable RICO can override.
/// </summary>
namespace PloppableRICO
{
    [HarmonyPatch(typeof(PrivateBuildingAI), "BuildingLoaded")]
    [HarmonyPriority(Priority.VeryHigh)]
    public static class RICOBuildingLoaded
	{
		/// <summary>
		/// Prefix to force settings reset on load (if enabled) for RICO buildings (resetting to current settings).
		/// </summary>
		/// <param name="__instance">Original object instance reference</param>
		/// <param name="buildingID">Building instance ID</param>
		/// <param name="data">Building data</param>
		public static bool Prefix(PrivateBuildingAI __instance, ushort buildingID, ref Building data)
		{
			// Don't do anything if the flag isn't set.
			if (!ModSettings.resetOnLoad)
            {
				// Carry on to original method.
				return true;
            }

			// Check to see if we've preloaded a local settings file.
			if (Loading.localRicoDef != null)
			{
				// Step through each definition from the local settings file, looking for a match.
				foreach (RICOBuilding building in Loading.localRicoDef.Buildings)
				{
					if (building.ricoEnabled && __instance.m_info.name.Equals(building.Name))
					{
						// m_level is one less than building.level.
						byte newLevel = (byte)(building.level - 1);

						if (data.m_level != newLevel)
						{
							Logging.Message("found building '", building.Name, "' with level ", (data.m_level + 1).ToString(), ", overriding to level ", building.level.ToString());
							data.m_level = newLevel;
						}

						// Basic game code processing to continue initialisation.
						__instance.CalculateWorkplaceCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length, out int level, out int level2, out int level3, out int level4);
						__instance.AdjustWorkplaceCount(buildingID, ref data, ref level, ref level2, ref level3, ref level4);

						int workCount = level + level2 + level3 + level4;
						int targetHomeCount = 0;

						// Update visitor count.
						int visitCount = __instance.CalculateVisitplaceCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length);

						// Check to see if residential building homecounts differ from settings.
						if (building.service == "residential") 
						{
							int currentHomeCount = 0;

							// Count currently applied citizen units (households).
							if (data.m_citizenUnits != 0)
							{
								// At least one household here; get the first.
								CitizenUnit citizenUnit = Singleton<CitizenManager>.instance.m_units.m_buffer[data.m_citizenUnits];
								currentHomeCount = 1;

								// Step through all applied citizen units (linked via m_nextUnit), counting as we go,
								while (citizenUnit.m_nextUnit != 0)
								{
									citizenUnit = Singleton<CitizenManager>.instance.m_units.m_buffer[citizenUnit.m_nextUnit];
									currentHomeCount++;
								}
							}

							// Determine target household count.
							targetHomeCount = __instance.CalculateHomeCount((ItemClass.Level)data.m_level, new Randomizer(buildingID), data.Width, data.Length);

							// If target household count is lower than the current household count, we need to perform a forced reset.
							// The reverse case, targets greater than current, will be caught with the base-case call to EnsureCitizenUnits below.
							if (targetHomeCount < currentHomeCount)
							{
								Logging.Message("found Residential prefab ", building.Name, " with target homecount ", targetHomeCount.ToString(), " and citizen units ", currentHomeCount.ToString(), "; forcing homecount reset");
								RealisticCitizenUnits.EnsureCitizenUnits(ref __instance, buildingID, ref data, targetHomeCount, workCount, visitCount, 0);
							}
						}

						// Update citizen units to match new totals.
						EnsureCitizenUnitsRev(__instance, buildingID, ref data, targetHomeCount, workCount, visitCount, 0);

						// Clear any problems (so we don't have any residual issues from changing service types, for example (new) residential buildings showing 'not enough goods'.
						// Any 'genuine' problems will be quickly reapplied by the game.
						data.m_problems = 0;

						// We've set things up here for Ploppable RICO - don't fall through to game code.
						return false;
					}
				}
			}

			// If we've hit this point, then no Ploppable RICO setup has occured - fall through to game code.
			return true;
		}


		/// <summary>
		/// Postfix to attempt to catch issues where homecounts seem to be reset to zero for specific buildings.
		/// </summary>
		/// <param name="__instance">Original object instance reference</param>
		/// <param name="buildingID">Building instance ID</param>
		/// <param name="data">Building data</param>
		public static void Postfix(PrivateBuildingAI __instance, ushort buildingID, ref Building data)
        {
			// Check to see if this is one of ours and that's not abandoned.
			if (__instance is GrowableResidentialAI && (data.m_flags & Building.Flags.Abandoned) == 0)
			{
				CitizenUnit[] citizenBuffer = Singleton<CitizenManager>.instance.m_units.m_buffer; 

				int population = 0;
				uint currentCitizenUnit = data.m_citizenUnits;

				while (currentCitizenUnit != 0)
                {
					++population;
					currentCitizenUnit = citizenBuffer[currentCitizenUnit].m_nextUnit;
                }

				// Check to see if no citizen units are set.
				if (population == 0)
                {
					// Uh oh...
					Logging.Error("no citizenUnits for building ", buildingID.ToString(), " : ", data.Info.name, "; attempting reset");
					
					// Backup resetOnLoad setting and force to true for reset attempt.
					bool oldReset = ModSettings.resetOnLoad;
					ModSettings.resetOnLoad = true;

					// Attempt reset for this building.
					Prefix(__instance, buildingID, ref data);

					// Restore original resetOnLoad seeting.
					ModSettings.resetOnLoad = oldReset;
                }
				else
                {
					Logging.Message("building ", buildingID.ToString(), " : ", data.Info.name, " passed CitizenUnits check with " , population.ToString(), " households");
                }
            }
        }


		/// <summary>
		/// Reverse patch for BuildingAI.EnsureCitizenUnits to access private method of original instance.
		/// </summary>
		/// <param name="instance">Object instance</param>
		/// <param name="buildingID">ID of this building (for game method)</param>
		/// <param name="data">Building data (for game method)</param>
		/// <param name="homeCount">Household count (for game method)</param>
		/// <param name="workCount">Workplace count (for game method)</param>
		/// <param name="visitCount">Visitor count (for game method)</param>
		/// <param name="studentCount">Student count (for game method)</param>
		[HarmonyReversePatch]
		[HarmonyPatch((typeof(BuildingAI)), "EnsureCitizenUnits")]
		[MethodImpl(MethodImplOptions.NoInlining)]
		public static void EnsureCitizenUnitsRev(object instance, ushort buildingID, ref Building data, int homeCount, int workCount, int visitCount, int studentCount)
		{
			Logging.Error("EnsureCitizenUnits reverse Harmony patch wasn't applied with params ", instance.ToString(), ":", buildingID.ToString(), ":", data.ToString(), ":", homeCount.ToString(), ":", workCount.ToString(), ":", visitCount.ToString(), ":", studentCount.ToString());
			throw new NotImplementedException("Harmony reverse patch not applied");
		}
	}
}