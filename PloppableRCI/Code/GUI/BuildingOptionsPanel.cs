// <copyright file="BuildingOptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// The far right column of the settings panel. Contains the drop downs and entry fields that allows players to assign RICO settings.
    /// </summary>
    internal class BuildingOptionsPanel : UIPanel
    {
        // Whole buncha UI strings.
        private readonly string[] _services = new string[(int)ServiceIndex.NumServes]
        {
            Translations.Translate("PRR_SRV_NON"),
            Translations.Translate("PRR_SRV_RES"),
            Translations.Translate("PRR_SRV_IND"),
            Translations.Translate("PRR_SRV_OFF"),
            Translations.Translate("PRR_SRV_COM"),
            Translations.Translate("PRR_SRV_EXT"),
            Translations.Translate("PRR_SRV_DUM"),
        };

        private readonly string[] _offSubs = new string[(int)OffSubIndex.NumSubs]
        {
            Translations.Translate("PRR_SUB_GEN"),
            Translations.Translate("PRR_SUB_W2W"),
            Translations.Translate("PRR_SUB_FIN"),
            Translations.Translate("PRR_SUB_ITC"),
        };

        private readonly string[] _resSubs = new string[(int)ResSubIndex.NumSubs]
        {
            Translations.Translate("PRR_SUB_HIG"),
            Translations.Translate("PRR_SUB_LOW"),
            Translations.Translate("PRR_SUB_W2W"),
            Translations.Translate("PRR_SUB_HEC"),
            Translations.Translate("PRR_SUB_LEC"),
        };

        private readonly string[] _comSubs = new string[(int)ComSubIndex.NumSubs]
        {
            Translations.Translate("PRR_SUB_HIG"),
            Translations.Translate("PRR_SUB_LOW"),
            Translations.Translate("PRR_SUB_W2W"),
            Translations.Translate("PRR_SUB_LEI"),
            Translations.Translate("PRR_SUB_TOU"),
            Translations.Translate("PRR_SUB_ORG"),
        };

        private readonly string[] _indSubs = new string[(int)IndSubIndex.NumSubs]
        {
            Translations.Translate("PRR_SUB_GEN"),
            Translations.Translate("PRR_SUB_FAR"),
            Translations.Translate("PRR_SUB_FOR"),
            Translations.Translate("PRR_SUB_OIL"),
            Translations.Translate("PRR_SUB_ORE"),
        };

        private readonly string[] _extSubs = new string[(int)ExtSubIndex.NumSubs]
        {
            Translations.Translate("PRR_SUB_FAR"),
            Translations.Translate("PRR_SUB_FOR"),
            Translations.Translate("PRR_SUB_OIL"),
            Translations.Translate("PRR_SUB_ORE"),
        };

        private readonly string[] _dumSubs = new string[]
        {
            Translations.Translate("PRR_SRV_NON"),
        };

        private readonly string[] _workLevels = new string[]
        {
            "1",
            "2",
            "3",
        };

        private readonly string[] _resLevels = new string[]
        {
            "1",
            "2",
            "3",
            "4",
            "5",
        };

        private readonly string[] _singleLevel = new string[]
        {
            "1",
        };

        // Panel title.
        private UIPanel _titlePanel;
        private UILabel _titleLabel;

        // Settings selection.
        private UIDropDown _settingDropDown;

        // Enable RICO.
        private UIPanel _enableRICOPanel;
        private UICheckBox _ricoEnabled;

        // Cetegory menus.
        private UIDropDown _serviceMenu;
        private UIDropDown _subServiceMenu;
        private UIDropDown _levelMenu;
        private UIDropDown _uiCategoryMenu;

        // Households / workplaces.
        private UITextField _manualPopField;
        private UITextField _uneducatedWorkerField;
        private UITextField _educatedWorkerField;
        private UITextField _wellEducatedWorkerField;
        private UITextField _highlyEducatedWorkerField;

        // Checkboxes.
        private UICheckBox _growableCheck;
        private UICheckBox _pollutionEnabledCheck;
        private UICheckBox _realityIgnoredCheck;

        // Construction cost.
        private UITextField _constructionCostField;

        // Event handling.
        private bool _disableEvents;

        // Current selections.
        private BuildingData _currentBuildingData;
        private RICOBuilding _currentSettings;

        // Setting type indexes.
        private enum SettingTypeIndex
        {
            Local = 0,
            Author,
            Mod,
        }

        // Service indexes.
        private enum ServiceIndex
        {
            None = 0,
            Residential,
            Industrial,
            Office,
            Commercial,
            Extractor,
            Dummy,
            NumServes,
        }

        // Sub-service indexes - residential.
        private enum ResSubIndex
        {
            High = 0,
            Low,
            Wall2Wall,
            HighEco,
            LowEco,
            NumSubs,
        }

        // Sub-service indexes - commercial.
        private enum ComSubIndex
        {
            High = 0,
            Low,
            Wall2Wall,
            Leisure,
            Tourist,
            Eco,
            NumSubs,
        }

        // Sub-service indexes - industrial.
        private enum IndSubIndex
        {
            Generic = 0,
            Farming,
            Forestry,
            Oil,
            Ore,
            NumSubs,
        }

        // Sub-service indexes - extractor.
        private enum ExtSubIndex
        {
            Farming = 0,
            Forestry,
            Oil,
            Ore,
            NumSubs,
        }

        // Sub-service indexes - office.
        private enum OffSubIndex
        {
            Generic = 0,
            Wall2Wall,
            Financial,
            IT,
            NumSubs,
        }

        // UI category indexes.
        private enum UICatIndex
        {
            ResLow = 0,
            ResHigh,
            ComLow,
            ComHigh,
            Office,
            Industrial,
            Farming,
            Forestry,
            Oil,
            Ore,
            Leisure,
            Tourist,
            Organic,
            HighTech,
            SelfSufficient,
            None,
        }

        /// <summary>
        /// Called by Unity when the object is created.
        /// Used to perform setup.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Basic setup.
            isVisible = true;
            canFocus = true;
            isInteractive = true;
            backgroundSprite = "UnlockingPanel";

            // Layout.
            autoLayout = true;
            autoLayoutDirection = LayoutDirection.Vertical;
            autoLayoutPadding.top = 5;
            autoLayoutPadding.right = 5;
            clipChildren = true;

            // Controls.
            builtinKeyNavigation = true;

            // Subpanels.
            _titlePanel = this.AddUIComponent<UIPanel>();
            _titlePanel.height = 20f;

            // Title panel.
            _titleLabel = _titlePanel.AddUIComponent<UILabel>();
            _titleLabel.relativePosition = new Vector3(80f, 0f);
            _titleLabel.width = 270f;
            _titleLabel.textAlignment = UIHorizontalAlignment.Center;
            _titleLabel.text = Translations.Translate("PRR_SET_HASNON");

            // Setting selection dropdown.
            _settingDropDown = RICODropDown(this, 30f, Translations.Translate("PRR_OPT_SET"));
            _settingDropDown.items = new string[] { Translations.Translate("PRR_OPT_LOC"), Translations.Translate("PRR_OPT_AUT"), Translations.Translate("PRR_OPT_MOD") };

            // Set selected index to -1 to ensure correct application of initial settings via event handler.
            _settingDropDown.selectedIndex = -1;
            _settingDropDown.eventSelectedIndexChanged += UpdateSettingSelection;

            // RICO enabled.
            _ricoEnabled = RICOCheckBar(this, Translations.Translate("PRR_OPT_ENA"));
            _enableRICOPanel = this.AddUIComponent<UIPanel>();
            _enableRICOPanel.height = 0;
            _enableRICOPanel.isVisible = false;
            _enableRICOPanel.name = "OptionsPanel";
            _ricoEnabled.Disable();

            _ricoEnabled.eventCheckChanged += (c, isEnabled) =>
            {
                // Show RICO options panel if enabled and there's a valid current selection.
                if (isEnabled)
                {
                    _enableRICOPanel.height = 240f;
                    _enableRICOPanel.isVisible = true;
                }
                else
                {
                    _enableRICOPanel.height = 0f;
                    _enableRICOPanel.isVisible = false;
                }
            };

            // Dropdown menu - service.
            _serviceMenu = RICODropDown(_enableRICOPanel, 30f, Translations.Translate("PRR_OPT_SER"));
            _serviceMenu.items = _services;
            _serviceMenu.eventSelectedIndexChanged += ServiceChanged;

            // Dropdown menu - sub-service.
            _subServiceMenu = RICODropDown(_enableRICOPanel, 60f, Translations.Translate("PRR_OPT_SUB"));
            _subServiceMenu.eventSelectedIndexChanged += SubServiceChanged;

            // Dropdown menu - UI category.
            _uiCategoryMenu = RICODropDown(_enableRICOPanel, 90f, Translations.Translate("PRR_OPT_UIC"));
            _uiCategoryMenu.items = new UICategories().Names;

            // Dropdown menu - building level.
            _levelMenu = RICODropDown(_enableRICOPanel, 120f, Translations.Translate("PRR_LEVEL"));
            _levelMenu.items = _singleLevel;

            // Update workplace allocations on level, service, and subservice change.
            _levelMenu.eventSelectedIndexChanged += (c, value) => UpdateWorkplaceBreakdowns();
            _serviceMenu.eventSelectedIndexChanged += (c, value) => UpdateWorkplaceBreakdowns();
            _subServiceMenu.eventSelectedIndexChanged += (c, value) => UpdateWorkplaceBreakdowns();

            // Base text fields.
            _constructionCostField = RICOTextField(_enableRICOPanel, 150f, Translations.Translate("PRR_OPT_CST"));
            _manualPopField = RICOTextField(_enableRICOPanel, 180f, Translations.Translate("PRR_OPT_CNT"));

            // Base checkboxes.
            _growableCheck = RICOLabelledCheckBox(_enableRICOPanel, 0f, Translations.Translate("PRR_OPT_GRO"));
            _pollutionEnabledCheck = RICOLabelledCheckBox(_enableRICOPanel, 210f, Translations.Translate("PRR_OPT_POL"));
            _realityIgnoredCheck = RICOLabelledCheckBox(_enableRICOPanel, 240f, Translations.Translate("PRR_OPT_POP"));

            // Workplace breakdown by education level.
            _uneducatedWorkerField = RICOTextField(_enableRICOPanel, 300f, Translations.Translate("PRR_OPT_JB0"));
            _educatedWorkerField = RICOTextField(_enableRICOPanel, 330f, Translations.Translate("PRR_OPT_JB1"));
            _wellEducatedWorkerField = RICOTextField(_enableRICOPanel, 360f, Translations.Translate("PRR_OPT_JB2"));
            _highlyEducatedWorkerField = RICOTextField(_enableRICOPanel, 390f, Translations.Translate("PRR_OPT_JB3"));

            // Event handlers to update employment totals on change.
            _manualPopField.eventTextChanged += (c, value) => UpdateWorkplaceBreakdowns();
            _uneducatedWorkerField.eventTextChanged += (c, value) => UpdateTotalJobs();
            _educatedWorkerField.eventTextChanged += (c, value) => UpdateTotalJobs();
            _wellEducatedWorkerField.eventTextChanged += (c, value) => UpdateTotalJobs();
            _highlyEducatedWorkerField.eventTextChanged += (c, value) => UpdateTotalJobs();

            // Event handler for realistic population checkbox to toggle state of population textfields.
            _realityIgnoredCheck.eventCheckChanged += SetTextfieldState;
        }

        /// <summary>
        /// Updates the options panel when the building selection changes, including showing/hiding relevant controls.
        /// </summary>
        /// <param name="buildingData">RICO building data.</param>
        internal void SelectionChanged(BuildingData buildingData)
        {
            // Set current data.
            _currentBuildingData = buildingData;

            int selectedIndex;

            // Set menu settings index.
            if (buildingData.HasLocal)
            {
                // Local settings have priority - select them if they exist.
                selectedIndex = (int)SettingTypeIndex.Local;
            }
            else if (buildingData.HasAuthor)
            {
                // Then author settings - select them if no local settings.
                selectedIndex = (int)SettingTypeIndex.Author;
            }
            else if (buildingData.HasMod)
            {
                // Finally, set mod settings if no other settings.
                selectedIndex = (int)SettingTypeIndex.Mod;
            }
            else
            {
                // No settings are available for this builidng.
                selectedIndex = -1;
            }

            // Is the new index a change from the current state?
            if (_settingDropDown.selectedIndex == selectedIndex)
            {
                // No - leave settings menu along and manually force a setting selection update.
                UpdateSettingSelection(null, selectedIndex);
            }
            else
            {
                // Yes - update settings menu selection, which will trigger update via event handler.
                _settingDropDown.selectedIndex = selectedIndex;
            }

            // Show or hide settings menu as approprite (hide if no valid settings for this building).
            _settingDropDown.parent.isVisible = selectedIndex >= (int)SettingTypeIndex.Local;
        }

        /// <summary>
        /// Reads current settings from UI elements, and saves them to XML.
        /// </summary>
        internal void SaveRICO()
        {
            // Don't do anything if no current local selection.
            if (!_currentBuildingData.HasLocal || _currentSettings == null)
            {
                return;
            }

            // Set service and subservice.
            GetService(out string serviceString, out string subServiceString);
            _currentSettings.m_service = serviceString;
            _currentSettings.m_subService = subServiceString;

            // Set level.
            _currentSettings.m_level = _levelMenu.selectedIndex + 1;

            // Get home/total worker count, with default of zero.
            int.TryParse(_manualPopField.text, out int manualCount);
            _currentSettings.m_homeCount = manualCount;

            // Get workplace breakdown.
            int[] a = new int[4] { 0, 0, 0, 0 };
            int.TryParse(_uneducatedWorkerField.text, out a[0]);
            int.TryParse(_educatedWorkerField.text, out a[1]);
            int.TryParse(_wellEducatedWorkerField.text, out a[2]);
            int.TryParse(_highlyEducatedWorkerField.text, out a[3]);

            // If no breakdown has been provided, then we try the total jobs instead.
            // Yeah, it's a bit clunky to add the elements individually like this, but saves bringing in System.Linq for just this one case.
            if (a[0] + a[1] + a[2] + a[3] == 0)
            {
                // No workplace breakdown provided (all fields zero); use total workplaces ('manual', previously parsed as manualCount) and allocate.
                int[] d = RICOUtils.WorkplaceDistributionOf(_currentSettings.m_service, _currentSettings.m_subService, "Level" + _currentSettings.m_level);
                a = WorkplaceAIHelper.DistributeWorkplaceLevels(manualCount, d);

                // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                a[0] += manualCount - a[0] - a[1] - a[2] - a[3];
            }

            _currentSettings.Workplaces = a;
            _currentSettings.ConstructionCost = int.Parse(_constructionCostField.text);

            // Write parsed (and filtered, e.g. minimum value 10) back to the construction cost text field so the user knows.
            _constructionCostField.text = _currentSettings.ConstructionCost.ToString();

            // UI categories from menu.
            switch (_uiCategoryMenu.selectedIndex)
            {
                case (int)UICatIndex.ResLow:
                    _currentSettings.UiCategory = "reslow";
                    break;
                case (int)UICatIndex.ResHigh:
                    _currentSettings.UiCategory = "reshigh";
                    break;
                case (int)UICatIndex.ComLow:
                    _currentSettings.UiCategory = "comlow";
                    break;
                case (int)UICatIndex.ComHigh:
                    _currentSettings.UiCategory = "comhigh";
                    break;
                case (int)UICatIndex.Office:
                    _currentSettings.UiCategory = "office";
                    break;
                case (int)UICatIndex.Industrial:
                    _currentSettings.UiCategory = "industrial";
                    break;
                case (int)UICatIndex.Farming:
                    _currentSettings.UiCategory = "farming";
                    break;
                case (int)UICatIndex.Forestry:
                    _currentSettings.UiCategory = "forest";
                    break;
                case (int)UICatIndex.Oil:
                    _currentSettings.UiCategory = "oil";
                    break;
                case (int)UICatIndex.Ore:
                    _currentSettings.UiCategory = "ore";
                    break;
                case (int)UICatIndex.Leisure:
                    _currentSettings.UiCategory = "leisure";
                    break;
                case (int)UICatIndex.Tourist:
                    _currentSettings.UiCategory = "tourist";
                    break;
                case (int)UICatIndex.Organic:
                    _currentSettings.UiCategory = "organic";
                    break;
                case (int)UICatIndex.HighTech:
                    _currentSettings.UiCategory = "hightech";
                    break;
                case (int)UICatIndex.SelfSufficient:
                    _currentSettings.UiCategory = "selfsufficient";
                    break;
                default:
                    _currentSettings.UiCategory = "none";
                    break;
            }

            // Remaining items.
            _currentSettings.m_ricoEnabled = _ricoEnabled.isChecked;
            _currentSettings.m_growable = _growableCheck.isChecked;
            _currentSettings.m_realityIgnored = !_realityIgnoredCheck.isChecked;
            _currentSettings.m_pollutionEnabled = _pollutionEnabledCheck.isChecked;
        }

        /// <summary>
        /// Automatically updates UI category selection based on selected service and subservice.
        /// </summary>
        internal void UpdateUICategory()
        {
            switch (_serviceMenu.selectedIndex)
            {
                case (int)ServiceIndex.Residential:
                    // Residential.
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)ResSubIndex.Low:
                            // Low residential.
                            _uiCategoryMenu.selectedIndex = (int)UICatIndex.ResLow;
                            break;
                        case (int)ResSubIndex.LowEco:
                        case (int)ResSubIndex.HighEco:
                            // High and low eco.
                            _uiCategoryMenu.selectedIndex = (int)UICatIndex.SelfSufficient;
                            break;
                        default:
                            // High residential.
                            _uiCategoryMenu.selectedIndex = (int)UICatIndex.ResHigh;
                            break;
                    }

                    break;

                case (int)ServiceIndex.Industrial:
                    _uiCategoryMenu.selectedIndex = _subServiceMenu.selectedIndex + 5;
                    break;

                case (int)ServiceIndex.Office:
                    _uiCategoryMenu.selectedIndex = (int)(_subServiceMenu.selectedIndex == (int)OffSubIndex.IT ? UICatIndex.HighTech : UICatIndex.Office);
                    break;

                case (int)ServiceIndex.Commercial:
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)ComSubIndex.High:
                        case (int)ComSubIndex.Wall2Wall:
                            // High commercial.
                            _uiCategoryMenu.selectedIndex = (int)UICatIndex.ComHigh;
                            break;

                        case (int)ComSubIndex.Low:
                            // Low commercial.
                            _uiCategoryMenu.selectedIndex = (int)UICatIndex.ComLow;
                            break;

                        default:
                            // Tourist, leisure or eco.
                            _uiCategoryMenu.selectedIndex = _subServiceMenu.selectedIndex + 8;
                            break;
                    }

                    break;

                case (int)ServiceIndex.Extractor:
                    _uiCategoryMenu.selectedIndex = _subServiceMenu.selectedIndex + 6;
                    break;

                default:
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.None;
                    break;
            }
        }

        /// <summary>
        /// Event handler - handles changes to settings selection.
        /// </summary>
        /// <param name="c">Calling UI component.</param>
        /// <param name="index">Settings menu index.</param>
        private void UpdateSettingSelection(UIComponent c, int index)
        {
            // Disable the event logic while dropdowns are being updated.
            _disableEvents = true;

            // Combination 'no settings' message text and status flag (left as null if valid settings are selected).
            string noSettingMessage = null;

            // Disable all input controls by default; activate them later if needed.
            _ricoEnabled.Disable();
            _growableCheck.Disable();
            _growableCheck.parent.Hide();
            _serviceMenu.Disable();
            _subServiceMenu.Disable();
            _levelMenu.Disable();
            _uiCategoryMenu.Disable();
            _constructionCostField.Disable();
            _manualPopField.Disable();
            _realityIgnoredCheck.Disable();
            _uneducatedWorkerField.Disable();
            _educatedWorkerField.Disable();
            _wellEducatedWorkerField.Disable();
            _highlyEducatedWorkerField.Disable();

            // Update UI components based on current setting selection.
            switch (index)
            {
                // Local settings.
                case (int)SettingTypeIndex.Local:
                    // Does the current building have local settings?
                    if (_currentBuildingData.HasLocal)
                    {
                        // Yes - update display.
                        _currentSettings = _currentBuildingData.Local;
                        _titleLabel.text = Translations.Translate("PRR_SET_HASLOC");

                        // (Re)enable input fields.
                        _ricoEnabled.Enable();
                        _ricoEnabled.parent.Show();
                        _serviceMenu.Enable();
                        _subServiceMenu.Enable();
                        _levelMenu.Enable();
                        _uiCategoryMenu.Enable();
                        _constructionCostField.Enable();
                        _manualPopField.Enable();
                        _realityIgnoredCheck.Enable();
                        _uneducatedWorkerField.Enable();
                        _educatedWorkerField.Enable();
                        _wellEducatedWorkerField.Enable();
                        _highlyEducatedWorkerField.Enable();

                        // 'Growable' can only be set in local settings.
                        // Only show growable checkbox where assets meet the prequisites:
                        // Growables can't have any dimension greater than 4 or contain any net structures.
                        if (_currentBuildingData.Prefab.GetWidth() <= 4 && _currentBuildingData.Prefab.GetLength() <= 4 && !(_currentBuildingData.Prefab.m_paths != null && _currentBuildingData.Prefab.m_paths.Length != 0))
                        {
                            _growableCheck.Enable();
                            _growableCheck.parent.Show();
                        }
                    }
                    else
                    {
                        // No local settings for this building.
                        noSettingMessage = Translations.Translate("PRR_SET_NOLOC");
                    }

                    break;

                // Author settings.
                case (int)SettingTypeIndex.Author:
                    // Does the current building have author settings?
                    if (_currentBuildingData.HasAuthor)
                    {
                        // Yes - leave input fields disabled and update display.
                        _currentSettings = _currentBuildingData.Author;
                        _titleLabel.text = Translations.Translate("PRR_SET_HASAUT");
                    }
                    else
                    {
                        // No author settings for this building.
                        noSettingMessage = Translations.Translate("PRR_SET_NOAUT");
                    }

                    break;

                // Mod settings.
                case (int)SettingTypeIndex.Mod:
                    // Does the current building have mod settings?
                    if (_currentBuildingData.HasMod)
                    {
                        // Yes - leave input fields disabled and update display.
                        _currentSettings = _currentBuildingData.Mod;
                        _titleLabel.text = Translations.Translate("PRR_SET_HASMOD");
                    }
                    else
                    {
                        // No mod settings for this building.
                        noSettingMessage = Translations.Translate("PRR_SET_NOMOD");
                    }

                    break;

                default:
                    noSettingMessage = Translations.Translate("PRR_SET_HASNON");
                    _currentSettings = null;
                    break;
            }

            // Update settings.
            if (_currentSettings != null)
            {
                // Show 'enable rico' check.
                _ricoEnabled.parent.Show();

                UpdateElementVisibility(_currentSettings.m_service);
                SettingChanged(_currentSettings);
            }

            // See if we've got no settings to display.
            if (!string.IsNullOrEmpty(noSettingMessage))
            {
                // No settings - hide panel (by unchecking 'enable rico' check) and then hide 'enable rico' check, too.
                _ricoEnabled.isChecked = false;
                _ricoEnabled.Disable();
                _ricoEnabled.parent.Hide();

                // Display appropriate message.
                _titleLabel.text = noSettingMessage;
            }

            // Restore event logic.
            _disableEvents = false;
        }

        /// <summary>
        /// Event handler - updates the options panel when the service dropdown is changed.
        /// </summary>
        /// <param name="c">Calling component.</param>
        /// <param name="value">New service dropdown selected index.</param>
        private void ServiceChanged(UIComponent c, int value)
        {
            // Ignore event if disabled flag is set.
            if (!_disableEvents)
            {
                // Translate index to relevant UpdateElements parameter.
                switch (value)
                {
                    case (int)ServiceIndex.None:
                        UpdateElementVisibility("none");
                        break;
                    case (int)ServiceIndex.Residential:
                        UpdateElementVisibility("residential");
                        break;
                    case (int)ServiceIndex.Industrial:
                        UpdateElementVisibility("industrial");
                        break;
                    case (int)ServiceIndex.Office:
                        UpdateElementVisibility("office");
                        break;
                    case (int)ServiceIndex.Commercial:
                        UpdateElementVisibility("commercial");
                        break;
                    case (int)ServiceIndex.Extractor:
                        UpdateElementVisibility("extractor");
                        break;
                    case (int)ServiceIndex.Dummy:
                        UpdateElementVisibility("dummy");
                        break;
                }

                // Update sub-service and level menus.
                UpdateSubServiceMenu();
                UpdateLevelMenu();
            }
        }

        /// <summary>
        /// Event handler - updates the options panel when the sub-service dropdown is changed.
        /// </summary>
        /// <param name="c">Calling component.</param>
        /// <param name="value">New service dropdown selected index (ignored).</param>
        private void SubServiceChanged(UIComponent c, int value)
        {
            // Ignore event if disabled flag is set.
            if (!_disableEvents)
            {
                UpdateUICategory();
                UpdateLevelMenu();
            }
        }

        /// <summary>
        /// Updates the total workplaces textfield with the sum of the workplace breakdown boxes.
        /// Does nothing if any workplace textfield cannot be parsed directly to int.
        /// </summary>
        private void UpdateTotalJobs()
        {
            // Ignore event if disabled flag is set.
            if (!_disableEvents)
            {
                // Disable events while we update, to avoid an infinite loop.
                _disableEvents = true;

                // Try to add up all workplace breakdown fields and update the total.  If an exception is thrown (most likely parsing error) then just do nothing.
                try
                {
                    _manualPopField.text = (int.Parse(_uneducatedWorkerField.text) + int.Parse(_educatedWorkerField.text) + int.Parse(_wellEducatedWorkerField.text) + int.Parse(_highlyEducatedWorkerField.text)).ToString();
                }
                catch
                {
                    // Don't care.
                }

                // Resume event handling.
                _disableEvents = false;
            }
        }

        /// <summary>
        /// Updates the values in the RICO options panel to match the selected building (control visibility should already be set).
        /// </summary>
        /// <param name="building">RICO building record.</param>
        private void SettingChanged(RICOBuilding building)
        {
            // Workplaces.
            _manualPopField.text = building.WorkplaceCount.ToString();
            _uneducatedWorkerField.text = building.Workplaces[0].ToString();
            _educatedWorkerField.text = building.Workplaces[1].ToString();
            _wellEducatedWorkerField.text = building.Workplaces[2].ToString();
            _highlyEducatedWorkerField.text = building.Workplaces[3].ToString();

            // Service and sub-service.
            switch (building.m_service)
            {
                case "residential":
                    _serviceMenu.selectedIndex = (int)ServiceIndex.Residential;

                    // Display homecount.
                    _manualPopField.text = building.m_homeCount.ToString();

                    // Update sub-service menu.
                    UpdateSubServiceMenu();

                    // Sub-service.
                    switch (_currentSettings.m_subService)
                    {
                        case "low":
                            _subServiceMenu.selectedIndex = (int)ResSubIndex.Low;
                            break;
                        case "wall2wall":
                            _subServiceMenu.selectedIndex = (int)ResSubIndex.Wall2Wall;
                            break;
                        case "high eco":
                            _subServiceMenu.selectedIndex = (int)ResSubIndex.HighEco;
                            break;
                        case "low eco":
                            _subServiceMenu.selectedIndex = (int)ResSubIndex.LowEco;
                            break;
                        default:
                            _subServiceMenu.selectedIndex = (int)ResSubIndex.High;
                            break;
                    }

                    break;

                case "industrial":
                    _serviceMenu.selectedIndex = (int)ServiceIndex.Industrial;

                    // Update sub-service menu.
                    UpdateSubServiceMenu();

                    // Sub-service selection.
                    switch (_currentSettings.m_subService)
                    {
                        case "farming":
                            _subServiceMenu.selectedIndex = (int)IndSubIndex.Farming;
                            break;
                        case "forest":
                            _subServiceMenu.selectedIndex = (int)IndSubIndex.Forestry;
                            break;
                        case "oil":
                            _subServiceMenu.selectedIndex = (int)IndSubIndex.Oil;
                            break;
                        case "ore":
                            _subServiceMenu.selectedIndex = (int)IndSubIndex.Ore;
                            break;
                        default:
                            _subServiceMenu.selectedIndex = (int)IndSubIndex.Generic;
                            break;
                    }

                    break;

                case "office":
                    _serviceMenu.selectedIndex = (int)ServiceIndex.Office;

                    // Update sub-service menu.
                    UpdateSubServiceMenu();

                    // Sub-service selection.
                    switch (_currentSettings.m_subService)
                    {
                        case "wall2wall":
                            _subServiceMenu.selectedIndex = (int)OffSubIndex.Wall2Wall;
                            break;
                        case "high tech":
                            _subServiceMenu.selectedIndex = (int)OffSubIndex.IT;
                            break;
                        case "financial":
                            _subServiceMenu.selectedIndex = (int)OffSubIndex.Financial;
                            break;
                        default:
                            _subServiceMenu.selectedIndex = (int)OffSubIndex.Generic;
                            break;
                    }

                    break;

                case "commercial":
                    _serviceMenu.selectedIndex = (int)ServiceIndex.Commercial;

                    // Update sub-service menu.
                    UpdateSubServiceMenu();

                    // Sub-service selection.
                    switch (_currentSettings.m_subService)
                    {
                        case "low":
                            _subServiceMenu.selectedIndex = (int)ComSubIndex.Low;
                            break;
                        case "wall2wall":
                            _subServiceMenu.selectedIndex = (int)ComSubIndex.Wall2Wall;
                            break;
                        case "leisure":
                            _subServiceMenu.selectedIndex = (int)ComSubIndex.Leisure;
                            break;
                        case "tourist":
                            _subServiceMenu.selectedIndex = (int)ComSubIndex.Tourist;
                            break;
                        case "eco":
                            _subServiceMenu.selectedIndex = (int)ComSubIndex.Eco;
                            break;
                        default:
                            _subServiceMenu.selectedIndex = (int)ComSubIndex.High;
                            break;
                    }

                    break;

                case "extractor":
                    _serviceMenu.selectedIndex = (int)ServiceIndex.Extractor;

                    // Update sub-service menu.
                    UpdateSubServiceMenu();

                    // Sub-service selection.
                    switch (_currentSettings.m_subService)
                    {
                        case "forest":
                            _subServiceMenu.selectedIndex = (int)ExtSubIndex.Forestry;
                            break;
                        case "oil":
                            _subServiceMenu.selectedIndex = (int)ExtSubIndex.Oil;
                            break;
                        case "ore":
                            _subServiceMenu.selectedIndex = (int)ExtSubIndex.Ore;
                            break;
                        default:
                            _subServiceMenu.selectedIndex = (int)ExtSubIndex.Farming;
                            break;
                    }

                    break;

                case "dummy":
                    _serviceMenu.selectedIndex = (int)ServiceIndex.Dummy;

                    // Update sub-service menu.
                    UpdateSubServiceMenu();
                    _subServiceMenu.selectedIndex = 0;

                    break;

                default:
                    _serviceMenu.selectedIndex = (int)ServiceIndex.None;

                    // Update sub-service menu.
                    UpdateSubServiceMenu();
                    _subServiceMenu.selectedIndex = 0;

                    break;
            }

            // UI category.
            switch (building.UiCategory)
            {
                case "reslow":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.ResLow;
                    break;
                case "reshigh":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.ResHigh;
                    break;
                case "comlow":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.ComLow;
                    break;
                case "comhigh":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.ComHigh;
                    break;
                case "office":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Office;
                    break;
                case "industrial":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Industrial;
                    break;
                case "farming":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Farming;
                    break;
                case "forest":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Forestry;
                    break;
                case "oil":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Oil;
                    break;
                case "ore":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Ore;
                    break;
                case "leisure":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Leisure;
                    break;
                case "tourist":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Tourist;
                    break;
                case "organic":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.Organic;
                    break;
                case "hightech":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.HighTech;
                    break;
                case "selfsufficient":
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.SelfSufficient;
                    break;
                default:
                    _uiCategoryMenu.selectedIndex = (int)UICatIndex.None;
                    break;
            }

            // Building level.
            UpdateLevelMenu();
            _levelMenu.selectedIndex = Mathf.Min(_levelMenu.items.Length, building.m_level) - 1;

            // Construction cost.
            _constructionCostField.text = building.ConstructionCost.ToString();

            // Use realistic population.
            _realityIgnoredCheck.isChecked = !building.m_realityIgnored;

            // Pollution enabled
            _pollutionEnabledCheck.isChecked = building.m_pollutionEnabled;

            // Growable.
            _growableCheck.isChecked = building.m_growable;

            // Enable RICO.
            _ricoEnabled.isChecked = building.m_ricoEnabled;
        }

        /// <summary>
        /// Updates the sub-service menu based on current service and sub-service selections.
        /// </summary>
        private void UpdateSubServiceMenu()
        {
            switch (_serviceMenu.selectedIndex)
            {
                case (int)ServiceIndex.Residential:
                    _subServiceMenu.items = _resSubs;
                    break;

                case (int)ServiceIndex.Industrial:
                    _subServiceMenu.items = _indSubs;
                    break;

                case (int)ServiceIndex.Commercial:
                    _subServiceMenu.items = _comSubs;
                    break;

                case (int)ServiceIndex.Office:
                    _subServiceMenu.items = _offSubs;
                    break;

                case (int)ServiceIndex.Extractor:
                    _subServiceMenu.items = _extSubs;
                    break;

                default:
                    _subServiceMenu.items = _dumSubs;
                    break;
            }

            // Set selected index of menu to be a valid range.
            _subServiceMenu.selectedIndex = Mathf.Max(0, Mathf.Min(_subServiceMenu.selectedIndex, _subServiceMenu.items.Length - 1));

            // Update UI category.
            UpdateUICategory();
        }

        /// <summary>
        /// Updates the level menu based on current service and sub-service selections.
        /// </summary>
        private void UpdateLevelMenu()
        {
            switch (_serviceMenu.selectedIndex)
            {
                case (int)ServiceIndex.None:
                    _levelMenu.items = _singleLevel;
                    break;

                case (int)ServiceIndex.Residential:
                    _levelMenu.items = _resLevels;
                    break;

                case (int)ServiceIndex.Industrial:
                    _levelMenu.items = _subServiceMenu.selectedIndex == (int)IndSubIndex.Generic ? _workLevels : _singleLevel;
                    break;

                case (int)ServiceIndex.Commercial:
                    _levelMenu.items = (_subServiceMenu.selectedIndex == (int)ComSubIndex.Low || _subServiceMenu.selectedIndex == (int)ComSubIndex.High || _subServiceMenu.selectedIndex == (int)ComSubIndex.Wall2Wall) ? _workLevels : _singleLevel;
                    break;

                case (int)ServiceIndex.Office:
                    _levelMenu.items = _subServiceMenu.selectedIndex == (int)OffSubIndex.IT ? _singleLevel : _workLevels;
                    break;

                case (int)ServiceIndex.Extractor:
                    _levelMenu.items = _singleLevel;
                    break;
            }

            // Set selected index of menu to be a valid range.
            _levelMenu.selectedIndex = Mathf.Max(0, Mathf.Min(_levelMenu.selectedIndex, _levelMenu.items.Length - 1));
        }

        /// <summary>
        /// Reconfigures the RICO options panel to display relevant options for a given service.
        /// This simply hides/shows different option fields for the various services.
        /// </summary>
        /// <param name="service">RICO service to display.</param>
        private void UpdateElementVisibility(string service)
        {
            // Reconfigure the RICO options panel to display relevant options for a given service.
            // This simply hides/shows different option fields for the various services.

            // Defaults by probability.  Pollution is only needed for industrial and extractor, and workplaces are shown for everything except residential.
            _pollutionEnabledCheck.enabled = false;
            _pollutionEnabledCheck.parent.Hide();
            _uneducatedWorkerField.parent.Show();
            _educatedWorkerField.parent.Show();
            _wellEducatedWorkerField.parent.Show();
            _highlyEducatedWorkerField.parent.Show();

            switch (service)
            {
                case "residential":
                    // No workplaces breakdown for residential - hide them.
                    _uneducatedWorkerField.parent.Hide();
                    _educatedWorkerField.parent.Hide();
                    _wellEducatedWorkerField.parent.Hide();
                    _highlyEducatedWorkerField.parent.Hide();
                    break;

                case "industrial":
                    // Industries can pollute.
                    _pollutionEnabledCheck.enabled = true;
                    _pollutionEnabledCheck.parent.Show();
                    break;

                case "extractor":
                    // Extractors can pollute.
                    _pollutionEnabledCheck.enabled = true;
                    _pollutionEnabledCheck.parent.Show();
                    break;
            }
        }

        /// <summary>
        /// Updates the textfield state depending on the state of the 'use realistic population' checkbox state.
        /// </summary>
        private void SetTextfieldState(UIComponent control, bool useReality)
        {
            if (useReality)
            {
                // Using Realistic Population - disable population textfields.
                _manualPopField.Disable();
                _uneducatedWorkerField.Disable();
                _educatedWorkerField.Disable();
                _wellEducatedWorkerField.Disable();
                _highlyEducatedWorkerField.Disable();

                // Set explanatory tooltip.
                string tooltip = Translations.Translate("PRR_OPT_URP");
                _manualPopField.tooltip = tooltip;
                _uneducatedWorkerField.tooltip = tooltip;
                _educatedWorkerField.tooltip = tooltip;
                _wellEducatedWorkerField.tooltip = tooltip;
                _highlyEducatedWorkerField.tooltip = tooltip;
            }
            else
            {
                // Not using Realistic Population - enable population textfields.
                _manualPopField.Enable();
                _uneducatedWorkerField.Enable();
                _educatedWorkerField.Enable();
                _wellEducatedWorkerField.Enable();
                _highlyEducatedWorkerField.Enable();

                // Set default tooltips.
                _manualPopField.tooltip = Translations.Translate("PRR_OPT_CNT");
                _uneducatedWorkerField.tooltip = Translations.Translate("PRR_OPT_JB0");
                _educatedWorkerField.tooltip = Translations.Translate("PRR_OPT_JB1");
                _wellEducatedWorkerField.tooltip = Translations.Translate("PRR_OPT_JB2");
                _highlyEducatedWorkerField.tooltip = Translations.Translate("PRR_OPT_JB3");
            }
        }

        /// <summary>
        /// Returns the current service and subservice based on current menu selections.
        /// </summary>
        private void GetService(out string serviceName, out string subServiceName)
        {
            // Default return value for subservice if we can't match it below.
            subServiceName = "none";

            switch (_serviceMenu.selectedIndex)
            {
                case (int)ServiceIndex.None:
                    serviceName = "none";
                    subServiceName = "none";
                    break;
                case (int)ServiceIndex.Residential:
                    serviceName = "residential";
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)ResSubIndex.High:
                            subServiceName = "high";
                            break;
                        case (int)ResSubIndex.Low:
                            subServiceName = "low";
                            break;
                        case (int)ResSubIndex.Wall2Wall:
                            subServiceName = "wall2wall";
                            break;
                        case (int)ResSubIndex.HighEco:
                            subServiceName = "high eco";
                            break;
                        case (int)ResSubIndex.LowEco:
                            subServiceName = "low eco";
                            break;
                    }

                    break;

                case (int)ServiceIndex.Industrial:
                    serviceName = "industrial";
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)IndSubIndex.Generic:
                            subServiceName = "generic";
                            break;
                        case (int)IndSubIndex.Farming:
                            subServiceName = "farming";
                            break;
                        case (int)IndSubIndex.Forestry:
                            subServiceName = "forest";
                            break;
                        case (int)IndSubIndex.Oil:
                            subServiceName = "oil";
                            break;
                        case (int)IndSubIndex.Ore:
                            subServiceName = "ore";
                            break;
                    }

                    break;

                case (int)ServiceIndex.Office:
                    serviceName = "office";
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)OffSubIndex.Generic:
                            subServiceName = "none";
                            break;
                        case (int)OffSubIndex.Wall2Wall:
                            subServiceName = "wall2wall";
                            break;
                        case (int)OffSubIndex.Financial:
                            subServiceName = "financial";
                            break;
                        case (int)OffSubIndex.IT:
                            subServiceName = "high tech";
                            break;
                    }

                    break;

                case (int)ServiceIndex.Commercial:
                    serviceName = "commercial";
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)ComSubIndex.High:
                            subServiceName = "high";
                            break;
                        case (int)ComSubIndex.Low:
                            subServiceName = "low";
                            break;
                        case (int)ComSubIndex.Wall2Wall:
                            subServiceName = "wall2wall";
                            break;
                        case (int)ComSubIndex.Leisure:
                            subServiceName = "leisure";
                            break;
                        case (int)ComSubIndex.Tourist:
                            subServiceName = "tourist";
                            break;
                        case (int)ComSubIndex.Eco:
                            subServiceName = "eco";
                            break;
                    }

                    break;

                case (int)ServiceIndex.Extractor:
                    serviceName = "extractor";
                    switch (_subServiceMenu.selectedIndex)
                    {
                        case (int)ExtSubIndex.Farming:
                            subServiceName = "farming";
                            break;
                        case (int)ExtSubIndex.Forestry:
                            subServiceName = "forest";
                            break;
                        case (int)ExtSubIndex.Oil:
                            subServiceName = "oil";
                            break;
                        case (int)ExtSubIndex.Ore:
                            subServiceName = "ore";
                            break;
                    }

                    break;

                default:
                    serviceName = "dummy";
                    subServiceName = "none";
                    break;
            }
        }

        /// <summary>
        /// Updates workplace breakdowns to ratios applicable to current settings.
        /// </summary>
        private void UpdateWorkplaceBreakdowns()
        {
            int[] allocation = new int[4];
            int totalJobs;

            // Ignore event if disabled flag is set.
            if (_disableEvents)
            {
                return;
            }

            // If we catch an exception while parsing the manual textfield, it's probably because it's not ready yet (initial asset selection).
            // Simply return without doing anything.
            try
            {
                totalJobs = int.Parse(_manualPopField.text);
            }
            catch
            {
                return;
            }

            if (totalJobs > 0)
            {
                // Get current service and sub-service.
                GetService(out string serviceString, out string subServiceString);

                // Allocate out total workplaces ('manual').
                int[] distribution = RICOUtils.WorkplaceDistributionOf(serviceString, subServiceString, "Level" + (_levelMenu.selectedIndex + 1));
                allocation = WorkplaceAIHelper.DistributeWorkplaceLevels(int.Parse(_manualPopField.text), distribution);

                // Check and adjust for any rounding errors, assigning 'leftover' jobs to the lowest education level.
                allocation[0] += int.Parse(_manualPopField.text) - allocation[0] - allocation[1] - allocation[2] - allocation[3];
            }

            // Disable event handling while we update textfields.
            _disableEvents = true;

            // Update workplace textfields.
            _uneducatedWorkerField.text = allocation[0].ToString();
            _educatedWorkerField.text = allocation[1].ToString();
            _wellEducatedWorkerField.text = allocation[2].ToString();
            _highlyEducatedWorkerField.text = allocation[3].ToString();

            // Resume event handling.
            _disableEvents = false;
        }

        /// <summary>
        /// Creates a RICO-style dropdown menu.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="label">Label text.</param>
        /// <returns>New dropdown menu.</returns>
        private UIDropDown RICODropDown(UIComponent parent, float yPos, string label)
        {
            // Parent container.
            UIPanel container = parent.AddUIComponent<UIPanel>();
            container.height = 25f;
            container.relativePosition = new Vector2(0, yPos);

            // Label.
            UILabel serviceLabel = container.AddUIComponent<UILabel>();
            serviceLabel.textScale = 0.8f;
            serviceLabel.text = label;
            serviceLabel.relativePosition = new Vector2(15f, 6f);

            // Dropdown menu.
            UIDropDown dropDown = container.AddUIComponent<UIDropDown>();
            dropDown.size = new Vector2(180f, 25f);
            dropDown.listBackground = "GenericPanelLight";
            dropDown.itemHeight = 20;
            dropDown.itemHover = "ListItemHover";
            dropDown.itemHighlight = "ListItemHighlight";
            dropDown.normalBgSprite = "ButtonMenu";
            dropDown.disabledBgSprite = "ButtonMenuDisabled";
            dropDown.hoveredBgSprite = "ButtonMenuHovered";
            dropDown.focusedBgSprite = "ButtonMenu";
            dropDown.listWidth = 180;
            dropDown.listHeight = 500;
            dropDown.foregroundSpriteMode = UIForegroundSpriteMode.Stretch;
            dropDown.popupColor = new Color32(45, 52, 61, 255);
            dropDown.popupTextColor = new Color32(170, 170, 170, 255);
            dropDown.zOrder = 1;
            dropDown.textScale = 0.7f;
            dropDown.verticalAlignment = UIVerticalAlignment.Middle;
            dropDown.horizontalAlignment = UIHorizontalAlignment.Left;

            dropDown.textFieldPadding = new RectOffset(8, 0, 8, 0);
            dropDown.itemPadding = new RectOffset(14, 0, 8, 0);

            dropDown.relativePosition = new Vector2(112f, 0);

            UIButton button = dropDown.AddUIComponent<UIButton>();
            dropDown.triggerButton = button;
            button.text = string.Empty;
            button.size = new Vector2(180f, 25f);
            button.relativePosition = new Vector3(0f, 0f);
            button.textVerticalAlignment = UIVerticalAlignment.Middle;
            button.textHorizontalAlignment = UIHorizontalAlignment.Left;
            button.normalFgSprite = "IconDownArrow";
            button.hoveredFgSprite = "IconDownArrowHovered";
            button.pressedFgSprite = "IconDownArrowPressed";
            button.focusedFgSprite = "IconDownArrowFocused";
            button.disabledFgSprite = "IconDownArrowDisabled";
            button.spritePadding = new RectOffset(3, 3, 3, 3);
            button.foregroundSpriteMode = UIForegroundSpriteMode.Fill;
            button.horizontalAlignment = UIHorizontalAlignment.Right;
            button.verticalAlignment = UIVerticalAlignment.Middle;
            button.zOrder = 0;
            button.textScale = 0.8f;

            dropDown.eventSizeChanged += new PropertyChangedEventHandler<Vector2>((c, t) =>
            {
                button.size = t;
                dropDown.listWidth = (int)t.x;
            });

            // Allow for long translation strings.
            dropDown.autoListWidth = true;

            return dropDown;
        }

        /// <summary>
        /// Creates a RICO-style checkbox with a label.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="label">Label text.</param>
        /// <returns>New checkbox.</returns>
        private UICheckBox RICOLabelledCheckBox(UIComponent parent, float yPos, string label)
        {
            // Create containing panel.
            UIPanel container = parent.AddUIComponent<UIPanel>();
            container.height = 25f;
            container.width = 270f;
            container.relativePosition = new Vector2(0, yPos);

            // Add checkbox.
            UICheckBox checkBox = RICOCheckBox(container, 210f);

            // Checkbox label.
            UILabel serviceLabel = container.AddUIComponent<UILabel>();
            serviceLabel.textScale = 0.8f;
            serviceLabel.text = label;
            serviceLabel.relativePosition = new Vector2(15, 6);

            // Label behaviour.
            serviceLabel.autoSize = false;
            serviceLabel.width = 180f;
            serviceLabel.autoHeight = true;
            serviceLabel.wordWrap = true;

            return checkBox;
        }

        /// <summary>
        /// Creates a checkbox bar.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="label">Label text.</param>
        /// <returns>New checkbox.</returns>
        private UICheckBox RICOCheckBar(UIComponent parent, string label)
        {
            // Create panel.
            UIPanel basePanel = parent.AddUIComponent<UIPanel>();
            basePanel.autoSize = false;
            basePanel.autoLayout = false;
            basePanel.height = 25f;
            basePanel.backgroundSprite = "ScrollbarTrack";
            basePanel.width = RICOSettingsPanel.RightWidth;
            basePanel.relativePosition = new Vector2(0f, 5f);

            // Add checkbox.
            UICheckBox checkBox = RICOCheckBox(basePanel, 7f);

            // Checkbox label.
            checkBox.label = checkBox.AddUIComponent<UILabel>();
            checkBox.label.text = label;
            checkBox.label.textScale = 0.8f;
            checkBox.label.autoSize = false;
            checkBox.label.size = new Vector2(190f, 18f);
            checkBox.label.textAlignment = UIHorizontalAlignment.Left;
            checkBox.label.relativePosition = new Vector2(25f, 2f);

            return checkBox;
        }

        /// <summary>
        /// Creates a Plopapble RICO-style checkbox.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="xPos">Relative X position.</param>
        /// <returns>New checkbox.</returns>
        private UICheckBox RICOCheckBox(UIComponent parent, float xPos)
        {
            // Add checkbox.
            UICheckBox checkBox = parent.AddUIComponent<UICheckBox>();
            checkBox.width = RICOSettingsPanel.RightWidth - 8f;
            checkBox.height = 20f;
            checkBox.clipChildren = true;
            checkBox.relativePosition = new Vector2(xPos, 4f);

            // Checkbox sprite.
            UISprite sprite = checkBox.AddUIComponent<UISprite>();
            sprite.spriteName = "ToggleBase";
            sprite.size = new Vector2(16f, 16f);
            sprite.relativePosition = Vector3.zero;

            checkBox.checkedBoxObject = sprite.AddUIComponent<UISprite>();
            ((UISprite)checkBox.checkedBoxObject).spriteName = "ToggleBaseFocused";
            checkBox.checkedBoxObject.size = new Vector2(16f, 16f);
            checkBox.checkedBoxObject.relativePosition = Vector3.zero;

            return checkBox;
        }

        /// <summary>
        /// Creates a Ploppable RICO-style textfield.
        /// </summary>
        /// <param name="parent">Parent component.</param>
        /// <param name="yPos">Relative Y position.</param>
        /// <param name="text">Text label.</param>
        /// <returns>New UITextField.</returns>
        private UITextField RICOTextField(UIComponent parent, float yPos, string text)
        {
            UIPanel container = parent.AddUIComponent<UIPanel>();
            container.height = 25;
            container.relativePosition = new Vector3(0, yPos, 0);

            UILabel label = container.AddUIComponent<UILabel>();
            label.textScale = 0.8f;
            label.text = text;
            label.relativePosition = new Vector3(15, 6, 0);

            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.autoSize = false;
            label.autoHeight = true;
            label.width = 170f;
            label.wordWrap = true;

            return UITextFields.AddTextField(container, 190f, 0f, 100f, 20f, vertPad: 3);
        }
    }
}
