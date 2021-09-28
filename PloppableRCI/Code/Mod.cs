using ICities;
using ColossalFramework.UI;
using CitiesHarmony.API;


namespace PloppableRICO
{
    /// <summary>
    /// The base mod class for instantiation by the game.
    /// </summary>
    public class PloppableRICOMod : IUserMod
    {
        // Public mod name and description.
        public string Name => ModName + " " + Version;
        public string Description => Translations.Translate("PRR_DESCRIPTION");

        // Internal and private name and version components.
        internal static string ModName => "RICO Revisited";
        internal static string Version => BaseVersion + " " + Beta;
        internal static string Beta => "";
        internal static int BetaVersion => 0;
        private static string BaseVersion => "2.4.8";


        /// <summary>
        /// Called by the game when the mod is enabled.
        /// </summary>
        public void OnEnabled()
        {
            // Apply Harmony patches via Cities Harmony.
            // Called here instead of OnCreated to allow the auto-downloader to do its work prior to launch.
            HarmonyHelper.DoOnHarmonyReady(() => Patcher.PatchAll());

            // Load settings file.
            SettingsUtils.LoadSettings();

            // Check to see if UIView is ready.
            if (UIView.GetAView() != null)
            {
                // It's ready - attach the hook now.
                OptionsPanel.OptionsEventHook();
            }
            else
            {
                // Otherwise, queue the hook for when the intro's finished loading.
                LoadingManager.instance.m_introLoaded += OptionsPanel.OptionsEventHook;
            }
        }


        /// <summary>
        /// Called by the game when the mod is disabled.
        /// </summary>
        public void OnDisabled()
        {
            // Unapply Harmony patches via Cities Harmony.
            if (HarmonyHelper.IsHarmonyInstalled)
            {
                Patcher.UnpatchAll();
            }
        }


        /// <summary>
        /// Called by the game when the mod options panel is setup.
        /// </summary>
        public void OnSettingsUI(UIHelperBase helper)
        {
            // Setup options panel reference.
            OptionsPanel.optionsPanel = ((UIHelper)helper).self as UIScrollablePanel;
            OptionsPanel.optionsPanel.autoLayout = false;
        }
    }
}
