// <copyright file="XMLSettingsFile.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.ComponentModel;
    using System.Xml.Serialization;
    using AlgernonCommons;
    using AlgernonCommons.XML;

    /// <summary>
    /// Defines the XML settings file.
    /// </summary>
    [XmlRoot("SettingsFile")]
    public class XMLSettingsFile : SettingsXMLBase
    {
        private const string SettingsFileName = "RICORevisited.xml";

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

        [XmlElement("NoStyleDespawn")]
        public bool NoStyleDespawn { get => PrivateBuildingSimStep.disableStyleDespawn; set => PrivateBuildingSimStep.disableStyleDespawn = value; }

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
        public bool DebugLogging { get => Logging.DetailLogging; set => Logging.DetailLogging = value; }

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

        /// <summary>
        /// Load settings from XML file.
        /// </summary>
        internal static void Load() => XMLFileUtils.Load<XMLSettingsFile>(SettingsFileName);

        /// <summary>
        /// Save settings to XML file.
        /// </summary>
        internal static void Save() => XMLFileUtils.Save<XMLSettingsFile>(SettingsFileName);
    }
}
