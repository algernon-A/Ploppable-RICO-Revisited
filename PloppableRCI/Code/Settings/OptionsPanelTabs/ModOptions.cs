// <copyright file="ModOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using ICities;

    /// <summary>
    /// Options panel for setting basic mod options.
    /// </summary>
    internal class ModOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal ModOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("PRR_OPTION_MOD"), tabIndex, out UIButton _, autoLayout: true);
            UIHelper helper = new UIHelper(panel);

            UIDropDown translationDropDown = (UIDropDown)helper.AddDropdown(Translations.Translate("TRN_CHOICE"), Translations.LanguageList, Translations.Index, (value) =>
            {
                Translations.Index = value;
                OptionsPanelManager<OptionsPanel>.LocaleChanged();
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
            });

            // Logging options.
            UIHelperBase logGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_LOG"));

            // Add logging checkbox.
            logGroup.AddCheckbox(Translations.Translate("PRR_OPTION_MOREDEBUG"), Logging.DetailLogging, isChecked =>
            {
                Logging.DetailLogging = isChecked;
            });

            // Thumbnail options.
            UIHelperBase thumbGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_TMB"));

            // Add thumbnail background dropdown.
            thumbGroup.AddDropdown(Translations.Translate("PRR_OPTION_THUMBACK"), ModSettings.ThumbBackNames, ModSettings.thumbBacks, (value) =>
            {
                ModSettings.thumbBacks = value;
            });

            // Add regenerate thumbnails button.
            thumbGroup.AddButton(Translations.Translate("PRR_OPTION_REGENTHUMBS"), () => PloppableTool.Instance.RegenerateThumbnails());

            // Add speed boost checkbox.
            UIHelperBase speedGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPDHDR"));
            speedGroup.AddCheckbox(Translations.Translate("PRR_OPTION_SPEED"), ModSettings.speedBoost, isChecked =>
            {
                ModSettings.speedBoost = isChecked;
            });
        }
    }
}