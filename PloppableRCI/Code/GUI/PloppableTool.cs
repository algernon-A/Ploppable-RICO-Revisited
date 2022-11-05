// <copyright file="PloppableTool.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AlgernonCommons;
    using AlgernonCommons.Translation;
    using AlgernonCommons.UI;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// This class draws the RICO panel, populates it with building buttons, and activates the building tool when buttons are clicked.
    /// </summary>
    public class PloppableTool : ToolBase
    {
        // Number of UI categories.
        private const int NumTypes = 14;

        // Number of UI tabs: +1 to account for 'Settings' tab.
        private const int NumTabs = NumTypes + 1;

        // Object instances.
        private static GameObject s_gameObject;
        private static PloppableTool s_instance;

        // UI components.
        private readonly UISprite[] _tabSprites = new UISprite[NumTabs];
        private readonly UIButton[] _tabButtons = new UIButton[NumTabs];

        // Names used to identify icons for tabs (specific game icon names - not just made up).
        private readonly string[] _names = new string[]
        {
            "ResidentialLow",
            "ResidentialHigh",
            "CommercialLow",
            "CommercialHigh",
            "Office",
            "Industrial",
            "Farming",
            "Forest",
            "Oil",
            "Ore",
            "Leisure",
            "Tourist",
            "Organic",
            "Hightech",
            "Selfsufficient",
        };

        // UI components.
        private UIScrollPanel _scrollPanel;
        private UIButton _ploppableButton;
        private UIPanel _buildingPanel;
        private UITabstrip _tabs;
        private UIButton _showSettings;

        // State flag.
        private bool _hasShown;

        /// <summary>
        /// Gets the current instance.
        /// </summary>
        public static PloppableTool Instance => s_instance;

        /// <summary>
        /// Initializes the Ploppable Tool (including panel).
        /// </summary>
        internal static void Initialize()
        {
            // Don't do anything if we're already setup.
            if (s_instance == null)
            {
                try
                {
                    // Creating our own gameObect helps finding the UI in ModTools.
                    s_gameObject = new GameObject("PloppableTool");
                    s_gameObject.transform.parent = UIView.GetAView().transform;

                    // Add tool and panel.
                    s_instance = s_gameObject.AddComponent<PloppableTool>();
                    s_instance.DrawPloppablePanel();

                    // Deactivate to start with if we're speed boosting.
                    if (ModSettings.speedBoost)
                    {
                        s_gameObject.SetActive(false);
                    }
                }
                catch (Exception e)
                {
                    Logging.LogException(e, "exception initializing ploppable tool");
                }
            }
        }

        /// <summary>
        /// Destroys the Ploppable Tool GameObject.
        /// </summary>
        internal static void Destroy()
        {
            try
            {
                if (s_gameObject != null)
                {
                    GameObject.Destroy(s_gameObject);
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception destorying PloppableTool");
            }
        }

        /// <summary>
        /// Adds/updates text components of the Ploppable Tool Panel (tooltips, settings button) according to the current language.
        /// </summary>
        internal void SetText()
        {
            // Set settings button text.
            _showSettings.text = Translations.Translate("PRR_SET");

            // Change RICO button tooltip.
            _ploppableButton.tooltip = Translations.Translate("PRR_NAME");

            // Populate tooltips.
            UICategories tooltips = new UICategories();
            for (int i = 0; i <= NumTypes; i++)
            {
                _tabButtons[i].tooltip = tooltips.Names[i];
            }
        }

        /// <summary>
        /// Regenerates all thumbnails.
        /// </summary>
        internal void RegenerateThumbnails()
        {
            // Only do this if the ploppable tool has been created.
            if (Instance != null)
            {
                Logging.Message("regenerating all thumbnails");

                // Step through each loaded and active RICO prefab.
                foreach (BuildingData buildingData in Loading.xmlManager.prefabHash.Values)
                {
                    // Cancel its atlas.
                    buildingData.thumbnailAtlas = null;
                }
            }
        }

        /// <summary>
        /// Awaken the Kraken! Or PloppableTool tool controller, whatever.
        /// </summary>
        protected override void Awake()
        {
            m_toolController = ToolsModifierControl.toolController;
        }

        /// <summary>
        /// Draws the Ploppable Tool panel.
        /// </summary>
        private void DrawPloppablePanel()
        {
            // Check to make sure that we haven't already done this.
            if (_ploppableButton == null)
            {
                // Set state flag; this is a new setup.
                _hasShown = false;

                // Main button on ingame toolbar.
                _ploppableButton = UIView.GetAView().FindUIComponent<UITabstrip>("MainToolstrip").AddUIComponent<UIButton>();
                _ploppableButton.size = new Vector2(43, 49);
                _ploppableButton.normalBgSprite = "ToolbarIconGroup6Normal";
                _ploppableButton.normalFgSprite = "IconPolicyBigBusiness";
                _ploppableButton.focusedBgSprite = "ToolbarIconGroup6Focused";
                _ploppableButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
                _ploppableButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
                _ploppableButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
                _ploppableButton.relativePosition = new Vector2(800, 0);
                _ploppableButton.name = "PloppableButton";

                // Event handler - show the Ploppable Tool panel when the button is clicked.
                _ploppableButton.eventClick += (component, clickEvent) =>
                {
                    component.Focus();
                    _buildingPanel.isVisible = true;
                };

                // Base panel.
                _buildingPanel = UIView.GetAView().FindUIComponent("TSContainer").AddUIComponent<UIPanel>();
                _buildingPanel.backgroundSprite = "SubcategoriesPanel";
                _buildingPanel.isVisible = false;
                _buildingPanel.name = "PloppableBuildingPanel";
                _buildingPanel.size = new Vector2(859, 109);
                _buildingPanel.relativePosition = new Vector2(0, 0);

                // Tabstrip.
                _tabs = UIView.GetAView().FindUIComponent("PloppableBuildingPanel").AddUIComponent<UITabstrip>();
                _tabs.size = new Vector2(832, 25);
                _tabs.relativePosition = new Vector2(13, -25);
                _tabs.pivot = UIPivotPoint.BottomCenter;
                _tabs.padding = new RectOffset(0, 3, 0, 0);

                // Get game sprite thumbnail atlas.
                UITextureAtlas gameIconAtlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Thumbnails");

                // Scroll panel.
                AddScrollPanel();

                // Tabs.
                for (int i = 0; i <= NumTypes; i++)
                {
                    // Draw tabs in tabstrip.
                    _tabButtons[i] = new UIButton();
                    _tabButtons[i] = _tabs.AddUIComponent<UIButton>();
                    _tabButtons[i].size = new Vector2(46f, 25f);
                    _tabButtons[i].normalBgSprite = "SubBarButtonBase";
                    _tabButtons[i].disabledBgSprite = "SubBarButtonBaseDisabled";
                    _tabButtons[i].pressedBgSprite = "SubBarButtonBasePressed";
                    _tabButtons[i].hoveredBgSprite = "SubBarButtonBaseHovered";
                    _tabButtons[i].focusedBgSprite = "SubBarButtonBaseFocused";
                    _tabButtons[i].state = UIButton.ButtonState.Normal;
                    _tabButtons[i].name = _names[i] + "Button";
                    _tabButtons[i].tabStrip = true;

                    _tabSprites[i] = new UISprite();
                    _tabSprites[i] = _tabButtons[i].AddUIComponent<UISprite>();

                    // Standard "Vanilla" categories (low and high residential, low and high commercial, and offices) - use standard zoning icons from original vanilla release.
                    if (i <= 5)
                    {
                        _tabSprites[i].atlas = gameIconAtlas;
                        SetTabSprite(_tabSprites[i], "Zoning" + _names[i]);
                    }
                    else
                    {
                        // Other types don't have standard zoning icons; use policy icons instead.
                        SetTabSprite(_tabSprites[i], "IconPolicy" + _names[i]);
                    }
                }

                // This can't happen in a loop, because the loop index is undefined after setup has occured (i.e. when the function is actually called).
                _tabButtons[0].eventClick += (component, clickEvent) => TabClicked(0, _tabSprites[0]);
                _tabButtons[1].eventClick += (component, clickEvent) => TabClicked(1, _tabSprites[1]);
                _tabButtons[2].eventClick += (component, clickEvent) => TabClicked(2, _tabSprites[2]);
                _tabButtons[3].eventClick += (component, clickEvent) => TabClicked(3, _tabSprites[3]);
                _tabButtons[4].eventClick += (component, clickEvent) => TabClicked(4, _tabSprites[4]);
                _tabButtons[5].eventClick += (component, clickEvent) => TabClicked(5, _tabSprites[5]);
                _tabButtons[6].eventClick += (component, clickEvent) => TabClicked(6, _tabSprites[6]);
                _tabButtons[7].eventClick += (component, clickEvent) => TabClicked(7, _tabSprites[7]);
                _tabButtons[8].eventClick += (component, clickEvent) => TabClicked(8, _tabSprites[8]);
                _tabButtons[9].eventClick += (component, clickEvent) => TabClicked(9, _tabSprites[9]);

                // Below are DLC categories - AD for first two, then GC for next 3.  Will be hidden if relevant DLC is not installed.
                _tabButtons[10].eventClick += (component, clickEvent) => TabClicked(10, _tabSprites[10]);
                _tabButtons[11].eventClick += (component, clickEvent) => TabClicked(11, _tabSprites[11]);
                _tabButtons[12].eventClick += (component, clickEvent) => TabClicked(12, _tabSprites[12]);
                _tabButtons[13].eventClick += (component, clickEvent) => TabClicked(13, _tabSprites[13]);
                _tabButtons[14].eventClick += (component, clickEvent) => TabClicked(14, _tabSprites[14]);

                // Hide AD tabs if AD is not installed.
                if (!Util.IsADinstalled())
                {
                    _tabButtons[10].isVisible = false;
                    _tabButtons[11].isVisible = false;
                }

                // Hide GC tabs if GC is not installed.
                if (!Util.IsGCinstalled())
                {
                    _tabButtons[12].isVisible = false;
                    _tabButtons[13].isVisible = false;
                    _tabButtons[14].isVisible = false;
                }

                // Settings tab.
                _showSettings = UIButtons.AddButton(_tabs, 0f, 0f, Translations.Translate("PRR_SET"), 100f, 25f, 0.9f, tooltip: Translations.Translate("PRR_NAME"));
                _showSettings.normalBgSprite = "SubBarButtonBase";
                _showSettings.eventClick += (component, clickEvent) =>
                {
                    SettingsPanelManager.Open(_scrollPanel?.selectedItem?.prefab);
                };

                // Add UI text.
                SetText();

                // Toggle active state on visibility changed if we're using the UI speed boost (deactivating when hidden to minimise UI workload and impact on performance).
                _buildingPanel.eventVisibilityChanged += (component, isVisible) =>
                {
                    // Additional check to allow for the case where speedboost has been deactivated mid-game while the panel was deactivated.
                    if (ModSettings.speedBoost || (isVisible && !_buildingPanel.gameObject.activeSelf))
                    {
                        _buildingPanel.gameObject.SetActive(isVisible);
                    }

                    // Other checks.
                    if (isVisible)
                    {
                        // If this is the first time we're visible, set the display to the initial default tab (low residential).
                        if (!_hasShown)
                        {
                            // Set initial default tab.
                            TabClicked(0, s_instance._tabSprites[0]);

                            // Done!
                            _hasShown = true;
                        }
                        else
                        {
                            // Clear previous selection and refresh panel.
                            _scrollPanel.selectedItem = null;
                            _scrollPanel.Refresh();
                        }
                    }
                    else
                    {
                        // Destroy thumbnail renderer if we're no longer visible.
                        ThumbnailManager.Close();
                    }
                };
            }
        }

        /// <summary>
        /// Sets the sprite for a given Ploppable Tool panel tab.
        /// </summary>
        /// <param name="sprite">Panel tab.</param>
        /// <param name="spriteName">Name of sprite to set.</param>
        private void SetTabSprite(UISprite sprite, string spriteName)
        {
            UISprite tabSprite = sprite;
            tabSprite.isInteractive = false;
            tabSprite.relativePosition = new Vector2(5, 0);
            tabSprite.spriteName = spriteName;
            tabSprite.size = new Vector2(35, 25);
        }

        /// <summary>
        /// Handles click events for Ploppable Tool panel tabs.
        /// </summary>
        /// <param name="uiCategory">The UI category index.</param>
        /// <param name="sprite">The sprite icon for the selected tab.</param>
        private void TabClicked(int uiCategory, UISprite sprite)
        {
            // Clear the scroll panel.
            _scrollPanel.Clear();

            // List of buildings in this category.
            List<BuildingData> buildingList = new List<BuildingData>();

            // Iterate through each prefab in our collection and see if it has RICO settings with a matching UI category.
            foreach (BuildingData buildingData in Loading.xmlManager.prefabHash.Values)
            {
                // Get the currently active RICO setting (if any) for this building.
                RICOBuilding ricoSetting = RICOUtils.CurrentRICOSetting(buildingData);

                // See if there's a valid RICO setting.
                if (ricoSetting != null)
                {
                    // Valid setting - if the UI category matches this one, add it to the list.
                    if (UICategoryIndex(ricoSetting.UiCategory) == uiCategory)
                    {
                        buildingList.Add(buildingData);
                    }
                }
            }

            // Set display FastList using our list of selected buildings, sorted alphabetically.
            _scrollPanel.itemsData.m_buffer = buildingList.OrderBy(x => x.DisplayName).ToArray();
            _scrollPanel.itemsData.m_size = buildingList.Count;

            // Display the scroll panel.
            _scrollPanel.DisplayAt(0);

            // Redraw all tab sprites in their base state (unfocused).
            for (int i = 0; i <= NumTypes; i++)
            {
                if (i <= 5)
                {
                    _tabSprites[i].spriteName = "Zoning" + _names[i];
                }
                else
                {
                    _tabSprites[i].spriteName = "IconPolicy" + _names[i];
                }
            }

            // Focus this sprite (no focused versions for AD or GC sprites so exclude those).
            if (sprite.spriteName != "IconPolicyLeisure" && sprite.spriteName != "IconPolicyTourist" && sprite.spriteName != "IconPolicyHightech" && sprite.spriteName != "IconPolicyOrganic" && sprite.spriteName != "IconPolicySelfsufficient")
            {
                sprite.spriteName += "Focused";
            }
        }

        /// <summary>
        /// Returns the UI category index for the given UI category string.
        /// </summary>
        /// <param name="uiCategory">Ploppable RICO UI category string.</param>
        /// <returns>UI category index.</returns>
        private int UICategoryIndex(string uiCategory)
        {
            switch (uiCategory)
            {
                case "reslow":
                    return 0;
                case "reshigh":
                    return 1;
                case "comlow":
                    return 2;
                case "comhigh":
                    return 3;
                case "office":
                    return 4;
                case "industrial":
                    return 5;
                case "farming":
                    return 6;
                case "forest":
                    return 7;
                case "oil":
                    return 8;
                case "ore":
                    return 9;
                case "leisure":
                    if (Util.IsADinstalled())
                    {
                        return 10;
                    }
                    else
                    {
                        // If AD is not installed, default to low commercial.
                        return 2;
                    }

                case "tourist":
                    if (Util.IsADinstalled())
                    {
                        return 11;
                    }
                    else
                    {
                        // If AD is not installed, fall back to low commercial.
                        return 2;
                    }

                case "organic":
                    if (Util.IsGCinstalled())
                    {
                        return 12;
                    }
                    else
                    {
                        // If GC is not installed, fall back to low commercial.
                        return 2;
                    }

                case "hightech":
                    // IT cluster.
                    if (Util.IsGCinstalled())
                    {
                        return 13;
                    }
                    else
                    {
                        // If GC is not installed, fall back to office.
                        return 4;
                    }

                case "selfsufficient":
                    // Self-sufficient (eco) residential.
                    if (Util.IsGCinstalled())
                    {
                        return 14;
                    }
                    else
                    {
                        // If GC is not installed, fall back to low residential.
                        return 0;
                    }

                default:
                    return 0;
            }
        }

        /// <summary>
        /// Sets up the building scroll panel.
        /// </summary>
        private void AddScrollPanel()
        {
            _scrollPanel = _buildingPanel.AddUIComponent<UIScrollPanel>();

            // Basic setup.
            _scrollPanel.name = "RICOScrollPanel";
            _scrollPanel.autoLayout = false;
            _scrollPanel.autoReset = false;
            _scrollPanel.autoSize = false;
            _scrollPanel.template = "PlaceableItemTemplate";
            _scrollPanel.itemWidth = 109f;
            _scrollPanel.itemHeight = 100f;
            _scrollPanel.canSelect = true;

            // Size and position.
            _scrollPanel.size = new Vector2(763, 100);
            _scrollPanel.relativePosition = new Vector3(48, 5);

            // 'Left' and 'Right' buttons to scroll panel.
            _scrollPanel.leftArrow = ArrowButton("ArrowLeft", 16f);
            _scrollPanel.rightArrow = ArrowButton("ArrowRight", 812f);

            // Event handler on grandparent size change.
            _buildingPanel.parent.eventSizeChanged += (c, size) =>
            {
                // If we're visible, resize to match the new grandparent size.
                if (_scrollPanel.isVisible)
                {
                    // New size.
                    _scrollPanel.size = new Vector2((int)((size.x - 40f) / _scrollPanel.itemWidth) * _scrollPanel.itemWidth, (int)(size.y / _scrollPanel.itemHeight) * _scrollPanel.itemHeight);

                    // New relative position.
                    _scrollPanel.relativePosition = new Vector3(_scrollPanel.relativePosition.x, Mathf.Floor((size.y - _scrollPanel.height) / 2));

                    // Move right arrow if it exists.
                    if (_scrollPanel.rightArrow != null)
                    {
                        _scrollPanel.rightArrow.relativePosition = new Vector3(_scrollPanel.relativePosition.x + _scrollPanel.width, 0);
                    }
                }
            };
        }

        /// <summary>
        /// Adds a left or right arrow button to the panel.
        /// </summary>
        /// <param name="name">Sprite base name.</param>
        /// <param name="xPos">Button x position.</param>
        /// <returns>New arrow button attached to the building panel.</returns>
        private UIButton ArrowButton(string name, float xPos)
        {
            // Create the button, attached to the building panel.
            UIButton arrowButton = _buildingPanel.AddUIComponent<UIButton>();

            // Size and position.
            arrowButton.size = new Vector2(32, 32);
            arrowButton.relativePosition = new Vector3(xPos, 33);
            arrowButton.horizontalAlignment = UIHorizontalAlignment.Center;
            arrowButton.verticalAlignment = UIVerticalAlignment.Middle;

            // Sprites.
            arrowButton.normalBgSprite = name;
            arrowButton.pressedBgSprite = name + "Pressed";
            arrowButton.hoveredBgSprite = name + "Hovered";
            arrowButton.disabledBgSprite = name + "Disabled";

            return arrowButton;
        }

        /// <summary>
        /// Scrollable building selection panel.
        /// </summary>
        public class UIScrollPanel : UIFastList<BuildingData, UIScrollPanelItem, UIButton>
        {
            // Empty - we only need the inheritence with the specified types.
        }
    }
}