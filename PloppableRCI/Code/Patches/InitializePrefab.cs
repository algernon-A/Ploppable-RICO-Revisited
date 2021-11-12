using System.IO;
using ColossalFramework.Packaging;
using HarmonyLib;


namespace PloppableRICO
{
	/// <summary>
	/// Patch for BuildingInfo.InitializePrefab, to read and apply RICO settings prior to prefab initialization.
	/// Doing it this way (as opposed to the previous approach of changing prefab settings after initialization) solves a whole lot of issues,
	/// and opens a lot of doors.
	/// </summary>
	[HarmonyPatch(typeof(BuildingInfo), "InitializePrefab")]
	public static class InitPatch
	{
		/// <summary>
		/// Harmony prefix patch for BuildingInfo.InitializePrefab.
		/// Reads and applies RICO settings prior to prefab initialization.
		/// Harmony priority is High, and before boformer's Prefab Hook, so that RICO assets are converted to their 'final' RICO state before initialization.
		/// </summary>
		/// <param name="__instance">Original method instance reference</param>
		/// <returns>Whether to continue InitializePrefab (always true)</returns>
		[HarmonyPriority(Priority.High)]
		[HarmonyBefore(new string[] { "github.com/boformer/PrefabHook" })]
		public static bool Prefix(BuildingInfo __instance)
		{
			// Basic sanity check before proceeding; if failed, don't do anything here - just continue on to game method.
			if (__instance.name == null)
			{
				return true;
			}

			// Create a new building record for this prefab and add it to our lists.
			BuildingData buildingData = new BuildingData
			{
				prefab = __instance,
				name = __instance.name,
				density = Loading.xmlManager.SetPrefabDensity(__instance),
				category = Loading.xmlManager.AssignCategory(__instance),
			};
			Loading.xmlManager.prefabHash[__instance] = buildingData;

			// Add to broken prefabs list (will be removed later if it's not broken).
			Loading.brokenPrefabs.Add(__instance);

			// Search for PloppableRICODefinition.xml files with this asset.
			// Need to use FindAssetByName(string, AssetType) because FindAssetByName(string) doesn't catch all assets at this stage of initialisation
			// (those two methods are more different than you might think - discovered that the hard way).
			Package.Asset asset = PackageManager.FindAssetByName(__instance.name, Package.AssetType.Object);

			// Get custom asset filesystem location (if CRP pacakge).
			string crpPath = asset?.package?.packagePath;

			if (!string.IsNullOrEmpty(crpPath))
			{
				// Look for RICO settings file.
				string ricoDefPath = Path.Combine(Path.GetDirectoryName(crpPath), "PloppableRICODefinition.xml");

				if (File.Exists(ricoDefPath))
				{
					// Parse the file.
					PloppableRICODefinition tempRicoDef = RICOReader.ParseRICODefinition(ricoDefPath);

					if (tempRicoDef != null)
					{
						foreach (RICOBuilding buildingDef in tempRicoDef.Buildings)
						{
							// Go through each building parsed and check to see if we've got a match for this prefab.
							if (MatchRICOName(buildingDef.Name, __instance.name, asset.package.packageName))
							{
								// Match!  Add these author settings to our prefab dictionary.
								Logging.Message("found author settings for ", buildingDef.Name);
								Loading.xmlManager.prefabHash[__instance].author = buildingDef;
								Loading.xmlManager.prefabHash[__instance].hasAuthor = true;
							}
						}
					}
				}
			}


			// Check for and add any local settings for this prefab to our list.
			if (Loading.localRicoDef != null)
			{
				// Step through our previously loaded local settings and see if we've got a match.
				foreach (RICOBuilding buildingDef in Loading.localRicoDef.Buildings)
				{
					if (buildingDef.Name.Equals(__instance.name))
					{
						// Match!  Add these author settings to our prefab dictionary.
						Loading.xmlManager.prefabHash[__instance].local = buildingDef;
						Loading.xmlManager.prefabHash[__instance].hasLocal = true;
					}
				}
			}


			// Check for any Workshop RICO mod settings for this prefab.
			if (Loading.mod1RicoDef != null)
			{
				// Step through our previously loaded local settings and see if we've got a match.
				foreach (RICOBuilding buildingDef in Loading.mod1RicoDef.Buildings)
				{
					if (buildingDef.Name.Equals(__instance.name))
					{
						// Match!  Add these author settings to our prefab dictionary.
						Loading.xmlManager.prefabHash[__instance].mod = buildingDef;
						Loading.xmlManager.prefabHash[__instance].hasMod = true;
					}
				}
			}

			// Check for Modern Japan CCP mod settings for this prefab.
			if (Loading.mod2RicoDef != null)
			{
				// Step through our previously loaded local settings and see if we've got a match.
				foreach (RICOBuilding buildingDef in Loading.mod2RicoDef.Buildings)
				{
					if (buildingDef.Name.Equals(__instance.name))
					{
						// Match!  Add these author settings to our prefab dictionary.
						Loading.xmlManager.prefabHash[__instance].mod = buildingDef;
						Loading.xmlManager.prefabHash[__instance].hasMod = true;
					}
				}
			}


			// Apply appropriate RICO settings to prefab.
			if (Loading.convertPrefabs != null)
			{
				// Start with local settings.
				if (Loading.xmlManager.prefabHash[__instance].hasLocal)
				{
					// If local settings disable RICO, dont convert.
					if (Loading.xmlManager.prefabHash[__instance].local.ricoEnabled)
					{
						Loading.convertPrefabs.ConvertPrefab(Loading.xmlManager.prefabHash[__instance].local, __instance);
					}
				}
				// If no local settings, apply author settings.
				else if (Loading.xmlManager.prefabHash[__instance].hasAuthor)
				{
					// If author settings disable RICO, dont convert.
					if (Loading.xmlManager.prefabHash[__instance].author.ricoEnabled)
					{
						Loading.convertPrefabs.ConvertPrefab(Loading.xmlManager.prefabHash[__instance].author, __instance);
					}
				}
				// If none of the above, apply mod settings.
				else if (Loading.xmlManager.prefabHash[__instance].hasMod)
				{
					// If mod settings disable RICO, dont convert.
					if (Loading.xmlManager.prefabHash[__instance].mod.ricoEnabled)
					{
						Loading.convertPrefabs.ConvertPrefab(Loading.xmlManager.prefabHash[__instance].mod, __instance);
					}
				}
				else 
				{
					// No RICO settings; replicate game InitializePrefab checks overridden by transpiler.
					int privateServiceIndex = ItemClass.GetPrivateServiceIndex(__instance.m_class.m_service);
					if (privateServiceIndex != -1)
					{
						if (__instance.m_placementStyle == ItemClass.Placement.Manual)
						{
							throw new PrefabException(__instance, "Private building cannot have manual placement style");
						}
						if (__instance.m_paths != null && __instance.m_paths.Length != 0)
						{
							throw new PrefabException(__instance, "Private building cannot include roads or other net types");
						}
					}
				}
			}
			else
			{
				// This means that there's been a significant failure.  Ploppable RICO settings can't be applied.
				Logging.Error("convertPrefabs not initialised");
			}

			// If we've made it this far, the patch is working fine - set the watchdog flag to confirm.
			Loading.patchOperating = true;

			// Continue on to execute game InitializePrefab.
			return true;
		}


		/// <summary>
		/// Harmony prefix patch for BuildingInfo.InitializePrefab.
		/// Confirms that the prefab made it through initialisation.
		/// </summary>
		/// <param name="__instance"></param>
		public static void Postfix(BuildingInfo __instance)
		{
			// If we've made it here, the asset has initialised correctly (no PrefabExceptions thrown); remove it from broken prefabs list.
			Loading.brokenPrefabs.Remove(__instance);
		}


		/// <summary>
		/// Attempts to match a given RICO name with a prefab name.
		/// </summary>
		/// <param name="ricoName">Name parsed from Ploppable RICO definition file</param>
		/// <param name="prefabName">BuildingInfo prefab name to match against</param>
		/// <param name="packageName">Prefab package name</param>
		/// <returns>True if a match was found, false otherwise.</returns>
		private static bool MatchRICOName(string ricoName, string prefabName, string packageName)
		{
			// Get common string combinations.
			string ricoNameData = ricoName + "_Data";
			string packagePrefix = packageName + ".";

			// Ordered in order of assumed probability.
			// Standard full workshop asset name - all local settings for workshop assets should match against this, as well as many author settings files.

			if (prefabName.Equals(packagePrefix + ricoNameData))
			{
				return true;
			}
			// Asset creators that append "_Data" after their name.
			if (prefabName.Equals(packagePrefix + ricoName))
			{
				return true;
			}
			// The workshop package ID isn't included in the RICO settings file - common amongst workshop assets.
			if (prefabName.Equals(ricoNameData))
			{
				return true;
			}
			// Direct match - mostly applies to game assets, but some workshop assets may also match here.
			else if (prefabName.Equals(ricoName))
			{
				return true;
			}
			// No match.
			return false;
		}
	}
}
