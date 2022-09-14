namespace PloppableRICO
{
    using System;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Xml.Serialization;
    using AlgernonCommons;

    /// <summary>
    /// Ploppable RICO XML building definition.
    /// This is the core mod data defintion for handling buildings.
    /// Cloneable to make it easy to make local copies.
    /// </summary>
    [XmlType( "Building" )]
    public class RICOBuilding : ICloneable
    {
        /// <summary>
        /// Creates an identical clone of the current instance.
        /// </summary>
        /// <returns>Instance clone</returns>
        public object Clone()
        {
            return this.MemberwiseClone();
        }


        /// <summary>
        /// Constructor.
        /// </summary>
        public RICOBuilding()
        {
            // Populate with null settings.
            _workplaces = new int[] { 0, 0, 0, 0 };
            name = "";
            service = "";
            subService = "";
            ConstructionCost = 10;
            UiCategory = "";
            homeCount = 0;
            level = 0;
            density = 0;

            // Default options.
            ricoEnabled = true;
            pollutionEnabled = true;
            RealityIgnored = false;
        }

        // Regex expression for integer values.
        private readonly Regex RegexXmlIntegerValue = new System.Text.RegularExpressions.Regex("^ *(\\d+) *$");
        private readonly Regex RegexXML4IntegerValues = new System.Text.RegularExpressions.Regex("^ *(\\d+) *, *(\\d+) *, *(\\d+) *, *(\\d+) *");


        /// <summary>
        /// Building name.
        /// </summary>
        [XmlAttribute("name")]
        public string Name
        {
            get => name;

            set
            {
                // Remove testing debugging crud.
                if (name.StartsWith("XXX"))
                {
                    name = value.Remove(0, 3);
                }
                else
                {
                    name = value;
                }
            }
        }
        private string name;


        /// <summary>
        /// Building service.
        /// </summary>
        [XmlAttribute("service")]
        public string service;


        /// <summary>
        /// Density - currently unused, but retained for possible future use.
        /// </summary>
        [XmlAttribute("density")]
        public int density;


        /// <summary>
        /// Building subservice (specialisation).
        /// </summary>
        [XmlAttribute("sub-service")]
        public string subService;


        /// <summary>
        /// Building ploppable construction cost.
        /// </summary>
        [XmlAttribute( "construction-cost" )]
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
        private int _constructionCost;
        

        /// <summary>
        /// Building UI category (for ploppable tool panel).
        /// </summary>
        [XmlAttribute( "ui-category" )]
        public string UiCategory
        {
            get
            {
                return _UICategory != "" ? _UICategory : Util.UICategoryOf(service, subService);
            }
            set
            {
                _UICategory = value;
            }
        }
        private string _UICategory;


        /// <summary>
        /// Building household count.
        /// </summary>
        [XmlAttribute("homes")]
        [DefaultValue(0)]
        public int homeCount;


        /// <summary>
        /// Building level.
        /// </summary>
        [XmlAttribute("level")]
        public int level;


        /// <summary>
        /// Whether or not RICO settings are enabled for this asset.
        /// </summary>
        [XmlAttribute("enable-rico")]
        [DefaultValue(true)]
        public bool ricoEnabled;


        /// <summary>
        /// Whether or not this asset is growable.
        /// </summary>
        [XmlAttribute("growable")]
        [DefaultValue(false)]
        public bool growable;


        /// <summary>
        /// Whether or not pollution is enabled for this building.
        /// </summary>
        [XmlAttribute("enable-pollution")]
        [DefaultValue(true)]
        public bool pollutionEnabled;


        /// <summary>
        /// Workplaces breakdown.
        /// </summary>
        [XmlAttribute( "workplaces" )]
        [DefaultValue( "0,0,0,0" )]
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
                return String.Join( ",", Workplaces.Select(n => n.ToString()).ToArray());
            }
            set
            {
                // See if we have an old-format (single value) or new-format (breakdown).
                if (RegexXmlIntegerValue.IsMatch(value))
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
                    if (RegexXML4IntegerValues.IsMatch(value))
                    {
                        // Yes - use this list to populate array.
                        Workplaces = value.Replace(" ", "").Split(',').Select(n => Convert.ToInt32(n)).ToArray();
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
        /// Whether or not to ignore realistic population mods calculations.
        /// </summary>
        [XmlAttribute("ignore-reality")]
        [DefaultValue(false)]
        public bool RealityIgnored { get; set; }


        /// <summary>
        /// Whether or not realistic population mod settings should be used.
        /// Considers both the building setting and whether or not such a mod is active.
        /// </summary>
        [XmlIgnore]
        public bool UseReality => !RealityIgnored && ModUtils.realPopEnabled;


        /// <summary>
        /// Returns the maximum building level for this service/subservice combination.
        /// </summary>
        [XmlIgnore]
        public int MaxLevel
        {
            get
            {
                return service == "residential" ? 5 :
                       service == "office" && subService != "high tech" ? 3 :
                       service == "commercial" && subService != "tourist" && subService != "leisure" ? 3 :
                       service == "industrial" && subService == "generic" ? 3 :
                       1;
            }
        }


        /// <summary>
        /// Whether or not the RICO data for this asset used an old-format workplace style.
        /// </summary>
        [XmlIgnore]
        public bool OldWorkplacesStyle => _oldWorkplacesStyle;
        private bool _oldWorkplacesStyle;


        /// <summary>
        /// Returns the total number of workplaces for this building.
        /// </summary>
        [XmlIgnore]
        public int WorkplaceCount => Workplaces.Sum();


        /// <summary>
        /// Workplace breakdown by education level, as an integer array.
        /// </summary>
        [XmlIgnore]
        public int[] Workplaces
        {
            get
            {
                // No workplaces for residential.
                if (service == "residential")
                {
                    return new int[] { 0, 0, 0, 0 };
                }

                // If we have old-style workplaces, we ned to allocate out the single value to workplace levels.
                if (OldWorkplacesStyle)
                {
                    // Get original workplace count.
                    int originalWorkplaces = _workplaces[0];

                    // Calculate distribution ratio for this service/subservice/level combination.
                    int[] distribution = Util.WorkplaceDistributionOf(service, subService, "Level" + level);
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

                    Logging.Message(originalWorkplaces, " old-format workplaces for building '", name, "'; replacing with workplaces ", _workplaces[0], " ", _workplaces[1], " ", _workplaces[2], " ", _workplaces[3]);

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
        private int[] _workplaces;


        /// <summary>
        /// Checks the parsed XML data for any fatal errors.
        /// </summary>
        [XmlIgnore]
        public StringBuilder FatalErrors
        {
            // Detect and report any fatal errors.
            get
            {
                StringBuilder errors = new StringBuilder();

                // Name errors.  Can't do anything with an invalid name.
                if (!new Regex(String.Format(@"[^<>:/\\\|\?\*{0}]", "\"")).IsMatch(name) || name == "* unnamed")
                {
                    errors.AppendLine(String.Format("A building has {0} name.", name == "" || name == "* unnamed" ? "no" : "a funny"));
                }

                // Service errors.  Can't do anything with an invalid service.
                if (!new Regex(@"^(residential|commercial|office|industrial|extractor|none|dummy)$").IsMatch(service))
                {
                    errors.AppendLine("Building '" + name + "' has " + (service == "" ? "no " : "an invalid ") + "service.");
                }


                // Sub-service errors.  We can work with office or industrial, but not commercial or residential.
                if (!new Regex(@"^(high|low|generic|farming|oil|forest|ore|none|tourist|leisure|high tech|eco|high eco|low eco|wall2wall)$").IsMatch(subService))
                {
                    // Allow for invalid subservices for office and industrial buildings.
                    if (!(service == "office" || service == "industrial"))
                    {
                        errors.AppendLine("Building '" + name + "' has " + (service == "" ? "no " : "an invalid ") + "sub-service.");
                    }
                    else
                    {
                        // If office or industrial, at least reset subservice to something decent.
                         errors.AppendLine("building '" + name + "' has " + (service == "" ? "no " : "an invalid ") + "sub-service.  Resetting to 'generic'");
                        subService = "generic";
                    }
                }

                // Workplaces.  Need something to go with, here.
                if (!(RegexXML4IntegerValues.IsMatch(WorkplacesString) || RegexXmlIntegerValue.IsMatch(WorkplacesString)))
                {
                    errors.AppendLine("Building '" + name + "' has an invalid value for 'workplaces'. Must be either a positive integer number or a comma separated list of 4 positive integer numbers.");
                }

                return errors;
            }
        }


        /// <summary>
        /// Checks the parsed XML data for any non-fatal errors.
        /// </summary>
        [XmlIgnore]
        public StringBuilder NonFatalErrors
        {
            // Errors that we can recover from, or at least work with.
            // Should only be used after fatalErrors so that we know we've got legitimate service and sub-service values to work with.
            get
            {
                StringBuilder errors = new StringBuilder();


                if (!new Regex(@"^(comlow|comhigh|reslow|reshigh|office|industrial|oil|ore|farming|forest|tourist|leisure|organic|hightech|selfsufficient|none)$").IsMatch(UiCategory))
                {
                    // Invalid UI Category; calculate new one from scratch.
                    string newCategory = string.Empty;

                    switch(service)
                    {
                        case "residential":
                            switch (subService)
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
                            switch(subService)
                            {
                                case "farming":
                                case "forest":
                                case "oil":
                                case "ore":
                                    newCategory = subService;
                                    break;
                                default:
                                    newCategory = "industrial";
                                    break;
                            }
                            break;
                        case "commercial":
                            switch(subService)
                            {
                                case "low":
                                    newCategory = "comlow";
                                    break;
                                case "tourist":
                                case "leisure":
                                    newCategory = subService;
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
                            if (subService == "high tech")
                                newCategory = "hightech";
                            else
                                newCategory = "generic";
                            break;
                    }

                    // If newCategory is still empty, we didn't work it out.
                    if (string.IsNullOrEmpty(newCategory))
                    {
                        newCategory = "none";
                    }
                    
                    // Report the error and update the UI category.
                    errors.AppendLine("Building '" + name + "' has an invalid ui-category '" + UiCategory + "'; reverting to '" + newCategory + "'.");
                    UiCategory = newCategory;
                }

                // Check home and workplace counts, as appropriate.
                if (service == "residential")
                {
                    if (homeCount == 0)
                    {
                        // If homeCount is zero, check to see if any workplace count has been entered instead.
                        if (_workplaces.Sum() > 0)
                        {
                            homeCount = _workplaces.Sum();
                            errors.AppendLine("Building '" + name + "' is 'residential' but has zero homes; using workplaces count of " + homeCount + " instead.");
                        }
                        else
                        {
                            errors.AppendLine("Building '" + name + "' is 'residential' but no homes are set.");
                        }
                    }
                }
                else
                {
                    // Any non-residential building should have jobs unless it's an empty or dummy service.
                    if ((WorkplaceCount == 0) && service != "" && service != "none" && service != "dummy")
                    {
                        _workplaces[0] = 1;
                        errors.AppendLine("Building '" + name + "' provides " + service + " jobs but no jobs are set; setting to 1.");
                    }
                }

                // Building level.  Basically check and clamp to 1 <= level <= maximum level (for this category and sub-category combination).
                int newLevel = Math.Min(Math.Max(level, 1), MaxLevel);

                if (newLevel != level)
                {
                    if (newLevel == 1)
                    {
                        // Don't bother reporting errors for levels reset to 1, as those are generally for buildings that only have one level anwyay and it's just annoying users.
                        Logging.Message("building '", name, "' has invalid level '", level, "'. Resetting to level '", newLevel);
                    }
                    else
                    {
                        errors.AppendLine("Building '" + name + "' has invalid level '" + level + "'. Resetting to level '" + newLevel + "'.");
                    }
                    level = newLevel;
                }

                return errors;
            }
        }
    }
}