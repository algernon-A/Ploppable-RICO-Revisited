// <copyright file="Mod.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Notifications;
    using AlgernonCommons.Patching;
    using AlgernonCommons.Translation;
    using ICities;

    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public sealed class Mod : PatcherMod<OptionsPanel, PatcherBase>, IUserMod
    {
        /// <summary>
        /// Gets the mod's base display name (name only).
        /// </summary>
        public override string BaseName => "RICO Revisited";

        /// <summary>
        /// Gets the mod's unique Harmony identfier.
        /// </summary>
        public override string HarmonyID => "com.github.algernon-A.csl.ploppablericorevisited";

        /// <summary>
        /// Gets the mod's description for display in the content manager.
        /// </summary>
        public string Description => Translations.Translate("PRR_DESCRIPTION");

        /// <summary>
        /// Gets the mod's what's new message array.
        /// </summary>
        public override WhatsNewMessage[] WhatsNewMessages => new WhatsNewMessageListing().Messages;

        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public override void OnEnabled()
        {
            base.OnEnabled();

            // Ensure delegate setup.
            CitizenUnitUtils.InitializeDelegates();
        }

        /// <summary>
        /// Saves settings file.
        /// </summary>
        public override void SaveSettings() => XMLSettingsFile.Save();

        /// <summary>
        /// Loads settings file.
        /// </summary>
        public override void LoadSettings() => XMLSettingsFile.Load();
    }
}
