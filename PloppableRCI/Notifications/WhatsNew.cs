﻿using System;
using System.Reflection;
using System.Collections.Generic;
using PloppableRICO.MessageBox;



namespace PloppableRICO
{
    /// <summary>
    /// "What's new" message box.  Based on macsergey's code in Intersection Marking Tool (Node Markup) mod.
    /// </summary>
    internal static class WhatsNew
    {
        // List of versions and associated update message lines (as translation keys).
        private static Dictionary<Version, List<string>> Versions => new Dictionary<Version, List<String>>
        {
            {
                new Version("2.3.7"),
                new List<string>
                {
                    "PRR_UPD_237_0"
                }
            },
            {
                new Version("2.3.6"),
                new List<string>
                {
                    "PRR_UPD_236_0"
                }
            },
            {
                new Version("2.3.5"),
                new List<string>
                {
                    "PRR_UPD_235_0",
                    "PRR_UPD_235_1",
                    "PRR_UPD_235_2"
                }
            },
            {
                new Version("2.3.4"),
                new List<string>
                {
                    "PRR_UPD_234_0",
                    "PRR_UPD_234_1",
                    "PRR_UPD_234_2"
                }
            },
            { 
                new Version("2.3"),
                new List<string>
                {
                    "PRR_UPD_23_2",
                    "PRR_UPD_23_3",
                    "PRR_UPD_23_4",
                    "PRR_UPD_23_5"
                }
            }
        };


        /// <summary>
        /// Close button action.
        /// </summary>
        /// <returns>True (always)</returns>
        public static bool Confirm() => true;

        /// <summary>
        /// 'Don't show again' button action.
        /// </summary>
        /// <returns>True (always)</returns>
        public static bool DontShowAgain()
        {
            // Save current version to settings file.
            ModSettings.whatsNewVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();
            ModSettings.whatsNewBeta = PloppableRICOMod.Beta;
            SettingsUtils.SaveSettings();

            return true;
        }


        /// <summary>
        /// Check if there's been an update since the last notification, and if so, show the update.
        /// </summary>
        internal static void ShowWhatsNew()
        {
            // Get last notified version and current mod version.
            Version whatsNewVersion = new Version(ModSettings.whatsNewVersion);
            Version modVersion = Assembly.GetExecutingAssembly().GetName().Version;

            // Don't show notification if we're already up to (or ahead of) this version AND there hasn't been a beta update.
            if (whatsNewVersion >= modVersion && ModSettings.whatsNewBeta.Equals(PloppableRICOMod.Beta))
            {
                return;
            }

            // Show messagebox.
            WhatsNewMessageBox messageBox = MessageBoxBase.ShowModal<WhatsNewMessageBox>();
            messageBox.Title = PloppableRICOMod.ModName + " " + PloppableRICOMod.Version;
            messageBox.DSAButton.eventClicked += (component, clickEvent) => DontShowAgain();
            messageBox.SetMessages(whatsNewVersion, Versions);
        }
    }
}