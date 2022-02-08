using System.ComponentModel;
using System.Xml.Serialization;


namespace PloppableRICO
{
    /// <summary>
    /// Class to hold global mod settings.
    /// </summary>
    internal static class ModSettings
    {
        // General mod settings.
        internal static bool speedBoost = false;
        internal static int thumbBacks = (byte)ThumbBackCats.skybox;

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
        internal static string whatsNewVersion = "0.0";
        internal static int whatsNewBetaVersion = 0;
        internal static bool showWhatsNew = true;

        // Soft conflict notification (don't show again) flags.
        internal static int dsaPTG = 0;

        // Thumbnail background category enum.
        public enum ThumbBackCats
        {
            color,
            plain,
            skybox,
            numCats
        }


        /// <summary>
        /// Thumbnail backround names (for dropdown menu).
        /// </summary>
        internal static string[] ThumbBackNames => new string[]
        {
            Translations.Translate("PRR_THUMB_COLOR"),
            Translations.Translate("PRR_THUMB_PLAIN"),
            Translations.Translate("PRR_THUMB_SKY")
        };
    }


    /// <summary>
    /// Defines the XML settings file.
    /// </summary>
    [XmlRoot("SettingsFile")]
    public class XMLSettingsFile
    {
        [XmlElement("WhatsNewVersion")]
        public string WhatsNewVersion { get => ModSettings.whatsNewVersion; set => ModSettings.whatsNewVersion = value; }

        [XmlElement("WhatsNewBetaVersion")]
        [DefaultValue(0)]
        public int WhatsNewBetaVersion { get => ModSettings.whatsNewBetaVersion; set => ModSettings.whatsNewBetaVersion = value; }

        [XmlElement("ShowWhatsNew")]
        public bool ShowWhatsNew { get => ModSettings.showWhatsNew; set => ModSettings.showWhatsNew = value; }

        [XmlElement("WarnedPTG")]
        public int WarnedPTG { get => ModSettings.dsaPTG; set => ModSettings.dsaPTG = value; }

        [XmlElement("PlopRico")]
        public bool PlopRico { get => ModSettings.plopRico; set => ModSettings.plopRico = value; }

        [XmlElement("PlopOther")]
        public bool PlopOther { get => ModSettings.plopOther; set => ModSettings.plopOther = value; }

        [XmlElement("NoCollapse")]
        public bool NoCollapse { get => ModSettings.noCollapse; set => ModSettings.noCollapse = value; }

        [XmlElement("NoZonesRico")]
        public bool NoZonesRico { get => ModSettings.noZonesRico; set => ModSettings.noZonesRico = value; }

        [XmlElement("NoZonesOther")]
        public bool NoZonesOther { get => ModSettings.noZonesOther; set => ModSettings.noZonesOther = value; }

        [XmlElement("NoSpecRico")]
        public bool NoSpecRico { get => ModSettings.noSpecRico; set => ModSettings.noSpecRico = value; }

        [XmlElement("NoSpecOther")]
        public bool NoSpecOther { get => ModSettings.noSpecOther; set => ModSettings.noSpecOther = value; }

        [XmlElement("NoValueRicoPlop")]
        public bool NoValueRicoPlop { get => ModSettings.noValueRicoPlop; set => ModSettings.noValueRicoPlop = value; }

        [XmlElement("NoValueRicoGrow")]
        public bool NoValueRicoGrow { get => ModSettings.noValueRicoGrow; set => ModSettings.noValueRicoGrow = value; }

        [XmlElement("NoValueOther")]
        public bool NoValueOther { get => ModSettings.noValueOther; set => ModSettings.noValueOther = value; }

        [XmlElement("NoServicesRicoPlop")]
        public bool NoServicesRicoPlop { get => ModSettings.noServicesRicoPlop; set => ModSettings.noServicesRicoPlop = value; }

        [XmlElement("NoServicesRicoGrow")]
        public bool NoServicesRicoGrow { get => ModSettings.noServicesRicoGrow; set => ModSettings.noServicesRicoGrow = value; }

        [XmlElement("NoServicesOther")]
        public bool NoServicesOther { get => ModSettings.noServicesOther; set => ModSettings.noServicesOther = value; }

        [XmlElement("MakeRicoHistorical")]
        public bool MakeRicoHistorical { get => ModSettings.historicalRico; set => ModSettings.historicalRico = value; }

        [XmlElement("MakeOtherHistorical")]
        public bool MakeOtherHistorical { get => ModSettings.historicalOther; set => ModSettings.historicalOther = value; }

        [XmlElement("LockRicoLevel")]
        public bool LockRicoLevel { get => ModSettings.lockLevelRico; set => ModSettings.lockLevelRico = value; }

        [XmlElement("LockOtherLevel")]
        public bool LockOtherLevel { get => ModSettings.lockLevelOther; set => ModSettings.lockLevelOther = value; }

        [XmlElement("SpeedBoost")]
        public bool SpeedBoost { get => ModSettings.speedBoost; set => ModSettings.speedBoost = value; }

        [XmlElement("DebugLogging")]
        public bool DebugLogging { get => Logging.detailLogging; set => Logging.detailLogging = value; }

        //[XmlElement("ResetOnLoad")]
        //public bool ResetOnLoad { get => ModSettings.resetOnLoad; set => ModSettings.resetOnLoad = value; }

        [XmlElement("WarnBulldoze")]
        public bool WarnBulldoze { get => ModSettings.warnBulldoze; set => ModSettings.warnBulldoze = value; }

        [XmlElement("AutoDemolish")]
        public bool AutoDemolish { get => ModSettings.autoDemolish; set => ModSettings.autoDemolish = value; }

        [XmlElement("ThumbBacks")]
        public int ThumbBacks
        {
            get => ModSettings.thumbBacks;
            set
            {
                ModSettings.thumbBacks = value;

                // Bounds check.
                if ((int)ModSettings.thumbBacks > (int)ModSettings.ThumbBackCats.numCats - 1 || ModSettings.thumbBacks < 0)
                {
                    ModSettings.thumbBacks = (int)ModSettings.ThumbBackCats.skybox;
                }
            }
        }
        [XmlElement("Language")]
        public string Language
        {
            get
            {
                return Translations.CurrentLanguage;
            }
            set
            {
                Translations.CurrentLanguage = value;
            }
        }


        // Legacy 'use plain thumbnails' conversion.
        [XmlElement("PlainThumbs")]
        public bool PlainThumbs
        {
            set
            {
                if (value)
                {
                    ModSettings.thumbBacks = (int)ModSettings.ThumbBackCats.plain;
                }
            }
        }

        // Cost Overrides.
        [XmlElement("OverrideCost")]
        public bool OverrideCost { get => ModSettings.overrideCost; set => ModSettings.overrideCost = value; }
        [XmlElement("CostPerHousehold")]
        public int CostPerHousehold { get => ModSettings.costPerHousehold; set => ModSettings.costPerHousehold = value; }
        [XmlElement("CostMultResLevel")]
        public int CostMultResLevel { get => ModSettings.costMultResLevel; set => ModSettings.costMultResLevel = value; }
        [XmlElement("CostPerUneducated")]
        public int CostPerJob0 { get => ModSettings.costPerJob0; set => ModSettings.costPerJob0 = value; }
        [XmlElement("CostPerEducated")]
        public int CostPerJob1 { get => ModSettings.costPerJob1; set => ModSettings.costPerJob1 = value; }
        [XmlElement("CostPerWellEducated")]
        public int CostPerJob2 { get => ModSettings.costPerJob2; set => ModSettings.costPerJob2 = value; }
        [XmlElement("CostPerHighlyEducated")]
        public int CostPerJob3 { get => ModSettings.costPerJob3; set => ModSettings.costPerJob3 = value; }
    }
}
