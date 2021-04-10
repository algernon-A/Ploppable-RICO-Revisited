using System;
using System.Linq;
using UnityEngine;
using ColossalFramework.UI;
using System.Collections.Generic;


namespace PloppableRICO
{
    /// <summary>
    /// This class draws the RICO panel, populates it with building buttons, and activates the building tool when buttons are clicked. 
    /// </summary>
    /// 
    public class PloppableTool : ToolBase
    {
        // Number of UI categories.
        private const int NumTypes = 14;
        // Number of UI tabs: +1 to account for 'Settings' tab.
        private const int NumTabs = NumTypes + 1;


        // Object instances.
        private static GameObject _gameObject;
        private static PloppableTool _instance;

        public static PloppableTool Instance => _instance;

        // For tracking growables with accelerated construction.
        internal Dictionary<BuildingInfo, int> constructionTimes;

        // UI components.
        private UIButton PloppableButton;
        private UIPanel buildingPanel;
        private UIScrollPanel scrollPanel;

        private UITabstrip Tabs;
        private readonly UISprite[] TabSprites = new UISprite[NumTabs];
        private readonly UIButton[] TabButtons = new UIButton[NumTabs];
        private UIButton showSettings;

        // State flag.
        private bool hasShown;


        // Names used to identify icons for tabs (specific game icon names - not just made up).
        private readonly string[] Names = new string[]
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
            "Selfsufficient"
        };


        /// <summary>
        /// Initializes the Ploppable Tool (including panel).
        /// </summary>
        internal static void Initialize()
        {
            // Don't do anything if we're already setup.
            if (_instance == null)
            {
                try
                {

                    // Creating our own gameObect helps finding the UI in ModTools.
                    _gameObject = new GameObject("PloppableTool");
                    _gameObject.transform.parent = UIView.GetAView().transform;

                    // Add tool and panel.
                    _instance = _gameObject.AddComponent<PloppableTool>();
                    _instance.DrawPloppablePanel();

                    // Deactivate to start with if we're speed boosting.
                    if (ModSettings.speedBoost)
                    {
                        _gameObject.SetActive(false);
                    }

                    // Initialise construction time dictionary.
                    _instance.constructionTimes = new Dictionary<BuildingInfo, int>();
                }
                catch (Exception e)
                {
                    Logging.LogException(e, "exception initializing ploppable tool");
                }
            }
        }
        

        /// <summary>
        /// Awaken the Kraken! Or PloppableTool tool controller, whatever.
        /// </summary>
        protected override void Awake()
        {
            this.m_toolController = ToolsModifierControl.toolController;
        }
        

        /// <summary>
        /// Destroys the Ploppable Tool GameObject.
        /// </summary>
        public static void Destroy()
        {
            try
            {
                if (_gameObject != null)
                    GameObject.Destroy(_gameObject);
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception destorying PloppableTool");
            }
        }


        /// <summary>
        /// Draws the Ploppable Tool panel.
        /// </summary>
        private void DrawPloppablePanel()
        {
            // Check to make sure that we haven't already done this.
            if (PloppableButton == null)
            {
                // Set state flag; this is a new setup.
                hasShown = false;

                // Main button on ingame toolbar.
                PloppableButton = UIView.GetAView().FindUIComponent<UITabstrip>("MainToolstrip").AddUIComponent<UIButton>();
                PloppableButton.size = new Vector2(43, 49);
                PloppableButton.normalBgSprite = "ToolbarIconGroup6Normal";
                PloppableButton.normalFgSprite = "IconPolicyBigBusiness";
                PloppableButton.focusedBgSprite = "ToolbarIconGroup6Focused";
                PloppableButton.hoveredBgSprite = "ToolbarIconGroup6Hovered";
                PloppableButton.pressedBgSprite = "ToolbarIconGroup6Pressed";
                PloppableButton.disabledBgSprite = "ToolbarIconGroup6Disabled";
                PloppableButton.relativePosition = new Vector2(800, 0);
                PloppableButton.name = "PloppableButton";
                PloppableButton.tooltip = Translations.Translate("PRR_NAME");

                // Event handler - show the Ploppable Tool panel when the button is clicked.
                PloppableButton.eventClick += (component, clickEvent) =>
                {
                    component.Focus();
                    buildingPanel.isVisible = true;
                };

                // Base panel.
                buildingPanel = UIView.GetAView().FindUIComponent("TSContainer").AddUIComponent<UIPanel>();
                buildingPanel.backgroundSprite = "SubcategoriesPanel";
                buildingPanel.isVisible = false;
                buildingPanel.name = "PloppableBuildingPanel";
                buildingPanel.size = new Vector2(859, 109);
                buildingPanel.relativePosition = new Vector2(0, 0);

                // Tabstrip.
                Tabs = UIView.GetAView().FindUIComponent("PloppableBuildingPanel").AddUIComponent<UITabstrip>();
                Tabs.size = new Vector2(832, 25);
                Tabs.relativePosition = new Vector2(13, -25);
                Tabs.pivot = UIPivotPoint.BottomCenter;
                Tabs.padding = new RectOffset(0, 3, 0, 0);

                // Get game sprite thumbnail atlas.
                UITextureAtlas gameIconAtlas = Resources.FindObjectsOfTypeAll<UITextureAtlas>().FirstOrDefault(a => a.name == "Thumbnails");

                // Scroll panel.
                AddScrollPanel();

                // Tabs.
                for (int i = 0; i <= NumTypes; i++)
                {
                    // Draw tabs in tabstrip.
                    TabButtons[i] = new UIButton(); 
                    TabButtons[i] = Tabs.AddUIComponent<UIButton>();
                    TabButtons[i].size = new Vector2(46, 25);
                    TabButtons[i].normalBgSprite = "SubBarButtonBase";
                    TabButtons[i].disabledBgSprite = "SubBarButtonBaseDisabled";
                    TabButtons[i].pressedBgSprite = "SubBarButtonBasePressed";
                    TabButtons[i].hoveredBgSprite = "SubBarButtonBaseHovered";
                    TabButtons[i].focusedBgSprite = "SubBarButtonBaseFocused";
                    TabButtons[i].state = UIButton.ButtonState.Normal;
                    TabButtons[i].name = Names[i] + "Button";
                    TabButtons[i].tabStrip = true;

                    TabSprites[i] = new UISprite();
                    TabSprites[i] = TabButtons[i].AddUIComponent<UISprite>();

                    // Standard "Vanilla" categories (low and high residential, low and high commercial, and offices) - use standard zoning icons from original vanilla release.
                    if (i <= 5)
                    {
                        TabSprites[i].atlas = gameIconAtlas;
                        SetTabSprite(TabSprites[i], "Zoning" + Names[i]);
                    }
                    else
                    {
                        // Other types don't have standard zoning icons; use policy icons instead.
                        SetTabSprite(TabSprites[i], "IconPolicy" + Names[i]);
                    }
                }

                // This can't happen in a loop, because the loop index is undefined after setup has occured (i.e. when the function is actually called).
                TabButtons[0].eventClick += (component, clickEvent) => TabClicked(0, TabSprites[0]);
                TabButtons[1].eventClick += (component, clickEvent) => TabClicked(1, TabSprites[1]);
                TabButtons[2].eventClick += (component, clickEvent) => TabClicked(2, TabSprites[2]);
                TabButtons[3].eventClick += (component, clickEvent) => TabClicked(3, TabSprites[3]);
                TabButtons[4].eventClick += (component, clickEvent) => TabClicked(4, TabSprites[4]);
                TabButtons[5].eventClick += (component, clickEvent) => TabClicked(5, TabSprites[5]);
                TabButtons[6].eventClick += (component, clickEvent) => TabClicked(6, TabSprites[6]);
                TabButtons[7].eventClick += (component, clickEvent) => TabClicked(7, TabSprites[7]);
                TabButtons[8].eventClick += (component, clickEvent) => TabClicked(8, TabSprites[8]);
                TabButtons[9].eventClick += (component, clickEvent) => TabClicked(9, TabSprites[9]);
                // Below are DLC categories - AD for first two, then GC for next 3.  Will be hidden if relevant DLC is not installed.
                TabButtons[10].eventClick += (component, clickEvent) => TabClicked(10, TabSprites[10]);
                TabButtons[11].eventClick += (component, clickEvent) => TabClicked(11, TabSprites[11]);
                TabButtons[12].eventClick += (component, clickEvent) => TabClicked(12, TabSprites[12]);
                TabButtons[13].eventClick += (component, clickEvent) => TabClicked(13, TabSprites[13]);
                TabButtons[14].eventClick += (component, clickEvent) => TabClicked(14, TabSprites[14]);

                // Activate low residential panel to start with (what the user sees on first opening the panel).
                //BuildingPanels[0].isVisible = true;

                // Hide AD tabs if AD is not installed.
                if (!Util.IsADinstalled())
                {
                    TabButtons[10].isVisible = false;
                    TabButtons[11].isVisible = false;
                }

                // Hide GC tabs if GC is not installed.
                if (!Util.IsGCinstalled())
                {
                    TabButtons[12].isVisible = false;
                    TabButtons[13].isVisible = false;
                    TabButtons[14].isVisible = false;
                }

                // Settings tab.
                showSettings = UIUtils.CreateButton(Tabs);
                showSettings.size = new Vector2(100, 25);
                showSettings.normalBgSprite = "SubBarButtonBase";
                showSettings.eventClick += (component, clickEvent) =>
                {
                    SettingsPanel.Open(scrollPanel?.selectedItem?.prefab);
                };

                // Add UI text.
                SetText();


                // Toggle active state on visibility changed if we're using the UI speed boost (deactivating when hidden to minimise UI workload and impact on performance).
                buildingPanel.eventVisibilityChanged += (component, isVisible) =>
                {
                    // Additional check to allow for the case where speedboost has been deactivated mid-game while the panel was deactivated.
                    if ((ModSettings.speedBoost) || (isVisible && !buildingPanel.gameObject.activeSelf))
                    {
                        buildingPanel.gameObject.SetActive(isVisible);
                    }

                    // Other checks.
                    if(isVisible)
                    {
                        // If this is the first time we're visible, set the display to the initial default tab (low residential).
                        if (!hasShown)
                        {
                            // Set initial default tab.
                            TabClicked(0, _instance.TabSprites[0]);

                            // Done!
                            hasShown = true;
                        }
                        else
                        {
                            // Clear previous selection and refresh panel.
                            scrollPanel.selectedItem = null;
                            scrollPanel.Refresh();
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
        /// Adds/updates text components of the Ploppable Tool Panel (tooltips, settings button) according to the current language.
        /// </summary>
        internal void SetText()
        {
            // Set settings button text.
            showSettings.text = Translations.Translate("PRR_SET");

            // Populate tooltips.
            UICategories tooltips = new UICategories();
            for (int i = 0; i <= NumTypes; i++)
            {
                TabButtons[i].tooltip = tooltips.names[i];
            }
        }


        /// <summary>
        /// Sets the sprite for a given Ploppable Tool panel tab.
        /// </summary>
        /// <param name="sprite">Panel tab</param>
        /// <param name="spriteName">Name of sprite to set</param>
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
        /// <param name="panel">The Ploppable Tool panel for the selected tab</param>
        /// <param name="sprite">The sprite icon for the selected tab</param>
        public void TabClicked(int uiCategory, UISprite sprite)
        {
            // Clear the scroll panel.
            scrollPanel.Clear();

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
            scrollPanel.itemsData.m_buffer = buildingList.OrderBy(x => x.DisplayName).ToArray();
            scrollPanel.itemsData.m_size = buildingList.Count;

            // Display the scroll panel.
            scrollPanel.DisplayAt(0);

            // Redraw all tab sprites in their base state (unfocused).
            for (int i = 0; i <= NumTypes; i++)
            {
                if (i <= 5)
                {
                    TabSprites[i].spriteName = "Zoning" + Names[i];

                }
                else
                {
                    TabSprites[i].spriteName = "IconPolicy" + Names[i];
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
        /// <param name="uiCategory">Ploppable RICO UI category string</param>
        /// <returns>UI category index</returns>
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
        /// Regenerates all thumbnails.
        /// Useful for e.g. regenerating thumbnails.
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
        /// Sets up the building scroll panel.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private void AddScrollPanel()
        {
            scrollPanel = buildingPanel.AddUIComponent<UIScrollPanel>();

            // Basic setup.
            scrollPanel.name = "RICOScrollPanel";
            scrollPanel.autoLayout = false;
            scrollPanel.autoReset = false;
            scrollPanel.autoSize = false;
            scrollPanel.template = "PlaceableItemTemplate";
            scrollPanel.itemWidth = 109f;
            scrollPanel.itemHeight = 100f;
            scrollPanel.canSelect = true;

            // Size and position.
            scrollPanel.size = new Vector2(763, 100);
            scrollPanel.relativePosition = new Vector3(48, 5);

            // 'Left' and 'Right' buttons to scroll panel.
            scrollPanel.leftArrow = ArrowButton("ArrowLeft", 16f);
            scrollPanel.rightArrow = ArrowButton("ArrowRight", 812f);

            // Event handler on grandparent size change.
            buildingPanel.parent.eventSizeChanged += (control, size) =>
            {
                // If we're visible, resize to match the new grandparent size.
                if (scrollPanel.isVisible)
                {
                    // New size.
                    scrollPanel.size = new Vector2((int)((size.x - 40f) / scrollPanel.itemWidth) * scrollPanel.itemWidth, (int)(size.y / scrollPanel.itemHeight) * scrollPanel.itemHeight);

                    // New relative position.
                    scrollPanel.relativePosition = new Vector3(scrollPanel.relativePosition.x, Mathf.Floor((size.y - scrollPanel.height) / 2));

                    // Move right arrow if it exists.
                    if (scrollPanel.rightArrow != null)
                    {
                        scrollPanel.rightArrow.relativePosition = new Vector3(scrollPanel.relativePosition.x + scrollPanel.width, 0);
                    }
                }
            };
        }


        /// <summary>
        /// Adds a left or right arrow button to the panel.
        /// </summary>
        /// <param name="name">Sprite base name</param>
        /// <param name="xPos">Button x position</param>
        /// <returns>New arrow button attached to the building panel</returns>
        private UIButton ArrowButton(string name, float xPos)
        {
            // Create the button, attached to the building panel.
            UIButton arrowButton = buildingPanel.AddUIComponent<UIButton>();

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
    }
}




