// <copyright file="UIScrollPanelItem.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using System.Linq;
    using System.Text;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using ColossalFramework.Math;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Individual building items for the building selection panel.
    /// </summary>
    public class UIScrollPanelItem : IUIFastListItem<BuildingData, UIButton>
    {
        // Information overlays.
        private UILabel _nameLabel;
        private UILabel _levelLabel;
        private UILabel _sizeLabel;

        // Currently active data.
        private BuildingData _currentData;

        /// <summary>
        /// Gets or sets the UIButton component.
        /// </summary>
        public UIButton Component { get; set; }

        /// <summary>
        /// Initialises the individual display item (as blank).
        /// </summary>
        public void Init()
        {
            Component.text = string.Empty;

            // Basic layout.
            Component.tooltipAnchor = UITooltipAnchor.Anchored;
            Component.horizontalAlignment = UIHorizontalAlignment.Left;
            Component.verticalAlignment = UIVerticalAlignment.Top;
            Component.pivot = UIPivotPoint.TopLeft;
            Component.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            Component.group = Component.parent;

            // Hide the "can't afford" big red crossout that's shown by default.
            UIComponent uIComponent = (Component.childCount <= 0) ? null : Component.components[0];
            if (uIComponent != null)
            {
                uIComponent.isVisible = false;
            }

            // Information label - building name.
            _nameLabel = Component.AddUIComponent<UILabel>();
            _nameLabel.textScale = 0.6f;
            _nameLabel.useDropShadow = true;
            _nameLabel.dropShadowColor = new Color32(80, 80, 80, 255);
            _nameLabel.dropShadowOffset = new Vector2(2, -2);
            _nameLabel.autoSize = false;
            _nameLabel.autoHeight = true;
            _nameLabel.wordWrap = true;
            _nameLabel.width = Component.width - 10;
            _nameLabel.isVisible = true;
            _nameLabel.relativePosition = new Vector3(5, 5);

            // Information label - building level.
            _levelLabel = Component.AddUIComponent<UILabel>();
            _levelLabel.textScale = 0.6f;
            _levelLabel.useDropShadow = true;
            _levelLabel.dropShadowColor = new Color32(80, 80, 80, 255);
            _levelLabel.dropShadowOffset = new Vector2(2, -2);
            _levelLabel.autoSize = true;
            _levelLabel.isVisible = true;
            _levelLabel.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Left;
            _levelLabel.relativePosition = new Vector3(5, Component.height - 10);

            // Information label - building size.
            _sizeLabel = Component.AddUIComponent<UILabel>();
            _sizeLabel.textScale = 0.6f;
            _sizeLabel.useDropShadow = true;
            _sizeLabel.dropShadowColor = new Color32(80, 80, 80, 255);
            _sizeLabel.dropShadowOffset = new Vector2(2, -2);
            _sizeLabel.autoSize = true;
            _sizeLabel.isVisible = true;
            _sizeLabel.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Left;

            // Tooltip.
            Component.eventMouseHover += (component, mouseEvent) =>
            {
                // Reset the tooltip before showing each time, as sometimes it gets clobbered either by the game or another mod.
                component.tooltip = BuildingTooltip(_currentData);
            };

            // Double-click to open building's settings.
            Component.eventDoubleClick += (component, mouseEvent) =>
            {
                SettingsPanelManager.Open(_currentData.Prefab);
            };
        }

        /// <summary>
        /// Displays a line item as required.
        /// </summary>
        /// <param name="data">RICO BuildingData record to display.</param>
        /// <param name="index">Index number of this item in the visible panel.</param>
        public void Display(BuildingData data, int index)
        {
            // Safety first!
            if (Component == null || data?.Prefab == null)
            {
                return;
            }

            try
            {
                // Set current data reference.
                _currentData = data;
                Component.name = data.Name;

                // Ensure component is unfocused.
                Component.Unfocus();

                // See if we've already got a thumbnail for this building.
                if (data.ThumbnailAtlas == null)
                {
                    // No thumbnail yet - clear the sprite and queue thumbnail for rendering.
                    ThumbnailManager.CreateThumbnail(_currentData);
                }

                // Apply icons.
                Component.atlas = _currentData.ThumbnailAtlas;
                Component.normalFgSprite = _currentData.DisplayName;
                Component.hoveredFgSprite = _currentData.DisplayName + "Hovered";
                Component.pressedFgSprite = _currentData.DisplayName + "Pressed";
                Component.focusedFgSprite = null;

                // Information label - building name.
                _nameLabel.text = data.DisplayName;

                // Information label - building level.
                _levelLabel.text = Translations.Translate("PRR_LVL") + " " + ((int)data.Prefab.m_class.m_level + 1);

                // Information label - building size.
                _sizeLabel.text = data.Prefab.GetWidth() + "x" + data.Prefab.GetLength();

                // Right anchor is unreliable, so have to set position manually.
                _sizeLabel.relativePosition = new Vector3(Component.width - _sizeLabel.width - 5, Component.height - 10);
            }
            catch (Exception e)
            {
                // Just carry on without displaying this button - don't stop the whole UI just for one failure.
                Logging.LogException(e, "exception displaying ScrollPanelItem");
            }
        }

        /// <summary>
        /// Selects an item, including setting the current tool to plop the selected building.
        /// </summary>
        /// <param name="index">Display list index number of building to select.</param>
        public void Select(int index)
        {
            // Focus the icon.
            Component.normalFgSprite = _currentData.DisplayName + "Focused";
            Component.hoveredFgSprite = _currentData.DisplayName + "Focused";

            // Apply building prefab to the tool.
            BuildingTool buildingTool = ToolsModifierControl.SetTool<BuildingTool>();
            {
                buildingTool.m_prefab = _currentData.Prefab;
                buildingTool.m_relocate = 0;
            }
        }

        /// <summary>
        /// Deselects an item.
        /// </summary>
        /// <param name="index">Display list index number of building to deselect.</param>
        public void Deselect(int index)
        {
            // Restore normal (unfocused) icons.
            Component.normalFgSprite = _currentData.DisplayName;
            Component.hoveredFgSprite = _currentData.DisplayName + "Hovered";
        }

        /// <summary>
        /// Creates a tooltip string for a building, including key stats.
        /// </summary>
        /// <param name="building">Building to generate for.</param>
        /// <returns>A tooltip string.</returns>
        private string BuildingTooltip(BuildingData building)
        {
            // Safety check.
            if (building?.Prefab == null)
            {
                return string.Empty;
            }

            StringBuilder tooltip = new StringBuilder();

            tooltip.AppendLine(building.DisplayName);

            // Construction cost.
            try
            {
                tooltip.AppendLine(LocaleFormatter.FormatCost(building.Prefab.GetConstructionCost(), false));
            }
            catch
            {
                // Don't care - just don't show construction cost in the tooltip.
            }

            // Only add households or workplaces for Private AI types, not for e.g. Beautification (dummy service).
            if (building.Prefab.GetAI() is PrivateBuildingAI thisAI)
            {
                // Household or workplace count.
                if (building.Prefab.GetService() == ItemClass.Service.Residential)
                {
                    // Residential - households.
                    tooltip.Append(Translations.Translate("PRR_HOU"));
                    tooltip.Append(": ");
                    tooltip.AppendLine(thisAI.CalculateHomeCount(building.Prefab.GetClassLevel(), default, building.Prefab.GetWidth(), building.Prefab.GetLength()).ToString());
                }
                else
                {
                    // Non-residential - workplaces.
                    int[] workplaces = new int[4];

                    tooltip.Append(Translations.Translate("PRR_WOR"));
                    tooltip.Append(": ");
                    thisAI.CalculateWorkplaceCount(building.Prefab.GetClassLevel(), default, building.Prefab.GetWidth(), building.Prefab.GetLength(), out workplaces[0], out workplaces[1], out workplaces[2], out workplaces[3]);
                    tooltip.AppendLine(workplaces.Sum().ToString());
                }
            }

            // Physical size.
            tooltip.Append("Size: ");
            tooltip.Append(building.Prefab.GetWidth());
            tooltip.Append("x");
            tooltip.AppendLine(building.Prefab.GetLength().ToString());

            return tooltip.ToString();
        }
    }
}
