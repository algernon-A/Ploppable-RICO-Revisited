using System;
using System.Linq;
using System.Text;
using UnityEngine;
using ColossalFramework.UI;
using ColossalFramework.Math;


namespace PloppableRICO
{
    /// <summary>
    /// Scrollable building selection panel.
    /// </summary>
    public class UIScrollPanel : UIFastList<BuildingData, UIScrollPanelItem, UIButton>
    {
        // Empty - we only need the inheritence with the specified types.
    }


    /// <summary>
    /// Individual building items for the building selection panel.
    /// </summary>
    public class UIScrollPanelItem : IUIFastListItem<BuildingData, UIButton>
    {
        // Information overlays.
        private UILabel nameLabel, levelLabel, sizeLabel;

        // Currently active data.
        internal BuildingData currentData;


        /// <summary>
        /// UIButton component.
        /// Property, not field, for inheritance.
        /// </summary>
        public UIButton component { get; set; }


        /// <summary>
        /// Initialises the individual display item (as blank).
        /// </summary>
        public void Init()
        {
            component.text = string.Empty;

            // Basic layout.
            component.tooltipAnchor = UITooltipAnchor.Anchored;
            component.horizontalAlignment = UIHorizontalAlignment.Left;
            component.verticalAlignment = UIVerticalAlignment.Top;
            component.pivot = UIPivotPoint.TopLeft;
            component.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            component.group = component.parent;

            // Hide the "can't afford" big red crossout that's shown by default.
            UIComponent uIComponent = (component.childCount <= 0) ? null : component.components[0];
            if (uIComponent != null)
            {
                uIComponent.isVisible = false;
            }

            // Information label - building name.
            nameLabel = component.AddUIComponent<UILabel>();
            nameLabel.textScale = 0.6f;
            nameLabel.useDropShadow = true;
            nameLabel.dropShadowColor = new Color32(80, 80, 80, 255);
            nameLabel.dropShadowOffset = new Vector2(2, -2);
            nameLabel.autoSize = false;
            nameLabel.autoHeight = true;
            nameLabel.wordWrap = true;
            nameLabel.width = component.width - 10;
            nameLabel.isVisible = true;
            nameLabel.relativePosition = new Vector3(5, 5);

            // Information label - building level.
            levelLabel = component.AddUIComponent<UILabel>();
            levelLabel.textScale = 0.6f;
            levelLabel.useDropShadow = true;
            levelLabel.dropShadowColor = new Color32(80, 80, 80, 255);
            levelLabel.dropShadowOffset = new Vector2(2, -2);
            levelLabel.autoSize = true;
            levelLabel.isVisible = true;
            levelLabel.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Left;
            levelLabel.relativePosition = new Vector3(5, component.height - 10);

            // Information label - building size.
            sizeLabel = component.AddUIComponent<UILabel>();
            sizeLabel.textScale = 0.6f;
            sizeLabel.useDropShadow = true;
            sizeLabel.dropShadowColor = new Color32(80, 80, 80, 255);
            sizeLabel.dropShadowOffset = new Vector2(2, -2);
            sizeLabel.autoSize = true;
            sizeLabel.isVisible = true;
            sizeLabel.anchor = UIAnchorStyle.Bottom | UIAnchorStyle.Left;

            // Tooltip.
            component.eventMouseHover += (component, mouseEvent) =>
            {
                // Reset the tooltip before showing each time, as sometimes it gets clobbered either by the game or another mod.
                component.tooltip = BuildingTooltip(currentData);
            };

            // Double-click to open building's settings.
            component.eventDoubleClick += (component, mouseEvent) =>
            {
                SettingsPanel.Open(currentData.prefab);
            };
        }


        /// <summary>
        /// Displays a line item as required.
        /// </summary>
        /// <param name="data">RICO BuildingData record to display</param>
        /// <param name="index">Index number of this item in the visible panel</param>
        public void Display(BuildingData data, int index)
        {
            // Safety first!
            if (component == null || data?.prefab == null)
            {
                return;
            }

            try
            {
                // Set current data reference.
                currentData = data;
                component.name = data.name;

                // Ensure component is unfocused.
                component.Unfocus();

                // See if we've already got a thumbnail for this building.
                if (data.thumbnailAtlas == null)
                {
                    // No thumbnail yet - clear the sprite and queue thumbnail for rendering.
                    ThumbnailManager.CreateThumbnail(currentData);
                }

                // Apply icons.
                component.atlas = currentData.thumbnailAtlas;
                component.normalFgSprite = currentData.DisplayName;
                component.hoveredFgSprite = currentData.DisplayName + "Hovered";
                component.pressedFgSprite = currentData.DisplayName + "Pressed";
                component.focusedFgSprite = null;

                // Information label - building name.
                nameLabel.text = data.DisplayName;

                // Information label - building level.
                levelLabel.text = Translations.Translate("PRR_LVL") + " " + ((int)data.prefab.m_class.m_level + 1);

                // Information label - building size.
                sizeLabel.text = data.prefab.GetWidth() + "x" + data.prefab.GetLength();
                // Right anchor is unreliable, so have to set position manually.
                sizeLabel.relativePosition = new Vector3(component.width - sizeLabel.width - 5, component.height - 10);
            }
            catch (Exception e)
            {
                // Just carry on without displaying this button - don't stop the whole UI just for one failure.
                Logging.LogException(e, "exception displaying ScrollPanelItem" );
            }
        }


        /// <summary>
        /// Selects an item, including setting the current tool to plop the selected building.
        /// </summary>
        /// <param name="index">Display list index number of building to select</param>
        public void Select(int index)
        {
            // Focus the icon.
            component.normalFgSprite = currentData.DisplayName + "Focused";
            component.hoveredFgSprite = currentData.DisplayName + "Focused";

            // Apply building prefab to the tool.
            BuildingTool buildingTool = ToolsModifierControl.SetTool<BuildingTool>();
            {
                buildingTool.m_prefab = currentData.prefab;
                buildingTool.m_relocate = 0;
            }
        }


        /// <summary>
        /// Deselects an item.
        /// </summary>
        /// <param name="index">Display list index number of building to deselect</param>
        public void Deselect(int index)
        {
            // Restore normal (unfocused) icons.
            component.normalFgSprite = currentData.DisplayName;
            component.hoveredFgSprite = currentData.DisplayName + "Hovered";
        }


        /// <summary>
        /// Creates a tooltip string for a building, including key stats.
        /// </summary>
        /// <param name="building">Building to generate for</param>
        /// <returns>A tooltip string</returns>
        private string BuildingTooltip(BuildingData building)
        {
            StringBuilder tooltip = new StringBuilder();

            tooltip.AppendLine(building.DisplayName);

            // Construction cost.
            try
            {
                tooltip.AppendLine(LocaleFormatter.FormatCost(building.prefab.GetConstructionCost(), false));
            }
            catch
            {
                // Don't care - just don't show construction cost in the tooltip.
            }

            // Only add households or workplaces for Private AI types, not for e.g. Beautification (dummy service).
            if (building.prefab.GetAI() is PrivateBuildingAI thisAI)
            {
                // Household or workplace count.
                if (building.prefab.GetService() == ItemClass.Service.Residential)
                {
                    // Residential - households.
                    tooltip.Append(Translations.Translate("PRR_HOU"));
                    tooltip.Append(": ");
                    tooltip.AppendLine(thisAI.CalculateHomeCount(building.prefab.GetClassLevel(), new Randomizer(), building.prefab.GetWidth(), building.prefab.GetLength()).ToString());
                }
                else
                {
                    // Non-residential - workplaces.
                    int[] workplaces = new int[4];

                    tooltip.Append(Translations.Translate("PRR_WOR"));
                    tooltip.Append(": ");
                    thisAI.CalculateWorkplaceCount(building.prefab.GetClassLevel(), new Randomizer(), building.prefab.GetWidth(), building.prefab.GetLength(), out workplaces[0], out workplaces[1], out workplaces[2], out workplaces[3]);
                    tooltip.AppendLine(workplaces.Sum().ToString());
                }
            }

            // Physical size.
            tooltip.Append("Size: ");
            tooltip.Append(building.prefab.GetWidth());
            tooltip.Append("x");
            tooltip.AppendLine(building.prefab.GetLength().ToString());

            return tooltip.ToString();
        }
    }
}
