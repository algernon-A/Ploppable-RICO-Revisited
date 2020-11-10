﻿using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using ColossalFramework;
using HarmonyLib;


namespace PloppableRICO
{
	/// <summary>
	/// Harmony Prefix patch for BuildingTool.IsImportantBuilding, to help protect RICO buildings from automated bulldozing.
	/// Based on boformers Larger Footprints mod.  Many thanks to him for his work.
	/// </summary>
	[HarmonyPatch(typeof(BuildingTool), "IsImportantBuilding")]
	[HarmonyPatch(new Type[] { typeof(BuildingInfo), typeof(Building) },
		new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
	internal static class ImportantBuildingPatch
	{
		/// <summary>
		/// Harmony Prefix patch to ensure that ploppable RICO buildings are classified as "important buildings".  This saves them from auto-demolition.
		/// </summary>
		/// <param name="__result">Original method result</param>
		/// <param name="info">Building prefab</param>
		/// <param name="building">Building instance data (ingnored)</param>
		/// <returns>False (don't continue execution chain) if this is a RICO building (original return value changed to true), true (continue exection chain) otherwise.</returns>
		private static bool Prefix (ref bool __result, BuildingInfo info, ref Building building)
		{
			// Only do this if our settings are set to ensure RICO buildings are important.
			if (!ModSettings.autoDemolish)
			{
				// All we want to do here is ensure that ploppable RICO buildings are classified as "Important Buildings" (to "spare them from the wrath of the BuildingTool"...)
				if (info.m_buildingAI is PloppableOfficeAI || info.m_buildingAI is PloppableExtractorAI || info.m_buildingAI is PloppableResidentialAI || info.m_buildingAI is PloppableCommercialAI || info.m_buildingAI is PloppableIndustrialAI)
				{
					// Found a ploppable RICO building - set original method return value.
					__result = true;

					// Don't execute base method after this.
					return false;
				}
			}

			// Didn't find a ploppable RICO building (or the RICO important setting isn't set) - go onto running the original game method.
            return true;
		}
	}


	/// <summary>
	/// Harmony Postfix patch for BuildingTool.Createbuilding, to enable instant construction for plopped RICO growables and to apply the 'Make Historical' setting as required. 
	/// </summary>
	[HarmonyPatch(typeof(BuildingTool), "CreateBuilding")]
	[HarmonyPatch(new Type[] { typeof(BuildingInfo), typeof(Vector3), typeof(float), typeof(int), typeof(bool), typeof(bool) },
		new ArgumentType[] { ArgumentType.Ref, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal, ArgumentType.Normal })]
	internal class CreateBuildingPatch
	{
		/// <summary>
		/// Harmony Postfix patch to skip 'gradual construction' for plopped RICO growables, and/or to apply the 'Make Historical' setting on building creation, accoriding to settings.
		/// </summary>
		/// <param name="__result">Original method result (unchanged)</param>
		/// <param name="info">BuildingInfo prefab for this building (unchanged)</param>
		/// <param name="position">Building position (ignored)</param>
		/// <param name="angle">Building rotation (ignored)</param>
		/// <param name="relocating">Building relocation (ignored)</param>
		/// <param name="needMoney">Is money needed (ignored)</param>
		/// <param name="fixedHeight">Fixed height (ignored)</param>
		private static void Postfix(ref ushort __result, ref BuildingInfo info, Vector3 position, float angle, int relocating, bool needMoney, bool fixedHeight)
		{
			// Check that we have a valid building ID.
			if (__result == 0)
			{
				return;
			}

			// Get building AI.
			PrivateBuildingAI buildingAI = info.GetAI() as PrivateBuildingAI;

			// Only interested in private building AI.
			if (buildingAI != null)
			{
				// Check if AI is a RICO custom AI type.
				bool isRICO = RICOUtils.IsRICOAI(buildingAI);

				// Check if it's a RICO custom AI type.
				// Enable 'ploppable growables' if option is set.
				if ((ModSettings.plopOther && !isRICO) || (ModSettings.plopRico && isRICO))
				{
					// Check to see if construction time is greater than zero.
					if (buildingAI.m_constructionTime > 0)
					{
						Building data = Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result];

						Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result].m_frame0.m_constructState = byte.MaxValue;
						BuildingCompletedRev(buildingAI, __result, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result]);

						// Have to do this manually as CommonBuildingAI.BuildingCompleted won't if construction time isn't zero.
						Singleton<BuildingManager>.instance.UpdateBuildingRenderer(__result, updateGroup: true);
					}
				}

				// Enable 'Make Historical' if option is set.
				if ((ModSettings.historicalOther && !isRICO) || (ModSettings.historicalRico && isRICO))
				{
					info.m_buildingAI.SetHistorical(__result, ref Singleton<BuildingManager>.instance.m_buildings.m_buffer[__result], historical: true);
				}
			}
		}


		/// <summary>
		/// Harmony reverse patch to access original protected method.
		/// </summary>
		/// <param name="instance">Original object instance</param>
		/// <param name="buildingID">Building instance ID</param>
		/// <param name="buildingData">Building instance data</param>
		[HarmonyReversePatch]
		[HarmonyPatch((typeof(CommonBuildingAI)), "BuildingCompleted")]
		[HarmonyPatch(new Type[] { typeof(ushort), typeof(Building) }, new ArgumentType[] { ArgumentType.Normal, ArgumentType.Ref })]
		[MethodImpl(MethodImplOptions.NoInlining)]
		protected static void BuildingCompletedRev(object instance, ushort buildingID, ref Building buildingData)
		{
			Debugging.Message("BuildingCompleted reverse Harmony patch wasn't applied");
		}
	}
}
