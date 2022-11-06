// <copyright file="ComplaintOptions.cs" company="algernon (K. Algernon A. Sheppard)">
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
    internal class ComplaintOptions
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComplaintOptions"/> class.
        /// </summary>
        /// <param name="tabStrip">Tab strip to add to.</param>
        /// <param name="tabIndex">Index number of tab.</param>
        internal ComplaintOptions(UITabstrip tabStrip, int tabIndex)
        {
            // Add tab and helper.
            UIPanel panel = UITabstrips.AddTextTab(tabStrip, Translations.Translate("PRR_OPTION_COM"), tabIndex, out UIButton _, autoLayout: true);
            UIHelper helper = new UIHelper(panel);

            // Add 'ignore low value complaint' checkboxes.
            UIHelperBase valueGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_VAL"));
            UICheckBox noValueRicoPlop = (UICheckBox)valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RPL"), LandValueComplaintPatches.NoValueRicoPlop, isChecked =>
            {
                LandValueComplaintPatches.NoValueRicoPlop = isChecked;
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), LandValueComplaintPatches.NoValueRicoGrow, isChecked =>
            {
                LandValueComplaintPatches.NoValueRicoGrow = isChecked;

                // If this is active, then the checkbox above also needs to be checked if it isn't already.
                if (isChecked && !noValueRicoPlop.isChecked)
                {
                    noValueRicoPlop.isChecked = true;
                }
            });
            valueGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), LandValueComplaintPatches.NoValueOther, isChecked =>
            {
                LandValueComplaintPatches.NoValueOther = isChecked;
            });

            // Add 'ignore too few services complaint' checkboxes.
            UIHelperBase servicesGroup = helper.AddGroup(Translations.Translate("PRR_OPTION_SVC"));
            UICheckBox noServicesRicoPlop = (UICheckBox)servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RPL"), TooFewServicesComplaintPatches.NoServicesRicoPlop, isChecked =>
            {
                TooFewServicesComplaintPatches.NoServicesRicoPlop = isChecked;
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_RCO"), TooFewServicesComplaintPatches.NoServicesRicoGrow, isChecked =>
            {
                TooFewServicesComplaintPatches.NoServicesRicoGrow = isChecked;

                // If this is active, then the checkbox above also needs to be checked if it isn't already.
                if (isChecked && !noServicesRicoPlop.isChecked)
                {
                    noServicesRicoPlop.isChecked = true;
                }
            });
            servicesGroup.AddCheckbox(Translations.Translate("PRR_OPTION_OTH"), TooFewServicesComplaintPatches.NoServicesOther, isChecked =>
            {
                TooFewServicesComplaintPatches.NoServicesOther = isChecked;
            });
        }
    }
}