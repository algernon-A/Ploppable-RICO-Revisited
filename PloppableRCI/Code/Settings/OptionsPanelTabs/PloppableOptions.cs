// <copyright file="PloppableOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class PloppableOptions
    {
        // Layout constants.
        private const float Margin = 5f;
        private const float TitleMarginX = 10f;
        private const float TitleMarginY = 15f;
        private const float LeftMargin = 24f;
        private const float GroupMargin = 40f;
        private const float CheckRowHeight = 22f;
        private const float SubTitleX = 49f;

        /// <summary>
        /// Initializes a new instance of the <see cref="PloppableOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal PloppableOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Y position indicator.
            float currentY = Margin;
            int tabbingIndex = 0;

            // Add tab and helper.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("PRR_OPTION_PLO"), tabIndex, out UIButton _);

            // Demolition options.
            UILabel demolishLabel = UILabels.AddLabel(panel, TitleMarginX, currentY, Translations.Translate("PRR_OPTION_DEM"), textScale: 1.125f);
            demolishLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Semibold");
            demolishLabel.tabIndex = ++tabbingIndex;
            currentY += demolishLabel.height + TitleMarginY;

            // Add 'warn if bulldozing ploppables' checkbox.
            UICheckBox demolishWarnCheck = UICheckBoxes.AddPlainCheckBox(panel, Translations.Translate("PRR_OPTION_BDZ"));
            demolishWarnCheck.relativePosition = new Vector2(LeftMargin, currentY);
            demolishWarnCheck.isChecked = PrefabManager.WarnBulldoze;
            demolishWarnCheck.eventCheckChanged += (c, isChecked) => PrefabManager.WarnBulldoze = isChecked;
            demolishWarnCheck.tabIndex = ++tabbingIndex;
            currentY += CheckRowHeight + Margin;

            // Add auto-demolish checkbox.
            UICheckBox demolishAutoCheck = UICheckBoxes.AddPlainCheckBox(panel, Translations.Translate("PRR_OPTION_IMP"));
            demolishAutoCheck.relativePosition = new Vector2(LeftMargin, currentY);
            demolishAutoCheck.isChecked = BuildingToolPatches.AutoDemolish;
            demolishAutoCheck.tabIndex = ++tabbingIndex;
            demolishAutoCheck.eventCheckChanged += (c, isChecked) => BuildingToolPatches.AutoDemolish = isChecked;
            currentY += CheckRowHeight;

            // Auto-demolish sub-label.
            UILabel demolishAutoLabel = UILabels.AddLabel(panel, SubTitleX, currentY, Translations.Translate("PRR_OPTION_IMP2"), textScale: 1.125f);
            demolishAutoLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Regular");
            currentY += CheckRowHeight + GroupMargin;

            // Cost options.
            UILabel costLabel = UILabels.AddLabel(panel, TitleMarginX, currentY, Translations.Translate("PRR_OPTION_CST"), textScale: 1.125f);
            costLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Semibold");
            currentY += costLabel.height + TitleMarginY;

            // Add override cost checkbox.
            UICheckBox overrideCostCheck = UICheckBoxes.AddPlainCheckBox(panel, Translations.Translate("PRR_OPTION_COV"));
            overrideCostCheck.relativePosition = new Vector2(LeftMargin, currentY);
            overrideCostCheck.isChecked = ModSettings.OverrideCost;
            overrideCostCheck.eventCheckChanged += OverrideCostCheckChanged;
            overrideCostCheck.tabIndex = ++tabbingIndex;
            currentY += CheckRowHeight + Margin;

            // Houshold costs.
            UITextField costPerHouseField = AddCostTextField(panel, "PRR_OPTION_CPH", ModSettings.CostPerHousehold, ref currentY);
            UITextField costMultResLevelField = AddCostTextField(panel, "PRR_OPTION_CHM", ModSettings.CostMultResLevel, ref currentY);
            costPerHouseField.eventTextSubmitted += (c, text) => TextSubmitted(c as UITextField, text, ref ModSettings.CostPerHousehold);
            costMultResLevelField.eventTextSubmitted += (c, text) => TextSubmitted(c as UITextField, text, ref ModSettings.CostMultResLevel);

            // Workplace costs.
            UITextField costPerJob0Field = AddCostTextField(panel, "PRR_OPTION_CJ0", ModSettings.CostPerJob0, ref currentY);
            UITextField costPerJob1Field = AddCostTextField(panel, "PRR_OPTION_CJ1", ModSettings.CostPerJob1, ref currentY);
            UITextField costPerJob2Field = AddCostTextField(panel, "PRR_OPTION_CJ2", ModSettings.CostPerJob2, ref currentY);
            UITextField costPerJob3Field = AddCostTextField(panel, "PRR_OPTION_CJ3", ModSettings.costPerJob3, ref currentY);
            costPerJob0Field.tabIndex = ++tabbingIndex;
            costPerJob1Field.tabIndex = ++tabbingIndex;
            costPerJob2Field.tabIndex = ++tabbingIndex;
            costPerJob3Field.tabIndex = ++tabbingIndex;
            costPerJob0Field.eventTextSubmitted += (c, text) => TextSubmitted(c as UITextField, text, ref ModSettings.CostPerJob0);
            costPerJob1Field.eventTextSubmitted += (c, text) => TextSubmitted(c as UITextField, text, ref ModSettings.CostPerJob1);
            costPerJob2Field.eventTextSubmitted += (c, text) => TextSubmitted(c as UITextField, text, ref ModSettings.CostPerJob2);
            costPerJob3Field.eventTextSubmitted += (c, text) => TextSubmitted(c as UITextField, text, ref ModSettings.costPerJob3);

            // Natural disasters.
            currentY += TitleMarginY;
            UILabel disasterLabel = UILabels.AddLabel(panel, TitleMarginX, currentY, Translations.Translate("PRR_OPTION_DIS"), textScale: 1.125f);
            disasterLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Semibold");
            currentY += disasterLabel.height + TitleMarginY;

            // Add auto-demolish checkbox.
            UICheckBox noCollapseCheck = UICheckBoxes.AddPlainCheckBox(panel, Translations.Translate("PRR_OPTION_NOC"));
            noCollapseCheck.relativePosition = new Vector2(LeftMargin, currentY);
            noCollapseCheck.isChecked = CommonBuildingAIPatches.NoCollapse;
            noCollapseCheck.tabIndex = ++tabbingIndex;
            noCollapseCheck.eventCheckChanged += (c, isChecked) => CommonBuildingAIPatches.NoCollapse = isChecked;
            currentY += CheckRowHeight;
        }

        /// <summary>
        /// Event handler for override cost checkbox.
        /// </summary>
        /// <param name="c">Calling UIComponent.</param>
        /// <param name="isChecked">New isChecked state.</param>
        private void OverrideCostCheckChanged(UIComponent c, bool isChecked)
        {
            ModSettings.OverrideCost = isChecked;
        }

        /// <summary>
        /// Procesesses text change events.
        /// </summary>
        /// <param name="textField">Textfield control.</param>
        /// <param name="text">Text to attempt to parse.</param>
        /// <param name="setting">Field to store result in.</param>
        private void TextSubmitted(UITextField textField, string text, ref int setting)
        {
            if (textField != null)
            {
                // Valid text to parse?
                if (!text.IsNullOrWhiteSpace())
                {
                    // Yes - attempt to parse.
                    if (uint.TryParse(text, out uint result))
                    {
                        // Sucessful parse; set value and return.
                        setting = (int)result;
                        return;
                    }
                }

                // If we got here, no valid value was parsed; set text field text to the currently stored value.
                textField.text = setting.ToString();
            }
        }

        /// <summary>
        /// Adds a cost-factor textfield to the panel.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="labelKey">Text label translation key.</param>
        /// <param name="initialValue">Initial value.</param>
        /// <param name="yPos">Relative Y position (will be incremented for next control).</param>
        /// <returns>New textfield.</returns>
        private UITextField AddCostTextField(UIComponent parent, string labelKey, int initialValue, ref float yPos)
        {
            UITextField costField = UITextFields.AddPlainTextfield(parent, Translations.Translate(labelKey));
            costField.parent.relativePosition = new Vector2(LeftMargin, yPos);
            costField.text = initialValue.ToString();
            yPos += costField.parent.height + Margin;

            return costField;
        }
    }
}