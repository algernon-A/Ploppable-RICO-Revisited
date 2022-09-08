namespace PloppableRICO
{
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
        /// Adds growable options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
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
            demolishWarnCheck.isChecked = ModSettings.warnBulldoze;
            demolishWarnCheck.eventCheckChanged += DemolishWarnCheckChanged;
            demolishWarnCheck.tabIndex = ++tabbingIndex;
            currentY += CheckRowHeight + Margin;

            // Add auto-demolish checkbox.
            UICheckBox demolishAutoCheck = UICheckBoxes.AddPlainCheckBox(panel, Translations.Translate("PRR_OPTION_IMP"));
            demolishAutoCheck.relativePosition = new Vector2(LeftMargin, currentY);
            demolishAutoCheck.isChecked = ModSettings.autoDemolish;
            demolishAutoCheck.tabIndex = ++tabbingIndex;
            demolishAutoCheck.eventCheckChanged += DemolishAutoCheckChanged;
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
            overrideCostCheck.isChecked = ModSettings.overrideCost;
            overrideCostCheck.eventCheckChanged += OverrideCostCheckChanged;
            overrideCostCheck.tabIndex = ++tabbingIndex;
            currentY += CheckRowHeight + Margin;

            // Houshold costs.
            UITextField costPerHouseField = AddCostTextField(panel, "PRR_OPTION_CPH", ModSettings.costPerHousehold, ref currentY);
            UITextField costMultResLevelField = AddCostTextField(panel, "PRR_OPTION_CHM", ModSettings.costMultResLevel, ref currentY);
            costPerHouseField.eventTextSubmitted += (control, text) => TextSubmitted(control as UITextField, text, ref ModSettings.costPerHousehold);
            costMultResLevelField.eventTextSubmitted += (control, text) => TextSubmitted(control as UITextField, text, ref ModSettings.costMultResLevel);

            // Workplace costs.
            UITextField costPerJob0Field = AddCostTextField(panel, "PRR_OPTION_CJ0", ModSettings.costPerJob0, ref currentY);
            UITextField costPerJob1Field = AddCostTextField(panel, "PRR_OPTION_CJ1", ModSettings.costPerJob1, ref currentY);
            UITextField costPerJob2Field = AddCostTextField(panel, "PRR_OPTION_CJ2", ModSettings.costPerJob2, ref currentY);
            UITextField costPerJob3Field = AddCostTextField(panel, "PRR_OPTION_CJ3", ModSettings.costPerJob3, ref currentY);
            costPerJob0Field.tabIndex = ++tabbingIndex;
            costPerJob1Field.tabIndex = ++tabbingIndex;
            costPerJob2Field.tabIndex = ++tabbingIndex;
            costPerJob3Field.tabIndex = ++tabbingIndex;
            costPerJob0Field.eventTextSubmitted += (control, text) => TextSubmitted(control as UITextField, text, ref ModSettings.costPerJob0);
            costPerJob1Field.eventTextSubmitted += (control, text) => TextSubmitted(control as UITextField, text, ref ModSettings.costPerJob1);
            costPerJob2Field.eventTextSubmitted += (control, text) => TextSubmitted(control as UITextField, text, ref ModSettings.costPerJob2);
            costPerJob3Field.eventTextSubmitted += (control, text) => TextSubmitted(control as UITextField, text, ref ModSettings.costPerJob3);

            // Natural disasters.
            currentY += TitleMarginY;
            UILabel disasterLabel = UILabels.AddLabel(panel, TitleMarginX, currentY, Translations.Translate("PRR_OPTION_DIS"), textScale: 1.125f);
            disasterLabel.font = Resources.FindObjectsOfTypeAll<UIFont>().FirstOrDefault((UIFont f) => f.name == "OpenSans-Semibold");
            currentY += disasterLabel.height + TitleMarginY;

            // Add auto-demolish checkbox.
            UICheckBox noCollapseCheck = UICheckBoxes.AddPlainCheckBox(panel, Translations.Translate("PRR_OPTION_NOC"));
            noCollapseCheck.relativePosition = new Vector2(LeftMargin, currentY);
            noCollapseCheck.isChecked = ModSettings.noCollapse;
            noCollapseCheck.tabIndex = ++tabbingIndex;
            noCollapseCheck.eventCheckChanged += NoCollapseCheckChanged;
            currentY += CheckRowHeight;
        }


        /// <summary>
        /// Event handler for demolish warning checkbox.
        /// </summary>
        /// <param name="control">Calling UIComponent</param>
        /// <param name="isChecked">New isChecked state</param>
        private void DemolishWarnCheckChanged(UIComponent control, bool isChecked)
        {
            // Update mod settings.
            ModSettings.warnBulldoze = isChecked;

            // If we're in-game (dictionary has been initialized), iterate through dictionary, looking for RICO ploppable buildings and updating their auto-remove flags.
            if (Loading.xmlManager?.prefabHash != null)
            {
                foreach (BuildingInfo prefab in Loading.xmlManager.prefabHash.Keys)
                {
                    // Get active RICO settings.
                    RICOBuilding building = RICOUtils.CurrentRICOSetting(Loading.xmlManager.prefabHash[prefab]);

                    // Check that it's enabled and isn't growable.
                    if (building != null && building.ricoEnabled && !building.growable)
                    {
                        // Apply flag.
                        prefab.m_autoRemove = !isChecked;
                    }
                }
            }
        }


        /// <summary>
        /// Event handler for auto demolish checkbox.
        /// </summary>
        /// <param name="control">Calling UIComponent</param>
        /// <param name="isChecked">New isChecked state</param>
        private void DemolishAutoCheckChanged(UIComponent control, bool isChecked)
        {
            ModSettings.autoDemolish = isChecked;
        }


        /// <summary>
        /// Event handler for override cost checkbox.
        /// </summary>
        /// <param name="control">Calling UIComponent</param>
        /// <param name="isChecked">New isChecked state</param>
        private void OverrideCostCheckChanged(UIComponent control, bool isChecked)
        {
            ModSettings.overrideCost = isChecked;
        }


        /// <summary>
        /// Event handler for no collapse checkbox.
        /// </summary>
        /// <param name="control">Calling UIComponent</param>
        /// <param name="isChecked">New isChecked state</param>
        private void NoCollapseCheckChanged(UIComponent control, bool isChecked)
        {
            ModSettings.noCollapse = isChecked;
        }


        /// <summary>
        /// Procesesses text change events.
        /// </summary>
        /// <param name="textField">Textfield control</param
        /// <param name="text">Text to attempt to parse</param
        /// <param name="setting">Field to store result in</param>
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

                // If we got here, no valid value was oarsed; set text field text to the currently stored value.
                textField.text = setting.ToString();
            }
        }


        /// <summary>
        /// Adds a cost-factor textfield to the panel.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="labelKey">Text label translation key</param>
        /// <param name="initialValue">Initial value</param>
        /// <param name="yPos">Relative Y position (will be incremented for next control)</param>
        /// <returns>New textfield</returns>
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