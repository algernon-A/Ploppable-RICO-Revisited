// <copyright file="RICOBuilding.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using AlgernonCommons;
    using ColossalFramework;

    /// <summary>
    /// Ploppable RICO XML building definition.
    /// This is the core mod data defintion for handling buildings.
    /// Cloneable to make it easy to make local copies.
    /// </summary>
    [XmlType("Building")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Data class")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Follow dotnet/runtime coding style")]
    public class RICOBuilding : ICloneable
    {
        /// <summary>
        /// Building service.
        /// </summary>
        [XmlAttribute("service")]
        public string m_service;

        /// <summary>
        /// Density - currently unused, but retained for possible future use.
        /// </summary>
        [XmlAttribute("density")]
        public int m_density;

        /// <summary>
        /// Building subservice (specialisation).
        /// </summary>
        [XmlAttribute("sub-service")]
        public string m_subService;

        /// <summary>
        /// Building household count.
        /// </summary>
        [XmlAttribute("homes")]
        [DefaultValue(0)]
        public int m_homeCount;

        /// <summary>
        /// Building level.
        /// </summary>
        [XmlAttribute("level")]
        public int m_level;

        /// <summary>
        /// Whether or not RICO settings are enabled for this asset.
        /// </summary>
        [XmlAttribute("enable-rico")]
        [DefaultValue(true)]
        public bool m_ricoEnabled;

        /// <summary>
        /// Whether or not this asset is growable.
        /// </summary>
        [XmlAttribute("growable")]
        [DefaultValue(false)]
        public bool m_growable;

        /// <summary>
        /// Whether or not to ignore realistic population mods calculations.
        /// </summary>
        [XmlAttribute("ignore-reality")]
        [DefaultValue(false)]
        public bool m_realityIgnored;

        /// <summary>
        /// Whether or not pollution is enabled for this building.
        /// </summary>
        [XmlAttribute("enable-pollution")]
        [DefaultValue(true)]
        public bool m_pollutionEnabled;

        // Regex expression for integer values.
        private readonly Regex _regexXmlIntegerValue = new Regex("^ *(\\d+) *$");
        private readonly Regex _regexXML4IntegerValues = new Regex("^ *(\\d+) *, *(\\d+) *, *(\\d+) *, *(\\d+) *");

        // Internal data.
        private string _name;
        private int _constructionCost;
        private string _uiCategory;
        private bool _oldWorkplacesStyle;
        private int[] _workplaces;

        /// <summary>
        /// Initializes a new instance of the <see cref="RICOBuilding"/> class.
        /// </summary>
        public RICOBuilding()
        {
            // Populate with null settings.
            _workplaces = new int[] { 0, 0, 0, 0 };
            _name = string.Empty;
            m_service = string.Empty;
            m_subService = string.Empty;
            ConstructionCost = 10;
            UiCategory = string.Empty;
            m_homeCount = 0;
            m_level = 0;
            m_density = 0;

            // Default options.
            m_ricoEnabled = true;
            m_pollutionEnabled = true;
            m_realityIgnored = false;
        }

        /// <summary>
        /// Gets or sets the building name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get => _name;

            set
            {
                // Remove testing debugging crud.
                if (_name.StartsWith("XXX"))
                {
                    _name = value.Remove(0, 3);
                }
                else
                {
                    _name = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets the building construction cost.
        /// </summary>
        [XmlAttribute("construction-cost")]
        public int ConstructionCost
        {
            get
            {
                // Enforce minimum construction cost of 10 for compatability with other mods (e.g. Real Time, Realistic Construction)
                _constructionCost = Math.Max(_constructionCost, 10);
                return _constructionCost;
            }

            set
            {
                _constructionCost = value;
            }
        }

        /// <summary>
        /// Gets or sets the building UI category (for ploppable tool panel).
        /// </summary>
        [XmlAttribute("ui-category")]
        public string UiCategory
        {
            get
            {
                return _uiCategory.IsNullOrWhiteSpace() ? RICOUtils.UICategoryOf(m_service, m_subService) : _uiCategory;
            }

            set
            {
                _uiCategory = value;
            }
        }

        /// <summary>
        /// Gets or sets the workplaces breakdown.
        /// </summary>
        [XmlAttribute("workplaces")]
        [DefaultValue("0,0,0,0")]
        public string WorkplacesString
        {
            get
            {
                // Return 'zero-string' if no workplaces.
                if (WorkplaceCount == 0)
                {
                    return "0,0,0,0";
                }

                // Otherwise, return a comma-separated list of our workplace breakdowns.
                return string.Join(",", Workplaces.Select(n => n.ToString()).ToArray());
            }

            set
            {
                // See if we have an old-format (single value) or new-format (breakdown).
                if (_regexXmlIntegerValue.IsMatch(value))
                {
                    // We have an old workplace format - return with all workplaces assigned to the lowest level (these will be allocated out later).
                    _oldWorkplacesStyle = true;
                    Workplaces = new int[] { Convert.ToInt32(value), 0, 0, 0 };
                }
                else
                {
                    // We don't have a single integer.
                    _oldWorkplacesStyle = false;

                    // See if we've got a properly formatted comma-separated list of integers.
                    if (_regexXML4IntegerValues.IsMatch(value))
                    {
                        // Yes - use this list to populate array.
                        Workplaces = value.Replace(" ", string.Empty).Split(',').Select(n => Convert.ToInt32(n)).ToArray();
                    }
                    else
                    {
                        // Garbage input - return zero workplace count.
                        Workplaces = new int[] { 0, 0, 0, 0 };
                    }
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not realistic population mod settings should be used.
        /// Considers both the building setting and whether or not such a mod is active.
        /// </summary>
        [XmlIgnore]
        public bool UseReality => !m_realityIgnored && ModUtils.RealPopEnabled;

        /// <summary>
        /// Gets the maximum building level for this service/subservice combination.
        /// </summary>
        [XmlIgnore]
        public int MaxLevel
        {
            get
            {
                return m_service == "residential" ? 5 :
                       m_service == "office" && m_subService != "high tech" ? 3 :
                       m_service == "commercial" && m_subService != "tourist" && m_subService != "leisure" ? 3 :
                       m_service == "industrial" && m_subService == "generic" ? 3 :
                       1;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the RICO data for this asset used an old-format workplace style.
        /// </summary>
        [XmlIgnore]
        public bool OldWorkplacesStyle => _oldWorkplacesStyle;

        /// <summary>
        /// Gets the total number of workplaces for this building.
        /// </summary>
        [XmlIgnore]
        public int WorkplaceCount => Workplaces.Sum();

        /// <summary>
        /// Gets or sets the workplace breakdown by education level, as an integer array.
        /// </summary>
        [XmlIgnore]
        public int[] Workplaces
        {
            get
            {
                // No workplaces for residential.
                if (m_service == "residential")
                {
                    return new int[] { 0, 0, 0, 0 };
                }

                // If we have old-style workplaces, we ned to allocate out the single value to workplace levels.
                if (OldWorkplacesStyle)
                {
                    // Get original workplace count.
                    int originalWorkplaces = _workplaces[0];

                    // Calculate distribution ratio for this service/subservice/level combination.
                    int[] distribution = RICOUtils.WorkplaceDistributionOf(m_service, m_subService, "Level" + m_level);
                    if (distribution == null)
                    {
                        // Failsafe - allocate all jobs to lowest level.
                        distribution = new int[] { 100, 100, 0, 0, 0 };
                    }

                    // Distribute jobs.
                    int[] allocation = WorkplaceAIHelper.DistributeWorkplaceLevels(originalWorkplaces, distribution);
                    for (int i = 0; i < 4; ++i)
                    {
                        _workplaces[i] = allocation[i];
                    }

                    // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                    _workplaces[0] += originalWorkplaces - _workplaces.Sum();

                    Logging.Message(originalWorkplaces, " old-format workplaces for building '", _name, "'; replacing with workplaces ", _workplaces[0], " ", _workplaces[1], " ", _workplaces[2], " ", _workplaces[3]);

                    // Reset flag; these workplaces are now updated.
                    _oldWorkplacesStyle = false;
                }

                return _workplaces;
            }

            set
            {
                _workplaces = value;
            }
        }

        /// <summary>
        /// Checks the data for any errors.
        /// </summary>
        /// <returns>A stringbuilder containing a list of errors (empty if none).</returns>
        public StringBuilder CheckFatalErrors()
        {
            StringBuilder errors = new StringBuilder();

            // Name errors.  Can't do anything with an invalid name.
            if (!new Regex(string.Format(@"[^<>:/\\\|\?\*{0}]", "\"")).IsMatch(_name) || _name == "* unnamed")
            {
                errors.AppendLine(string.Format("A building has {0} name.", _name == string.Empty || _name == "* unnamed" ? "no" : "a funny"));
            }

            // Service errors.  Can't do anything with an invalid service.
            if (!new Regex(@"^(residential|commercial|office|industrial|extractor|none|dummy)$").IsMatch(m_service))
            {
                errors.AppendLine("Building '" + _name + "' has " + (m_service == string.Empty ? "no " : "an invalid ") + "service.");
            }

            // Sub-service errors.  We can work with office or industrial, but not commercial or residential.
            if (!new Regex(@"^(high|low|generic|farming|oil|forest|ore|none|tourist|leisure|high tech|eco|high eco|low eco|wall2wall|financial)$").IsMatch(m_subService))
            {
                // Allow for invalid subservices for office and industrial buildings.
                if (!(m_service == "office" || m_service == "industrial"))
                {
                    errors.AppendLine("Building '" + _name + "' has " + (m_service == string.Empty ? "no " : "an invalid ") + "sub-service.");
                }
                else
                {
                    // If office or industrial, at least reset subservice to something decent.
                    errors.AppendLine("building '" + _name + "' has " + (m_service == string.Empty ? "no " : "an invalid ") + "sub-service.  Resetting to 'generic'");
                    m_subService = "generic";
                }
            }

            // Workplaces.  Need something to go with, here.
            if (!(_regexXML4IntegerValues.IsMatch(WorkplacesString) || _regexXmlIntegerValue.IsMatch(WorkplacesString)))
            {
                errors.AppendLine("Building '" + _name + "' has an invalid value for 'workplaces'. Must be either a positive integer number or a comma separated list of 4 positive integer numbers.");
            }

            return errors;
        }

        /// <summary>
        /// Checks the data for any non-fatal errors (errors that we can recover from, or at least work with).
        /// Should only be used after fatalErrors so that we know we've got legitimate service and sub-service values to work with.
        /// </summary>
        /// <returns>A stringbuilder containing a list of errors (empty if none).</returns>
        public StringBuilder CheckNonFatalErrors()
        {
            StringBuilder errors = new StringBuilder();

            if (!new Regex(@"^(comlow|comhigh|reslow|reshigh|office|industrial|oil|ore|farming|forest|tourist|leisure|organic|hightech|selfsufficient|none)$").IsMatch(UiCategory))
            {
                // Invalid UI Category; calculate new one from scratch.
                string newCategory = string.Empty;

                switch (m_service)
                {
                    case "residential":
                        switch (m_subService)
                        {
                            case "low":
                                newCategory = "reslow";
                                break;
                            case "high eco":
                            case "low eco":
                                newCategory = "selfsufficient";
                                break;
                            default:
                                newCategory = "reshigh";
                                break;
                        }

                        break;

                    case "industrial":
                        switch (m_subService)
                        {
                            case "farming":
                            case "forest":
                            case "oil":
                            case "ore":
                                newCategory = m_subService;
                                break;
                            default:
                                newCategory = "industrial";
                                break;
                        }

                        break;

                    case "commercial":
                        switch (m_subService)
                        {
                            case "low":
                                newCategory = "comlow";
                                break;
                            case "tourist":
                            case "leisure":
                                newCategory = m_subService;
                                break;
                            case "eco":
                                newCategory = "organic";
                                break;
                            default:
                                newCategory = "comhigh";
                                break;
                        }

                        break;

                    case "office":
                        if (m_subService == "high tech")
                        {
                            newCategory = "hightech";
                        }
                        else
                        {
                            newCategory = "generic";
                        }

                        break;
                }

                // If newCategory is still empty, we didn't work it out.
                if (string.IsNullOrEmpty(newCategory))
                {
                    newCategory = "none";
                }

                // Report the error and update the UI category.
                errors.AppendLine("Building '" + _name + "' has an invalid ui-category '" + UiCategory + "'; reverting to '" + newCategory + "'.");
                UiCategory = newCategory;
            }

            // Check home and workplace counts, as appropriate.
            if (m_service == "residential")
            {
                if (m_homeCount == 0)
                {
                    // If homeCount is zero, check to see if any workplace count has been entered instead.
                    if (_workplaces.Sum() > 0)
                    {
                        m_homeCount = _workplaces.Sum();
                        errors.AppendLine("Building '" + _name + "' is 'residential' but has zero homes; using workplaces count of " + m_homeCount + " instead.");
                    }
                    else
                    {
                        errors.AppendLine("Building '" + _name + "' is 'residential' but no homes are set.");
                    }
                }
            }
            else
            {
                // Any non-residential building should have jobs unless it's an empty or dummy service.
                if ((WorkplaceCount == 0) && !string.IsNullOrEmpty(m_service) && m_service != "none" && m_service != "dummy")
                {
                    _workplaces[0] = 1;
                    errors.AppendLine("Building '" + _name + "' provides " + m_service + " jobs but no jobs are set; setting to 1.");
                }
            }

            // Building level.  Basically check and clamp to 1 <= level <= maximum level (for this category and sub-category combination).
            int newLevel = Math.Min(Math.Max(m_level, 1), MaxLevel);

            if (newLevel != m_level)
            {
                if (newLevel == 1)
                {
                    // Don't bother reporting errors for levels reset to 1, as those are generally for buildings that only have one level anwyay and it's just annoying users.
                    Logging.Message("building '", _name, "' has invalid level '", m_level, "'. Resetting to level '", newLevel);
                }
                else
                {
                    errors.AppendLine("Building '" + _name + "' has invalid level '" + m_level + "'. Resetting to level '" + newLevel + "'.");
                }

                m_level = newLevel;
            }

            return errors;
        }

        /// <summary>
        /// Creates an identical clone of the current instance.
        /// </summary>
        /// <returns>Instance clone.</returns>
        public object Clone() => MemberwiseClone();
    }
}