// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Translation;

    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    internal static class ModSettings
    {
        // General mod settings.
        internal static bool speedBoost = false;
        internal static int thumbBacks = (byte)ThumbBackCats.Skybox;

        // Growable plopping settings.
        internal static bool plopRico = true;
        internal static bool plopOther = true;
        internal static bool noZonesRico = true;
        internal static bool noZonesOther = true;
        internal static bool noSpecRico = true;
        internal static bool noSpecOther = true;

        // Ignore complaint settings.
        internal static bool noValueRicoPlop = true;
        internal static bool noValueRicoGrow = true;
        internal static bool noValueOther = false;
        internal static bool noServicesRicoPlop = true;
        internal static bool noServicesRicoGrow = true;
        internal static bool noServicesOther = false;

        // Levelling settings.
        internal static bool historicalRico = true;
        internal static bool historicalOther = false;
        internal static bool lockLevelRico = false;
        internal static bool lockLevelOther = false;

        // Ploppable demolition warnings.
        internal static bool warnBulldoze = false;
        internal static bool autoDemolish = false;

        // Disasters DLC behaviour.
        internal static bool noCollapse = true;

        // Cost overides.
        internal static bool overrideCost = false;
        internal static int costPerHousehold = 100;
        internal static int costMultResLevel = 20;
        internal static int costPerJob0 = 20;
        internal static int costPerJob1 = 25;
        internal static int costPerJob2 = 30;
        internal static int costPerJob3 = 35;

        // What's new notification version.
        internal static bool showWhatsNew = true;

        // Soft conflict notification (don't show again) flags.
        internal static int dsaPTG = 0;

        /// <summary>
        /// Thumbnail background category enum.
        /// </summary>
        public enum ThumbBackCats
        {
            /// <summary>
            /// Colored background.
            /// </summary>
            Color,

            /// <summary>
            /// Plain background.
            /// </summary>
            Plain,

            /// <summary>
            /// Skybox background.
            /// </summary>
            Skybox,

            /// <summary>
            /// Number of categories.
            /// </summary>
            NumCats,
        }

        /// <summary>
        /// Gets the array of thumbnail backround names (for dropdown menu).
        /// </summary>
        internal static string[] ThumbBackNames => new string[]
        {
            Translations.Translate("PRR_THUMB_COLOR"),
            Translations.Translate("PRR_THUMB_PLAIN"),
            Translations.Translate("PRR_THUMB_SKY"),
        };
    }
}
