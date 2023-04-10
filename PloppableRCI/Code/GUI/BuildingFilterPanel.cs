// <copyright file="BuildingFilterPanel.cs" company="algernon (K. Algernon A. Sheppard)">
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
    /// The building filter panel at the top of the settings panel.
    /// </summary>
    internal class BuildingFilterPanel : UIPanel
    {
        /// <summary>
        /// Filter bar height.
        /// </summary>
        internal const float FilterBarHeight = SecondRowY + SecondRowSize + Margin;

        /// <summary>
        /// Settings filter height.
        /// </summary>
        internal const float SettingsFilterHeight = SettingsCheckSize + (Margin * 2f);

        // Constants.
        private const int NumOfCategories = (int)Category.NumCategories;
        private const int NumOfSettings = 4;
        private const int SecondRow = (int)Category.Education;

        // Layout constants.
        private const float Margin = 5f;
        private const float FirstRowY = 0f;
        private const float FirstRowSize = 35f;
        private const float SecondRowY = FirstRowY + FirstRowSize + Margin;
        private const float SecondRowSize = 25f;
        private const float SettingsFilterY = FilterBarHeight + Margin;
        private const float SettingsCheckSize = 20f;

        // Panel components.
        private UICheckBox[] _categoryToggles;
        private UICheckBox[] _settingsFilter;
        private UIButton _allCats;
        private UIButton _noCats;
        private UITextField _nameFilter;

        /// <summary>
        /// Triggered when the filter changes.
        /// </summary>
        internal event PropertyChangedEventHandler<int> EventFilteringChanged;

        /// <summary>
        /// Gets the settings filter checkbox array.
        /// </summary>
        internal UICheckBox[] SettingsFilter => _settingsFilter;

        /// <summary>
        /// Gets the trimmed current text contents of the name filter textfield.
        /// </summary>
        internal string FilterString => _nameFilter.text.Trim();

        /// <summary>
        /// Called by Unity when the object is created.
        /// Used to perform setup.
        /// </summary>
        public override void Awake()
        {
            base.Awake();

            // Set width.
            width = RICOSettingsPanel.ContentWidth;

            // Category toggles.
            _categoryToggles = new UICheckBox[NumOfCategories];

            // First row.
            for (int i = 0; i < SecondRow; i++)
            {
                _categoryToggles[i] = UICheckBoxes.AddIconToggle(
                    this,
                    (FirstRowSize + Margin) * i,
                    FirstRowY,
                    OriginalCategories.Atlases[i],
                    OriginalCategories.SpriteNames[i],
                    OriginalCategories.SpriteNames[i] + "Disabled",
                    tooltip: Translations.Translate(OriginalCategories.TooltipKeys[i]));
                _categoryToggles[i].isChecked = true;
                _categoryToggles[i].readOnly = true;
                _categoryToggles[i].eventClick += (c, p) => ToggleCat(c as UICheckBox);
            }

            // Second row (starts disabled).
            for (int i = SecondRow; i < NumOfCategories; i++)
            {
                _categoryToggles[i] = UICheckBoxes.AddIconToggle(
                    this,
                    (SecondRowSize + Margin) * (i - SecondRow),
                    SecondRowY,
                    OriginalCategories.Atlases[i],
                    OriginalCategories.SpriteNames[i],
                    OriginalCategories.SpriteNames[i] + "Disabled",
                    width: 25f,
                    height: 25f,
                    tooltip: Translations.Translate(OriginalCategories.TooltipKeys[i]));
                _categoryToggles[i].isChecked = true;
                _categoryToggles[i].readOnly = true;
                _categoryToggles[i].eventClick += (c, p) => ToggleCat(c as UICheckBox);

                // Start deselected (need to toggle here after setting as checked above to force correct initial 'unchecked' background state, otherwise the 'checked' background is used).
                _categoryToggles[i].isChecked = false;
            }

            // 'Select all' button.
            _allCats = UIButtons.AddButton(this, (FirstRowSize + Margin) * SecondRow, Margin, Translations.Translate("PRR_FTR_ALL"), 55f);
            _allCats.eventClick += (c, p) =>
            {
                // Iterate through all toggles in top row and activate.
                for (int i = 0; i < SecondRow; i++)
                {
                    _categoryToggles[i].isChecked = true;
                }

                // Trigger an update.
                EventFilteringChanged(this, 0);
            };

            // 'Select none'button.
            _noCats = UIButtons.AddButton(this, _allCats.relativePosition.x + _allCats.width + Margin, Margin, Translations.Translate("PRR_FTR_NON"), 55f);
            _noCats.eventClick += (c, p) =>
            {
                // Iterate through all toggles and deactivate.
                for (int i = 0; i < NumOfCategories; ++i)
                {
                    _categoryToggles[i].isChecked = false;
                }

                // Trigger an update.
                EventFilteringChanged(this, 0);
            };

            // Name filter textfield.
            _nameFilter = UITextFields.AddBigLabelledTextField(this, width - 200f, 0, Translations.Translate("PRR_FTR_NAM") + ": ");

            // Trigger events when textfield is updated.
            _nameFilter.eventTextChanged += (c, value) => EventFilteringChanged(this, 5);
            _nameFilter.eventTextSubmitted += (c, value) => EventFilteringChanged(this, 5);

            // Create settings filters.
            UILabel filterLabel = this.AddUIComponent<UILabel>();
            filterLabel.textScale = 0.7f;
            filterLabel.text = Translations.Translate("PRR_FTR_SET");
            filterLabel.verticalAlignment = UIVerticalAlignment.Middle;
            filterLabel.autoSize = false;
            filterLabel.width = 270f;
            filterLabel.autoHeight = true;
            filterLabel.wordWrap = true;
            filterLabel.relativePosition = new Vector2(1f, SettingsFilterY + ((SettingsCheckSize - filterLabel.height) / 2f));

            // Setting filter checkboxes.
            _settingsFilter = new UICheckBox[NumOfSettings];
            for (int i = 0; i < NumOfSettings; ++i)
            {
                _settingsFilter[i] = this.AddUIComponent<UICheckBox>();
                _settingsFilter[i].width = SettingsCheckSize;
                _settingsFilter[i].height = SettingsCheckSize;
                _settingsFilter[i].clipChildren = true;
                _settingsFilter[i].relativePosition = new Vector2(280 + (30f * i), SettingsFilterY);

                // Checkbox sprites.
                UISprite sprite = _settingsFilter[i].AddUIComponent<UISprite>();
                sprite.spriteName = "ToggleBase";
                sprite.size = new Vector2(20f, 20f);
                sprite.relativePosition = Vector3.zero;

                _settingsFilter[i].checkedBoxObject = sprite.AddUIComponent<UISprite>();
                ((UISprite)_settingsFilter[i].checkedBoxObject).spriteName = "ToggleBaseFocused";
                _settingsFilter[i].checkedBoxObject.size = new Vector2(20f, 20f);
                _settingsFilter[i].checkedBoxObject.relativePosition = Vector3.zero;

                // Special event handling for 'any' checkbox.
                if (i == (NumOfSettings - 1))
                {
                    _settingsFilter[i].eventCheckChanged += (c, isChecked) =>
                    {
                        if (isChecked)
                        {
                            // Unselect all other checkboxes if 'any' is checked.
                            _settingsFilter[0].isChecked = false;
                            _settingsFilter[1].isChecked = false;
                            _settingsFilter[2].isChecked = false;
                        }
                    };
                }
                else
                {
                    // Non-'any' checkboxes.
                    // Unselect 'any' checkbox if any other is checked.
                    _settingsFilter[i].eventCheckChanged += (c, isChecked) =>
                    {
                        if (isChecked)
                        {
                            _settingsFilter[3].isChecked = false;
                        }
                    };
                }

                // Trigger filtering changed event if any checkbox is changed.
                _settingsFilter[i].eventCheckChanged += (c, isChecked) => { EventFilteringChanged(this, 0); };
            }

            // Add settings filter tooltips.
            _settingsFilter[0].tooltip = Translations.Translate("PRR_SET_HASMOD");
            _settingsFilter[1].tooltip = Translations.Translate("PRR_SET_HASAUT");
            _settingsFilter[2].tooltip = Translations.Translate("PRR_SET_HASLOC");
            _settingsFilter[3].tooltip = Translations.Translate("PRR_SET_HASANY");
        }

        /// <summary>
        /// Checks whether or not the specified category is currently selected.
        /// </summary>
        /// <param name="category">Category to query.</param>
        /// <returns>True if selected; false otherwise.</returns>
        internal bool IsCatSelected(Category category) => _categoryToggles[(int)category].isChecked;

        /// <summary>
        /// Checks whether or not all categories are currently selected.
        /// </summary>
        /// <returns>True if all categories are selected; false otherwise.</returns>
        internal bool AllCatsSelected()
        {
            return _categoryToggles[(int)Category.Monument].isChecked &&
                _categoryToggles[(int)Category.Beautification].isChecked &&
                _categoryToggles[(int)Category.Education].isChecked &&
                _categoryToggles[(int)Category.Power].isChecked &&
                _categoryToggles[(int)Category.Water].isChecked &&
                _categoryToggles[(int)Category.Health].isChecked &&
                _categoryToggles[(int)Category.Residential].isChecked &&
                _categoryToggles[(int)Category.Commercial].isChecked &&
                _categoryToggles[(int)Category.Office].isChecked &&
                _categoryToggles[(int)Category.Industrial].isChecked;
        }

        /// <summary>
        /// Returns the current filter state as a boolean array.
        /// </summary>
        /// <returns>Current filter state.</returns>
        internal bool[] GetFilter()
        {
            // Stores category toggle states and settings filter states, in that order.
            bool[] filterState = new bool[NumOfCategories + NumOfSettings];

            // Iterate through all toggle states and add them to return array.
            for (int i = 0; i < NumOfCategories; i++)
            {
                filterState[i] = _categoryToggles[i].isChecked;
            }

            // Iterate through all settings filter states and add them to return array, after the toggle states.
            for (int i = 0; i < NumOfSettings; i++)
            {
                filterState[i + NumOfCategories] = _settingsFilter[i].isChecked;
            }

            return filterState;
        }

        /// <summary>
        /// Sets the current filter configuration from provided boolean array.
        /// </summary>
        /// <param name="filterState">Filter state to apply.</param>
        internal void SetFilter(bool[] filterState)
        {
            // Set toggle states from array.
            for (int i = 0; i < NumOfCategories; i++)
            {
                _categoryToggles[i].isChecked = filterState[i];
            }

            // Set settings filter states from array (appended after category toggle states).
            for (int i = 0; i < NumOfSettings; i++)
            {
                _settingsFilter[i].isChecked = filterState[i + NumOfCategories];
            }
        }

        /// <summary>
        /// Sets the category toggles so that the one that includes the provided category is on, and the rest are off.
        /// </summary>
        /// <param name="category">RICO category of the building (to match toggle categories).</param>
        internal void SelectBuildingCategory(Category category)
        {
            // Iterate through each category.
            for (int i = 0; i < NumOfCategories; i++)
            {
                if ((int)category == i)
                {
                    // Category match - select this toggle.
                    _categoryToggles[i].isChecked = true;
                }
                else
                {
                    // Otherwise, deselect.
                    _categoryToggles[i].isChecked = false;
                }
            }

            // Clear setting filter checkboxes.
            for (int i = 0; i < 4; i++)
            {
                _settingsFilter[i].isChecked = false;
                _settingsFilter[1].isChecked = false;
                _settingsFilter[2].isChecked = false;
            }

            // Clear name search.
            _nameFilter.text = string.Empty;
        }

        /// <summary>
        /// Category togle button event handler.  Toggles the button state and updates the filter accordingly.
        /// </summary>
        /// <param name="c">Callling component.</param>
        private void ToggleCat(UICheckBox c)
        {
            // If either shift or control is NOT held down, deselect all other toggles and select this one.
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                for (int i = 0; i < NumOfCategories; i++)
                {
                    _categoryToggles[i].isChecked = false;
                }

                // Select this toggle.
                c.isChecked = true;
            }
            else
            {
                // Shift or control IS held down; toggle this control.
                c.isChecked = !c.isChecked;
            }

            // Trigger an update.
            EventFilteringChanged(this, 0);
        }
    }
}