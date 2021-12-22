using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using ColossalFramework;
using ColossalFramework.UI;
using ColossalFramework.Math;


namespace PloppableRICO
{
    /// <summary>
    ///This panel is in the middle column on the bottom. It contains buttons to action changes to the RICO settings file and apply changes to the live game.
    /// </summary>
    public class UISavePanel : UIPanel
    {
        // Panel components.
        private UIButton saveButton;
        private UIButton addLocalButton;
        private UIButton removeLocalButton;
        private UIButton applyButton;

        // Selection reference.
        private BuildingData currentSelection;


        public void SelectionChanged(BuildingData buildingData)
        {
            currentSelection = buildingData;
        }

        /// <summary>
        /// Performs initial setup for the panel; we no longer use Start() as that's not sufficiently reliable (race conditions), and is no longer needed, with the new create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Basic setup.
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding.top = 5;
            autoLayoutPadding.left = 5;
            autoLayoutPadding.right = 5;
            builtinKeyNavigation = true;
            clipChildren = true;

            // Standardise button widths.
            float buttonWidth = this.width - autoLayoutPadding.left - autoLayoutPadding.right;

            // Save button.
            saveButton = UIControls.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_SAV"), buttonWidth);
            saveButton.eventClick += (control, clickEvent) => Save();

            // Add local settings button.
            addLocalButton = UIControls.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_ADD"), buttonWidth);
            addLocalButton.eventClick += (control, clickEvent) => AddLocal();

            // 'Remove local settings' button.
            removeLocalButton = UIControls.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_REM"), buttonWidth);
            removeLocalButton.eventClick += (control, clickEvent) => RemoveLocal();

            // Warning label for 'apply changes' being experimental.
            UILabel warningLabel = this.AddUIComponent<UILabel>();
            warningLabel.textAlignment = UIHorizontalAlignment.Center;
            warningLabel.autoSize = false;
            warningLabel.autoHeight = true;
            warningLabel.wordWrap = true;
            warningLabel.width = this.width - autoLayoutPadding.left - autoLayoutPadding.right;
            warningLabel.text = "\r\n" + Translations.Translate("PRR_EXP");

            // 'Save and apply changes' button.
            applyButton = UIControls.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_APP"), buttonWidth, scale: 0.8f);
            applyButton.eventClick += (control, clickEvent) => SaveAndApply();
            applyButton.wordWrap = true;
        }


        /// <summary>
        /// Saves the current RICO settings to file.
        /// </summary>
        private void Save()
        {
            // Read current settings from UI elements and convert to XML.
            SettingsPanel.Panel.Save();

            // If the local settings file doesn't already exist, create a new blank template.
            if (!File.Exists("LocalRICOSettings.xml"))
            {
                PloppableRICODefinition newLocalSettings = new PloppableRICODefinition();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));

                // Create blank file template.
                using (XmlWriter writer = XmlWriter.Create("LocalRICOSettings.xml"))
                {
                    xmlSerializer.Serialize(writer, newLocalSettings);
                }
            }

            // Check that file exists before continuing (it really should at this point, but just in case).
            if (File.Exists("LocalRICOSettings.xml"))
            {
                PloppableRICODefinition oldLocalSettings;
                PloppableRICODefinition newLocalSettings = new PloppableRICODefinition();
                XmlSerializer xmlSerializer = new XmlSerializer(typeof(PloppableRICODefinition));

                // Read existing file.
                using (StreamReader streamReader = new StreamReader("LocalRICOSettings.xml"))
                {
                    oldLocalSettings = xmlSerializer.Deserialize(streamReader) as PloppableRICODefinition;
                }

                // Loop though all buildings in the existing file. If they aren't the current selection, write them back to the replacement file.
                foreach (RICOBuilding buildingDef in oldLocalSettings.Buildings)
                {
                    if (buildingDef.Name != currentSelection.name)
                    {
                        newLocalSettings.Buildings.Add(buildingDef);
                    }
                }

                // If current selection has local settings, add them to the replacement file. 
                if (currentSelection.hasLocal)
                {
                    newLocalSettings.Buildings.Add(currentSelection.local);
                }

                // Write replacement file to disk.
                using (TextWriter writer = new StreamWriter("LocalRICOSettings.xml"))
                {
                    xmlSerializer.Serialize(writer, newLocalSettings);
                }
            }
            else
            {
                Logging.Error("couldn't find local settings file to save");
            }

            // Force an update of all panels with current values.
            SettingsPanel.Panel.UpdateSelectedBuilding(currentSelection);
        }


        /// <summary>
        /// Saves the current RICO settings to file and then applies them live in-game.
        /// </summary>
        private void SaveAndApply()
        {
            // Find current prefab instance.
            BuildingData currentBuildingData = Loading.xmlManager.prefabHash[currentSelection.prefab];

            // Save first.
            Save();

            // If we're converting a residential building to something else, then we first should clear out all households.
            if (currentBuildingData.prefab.GetService() == ItemClass.Service.Residential && !IsCurrentResidential())
            {
                // removeAll argument to true to remove all households.
                UpdateHouseholds(currentBuildingData.prefab.name, removeAll: true);
            }

            // Get the currently applied RICO settings (local, author, mod).
            RICOBuilding currentData = RICOUtils.CurrentRICOSetting(currentSelection);

            if (currentData != null)
            {
                // Convert the 'live' prefab (instance in PrefabCollection) and update household count and builidng level for all current instances.
                Loading.convertPrefabs.ConvertPrefab(currentData, PrefabCollection<BuildingInfo>.FindLoaded(currentBuildingData.prefab.name));
                UpdateHouseholds(currentBuildingData.prefab.name, currentData.level);
            }
            else
            {
                Logging.Message("no current RICO settings to apply to prefab ", currentBuildingData.prefab.name);
            }

            // Force an update of all panels with current values.
            SettingsPanel.Panel.UpdateSelectedBuilding(currentSelection);
        }


        /// <summary>
        /// Adds new (default) local RICO settings to the selected building.
        /// </summary>
        private void AddLocal()
        {
            // Don't do anything if there's already local settings.
            if (currentSelection.hasLocal)
            {
                return;
            }

            // Log warning for any service building conversion.
            ItemClass.Service originalService = currentSelection.prefab.GetService();
            if (originalService == ItemClass.Service.Education || originalService == ItemClass.Service.Electricity || originalService == ItemClass.Service.FireDepartment || originalService == ItemClass.Service.PoliceDepartment || originalService == ItemClass.Service.PublicTransport || originalService == ItemClass.Service.PlayerEducation || originalService == ItemClass.Service.PlayerIndustry || originalService == ItemClass.Service.Water || originalService == ItemClass.Service.HealthCare || originalService == ItemClass.Service.Disaster || originalService == ItemClass.Service.Garbage)
            {
                Logging.KeyMessage("Prefab ", currentSelection.prefab?.name ?? "null", " is being converted to local RICO settings despite being a service building");
            }

            // Create new local settings.
            currentSelection.local = new RICOBuilding();
            currentSelection.hasLocal = true;

            // If selected asset has author or mod settings (in order), copy those to the local settings.
            if (currentSelection.hasAuthor)
            {
                currentSelection.local = (RICOBuilding)currentSelection.author.Clone();

                // Overwrite author name with true prefab name.
                currentSelection.local.Name = currentSelection.name; 
            }
            else if (currentSelection.hasMod)
            {
                currentSelection.local = (RICOBuilding)currentSelection.mod.Clone();
            }
            else
            {
                // Set some basic settings for assets with no settings.
                currentSelection.local.Name = currentSelection.name;
                currentSelection.local.ricoEnabled = true;
                currentSelection.local.service = GetRICOService();
                currentSelection.local.subService = GetRICOSubService();
                currentSelection.local.level = (int)currentSelection.prefab.GetClassLevel() + 1;
                currentSelection.local.ConstructionCost = 10;

                // See if selected 'virgin' prefab has Private AI.
                if (currentSelection.prefab.GetAI() is PrivateBuildingAI privateAI)
                {
                    // It does - let's copy across growable statuts and household/workplace info.
                    int buildingWidth = currentSelection.prefab.GetWidth();
                    int buildingLength = currentSelection.prefab.GetLength();

                    // Set homes/workplaces.
                    if (privateAI is ResidentialBuildingAI)
                    {
                        // It's residential - set homes.
                        currentSelection.local.homeCount = privateAI.CalculateHomeCount(currentSelection.prefab.GetClassLevel(), new Randomizer(), buildingWidth, buildingLength);
                    }
                    else
                    {
                        // Not residential - set workplaces.
                        int[] workplaces = new int[4];

                        privateAI.CalculateWorkplaceCount(currentSelection.prefab.GetClassLevel(), new Randomizer(), buildingWidth, buildingLength, out workplaces[0], out workplaces[1], out workplaces[2], out workplaces[3]);

                        currentSelection.local.Workplaces = workplaces;
                    }

                    // Set as growable if building is appropriate size.
                    if (buildingWidth <= 4 && buildingLength <= 4)
                    {
                        currentSelection.local.growable = true;
                    }
                }
                else
                {
                    // Basic catchall defaults for homes and workplaces.
                    currentSelection.local.homeCount = 1;
                    currentSelection.local.Workplaces = new int[] { 1, 0, 0, 0 };
                }

                // UI Category will be updated later.
                currentSelection.local.UiCategory = "none";
            }

            // Update settings panel with new settings if RICO is enabled for this building.
            SettingsPanel.Panel.UpdateSelectedBuilding(currentSelection);

            // Refresh the selection list (to make sure settings checkboxes reflect new state).
            SettingsPanel.Panel.RefreshList();

            // Update UI category.
            SettingsPanel.Panel.UpdateUICategory();

            // Save new settings to file.
            Save();
        }


        /// <summary>
        /// Removes RICO local settings from the currently selected prefab.
        /// </summary>
        private void RemoveLocal()
        {
            // Don't do anything if there's no selection or selection has no local settings.
            if (currentSelection == null || !currentSelection.hasLocal)
            {
                return;
            }

            // Destroy local settings.
            currentSelection.local = null;
            currentSelection.hasLocal = false;

            // Update the current selection now that it no longer has local settings.
            SettingsPanel.Panel.UpdateSelectedBuilding(currentSelection);

            // Refresh the selection list (to make sure settings checkboxes reflect new state).
            SettingsPanel.Panel.RefreshList();

            // Update settings file with change.
            Save();
        }


        /// <summary>
        /// Returns the RICO service (as a string) of the currently selected prefab.
        /// Used to populate intial values when local settings are created from 'virgin' prefabs.
        /// </summary>
        /// <returns>RICO service</returns>
        private string GetRICOService()
        {
            switch (currentSelection.prefab.m_class.m_service)
            {
                case ItemClass.Service.Commercial:
                    return "commercial";
                case ItemClass.Service.Industrial:
                    return "industrial";
                case ItemClass.Service.Office:
                    return "office";
                default:
                    return "residential";
            }
        }


        /// <summary>
        /// Returns the RICO subservice (as a string) of the currently selected prefab.
        /// Used to populate intial values when local settings are created from 'virgin' prefabs.
        /// </summary>
        /// <returns>RICO subservice</returns>
        private string GetRICOSubService()
        {
            switch (currentSelection.prefab.m_class.m_subService)
            {
                case ItemClass.SubService.CommercialLow:
                    return "low";
                case ItemClass.SubService.CommercialHigh:
                    return "high";
                case ItemClass.SubService.CommercialTourist:
                    return "tourist";
                case ItemClass.SubService.CommercialLeisure:
                    return "leisure";
                case ItemClass.SubService.CommercialEco:
                    return "eco";
                case ItemClass.SubService.IndustrialGeneric:
                    return "generic";
                case ItemClass.SubService.IndustrialFarming:
                    return "farming";
                case ItemClass.SubService.IndustrialForestry:
                    return "forest";
                case ItemClass.SubService.IndustrialOil:
                    return "oil";
                case ItemClass.SubService.IndustrialOre:
                    return "ore";
                case ItemClass.SubService.OfficeGeneric:
                    return "none";
                case ItemClass.SubService.OfficeHightech:
                    return "high tech";
                case ItemClass.SubService.ResidentialLowEco:
                    return "low eco";
                case ItemClass.SubService.ResidentialHighEco:
                    return "high eco";
                case ItemClass.SubService.ResidentialLow:
                    return "low";
                default:
                    return "high";
            }
        }


        /// <summary>
        /// Updates household counts for all buildings in scene with the given prefab name.
        /// Can also remove all households, setting the total to zero.
        /// </summary>
        /// <param name="prefabName">Prefab name</param>
        /// <param name="removeAll">If true, all households will be removed (count set to 0)</param>
        private void UpdateHouseholds(string prefabName, int level = 0, bool removeAll = false)
        {
            int homeCount = 0;
            int visitCount = 0;
            int homeCountChanged = 0;

            // Get game buildings.
            Building[] buildingBuffer = Singleton<BuildingManager>.instance.m_buildings.m_buffer;


            // Iterate through each building in the scene.
            for (ushort i = 0; i < buildingBuffer.Length; i++)
            {
                // Check for matching name.
                if (buildingBuffer[i].Info != null && buildingBuffer[i].Info.name != null && buildingBuffer[i].Info.name.Equals(prefabName))
                {
                    // Got a match!  Check level if applicable.
                    if (level > 0)
                    {
                        // m_level is one less than building.level.
                        byte newLevel = (byte)(level - 1);

                        if (buildingBuffer[i].m_level != newLevel)
                        {
                            Logging.Message("found building '", prefabName, "' with level ", (buildingBuffer[i].m_level + 1).ToString(), ", overriding to level ", level.ToString());
                            buildingBuffer[i].m_level = newLevel;
                        }
                    }

                    // Update homecounts for any residential buildings.
                    PrivateBuildingAI thisAI = buildingBuffer[i].Info.GetAI() as ResidentialBuildingAI;
                    if (thisAI != null)
                    {
                        // This is residential! If we're not removing all households, recalculate home and visit counts using AI method.
                        if (!removeAll)
                        {
                            homeCount = thisAI.CalculateHomeCount((ItemClass.Level)buildingBuffer[i].m_level, new Randomizer(i), buildingBuffer[i].Width, buildingBuffer[i].Length);
                            visitCount = thisAI.CalculateVisitplaceCount((ItemClass.Level)buildingBuffer[i].m_level, new Randomizer(i), buildingBuffer[i].Width, buildingBuffer[i].Length);
                        }

                        // Apply changes via direct call to EnsureCitizenUnits prefix patch from this mod and increment counter.
                        RealisticCitizenUnits.EnsureCitizenUnits(ref thisAI, i, ref buildingBuffer[i], homeCount, 0, visitCount, 0);
                        homeCountChanged++;
                    }

                    // Clear any problems.
                    buildingBuffer[i].m_problems = 0;
                }
            }

            Logging.Message("set household counts to ", homeCount.ToString(), " for ", homeCountChanged.ToString(), " '", prefabName, "' buildings");
        }



        /// <summary>
        /// Checks to see if the currently applied RICO service for the selected building is residential.
        /// </summary>
        /// <returns>True if residential, otherwise false.</returns>
        private bool IsCurrentResidential()
        {
            if (currentSelection.hasLocal)
            {
                return currentSelection.local.service == "residential";
            }
            else if (currentSelection.hasAuthor)
            {
                return currentSelection.author.service == "residential";
            }
            else if (currentSelection.hasMod)
            {
                return currentSelection.mod.service == "residential";
            }

            return false;
        }
    }
}