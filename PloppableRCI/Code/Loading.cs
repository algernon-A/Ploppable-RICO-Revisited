// <copyright file="Loading.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using System.IO;
    using AlgernonCommons;
    using AlgernonCommons.Notifications;
    using AlgernonCommons.Patching;
    using AlgernonCommons.Translation;
    using ICities;

    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public sealed class Loading : PatcherLoadingBase<OptionsPanel, PatcherBase>
    {
        // Internal instances.
        internal static RICOPrefabManager xmlManager;
        internal static ConvertPrefabs convertPrefabs;

        // Broken prefabs list.
        internal static List<BuildingInfo> brokenPrefabs;

        // RICO definitions.
        internal static PloppableRICODefinition localRicoDef;
        internal static PloppableRICODefinition mod1RicoDef;
        internal static PloppableRICODefinition mod2RicoDef;

        private bool _softModConflct;

        /// <summary>
        /// Gets any text for a trailing confict notification paragraph (e.g. "These mods must be removed before this mod can operate").
        /// </summary>
        protected override string ConflictRemovedText => Translations.Translate("PRR_ERR_CON1");

        /// <summary>
        /// Checks for any mod conflicts.
        /// Called as part of checking prior to executing any OnCreated actions.
        /// </summary>
        /// <returns>A list of conflicting mod names (null or empty if none).</returns>
        protected override List<string> CheckModConflicts() => ConflictDetection.CheckConflictingMods();

        /// <summary>
        /// Performs any actions upon successful creation of the mod.
        /// E.g. Can be used to patch any other mods.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game or editor).</param>
        protected override void CreatedActions(ILoading loading)
        {
            // Check for other mods, including any soft conflicts.
            _softModConflct = ModUtils.CheckMods();

            // Check for Advanced Building Level Control.
            ModUtils.ABLCReflection();

            // Create instances if they don't already exist.
            if (convertPrefabs == null)
            {
                convertPrefabs = new ConvertPrefabs();
            }

            if (xmlManager == null)
            {
                xmlManager = new RICOPrefabManager
                {
                    prefabHash = new Dictionary<BuildingInfo, BuildingData>(),
                };
            }

            // Reset broken prefabs list.
            brokenPrefabs = new List<BuildingInfo>();

            // Read any local RICO settings.
            string ricoDefPath = "LocalRICOSettings.xml";
            localRicoDef = null;

            if (!File.Exists(ricoDefPath))
            {
                Logging.Message("no ", ricoDefPath, " file found");
            }
            else
            {
                localRicoDef = RICOReader.ParseRICODefinition(ricoDefPath, isLocal: true);

                if (localRicoDef == null)
                {
                    Logging.Message("no valid definitions in ", ricoDefPath);
                }
            }
        }

        /// <summary>
        /// Performs any actions upon successful level loading completion.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.).</param>
        protected override void LoadedActions(LoadMode mode)
        {
            // Report any 'soft' mod conflicts.
            if (_softModConflct)
            {
                // Soft conflict detected - display warning notification for each one.
                foreach (string mod in ModUtils.conflictingModNames)
                {
                    if (mod.Equals("PTG") && ModSettings.dsaPTG == 0)
                    {
                        // Plop the Growables.
                        DontShowAgainNotification softConflictBox = NotificationBase.ShowNotification<DontShowAgainNotification>();
                        softConflictBox.AddParas(Translations.Translate("PRR_CON_PTG0"), Translations.Translate("PRR_CON_PTG1"), Translations.Translate("PRR_CON_PTG2"));
                        softConflictBox.DSAButton.eventClicked += (component, clickEvent) => { ModSettings.dsaPTG = 1; XMLSettingsFile.Save(); };
                    }
                }
            }

            // Report any broken assets and remove from our prefab dictionary.
            foreach (BuildingInfo prefab in brokenPrefabs)
            {
                Logging.Error("broken prefab: ", prefab.name);
                xmlManager.prefabHash.Remove(prefab);
            }
            brokenPrefabs.Clear();

            // Init Ploppable Tool panel.
            PloppableTool.Initialize();

            // Add buttons to access building details from zoned building info panels.
            SettingsPanel.AddInfoPanelButtons();
        }
    }
}