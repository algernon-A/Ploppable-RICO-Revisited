namespace PloppableRICO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons;
    using AlgernonCommons.UI;
    using AlgernonCommons.Translation;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Settings panel manager static class.
    /// </summary>
    public static class SettingsPanel
    {
        // Instance references.
        private static GameObject uiGameObject;
        private static RICOSettingsPanel _panel;
        public static RICOSettingsPanel Panel => _panel;

        // Previous selection.
        private static BuildingInfo lastSelection;
        private static bool[] lastFilter;
        private static int lastPostion;
        private static int lastIndex = -1;


        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Open(BuildingInfo selected = null)
        {
            try
            {
                // If no instance already set, create one.
                if (uiGameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    uiGameObject = new GameObject("RICOSettingsPanel");
                    uiGameObject.transform.parent = UIView.GetAView().transform;

                    _panel = uiGameObject.AddComponent<RICOSettingsPanel>();

                    // Set up panel.
                    Panel.Setup();
                }

                // Select appropriate building if there's a preselection.
                if (selected != null)
                {
                    Logging.Message("selecting preselected building ", selected.name);
                    Panel.SelectBuilding(selected);
                }
                else if (lastSelection != null)
                {
                    // Restore previous filter state.
                    if (lastFilter != null)
                    {
                        Panel.SetFilter(lastFilter);
                    }

                    // Restore previous building selection list postion and selected item (specifically in that order to ensure correct item is selected).
                    Panel.SetListPosition(lastIndex, lastPostion);
                    Panel.SelectBuilding(lastSelection);
                }

                Panel.Show();
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception opening settings panel");
                return;
            }
        }


        /// <summary>
        /// Closes the panel by destroying the object (removing any ongoing UI overhead).
        /// </summary>
        internal static void Close()
        {
            // Save current selection for next time.
            lastSelection = Panel?.currentSelection?.prefab;
            lastFilter = Panel?.GetFilter();
            Panel?.GetListPosition(out lastIndex, out lastPostion);

            GameObject.Destroy(_panel);
            GameObject.Destroy(uiGameObject);
        }


        /// <summary>
        /// Adds Ploppable RICO settings buttons to building info panels to directly access that building's RICO settings.
        /// </summary>
        internal static void AddInfoPanelButtons()
        {
            // Zoned building (PrivateBuilding) info panel.
            AddInfoPanelButton(UIView.library.Get<ZonedBuildingWorldInfoPanel>(typeof(ZonedBuildingWorldInfoPanel).Name));

            // Service building (PlayerBuilding) info panel.
            AddInfoPanelButton(UIView.library.Get<CityServiceWorldInfoPanel>(typeof(CityServiceWorldInfoPanel).Name));
        }


        /// <summary>
        /// Adds a Ploppable RICO button to a building info panel to directly access that building's RICO settings.
        /// The button will be added to the right of the panel with a small margin from the panel edge, at the relative Y position specified.
        /// </summary>
        /// <param name="infoPanel">Infopanel to apply the button to</param>
        private static void AddInfoPanelButton(BuildingWorldInfoPanel infoPanel)
        {
            UIButton panelButton = infoPanel.component.AddUIComponent<UIButton>();

            // Basic button setup.
            panelButton.size = new Vector2(34, 34);
            panelButton.normalBgSprite = "ToolbarIconGroup6Normal";
            panelButton.normalFgSprite = "IconPolicyBigBusiness";
            panelButton.focusedBgSprite = "ToolbarIconGroup6Focused";
            panelButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
            panelButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
            panelButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
            panelButton.name = "PloppableButton";
            panelButton.tooltip = Translations.Translate("PRR_SET_RICO");

            // Find ProblemsPanel relative position to position button.
            // We'll use 40f as a default relative Y in case something doesn't work.
            UIComponent problemsPanel;
            float relativeY = 40f;

            // Player info panels have wrappers, zoned ones don't.
            UIComponent wrapper = infoPanel.Find("Wrapper");
            if (wrapper == null)
            {
                problemsPanel = infoPanel.Find("ProblemsPanel");
            }
            else
            {
                problemsPanel = wrapper.Find("ProblemsPanel");
            }

            try
            {
                // Position button vertically in the middle of the problems panel.  If wrapper panel exists, we need to add its offset as well.
                relativeY = (wrapper == null ? 0 : wrapper.relativePosition.y) + problemsPanel.relativePosition.y + ((problemsPanel.height - 34) / 2);
            }
            catch
            {
                // Don't really care; just use default relative Y.
                Logging.Message("couldn't find ProblemsPanel relative position");
            }

            // Set position.
            panelButton.AlignTo(infoPanel.component, UIAlignAnchor.TopRight);
            panelButton.relativePosition += new Vector3(-5f, relativeY, 0f);

            // Event handler.
            panelButton.eventClick += (control, clickEvent) =>
            {
                // Select current building in the building details panel and show.
                Open(InstanceManager.GetPrefabInfo(WorldInfoPanel.GetCurrentInstanceID()) as BuildingInfo);

                // Manually unfocus control, otherwise it can stay focused until next UI event (looks untidy).
                control.Unfocus();
            };
        }
    }


    /// <summary>
    /// Base class of the RICO settings panel.  Based (via AJ3D's Ploppable RICO) ultimately on SamsamTS's Building Themes panel; many thanks to him for his work.
    /// </summary>
    public class RICOSettingsPanel : UIPanel
    {
        // Layout constants.
        private const float LeftWidth = 400f;
        private const float MiddleWidth = 250f;
        private const float RightWidth = 300f;
        private const float FilterHeight = UIBuildingFilter.FilterBarHeight;
        private const float PanelHeight = 550f;
        private const float BottomMargin = 10f;
        private const float Spacing = 5f;
        private const float CheckFilterHeight = UIBuildingFilter.SettingsFilterHeight;
        private const float TitleHeight = 40f;

        // Panel components.
        private UIBuildingFilter filterBar;
        private UIList buildingSelection;
        private UIPreviewPanel previewPanel;
        private UISavePanel savePanel;
        private UIBuildingOptions buildingOptionsPanel;

        // Selected items.
        internal BuildingData currentSelection;


        /// <summary>
        /// Called to save building data.
        /// </summary>
        internal void Save() => buildingOptionsPanel.SaveRICO();


        /// <summary>
        /// Refreshes the building selection list.
        /// </summary>
        public void RefreshList() => buildingSelection.Refresh();


        /// <summary>
        /// Updates the UI Category of the building in the options panel.
        /// </summary>
        internal void UpdateUICategory() => buildingOptionsPanel.UpdateUICategory();


        /// <summary>
        /// Gets the current filter state as a boolean array.
        /// </summary>
        /// <returns>Current filter toggle settings</returns>
        internal bool[] GetFilter() => filterBar.GetFilter();


        /// <summary>
        /// Sets the filter state to match a boolean array.
        /// </summary>
        internal void SetFilter(bool[] filterState) => filterBar.SetFilter(filterState);


        /// <summary>
        /// Gets the current index and list positions of the building selection list.
        /// </summary>
        /// <param name="selectedIndex">Index of currently selected item</param>
        /// <param name="listPosition">Current list position</param>
        internal void GetListPosition(out int selectedIndex, out int listPosition)
        {
            listPosition = buildingSelection.CurrentPosition;
            selectedIndex = buildingSelection.SelectedIndex;
        }


        /// <summary>
        /// Sets the current index and list positions of the building selection list.
        /// </summary>
        /// <param name="selectedIndex">Selected item index to set</param>
        /// <param name="listPosition">List position to set</param>
        internal void SetListPosition(int selectedIndex, int listPosition)
        {
            buildingSelection.CurrentPosition = listPosition;
            buildingSelection.SelectedIndex = selectedIndex;
        }


        /// <summary>
        /// Called to select a building from 'outside' the building details editor (e.g. by button on building info panel).
        /// Sets the filter to only display the relevant category for the relevant building, and makes that building selected in the list.
        /// </summary>
        /// <param name="buildingInfo">The BuildingInfo record for this building.</param>
        internal void SelectBuilding(BuildingInfo buildingInfo)
        {
            // Get the RICO BuildingData associated with this prefab.
            BuildingData building = Loading.xmlManager.prefabHash[buildingInfo];

            // Ensure the fastlist is filtered to include this building category only.
            filterBar.SelectBuildingCategory(building.category);
            buildingSelection.Data = GenerateFastList();

            // Find and select the building in the fastlist.
            buildingSelection.FindItem<BuildingData>(x => x.name.Equals(building.name));

            // Update the selected building to the current.
            UpdateSelectedBuilding(building);
        }


        /// <summary>
        /// Called when the building selection changes to update other panels.
        /// </summary>
        /// <param name="building"></param>
        internal void UpdateSelectedBuilding(BuildingData building)
        {
            if (building != null)
            {
                // Update sub-panels.
                currentSelection = Loading.xmlManager.prefabHash[building.prefab];

                buildingOptionsPanel.SelectionChanged(currentSelection);
                savePanel.SelectionChanged(currentSelection);
                previewPanel.Show(currentSelection);
            }
        }


        /// <summary>
        /// Performs initial setup for the panel; we no longer use Start() as that's not sufficiently reliable (race conditions), and is no longer needed, with the new create/destroy process.
        /// </summary>
        internal void Setup()
        {
            try
            {
                // Hide while we're setting up.
                isVisible = false;

                // Basic setup.
                canFocus = true;
                isInteractive = true;
                width = LeftWidth + MiddleWidth + RightWidth + (Spacing * 4);
                height = PanelHeight + TitleHeight + FilterHeight + (Spacing * 2) + BottomMargin;
                relativePosition = new Vector3(Mathf.Floor((GetUIView().fixedWidth - width) / 2), Mathf.Floor((GetUIView().fixedHeight - height) / 2));
                backgroundSprite = "UnlockingPanel2";

                // Make it draggable.
                UIDragHandle dragHandle = AddUIComponent<UIDragHandle>();
                dragHandle.width = width - 50;
                dragHandle.height = height;
                dragHandle.relativePosition = Vector3.zero;
                dragHandle.target = parent;

                // Decorative icon (top-left).
                UISprite iconSprite = AddUIComponent<UISprite>();
                iconSprite.spriteName = "ToolbarIconZoomOutCity";
                UISprites.ResizeSprite(iconSprite, 30f, 30f);
                iconSprite.relativePosition = new Vector2(10f, 5f);

                // Titlebar label.
                UILabel titleLabel = AddUIComponent<UILabel>();
                titleLabel.relativePosition = new Vector2(50f, 13f);
                titleLabel.text = Mod.Instance.Name;

                // Close button.
                UIButton closeButton = AddUIComponent<UIButton>();
                closeButton.relativePosition = new Vector3(width - 35f, 2f);
                closeButton.normalBgSprite = "buttonclose";
                closeButton.hoveredBgSprite = "buttonclosehover";
                closeButton.pressedBgSprite = "buttonclosepressed";
                closeButton.eventClick += (c, clickEvent) =>
                {
                    SettingsPanel.Close();
                };

                // Filter.
                filterBar = AddUIComponent<UIBuildingFilter>();
                filterBar.width = width - (Spacing * 2);
                filterBar.height = FilterHeight;
                filterBar.relativePosition = new Vector3(Spacing, TitleHeight);

                // Event handler to dealth with changes to filtering.
                filterBar.EventFilteringChanged += (component, value) =>
                {
                    if (value == -1) return;

                    int listCount = buildingSelection.Data.m_size;
                    int position = buildingSelection.CurrentPosition;

                    buildingSelection.Data = GenerateFastList();
                };

                // Set up panels.

                // Middle panel - building preview and edit panels.
                UIPanel middlePanel = AddUIComponent<UIPanel>();
                middlePanel.width = MiddleWidth;
                middlePanel.height = PanelHeight;
                middlePanel.relativePosition = new Vector3(LeftWidth + (Spacing * 2), TitleHeight + FilterHeight + Spacing);

                previewPanel = middlePanel.AddUIComponent<UIPreviewPanel>();
                previewPanel.width = middlePanel.width;
                previewPanel.height = (PanelHeight - Spacing) / 2;
                previewPanel.relativePosition = Vector3.zero;
                previewPanel.Setup();

                savePanel = middlePanel.AddUIComponent<UISavePanel>();
                savePanel.width = middlePanel.width;
                savePanel.height = (PanelHeight - Spacing) / 2;
                savePanel.relativePosition = new Vector3(0, previewPanel.height + Spacing);
                savePanel.Setup();

                // Right panel - mod calculations.
                UIPanel rightPanel = AddUIComponent<UIPanel>();
                rightPanel.width = RightWidth;
                rightPanel.height = PanelHeight;
                rightPanel.relativePosition = new Vector3(LeftWidth + MiddleWidth + (Spacing * 3), TitleHeight + FilterHeight + Spacing);

                buildingOptionsPanel = rightPanel.AddUIComponent<UIBuildingOptions>();
                buildingOptionsPanel.width = RightWidth;
                buildingOptionsPanel.height = PanelHeight;
                buildingOptionsPanel.relativePosition = Vector3.zero;
                buildingOptionsPanel.Setup();

                // Building selection list.
                buildingSelection = UIList.AddUIList<UIBuildingRow>(this, Spacing, TitleHeight + FilterHeight + CheckFilterHeight + Spacing, LeftWidth, PanelHeight - CheckFilterHeight, UIBuildingRow.CustomRowHeight);
                buildingSelection.EventSelectionChanged += (c, item) => UpdateSelectedBuilding(item as BuildingData);

                // Set up filterBar to make sure selection filters are properly initialised before calling GenerateFastList.
                filterBar.Setup();

                // Populate the list.
                buildingSelection.Data = GenerateFastList();
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception setting up settings panel");
            }
        }


        /// <summary>
        /// Generates the list of buildings depending on current filter settings.
        /// </summary>
        /// <returns></returns>
        private FastList<object> GenerateFastList()
        {
            // List to store all building prefabs that pass the filter.
            List<BuildingData> filteredList = new List<BuildingData>();

            // Iterate through all loaded building prefabs and add them to the list if they meet the filter conditions.
            foreach (BuildingData item in Loading.xmlManager.prefabHash.Values)
            {
                // Skip any null or invalid prefabs.
                if (item?.prefab == null)
                {
                    continue;
                }

                // Filter by zoning category.
                if (!filterBar.AllCatsSelected())
                {
                    Category category = item.category;
                    if (category == Category.None || !filterBar.IsCatSelected(category))
                    {
                        continue;
                    }
                }

                // Filter by settings.
                if (filterBar.SettingsFilter[0].isChecked && !item.hasMod) continue;
                if (filterBar.SettingsFilter[1].isChecked && !item.hasAuthor) continue;
                if (filterBar.SettingsFilter[2].isChecked && !item.hasLocal) continue;
                if (filterBar.SettingsFilter[3].isChecked && !(item.hasMod || item.hasAuthor || item.hasLocal)) continue;

                // Filter by name.
                if (!filterBar.FilterString.IsNullOrWhiteSpace() && !item.name.ToLower().Contains(filterBar.FilterString.ToLower()))
                {
                    continue;
                }

                // Finally!  We've got an item that's passed all filters; add it to the list.
                filteredList.Add(item);
            }

            // Create return list with our filtered list, sorted alphabetically.
            FastList<object> fastList = new FastList<object>
            {
                m_buffer = filteredList.OrderBy(x => x.DisplayName).ToArray(),
                m_size = filteredList.Count
            };

            return fastList;
        }
    }
}
