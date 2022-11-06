// <copyright file="SavePanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.IO;
    using System.Xml;
    using System.Xml.Serialization;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.Math;
    using ColossalFramework.UI;

    /// <summary>
    /// This panel is in the middle column on the bottom. It contains buttons to action changes to the RICO settings file and apply changes to the live game.
    /// </summary>
    internal class SavePanel : UIPanel
    {
        // Panel components.
        private UIButton _saveButton;
        private UIButton _addLocalButton;
        private UIButton _removeLocalButton;
        private UIButton _applyButton;

        // Selection reference.
        private BuildingData currentSelection;

        /// <summary>
        /// Updates the current selection.
        /// </summary>
        /// <param name="buildingData">New selection.</param>
        internal void SelectionChanged(BuildingData buildingData)
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
            _saveButton = UIButtons.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_SAV"), buttonWidth);
            _saveButton.eventClick += (c, p) => Save();

            // Add local settings button.
            _addLocalButton = UIButtons.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_ADD"), buttonWidth);
            _addLocalButton.eventClick += (c, p) => AddLocal();

            // 'Remove local settings' button.
            _removeLocalButton = UIButtons.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_REM"), buttonWidth);
            _removeLocalButton.eventClick += (c, p) => RemoveLocal();

            // Warning label for 'apply changes' being experimental.
            UILabel warningLabel = this.AddUIComponent<UILabel>();
            warningLabel.textAlignment = UIHorizontalAlignment.Center;
            warningLabel.autoSize = false;
            warningLabel.autoHeight = true;
            warningLabel.wordWrap = true;
            warningLabel.width = this.width - autoLayoutPadding.left - autoLayoutPadding.right;
            warningLabel.text = "\r\n" + Translations.Translate("PRR_EXP");

            // 'Save and apply changes' button.
            _applyButton = UIButtons.AddButton(this, autoLayoutPadding.left, 0f, Translations.Translate("PRR_SAV_APP"), buttonWidth, scale: 0.8f);
            _applyButton.eventClick += (c, p) => SaveAndApply();
            _applyButton.wordWrap = true;
        }

        /// <summary>
        /// Saves the current RICO settings to file.
        /// </summary>
        private void Save()
        {
            // Read current settings from UI elements and convert to XML.
            SettingsPanelManager.Panel.Save();

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
                    if (buildingDef.Name != currentSelection.Name)
                    {
                        newLocalSettings.Buildings.Add(buildingDef);
                    }
                }

                // If current selection has local settings, add them to the replacement file.
                if (currentSelection.HasLocal)
                {
                    newLocalSettings.Buildings.Add(currentSelection.Local);
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
            SettingsPanelManager.Panel.UpdateSelectedBuilding(currentSelection);
        }

        /// <summary>
        /// Saves the current RICO settings to file and then applies them live in-game.
        /// </summary>
        private void SaveAndApply()
        {
            // Find current prefab instance.
            BuildingData currentBuildingData = PrefabManager.PrefabDictionary[currentSelection.Prefab];

            // Save first.
            Save();

            // Get the currently applied RICO settings (local, author, mod).
            RICOBuilding currentData = currentSelection.ActiveSetting;

            if (currentData != null)
            {
                // Convert the 'live' prefab (instance in PrefabCollection) and update household count and builidng level for all current instances.
                PrefabManager.ConvertPrefab(currentData, PrefabCollection<BuildingInfo>.FindLoaded(currentBuildingData.Prefab.name));
                CitizenUnitUtils.UpdateCitizenUnits(currentBuildingData.Prefab, false);
            }
            else
            {
                Logging.Message("no current RICO settings to apply to prefab ", currentBuildingData.Prefab.name);
            }

            // Force an update of all panels with current values.
            SettingsPanelManager.Panel.UpdateSelectedBuilding(currentSelection);
        }

        /// <summary>
        /// Adds new (default) local RICO settings to the selected building.
        /// </summary>
        private void AddLocal()
        {
            // Don't do anything if there's already local settings.
            if (currentSelection.HasLocal)
            {
                return;
            }

            // Log warning for any service building conversion.
            ItemClass.Service originalService = currentSelection.Prefab.GetService();
            if (originalService == ItemClass.Service.Education || originalService == ItemClass.Service.Electricity || originalService == ItemClass.Service.FireDepartment || originalService == ItemClass.Service.PoliceDepartment || originalService == ItemClass.Service.PublicTransport || originalService == ItemClass.Service.PlayerEducation || originalService == ItemClass.Service.PlayerIndustry || originalService == ItemClass.Service.Water || originalService == ItemClass.Service.HealthCare || originalService == ItemClass.Service.Disaster || originalService == ItemClass.Service.Garbage)
            {
                Logging.KeyMessage("Prefab ", currentSelection.Prefab?.name ?? "null", " is being converted to local RICO settings despite being a service building");
            }

            // Create new local settings.
            currentSelection.Local = new RICOBuilding();
            currentSelection.HasLocal = true;

            // If selected asset has author or mod settings (in order), copy those to the local settings.
            if (currentSelection.HasAuthor)
            {
                currentSelection.Local = (RICOBuilding)currentSelection.Author.Clone();

                // Overwrite author name with true prefab name.
                currentSelection.Local.Name = currentSelection.Name;
            }
            else if (currentSelection.HasMod)
            {
                currentSelection.Local = (RICOBuilding)currentSelection.Mod.Clone();
            }
            else
            {
                // Set some basic settings for assets with no settings.
                currentSelection.Local.Name = currentSelection.Name;
                currentSelection.Local.m_ricoEnabled = true;
                currentSelection.Local.m_service = GetRICOService();
                currentSelection.Local.m_subService = GetRICOSubService();
                currentSelection.Local.m_level = (int)currentSelection.Prefab.GetClassLevel() + 1;
                currentSelection.Local.ConstructionCost = 10;

                // See if selected 'virgin' prefab has Private AI.
                if (currentSelection.Prefab.GetAI() is PrivateBuildingAI privateAI)
                {
                    // It does - let's copy across growable statuts and household/workplace info.
                    int buildingWidth = currentSelection.Prefab.GetWidth();
                    int buildingLength = currentSelection.Prefab.GetLength();

                    // Set homes/workplaces.
                    if (privateAI is ResidentialBuildingAI)
                    {
                        // It's residential - set homes.
                        currentSelection.Local.m_homeCount = privateAI.CalculateHomeCount(currentSelection.Prefab.GetClassLevel(), default, buildingWidth, buildingLength);
                    }
                    else
                    {
                        // Not residential - set workplaces.
                        int[] workplaces = new int[4];

                        privateAI.CalculateWorkplaceCount(currentSelection.Prefab.GetClassLevel(), default, buildingWidth, buildingLength, out workplaces[0], out workplaces[1], out workplaces[2], out workplaces[3]);

                        currentSelection.Local.Workplaces = workplaces;
                    }

                    // Set as growable if building is appropriate size.
                    if (buildingWidth <= 4 && buildingLength <= 4)
                    {
                        currentSelection.Local.m_growable = true;
                    }
                }
                else
                {
                    // Basic catchall defaults for homes and workplaces.
                    currentSelection.Local.m_homeCount = 1;
                    currentSelection.Local.Workplaces = new int[] { 1, 0, 0, 0 };
                }

                // UI Category will be updated later.
                currentSelection.Local.UiCategory = "none";
            }

            // Update settings panel with new settings if RICO is enabled for this building.
            SettingsPanelManager.Panel.UpdateSelectedBuilding(currentSelection);

            // Refresh the selection list (to make sure settings checkboxes reflect new state).
            SettingsPanelManager.Panel.RefreshList();

            // Update UI category.
            SettingsPanelManager.Panel.UpdateUICategory();

            // Save new settings to file.
            Save();
        }

        /// <summary>
        /// Removes RICO local settings from the currently selected prefab.
        /// </summary>
        private void RemoveLocal()
        {
            // Don't do anything if there's no selection or selection has no local settings.
            if (currentSelection == null || !currentSelection.HasLocal)
            {
                return;
            }

            // Destroy local settings.
            currentSelection.Local = null;
            currentSelection.HasLocal = false;

            // Update the current selection now that it no longer has local settings.
            SettingsPanelManager.Panel.UpdateSelectedBuilding(currentSelection);

            // Refresh the selection list (to make sure settings checkboxes reflect new state).
            SettingsPanelManager.Panel.RefreshList();

            // Update settings file with change.
            Save();
        }

        /// <summary>
        /// Returns the RICO service (as a string) of the currently selected prefab.
        /// Used to populate intial values when local settings are created from 'virgin' prefabs.
        /// </summary>
        /// <returns>RICO service for this prefab.</returns>
        private string GetRICOService()
        {
            switch (currentSelection.Prefab.m_class.m_service)
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
        /// <returns>RICO subservice for this prefab.</returns>
        private string GetRICOSubService()
        {
            switch (currentSelection.Prefab.m_class.m_subService)
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
    }
}