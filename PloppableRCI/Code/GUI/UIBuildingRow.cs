// <copyright file="UIBuildingRow.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// An individual row in the list of buildings.
    /// </summary>
    internal class UIBuildingRow : UIListRow
    {
        /// <summary>
        /// Row height.
        /// </summary>
        internal const float CustomRowHeight = 30f;

        // Panel components.
        private UILabel _buildingNameLabel;
        private BuildingData _buildingData;
        private UISprite _hasModSettings;
        private UISprite _hasAuthorSettings;
        private UISprite _hasLocalSettings;

        /// <summary>
        /// Gets the height for this row.
        /// </summary>
        public override float RowHeight => CustomRowHeight;

        /// <summary>
        /// Generates and displays a row.
        /// </summary>
        /// <param name="data">Object data to display.</param>
        /// <param name="rowIndex">Row index number (for background banding).</param>
        public override void Display(object data, int rowIndex)
        {
            // Perform initial setup for new rows.
            if (_buildingNameLabel == null)
            {
                _buildingNameLabel = AddLabel(10f, 270f, 1f);

                // Checkboxes to indicate which items have custom settings.
                _hasModSettings = AddSettingsCheck(280f, "PRR_SET_HASMOD");
                _hasAuthorSettings = AddSettingsCheck(310f, "PRR_SET_HASAUT");
                _hasLocalSettings = AddSettingsCheck(340f, "PRR_SET_HASLOC");
            }

            // Set selected building.
            _buildingData = data as BuildingData;
            _buildingNameLabel.text = _buildingData.DisplayName;

            // Update custom settings checkboxes to correct state.
            _hasModSettings.spriteName = _buildingData.HasMod ? "AchievementCheckedTrue" : "AchievementCheckedFalse";
            _hasAuthorSettings.spriteName = _buildingData.HasAuthor ? "AchievementCheckedTrue" : "AchievementCheckedFalse";
            _hasLocalSettings.spriteName = _buildingData.HasLocal ? "AchievementCheckedTrue" : "AchievementCheckedFalse";

            // Set initial background as deselected state.
            Deselect(rowIndex);
        }

        /// <summary>
        /// Adds a settings check to the current row.
        /// </summary>
        /// <param name="xPos">Check relative x-position.</param>
        /// <param name="translationKey">Tooltip translation key.</param>
        /// <returns>New settings check sprite.</returns>
        private UISprite AddSettingsCheck(float xPos, string translationKey)
        {
            UISprite newSprite = AddUIComponent<UISprite>();
            newSprite.size = new Vector2(20f, 20f);
            newSprite.relativePosition = new Vector2(xPos, Margin);
            newSprite.tooltip = Translations.Translate(translationKey);

            return newSprite;
        }
    }
}