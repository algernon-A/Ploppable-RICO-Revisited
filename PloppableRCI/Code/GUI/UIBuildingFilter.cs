namespace PloppableRICO
{
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// The building filter panel at the top of the settings panel.
    /// </summary>
    public class UIBuildingFilter : UIPanel
    {
        // Constants.
        private const int NumOfCategories = (int)Category.NumCategories;
        private const int NumOfSettings = 4;
        internal const int SecondRow = (int)Category.Education;

        // Layout constants.
        private const float Margin = 5f;
        private const float FirstRowY = 0f;
        private const float FirstRowSize = 35f;
        private const float SecondRowY = FirstRowY + FirstRowSize + Margin;
        private const float SecondRowSize = 25f;
        internal const float FilterBarHeight = SecondRowY + SecondRowSize + Margin;
        private const float SettingsFilterY = FilterBarHeight + Margin;
        private const float SettingsCheckSize = 20f;
        internal const float SettingsFilterHeight = SettingsCheckSize + (Margin * 2f);

        // Panel components.
        private UICheckBox[] categoryToggles;
        private UICheckBox[] settingsFilter;
        private UIButton allCats;
        private UIButton noCats;
        private UITextField nameFilter;


        // FIlter by settings checkboxes.
        internal UICheckBox[] SettingsFilter => settingsFilter;


        // Event to trigger when filtering changes.
        public event PropertyChangedEventHandler<int> EventFilteringChanged;


        /// <summary>
        /// The trimmed current text contents of the name filter textfield.
        /// </summary>
        internal string FilterString => nameFilter.text.Trim();


        /// <summary>
        /// Checks whether or not the specified category is currently selected.
        /// </summary>
        /// <param name="category">Category to query</param>
        /// <returns>True if selected; false otherwise</returns>
        internal bool IsCatSelected(Category category) => categoryToggles[(int)category].isChecked;


        /// <summary>
        /// Category togle button event handler.  Toggles the button state and updates the filter accordingly.
        /// </summary>
        /// <param name="control"></param>
        public void ToggleCat(UICheckBox control)
        {
            // If either shift or control is NOT held down, deselect all other toggles and select this one.
            if (!(Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)))
            {
                for (int i = 0; i < NumOfCategories; i++)
                {
                    categoryToggles[i].isChecked = false;
                }
                
                // Select this toggle.
                control.isChecked = true;
            }
            else
            {
                // Shift or control IS held down; toggle this control.
                control.isChecked = !control.isChecked;
            }

            // Trigger an update.
            EventFilteringChanged(this, 0);
        }


        /// <summary>
        /// Checks whether or not all categories are currently selected.
        /// </summary>
        /// <returns>True if all categories are selected; false otherwise</returns>
        internal bool AllCatsSelected()
        {
            return categoryToggles[(int)Category.Monument].isChecked &&
                categoryToggles[(int)Category.Beautification].isChecked &&
                categoryToggles[(int)Category.Education].isChecked &&
                categoryToggles[(int)Category.Power].isChecked &&
                categoryToggles[(int)Category.Water].isChecked &&
                categoryToggles[(int)Category.Health].isChecked&&
                categoryToggles[(int)Category.Residential].isChecked &&
                categoryToggles[(int)Category.Commercial].isChecked &&
                categoryToggles[(int)Category.Office].isChecked &&
                categoryToggles[(int)Category.Industrial].isChecked;
        }


        /// <summary>
        /// Returns the current filter state as a boolean array.
        /// </summary>
        /// <returns>Current filter state</returns>
        internal bool[] GetFilter()
        {
            // Stores category toggle states and settings filter states, in that order.
            bool[] filterState = new bool[NumOfCategories + NumOfSettings];

            // Iterate through all toggle states and add them to return array.
            for (int i = 0; i < NumOfCategories; i ++)
            {
                filterState[i] = categoryToggles[i].isChecked;
            }

            // Iterate through all settings filter states and add them to return array, after the toggle states.
            for (int i = 0; i < NumOfSettings; i++)
            {
                filterState[i + NumOfCategories] = settingsFilter[i].isChecked;
            }

            return filterState;
        }


        /// <summary>
        /// Sets the current filter configuration from provided boolean array.
        /// </summary>
        /// <param name="filterState">Filter state to apply</param>
        internal void SetFilter(bool[] filterState)
        {
            // Set toggle states from array.
            for (int i = 0; i < NumOfCategories; i++)
            {
                categoryToggles[i].isChecked = filterState[i];
            }

            // Set settings filter states from array (appended after category toggle states).
            for (int i = 0; i < NumOfSettings; i++)
            {
                settingsFilter[i].isChecked = filterState[i + NumOfCategories];
            }
        }


        /// <summary>
        /// Performs initial setup for the panel; we no longer use Start() as that's not sufficiently reliable (race conditions), and is no longer needed, with the new create/destroy process.
        /// </summary>
        internal void Setup()
        {
            // Category toggles.
            categoryToggles = new UICheckBox[NumOfCategories];

            // First row.
            for (int i = 0; i < SecondRow; i++)
            {
                categoryToggles[i] = UIUtils.CreateIconToggle(this, OriginalCategories.atlases[i], OriginalCategories.spriteNames[i], OriginalCategories.spriteNames[i] + "Disabled");
                categoryToggles[i].tooltip = Translations.Translate(OriginalCategories.tooltipKeys[i]);
                categoryToggles[i].relativePosition = new Vector2((FirstRowSize + Margin) * i, FirstRowY);
                categoryToggles[i].isChecked = true;
                categoryToggles[i].readOnly = true;
                categoryToggles[i].eventClick += (control, clickEvent) => ToggleCat(control as UICheckBox);
            }

            // Second row (starts disabled).
            for (int i = SecondRow; i < NumOfCategories; i++)
            {
                categoryToggles[i] = UIUtils.CreateIconToggle(this, OriginalCategories.atlases[i], OriginalCategories.spriteNames[i], OriginalCategories.spriteNames[i] + "Disabled", 25f);
                categoryToggles[i].tooltip = Translations.Translate(OriginalCategories.tooltipKeys[i]);
                categoryToggles[i].relativePosition = new Vector2((SecondRowSize + Margin) * (i - SecondRow), SecondRowY);
                categoryToggles[i].isChecked = true;
                categoryToggles[i].readOnly = true;
                categoryToggles[i].eventClick += (control, clickEvent) => ToggleCat(control as UICheckBox);

                // Start deselected (need to toggle here after setting as checked above to force correct initial 'unchecked' background state, otherwise the 'checked' background is used).
                categoryToggles[i].isChecked = false;
            }

            // 'Select all' button.
            allCats = UIButtons.AddButton(this, (FirstRowSize + Margin) * SecondRow, Margin, Translations.Translate("PRR_FTR_ALL"), 55f);
            allCats.eventClick += (control, clickEvent) =>
            {
                // Iterate through all toggles in top row and activate.
                for (int i = 0; i < SecondRow; i++)
                {
                    categoryToggles[i].isChecked = true;
                }

                // Trigger an update.
                EventFilteringChanged(this, 0);
            };

            // 'Select none'button.
            noCats = UIButtons.AddButton(this, allCats.relativePosition.x + allCats.width + Margin, Margin, Translations.Translate("PRR_FTR_NON"), 55f);
            noCats.eventClick += (c, p) =>
            {
                // Iterate through all toggles and deactivate.
                for (int i = 0; i < NumOfCategories; ++i)
                {
                    categoryToggles[i].isChecked = false;
                }

                // Trigger an update.
                EventFilteringChanged(this, 0);
            };

            // Name filter textfield.
            nameFilter = UITextFields.AddBigLabelledTextField(this, width - 200f, 0, Translations.Translate("PRR_FTR_NAM") + ": ");

            // Trigger events when textfield is updated.
            nameFilter.eventTextChanged += (control, value) => EventFilteringChanged(this, 5);
            nameFilter.eventTextSubmitted += (control, value) => EventFilteringChanged(this, 5);

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
            settingsFilter = new UICheckBox[NumOfSettings];
            for (int i = 0; i < NumOfSettings; ++i)
            {
                settingsFilter[i] = this.AddUIComponent<UICheckBox>();
                settingsFilter[i].width = SettingsCheckSize;
                settingsFilter[i].height = SettingsCheckSize;
                settingsFilter[i].clipChildren = true;
                settingsFilter[i].relativePosition = new Vector2(280 + (30f * i), SettingsFilterY);

                // Checkbox sprites.
                UISprite sprite = settingsFilter[i].AddUIComponent<UISprite>();
                sprite.spriteName = "ToggleBase";
                sprite.size = new Vector2(20f, 20f);
                sprite.relativePosition = Vector3.zero;

                settingsFilter[i].checkedBoxObject = sprite.AddUIComponent<UISprite>();
                ((UISprite)settingsFilter[i].checkedBoxObject).spriteName = "ToggleBaseFocused";
                settingsFilter[i].checkedBoxObject.size = new Vector2(20f, 20f);
                settingsFilter[i].checkedBoxObject.relativePosition = Vector3.zero;

                // Special event handling for 'any' checkbox.
                if (i == (NumOfSettings - 1))
                {
                    settingsFilter[i].eventCheckChanged += (control, isChecked) =>
                    {
                        if (isChecked)
                        {
                            // Unselect all other checkboxes if 'any' is checked.
                            settingsFilter[0].isChecked = false;
                            settingsFilter[1].isChecked = false;
                            settingsFilter[2].isChecked = false;
                        }
                    };
                }
                else
                {
                    // Non-'any' checkboxes.
                    // Unselect 'any' checkbox if any other is checked.
                    settingsFilter[i].eventCheckChanged += (control, isChecked) =>
                    {
                        if (isChecked)
                        {
                            settingsFilter[3].isChecked = false;
                        }
                    };
                }

                // Trigger filtering changed event if any checkbox is changed.
                settingsFilter[i].eventCheckChanged += (control, isChecked) => { EventFilteringChanged(this, 0); };
            }

            // Add settings filter tooltips.
            settingsFilter[0].tooltip = Translations.Translate("PRR_SET_HASMOD");
            settingsFilter[1].tooltip = Translations.Translate("PRR_SET_HASAUT");
            settingsFilter[2].tooltip = Translations.Translate("PRR_SET_HASLOC");
            settingsFilter[3].tooltip = Translations.Translate("PRR_SET_HASANY");
        }


        /// <summary>
        /// Sets the category toggles so that the one that includes the provided category is on, and the rest are off.
        /// </summary>
        /// <param name="buildingClass">RICO category of the building (to match toggle categories)</param>
        internal void SelectBuildingCategory(Category category)
        {
            // Iterate through each category.
            for (int i = 0; i < NumOfCategories; i ++)
            {
                if ((int)category == i)
                {
                    // Category match - select this toggle.
                    categoryToggles[i].isChecked = true;
                }
                else
                {
                    // Otherwise, deselect.
                    categoryToggles[i].isChecked = false;
                }
            }

            // Clear setting filter checkboxes.
            for (int i = 0; i < 4; i++)
            {
                settingsFilter[i].isChecked = false;
                settingsFilter[1].isChecked = false;
                settingsFilter[2].isChecked = false;
            }

            // Clear name search.
            nameFilter.text = string.Empty;
        }
    }
}