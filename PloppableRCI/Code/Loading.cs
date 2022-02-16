using System.IO;
using System.Collections.Generic;
using ICities;
using PloppableRICO.MessageBox;


namespace PloppableRICO
{
    /// <summary>
    /// Main loading class: the mod runs from here.
    /// </summary>
    public class Loading : LoadingExtensionBase
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

        // Internal flags.
        internal static bool isModEnabled = false;
        private static bool harmonyLoaded = false;

        // Used to flag if conflicting mods are running.
        private static bool conflictingMod = false;
        internal static bool softModConflct;


        /// <summary>
        /// Called by the game when the mod is initialised at the start of the loading process.
        /// </summary>
        /// <param name="loading">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnCreated(ILoading loading)
        {
            base.OnCreated(loading);

            // Don't do anything if not in game (e.g. if we're going into an editor).
            if (loading.currentMode != AppMode.Game)
            {
                isModEnabled = false;
                Logging.KeyMessage("not loading into game, skipping activation");

                // Set harmonyLoaded flag to suppress Harmony warning when e.g. loading into editor.
                harmonyLoaded = true;

                // Unload Harmony patches and exit before doing anything further.
                Patcher.UnpatchAll();
                return;
            }

            // Ensure that Harmony patches have been applied.
            harmonyLoaded = Patcher.Patched;
            if (!harmonyLoaded)
            {
                isModEnabled = false;
                Logging.Error("Harmony patches not applied; aborting");
                return;
            }

            // Check for mod conflicts.
            if (ModUtils.IsModConflict())
            {
                // Conflict detected.
                conflictingMod = true;
                isModEnabled = false;

                // Unload Harmony patches and exit before doing anything further.
                Patcher.UnpatchAll();
                return;
            }

            // Passed all checks - okay to load (if we haven't already fo some reason).
            if (!isModEnabled)
            {
                isModEnabled = true;
                Logging.KeyMessage("v " + PloppableRICOMod.Version + " loading");

                // Check for other mods, including any soft conflicts.
                softModConflct = ModUtils.CheckMods();

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
        }


        /// <summary>
        /// Called by the game when level loading is complete.
        /// </summary>
        /// <param name="mode">Loading mode (e.g. game, editor, scenario, etc.)</param>
        public override void OnLevelLoaded(LoadMode mode)
        {
            base.OnLevelLoaded(mode);

            // Check to see that Harmony 2 was properly loaded.
            if (!harmonyLoaded)
            {
                // Harmony 2 wasn't loaded; display warning notification and exit.
                ListMessageBox harmonyBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                harmonyBox.AddParas(Translations.Translate("ERR_HAR0"), Translations.Translate("PRR_ERR_HAR"), Translations.Translate("PRR_ERR_FAT"), Translations.Translate("ERR_HAR1"));

                // List of dot points.
                harmonyBox.AddList(Translations.Translate("ERR_HAR2"), Translations.Translate("ERR_HAR3"));

                // Closing para.
                harmonyBox.AddParas(Translations.Translate("MES_PAGE"));
            }

            // Check to see if a conflicting mod has been detected.
            if (conflictingMod)
            {
                // Mod conflict detected - display warning notification and exit.
                ListMessageBox modConflictBox = MessageBoxBase.ShowModal<ListMessageBox>();

                // Key text items.
                modConflictBox.AddParas(Translations.Translate("ERR_CON0"), Translations.Translate("PRR_ERR_CON0"), Translations.Translate("PRR_ERR_FAT"), Translations.Translate("ERR_CON1"));

                // Add conflicting mod name(s).
                modConflictBox.AddList(ModUtils.conflictingModNames.ToArray());

                // Closing para.
                modConflictBox.AddParas(Translations.Translate("PRR_ERR_CON1"));
            }
        }
    }
}