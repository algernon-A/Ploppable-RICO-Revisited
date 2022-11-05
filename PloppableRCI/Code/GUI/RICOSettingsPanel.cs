// <copyright file="RICOSettingsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons;
    using AlgernonCommons.UI;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

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
        private readonly UIBuildingFilter _filterBar;
        private readonly UIList _buildingSelection;
        private readonly UIPreviewPanel _previewPanel;
        private readonly UISavePanel _savePanel;
        private readonly UIBuildingOptions _buildingOptionsPanel;

        // Selected items.
        internal BuildingData currentSelection;

        /// <summary>
        /// Called to save building data.
        /// </summary>
        internal void Save() => _buildingOptionsPanel.SaveRICO();

        /// <summary>
        /// Refreshes the building selection list.
        /// </summary>
        public void RefreshList() => _buildingSelection.Refresh();

        /// <summary>
        /// Updates the UI Category of the building in the options panel.
        /// </summary>
        internal void UpdateUICategory() => _buildingOptionsPanel.UpdateUICategory();

        /// <summary>
        /// Gets the current filter state as a boolean array.
        /// </summary>
        /// <returns>Current filter toggle settings</returns>
        internal bool[] GetFilter() => _filterBar.GetFilter();

        /// <summary>
        /// Sets the filter state to match a boolean array.
        /// </summary>
        internal void SetFilter(bool[] filterState) => _filterBar.SetFilter(filterState);

        /// <summary>
        /// Gets the current index and list positions of the building selection list.
        /// </summary>
        /// <param name="selectedIndex">Index of currently selected item</param>
        /// <param name="listPosition">Current list position</param>
        internal void GetListPosition(out int selectedIndex, out int listPosition)
        {
            listPosition = _buildingSelection.CurrentPosition;
            selectedIndex = _buildingSelection.SelectedIndex;
        }

        /// <summary>
        /// Sets the current index and list positions of the building selection list.
        /// </summary>
        /// <param name="selectedIndex">Selected item index to set</param>
        /// <param name="listPosition">List position to set</param>
        internal void SetListPosition(int selectedIndex, int listPosition)
        {
            _buildingSelection.CurrentPosition = listPosition;
            _buildingSelection.SelectedIndex = selectedIndex;
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
            _filterBar.SelectBuildingCategory(building.category);
            _buildingSelection.Data = GenerateFastList();

            // Find and select the building in the fastlist.
            _buildingSelection.FindItem<BuildingData>(x => x.name.Equals(building.name));

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

                _buildingOptionsPanel.SelectionChanged(currentSelection);
                _savePanel.SelectionChanged(currentSelection);
                _previewPanel.Show(currentSelection);
            }
        }

        /// <summary>
        /// Performs initial setup for the panel.
        /// </summary>
        internal RICOSettingsPanel()
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
                    SettingsPanelManager.Close();
                };

                // Filter.
                _filterBar = AddUIComponent<UIBuildingFilter>();
                _filterBar.width = width - (Spacing * 2);
                _filterBar.height = FilterHeight;
                _filterBar.relativePosition = new Vector3(Spacing, TitleHeight);

                // Event handler to dealth with changes to filtering.
                _filterBar.EventFilteringChanged += (component, value) =>
                {
                    if (value == -1) return;

                    int listCount = _buildingSelection.Data.m_size;
                    int position = _buildingSelection.CurrentPosition;

                    _buildingSelection.Data = GenerateFastList();
                };

                // Set up panels.

                // Middle panel - building preview and edit panels.
                UIPanel middlePanel = AddUIComponent<UIPanel>();
                middlePanel.width = MiddleWidth;
                middlePanel.height = PanelHeight;
                middlePanel.relativePosition = new Vector3(LeftWidth + (Spacing * 2), TitleHeight + FilterHeight + Spacing);

                _previewPanel = middlePanel.AddUIComponent<UIPreviewPanel>();
                _previewPanel.width = middlePanel.width;
                _previewPanel.height = (PanelHeight - Spacing) / 2;
                _previewPanel.relativePosition = Vector3.zero;
                _previewPanel.Setup();

                _savePanel = middlePanel.AddUIComponent<UISavePanel>();
                _savePanel.width = middlePanel.width;
                _savePanel.height = (PanelHeight - Spacing) / 2;
                _savePanel.relativePosition = new Vector3(0, _previewPanel.height + Spacing);
                _savePanel.Setup();

                // Right panel - mod calculations.
                UIPanel rightPanel = AddUIComponent<UIPanel>();
                rightPanel.width = RightWidth;
                rightPanel.height = PanelHeight;
                rightPanel.relativePosition = new Vector3(LeftWidth + MiddleWidth + (Spacing * 3), TitleHeight + FilterHeight + Spacing);

                _buildingOptionsPanel = rightPanel.AddUIComponent<UIBuildingOptions>();
                _buildingOptionsPanel.width = RightWidth;
                _buildingOptionsPanel.height = PanelHeight;
                _buildingOptionsPanel.relativePosition = Vector3.zero;
                _buildingOptionsPanel.Setup();

                // Building selection list.
                _buildingSelection = UIList.AddUIList<UIBuildingRow>(this, Spacing, TitleHeight + FilterHeight + CheckFilterHeight + Spacing, LeftWidth, PanelHeight - CheckFilterHeight, UIBuildingRow.CustomRowHeight);
                _buildingSelection.EventSelectionChanged += (c, item) => UpdateSelectedBuilding(item as BuildingData);

                // Set up filterBar to make sure selection filters are properly initialised before calling GenerateFastList.
                _filterBar.Setup();

                // Populate the list.
                _buildingSelection.Data = GenerateFastList();
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
                if (!_filterBar.AllCatsSelected())
                {
                    Category category = item.category;
                    if (category == Category.None || !_filterBar.IsCatSelected(category))
                    {
                        continue;
                    }
                }

                // Filter by settings.
                if (_filterBar.SettingsFilter[0].isChecked && !item.hasMod) continue;
                if (_filterBar.SettingsFilter[1].isChecked && !item.hasAuthor) continue;
                if (_filterBar.SettingsFilter[2].isChecked && !item.hasLocal) continue;
                if (_filterBar.SettingsFilter[3].isChecked && !(item.hasMod || item.hasAuthor || item.hasLocal)) continue;

                // Filter by name.
                if (!_filterBar.FilterString.IsNullOrWhiteSpace() && !item.name.ToLower().Contains(_filterBar.FilterString.ToLower()))
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
