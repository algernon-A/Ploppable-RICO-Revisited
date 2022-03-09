using ICities;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class GrowableOptions
    {
        /// <summary>
        /// Adds growable options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal GrowableOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("PRR_OPTION_GRO"), tabIndex, true);
            UIHelper helper = new UIHelper(panel);

            // Add plop growables checkboxes.
            UIHelperBase plopGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_PLP"));
            plopGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), ModSettings.plopRico, isChecked =>
            {
                ModSettings.plopRico = isChecked;
            });
            plopGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.plopOther, isChecked =>
            {
                ModSettings.plopOther = isChecked;
            });

            // Add no zone checks checkboxes.
            UIHelperBase zoneGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_ZON"));
            zoneGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), ModSettings.noZonesRico, isChecked =>
            {
                ModSettings.noZonesRico = isChecked;
            });
            zoneGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noZonesOther, isChecked =>
            {
                ModSettings.noZonesOther = isChecked;
            });

            // Add no specialisation checks checkboxes.
            UIHelperBase specGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPC"));
            specGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), ModSettings.noSpecRico, isChecked =>
            {
                ModSettings.noSpecRico = isChecked;
            });
            specGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.noSpecOther, isChecked =>
            {
                ModSettings.noSpecOther = isChecked;
            });

            // Add 'make plopped growables historical' checkboxes.
            UIHelperBase histGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_HST"));
            histGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), ModSettings.historicalRico, isChecked =>
            {
                ModSettings.historicalRico = isChecked;
            });
            histGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.historicalOther, isChecked =>
            {
                ModSettings.historicalOther = isChecked;
            });

            // Add level control checkboxes.
            UIHelperBase levelGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_BLC"));

            // If we haven't already, check for Advanced Building Level Control.
            if (ModUtils.ablcLockBuildingLevel == null)
            {
                ModUtils.ABLCReflection();
            }

            // Is it (still) null?
            if (ModUtils.ablcLockBuildingLevel != null)
            {
                // ABLC installed; display checkboxes.
                levelGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), ModSettings.lockLevelRico, isChecked =>
                {
                    ModSettings.lockLevelRico = isChecked;
                });
                levelGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), ModSettings.lockLevelOther, isChecked =>
                {
                    ModSettings.lockLevelOther = isChecked;
                });
            }

            // Add 'disable style despawn' checkbox.
            UIHelperBase styleGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_STY"));
            styleGroup.AddCheckbox(Translations.Translate("PRR_OPTION_STR"), PrivateBuildingSimStep.disableStyleDespawn, isChecked =>
            {
                PrivateBuildingSimStep.disableStyleDespawn = isChecked;
            });
        }
    }
}