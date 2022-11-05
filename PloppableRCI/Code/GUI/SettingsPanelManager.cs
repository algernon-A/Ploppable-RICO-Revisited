// <copyright file="SettingsPanelManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Settings panel manager static class.
    /// </summary>
    public static class SettingsPanelManager
    {
        // Instance references.
        private static GameObject s_gameObject;
        private static RICOSettingsPanel s_panel;

        // Previous selection.
        private static BuildingInfo s_lastSelection;
        private static bool[] s_lastFilter;
        private static int s_lastPostion;
        private static int s_lastIndex = -1;

        /// <summary>
        /// Gets the active instance.
        /// </summary>
        public static RICOSettingsPanel Panel => s_panel;

        /// <summary>
        /// Creates the panel object in-game and displays it.
        /// </summary>
        internal static void Open(BuildingInfo selected = null)
        {
            try
            {
                // If no instance already set, create one.
                if (s_gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_gameObject = new GameObject("RICOSettingsPanel");
                    s_gameObject.transform.parent = UIView.GetAView().transform;

                    s_panel = s_gameObject.AddComponent<RICOSettingsPanel>();
                }

                // Select appropriate building if there's a preselection.
                if (selected != null)
                {
                    Logging.Message("selecting preselected building ", selected.name);
                    Panel.SelectBuilding(selected);
                }
                else if (s_lastSelection != null)
                {
                    // Restore previous filter state.
                    if (s_lastFilter != null)
                    {
                        Panel.SetFilter(s_lastFilter);
                    }

                    // Restore previous building selection list postion and selected item (specifically in that order to ensure correct item is selected).
                    Panel.SetListPosition(s_lastIndex, s_lastPostion);
                    Panel.SelectBuilding(s_lastSelection);
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
            s_lastSelection = Panel?.currentSelection?.prefab;
            s_lastFilter = Panel?.GetFilter();
            Panel?.GetListPosition(out s_lastIndex, out s_lastPostion);

            GameObject.Destroy(s_panel);
            GameObject.Destroy(s_gameObject);
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
}
