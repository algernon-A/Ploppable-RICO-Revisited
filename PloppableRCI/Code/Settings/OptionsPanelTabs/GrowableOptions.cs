// <copyright file="GrowableOptions.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using ICities;

    /// <summary>
    /// Options panel for setting growable building behaviour options.
    /// </summary>
    internal class GrowableOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GrowableOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal GrowableOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("PRR_OPTION_GRO"), tabIndex, out UIButton _, autoLayout: true);
            UIHelper helper = new UIHelper(panel);

            // Add plop growables checkboxes.
            UIHelperBase plopGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_PLP"));
            plopGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), BuildingToolPatches.InstantRicoConstruction, isChecked =>
            {
                BuildingToolPatches.InstantRicoConstruction = isChecked;
            });
            plopGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), BuildingToolPatches.InstantOtherConstruction, isChecked =>
            {
                BuildingToolPatches.InstantOtherConstruction = isChecked;
            });

            // Add no zone checks checkboxes.
            UIHelperBase zoneGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_ZON"));
            zoneGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), BuildingPatches.NoZonesRico, isChecked =>
            {
                BuildingPatches.NoZonesRico = isChecked;
            });
            zoneGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), BuildingPatches.NoZonesOther, isChecked =>
            {
                BuildingPatches.NoZonesOther = isChecked;
            });

            // Add no specialisation checks checkboxes.
            UIHelperBase specGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SPC"));
            specGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), SpecializationPatches.NoSpecRico, isChecked =>
            {
                SpecializationPatches.NoSpecRico = isChecked;
            });
            specGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), SpecializationPatches.NoSpecOther, isChecked =>
            {
                SpecializationPatches.NoSpecOther = isChecked;
            });

            // Add 'make plopped growables historical' checkboxes.
            UIHelperBase histGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_HST"));
            histGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), BuildingToolPatches.HistoricalRico, isChecked =>
            {
                BuildingToolPatches.HistoricalRico = isChecked;
            });
            histGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), BuildingToolPatches.HistoricalOther, isChecked =>
            {
                BuildingToolPatches.HistoricalOther = isChecked;
            });

            // Add level control checkboxes.
            UIHelperBase levelGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_BLC"));

            // If we haven't already, check for Advanced Building Level Control.
            if (BuildingToolPatches.LockBuildingLevel == null)
            {
                ModUtils.ABLCReflection();
            }

            // Is it (still) null?
            if (BuildingToolPatches.LockBuildingLevel != null)
            {
                // ABLC installed; display checkboxes.
                levelGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RGR"), BuildingToolPatches.LockLevelRico, isChecked =>
                {
                    BuildingToolPatches.LockLevelRico = isChecked;
                });
                levelGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), BuildingToolPatches.LockLevelOther, isChecked =>
                {
                    BuildingToolPatches.LockLevelOther = isChecked;
                });
            }

            // Add 'disable style despawn' checkbox.
            UIHelperBase styleGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_STY"));
            styleGroup.AddCheckbox(Translations.Translate("PRR_OPTION_STR"), PrivateBuildingAIPatches.DisableStyleDespawn, isChecked =>
            {
                PrivateBuildingAIPatches.DisableStyleDespawn = isChecked;
            });
        }
    }
}