// <copyright file="XMLSettingsFile.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
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

        /// <summary>
        /// Gets or sets a value indicating whether the 'whats new' messages should be shown.
        /// </summary>
        [XmlElement("ShowWhatsNew")]
        public bool ShowWhatsNew { get => ModSettings.showWhatsNew; set => ModSettings.showWhatsNew = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the 'Plop the Growables' warning has been shown.
        /// </summary>
        [XmlElement("WarnedPTG")]
        public int WarnedPTG { get => ModSettings.dsaPTG; set => ModSettings.dsaPTG = value; }

        /// <summary>
        /// Gets or sets a value indicating whether plopped Ploppable RICO growables have zero construction time (instant build).
        /// </summary>
        [XmlElement("PlopRico")]
        public bool InstantRicoConstruction { get => ModSettings.plopRico; set => ModSettings.plopRico = value; }

        /// <summary>
        /// Gets or sets a value indicating whether plopped generic growables have zero construction time (instant build).
        /// </summary>
        [XmlElement("PlopOther")]
        public bool InstantOtherConstruction { get => ModSettings.plopOther; set => ModSettings.plopOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether disaster collapsing is disabled for ploppable RICO buildings.
        /// </summary>
        [XmlElement("NoCollapse")]
        public bool NoCollapse { get => ModSettings.noCollapse; set => ModSettings.noCollapse = value; }

        /// <summary>
        /// Gets or sets a value indicating whether Ploppable RICO growables can survive outside of the correct zone.
        /// </summary>
        [XmlElement("NoZonesRico")]
        public bool NoZonesRico { get => ModSettings.noZonesRico; set => ModSettings.noZonesRico = value; }

        /// <summary>
        /// Gets or sets a value indicating whether generic growables can survive outside of the correct zone.
        /// </summary>
        [XmlElement("NoZonesOther")]
        public bool NoZonesOther { get => ModSettings.noZonesOther; set => ModSettings.noZonesOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether Ploppable RICO growables can survive outside of the correct district specialization.
        /// </summary>
        [XmlElement("NoSpecRico")]
        public bool NoSpecRico { get => ModSettings.noSpecRico; set => ModSettings.noSpecRico = value; }

        /// <summary>
        /// Gets or sets a value indicating whether generic growables can survive outside of the correct district specialization.
        /// </summary>
        [XmlElement("NoSpecOther")]
        public bool NoSpecOther { get => ModSettings.noSpecOther; set => ModSettings.noSpecOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether building style forced despawning is disabled (true means disabled).
        /// </summary>
        [XmlElement("NoStyleDespawn")]
        public bool NoStyleDespawn { get => PrivateBuildingSimStep.disableStyleDespawn; set => PrivateBuildingSimStep.disableStyleDespawn = value; }

        /// <summary>
        /// Gets or sets a value indicating whether land value complaints are disabled for Ploppable RICO ploppables (true means disabled).
        /// </summary>
        [XmlElement("NoValueRicoPlop")]
        public bool NoValueComplaintRicoPlop { get => ModSettings.noValueRicoPlop; set => ModSettings.noValueRicoPlop = value; }

        /// <summary>
        /// Gets or sets a value indicating whether land value complaints are disabled for Ploppable RICO growables (true means disabled).
        /// </summary>
        [XmlElement("NoValueRicoGrow")]
        public bool NoValueComplaintRicoGrow { get => ModSettings.noValueRicoGrow; set => ModSettings.noValueRicoGrow = value; }

        /// <summary>
        /// Gets or sets a value indicating whether land value complaints are disabled for generic growables (true means disabled).
        /// </summary>
        [XmlElement("NoValueOther")]
        public bool NoValueComplaintOther { get => ModSettings.noValueOther; set => ModSettings.noValueOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether not enough services complaints are disabled for Ploppable RICO ploppables (true means disabled).
        /// </summary>
        [XmlElement("NoServicesRicoPlop")]
        public bool NoServicesComplaintRicoPlop { get => ModSettings.noServicesRicoPlop; set => ModSettings.noServicesRicoPlop = value; }

        /// <summary>
        /// Gets or sets a value indicating whether not enough services complaints are disabled for Ploppable RICO growables (true means disabled).
        /// </summary>
        [XmlElement("NoServicesRicoGrow")]
        public bool NoServicesComplaintRicoGrow { get => ModSettings.noServicesRicoGrow; set => ModSettings.noServicesRicoGrow = value; }

        /// <summary>
        /// Gets or sets a value indicating whether not enough services complaints are disabled for generic growables (true means disabled).
        /// </summary>
        [XmlElement("NoServicesOther")]
        public bool NoServicesComplaintOther { get => ModSettings.noServicesOther; set => ModSettings.noServicesOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether plopped Ploppable RICO growables should be made historical.
        /// </summary>
        [XmlElement("MakeRicoHistorical")]
        public bool MakeRicoHistorical { get => ModSettings.historicalRico; set => ModSettings.historicalRico = value; }

        /// <summary>
        /// Gets or sets a value indicating whether plopped generic growables should be made historical.
        /// </summary>
        [XmlElement("MakeOtherHistorical")]
        public bool MakeOtherHistorical { get => ModSettings.historicalOther; set => ModSettings.historicalOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether plopped Ploppable RICO growables should have their levels locked via Advanced Building Level Control.
        /// </summary>
        [XmlElement("LockRicoLevel")]
        public bool LockRicoLevel { get => ModSettings.lockLevelRico; set => ModSettings.lockLevelRico = value; }

        /// <summary>
        /// Gets or sets a value indicating whether plopped generic growables should have their levels locked via Advanced Building Level Control.
        /// </summary>
        [XmlElement("LockOtherLevel")]
        public bool LockOtherLevel { get => ModSettings.lockLevelOther; set => ModSettings.lockLevelOther = value; }

        /// <summary>
        /// Gets or sets a value indicating whether the panel is disabled when not in use.
        /// </summary>
        [XmlElement("SpeedBoost")]
        public bool SpeedBoost { get => ModSettings.speedBoost; set => ModSettings.speedBoost = value; }

        /// <summary>
        /// Gets or sets a value indicating whether detailed debugging logging is in effect.
        /// </summary>
        [XmlElement("DebugLogging")]
        public bool DebugLogging { get => Logging.DetailLogging; set => Logging.DetailLogging = value; }

        /// <summary>
        /// Gets or sets a value indicating whether bulldozing Ploppable RICO ploppable buildings displays the confirmation dialog.
        /// </summary>
        [XmlElement("WarnBulldoze")]
        public bool WarnBulldoze { get => ModSettings.warnBulldoze; set => ModSettings.warnBulldoze = value; }

        /// <summary>
        /// Gets or sets a value indicating whether bulldozing Ploppable RICO ploppable buildings can be auto-demolished e.g. by building roads over them.
        /// </summary>
        [XmlElement("AutoDemolish")]
        public bool AutoDemolish { get => ModSettings.autoDemolish; set => ModSettings.autoDemolish = value; }

        /// <summary>
        /// Gets or sets the thumbnail background setting.
        /// </summary>
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

        /// <summary>
        /// Sets the thumbnail background setting from the legacy 'use plain thumbnails' conversion.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether calculated building cost overrides are in effect.
        /// </summary>
        [XmlElement("OverrideCost")]
        public bool OverrideCost { get => ModSettings.overrideCost; set => ModSettings.overrideCost = value; }

        /// <summary>
        /// Gets or sets the calculated cost override per household.
        /// </summary>
        [XmlElement("CostPerHousehold")]
        public int CostPerHousehold { get => ModSettings.costPerHousehold; set => ModSettings.costPerHousehold = value; }

        /// <summary>
        /// Gets or sets the calculated cost override multiplier by building level.
        /// </summary>
        [XmlElement("CostMultResLevel")]
        public int CostMultResLevel { get => ModSettings.costMultResLevel; set => ModSettings.costMultResLevel = value; }

        /// <summary>
        /// Gets or sets the calculated cost override per uneducated worker job.
        /// </summary>
        [XmlElement("CostPerUneducated")]
        public int CostPerJob0 { get => ModSettings.costPerJob0; set => ModSettings.costPerJob0 = value; }

        /// <summary>
        /// Gets or sets the calculated cost override per educated worker job.
        /// </summary>
        [XmlElement("CostPerEducated")]
        public int CostPerJob1 { get => ModSettings.costPerJob1; set => ModSettings.costPerJob1 = value; }

        /// <summary>
        /// Gets or sets the calculated cost override per well-educated worker job.
        /// </summary>
        [XmlElement("CostPerWellEducated")]
        public int CostPerJob2 { get => ModSettings.costPerJob2; set => ModSettings.costPerJob2 = value; }

        /// <summary>
        /// Gets or sets the calculated cost override per highly-educated worker job.
        /// </summary>
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
