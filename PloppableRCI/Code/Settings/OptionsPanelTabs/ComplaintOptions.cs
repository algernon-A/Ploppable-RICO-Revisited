namespace PloppableRICO
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using ICities;

    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class ComplaintOptions
    {
        /// <summary>
        /// Adds growable options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal ComplaintOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("PRR_OPTION_COM"), tabIndex, out UIButton _, autoLayout: true);
            UIHelper helper = new UIHelper(panel);

            // Add 'ignore low value complaint' checkboxes.
            UIHelperBase valueGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_VAL"));
            UICheckBox noValueRicoPlop = (UICheckBox)valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RPL"), ModSettings.noValueRicoPlop, isChecked =>
            {
                ModSettings.noValueRicoPlop = isChecked;
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noValueRicoGrow, isChecked =>
            {
                ModSettings.noValueRicoGrow = isChecked;

                // If this is active, then the checkbox above also needs to be checked if it isn't already.
                if (isChecked && !noValueRicoPlop.isChecked)
                {
                    noValueRicoPlop.isChecked = true;
                }
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noValueOther, isChecked =>
            {
                ModSettings.noValueOther = isChecked;
            });

            // Add 'ignore too few services complaint' checkboxes.
            UIHelperBase servicesGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SVC"));
            UICheckBox noServicesRicoPlop = (UICheckBox)servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RPL"), ModSettings.noServicesRicoPlop, isChecked =>
            {
                ModSettings.noServicesRicoPlop = isChecked;
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), ModSettings.noServicesRicoGrow, isChecked =>
            {
                ModSettings.noServicesRicoGrow = isChecked;

                // If this is active, then the checkbox above also needs to be checked if it isn't already.
                if (isChecked && !noServicesRicoPlop.isChecked)
                {
                    noServicesRicoPlop.isChecked = true;
                }
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noServicesOther, isChecked =>
            {
                ModSettings.noServicesOther = isChecked;
            });
        }
    }
}