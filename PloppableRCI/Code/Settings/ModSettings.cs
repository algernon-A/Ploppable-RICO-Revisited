﻿// <copyright file="ModSettings.cs" company="algernon (K. Algernon A. Sheppard)">
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

        // Cost overides.
        internal static bool OverrideCost = false;
        internal static int CostPerHousehold = 100;
        internal static int CostMultResLevel = 20;
        internal static int CostPerJob0 = 20;
        internal static int CostPerJob1 = 25;
        internal static int CostPerJob2 = 30;
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
