using ICities;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Options panel for setting basic mod options.
    /// </summary>
    internal class ModOptions
    {
        /// <summary>
        /// Adds mod options tab to tabstrip.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to</param>
        /// <param name="tabIndex">Index number of tab</param>
        internal ModOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = PanelUtils.AddTab(tabStrip, Translations.Translate("PRR_OPTION_MOD"), tabIndex, true);
            UIHelper helper = new UIHelper(panel);


            UIDropDown translationDropDown = (UIDropDown)helper.AddDropdown(Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index, (value) =>
            {
                Translations.Index = value;
                SettingsUtils.SaveSettings();
            });
            translationDropDown.autoSize = false;
            translationDropDown.width = 270f;

            // Game options.
            /*UIHelperBase gameGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_LOA"));

            // Add reset on load checkbox.
            gameGroup.AddCheckbox(Translations.Translate("PRR_OPTION_FORCERESET"), ModSettings.resetOnLoad, isChecked =>
            {
                ModSettings.resetOnLoad = isChecked;
                SettingsUtils.SaveSettings();
            });*/

            // Notification options.
            UIHelperBase notificationGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_NOT"));
            // Add logging checkbox.
            notificationGroup.AddCheckbox(Translations.Translate("PRR_OPTION_WHATSNEW"), ModSettings.showWhatsNew, isChecked =>
            {
                ModSettings.showWhatsNew = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Logging options.
            UIHelperBase logGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_LOG"));

            // Add logging checkbox.
            logGroup.AddCheckbox(Translations.Translate("PRR_OPTION_MOREDEBUG"), Logging.detailLogging, isChecked =>
            {
                Logging.detailLogging = isChecked;
                SettingsUtils.SaveSettings();
            });

            // Thumbnail options.
            UIHelperBase thumbGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_TMB"));

            // Add thumbnail background dropdown.
            thumbGroup.AddDropdown(Translations.Translate("PRR_OPTION_THUMBACK"), ModSettings.ThumbBackNames, ModSettings.thumbBacks, (value) =>
            {
                ModSettings.thumbBacks = value;
                SettingsUtils.SaveSettings();
            });

            // Add regenerate thumbnails button.
            thumbGroup.AddButton(Translations.Translate("PRR_OPTION_REGENTHUMBS"), () => PloppableTool.Instance.RegenerateThumbnails());

            // Add speed boost checkbox.
            UIHelperBase speedGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPDHDR"));
            speedGroup.AddCheckbox(Translations.Translate("PRR_OPTION_SPEED"), ModSettings.speedBoost, isChecked =>
            {
                ModSettings.speedBoost = isChecked;
                SettingsUtils.SaveSettings();
            });
        }
    }
}