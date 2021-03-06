﻿using UnityEngine;
using ColossalFramework.UI;


namespace PloppableRICO
{
    /// <summary>
    /// Static utilities class for creating UI controls.
    /// </summary>
    public static class UIControls
    {
        /// <summary>
        /// Adds a simple pushbutton.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="text">Button text</param>
        /// <param name="width">Button width (default 200)</param>
        /// <param name="height">Button height (default 30)</param>
        /// <param name="scale">Text scale (default 0.9)</param>
        /// <returns></returns>
        public static UIButton AddButton(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 30f, float scale = 0.9f)
        {
            UIButton button = parent.AddUIComponent<UIButton>();

            // Size and position.
            button.size = new Vector2(width, height);
            button.relativePosition = new Vector2(posX, posY);

            // Appearance.
            button.textScale = scale;
            button.normalBgSprite = "ButtonMenu";
            button.hoveredBgSprite = "ButtonMenuHovered";
            button.pressedBgSprite = "ButtonMenuPressed";
            button.disabledBgSprite = "ButtonMenuDisabled";
            button.disabledTextColor = new Color32(128, 128, 128, 255);
            button.canFocus = false;

            // Text.
            button.text = text;

            return button;
        }


        /// <summary>
        /// Adds a large textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Textfield width (default 200)</param>
        /// <returns>New large textfield with attached label</returns>
        public static UITextField BigLabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f) => LabelledTextField(parent, posX, posY, text, width, 30f, 1.2f, 6);


        /// <summary>
        /// Adds a textfield with an attached label to the left.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Textfield width (default 200)</param>
        /// <param name="height">Textfield height (default 22)</param>
        /// <param name="scale">Text scale (default 1.0)</param>
        /// <param name="vertPad">Vertical text padding within textfield box (default 4)</param>
        /// <returns>New textfield with attached label</returns>
        public static UITextField LabelledTextField(UIComponent parent, float posX, float posY, string text, float width = 200f, float height = 22f, float scale = 1.0f, int vertPad = 4)
        {
            UITextField textField = AddTextField(parent, posX, posY, width, height, scale, vertPad);

            // Label.
            UILabel label = textField.AddUIComponent<UILabel>();
            label.textScale = scale;
            label.text = text;
            label.autoSize = true;
            label.verticalAlignment = UIVerticalAlignment.Middle;
            label.wordWrap = true;

            // Set position.
            label.relativePosition = new Vector2(-(label.width + 5f), (height - label.height) / 2);

            return textField;
        }


        /// <summary>
        /// Adds an input text field at the specified coordinates.
        /// </summary>
        /// <param name="textField">Textfield object</param>
        /// <param name="posX">Relative X postion</param>
        /// <param name="posY">Relative Y position</param>
        /// <param name="parent">component to add to</param>
        /// <param name="height">Textfield height (default 22)</param>
        /// <param name="scale">Text scale (default 1.0)</param>
        /// <param name="vertPad">Vertical text padding within textfield box (default 4)</param>
        /// <param name="tooltip">Tooltip, if any</param>
        /// <returns>New textfield *without* attached label</returns>
        public static UITextField AddTextField(UIComponent parent, float posX, float posY, float width = 200f, float height = 22f, float scale = 1f, int vertPad = 4, string tooltip = null)
        {
            UITextField textField = parent.AddUIComponent<UITextField>();

            // Size and position.
            textField.size = new Vector2(width, height);
            textField.relativePosition = new Vector2(posX, posY);

            // Text settings.
            textField.textScale = scale;
            textField.padding = new RectOffset(6, 6, vertPad, 3);
            textField.horizontalAlignment = UIHorizontalAlignment.Center;

            // Behaviour.
            textField.builtinKeyNavigation = true;
            textField.isInteractive = true;
            textField.readOnly = false;

            // Appearance.
            textField.color = new Color32(255, 255, 255, 255);
            textField.textColor = new Color32(0, 0, 0, 255);
            textField.disabledTextColor = new Color32(0, 0, 0, 128);
            textField.selectionSprite = "EmptySprite";
            textField.selectionBackgroundColor = new Color32(0, 172, 234, 255);
            textField.normalBgSprite = "TextFieldPanelHovered";
            textField.disabledBgSprite = "TextFieldPanel";

            // Add tooltip.
            if (tooltip != null)
            {
                textField.tooltip = tooltip;
            }

            return textField;
        }


        /// <summary>
        /// Creates a plain textfield using the game's option panel checkbox template.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="text">Descriptive label text</param>
        /// <returns>New checkbox using the game's option panel template</returns>
        public static UITextField AddPlainTextfield(UIComponent parent, string text)
        {
            UIPanel textFieldPanel = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsTextfieldTemplate")) as UIPanel;

            // Set text label.
            textFieldPanel.Find<UILabel>("Label").text = text;
            return textFieldPanel.Find<UITextField>("Text Field");
        }


        /// <summary>
        /// Creates a plain checkbox using the game's option panel checkbox template.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="text">Descriptive label text</param>
        /// <returns>New checkbox using the game's option panel template</returns>
        public static UICheckBox AddPlainCheckBox(UIComponent parent, string text)
        {
            UICheckBox checkBox = parent.AttachUIComponent(UITemplateManager.GetAsGameObject("OptionsCheckBoxTemplate")) as UICheckBox;

            // Set text.
            checkBox.text = text;

            return checkBox;
        }


        /// <summary>
        /// Adds a plain text label to the specified UI panel.
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="xPos">Relative x position)</param>
        /// <param name="yPos">Relative y position</param>
        /// <param name="text">Label text</param>
        /// <param name="width">Label width (-1 (default) for autosize)</param>
        /// <param name="width">Text scale (default 1.0)</param>
        /// <returns>New text label</returns>
        public static UILabel AddLabel(UIComponent parent, float xPos, float yPos, string text, float width = -1f, float textScale = 1.0f)
        {
            // Add label.
            UILabel label = (UILabel)parent.AddUIComponent<UILabel>();

            // Set sizing options.
            if (width > 0f)
            {
                // Fixed width.
                label.autoSize = false;
                label.width = width;
                label.autoHeight = true;
                label.wordWrap = true;
            }
            else
            {
                // Autosize.
                label.autoSize = true;
                label.autoHeight = false;
                label.wordWrap = false;
            }

            // Text.
            label.textScale = textScale;
            label.text = text;

            // Position.
            label.relativePosition = new Vector2(xPos, yPos);

            return label;
        }


        /// <summary>
        /// Creates a vertical scrollbar
        /// </summary>
        /// <param name="parent">Parent component</param>
        /// <param name="scrollPanel">Panel to scroll</param>
        /// <returns>New vertical scrollbar linked to the specified scrollable panel, immediately to the right</returns>
        public static UIScrollbar AddScrollbar(UIComponent parent, UIScrollablePanel scrollPanel)
        {
            // Basic setup.
            UIScrollbar newScrollbar = parent.AddUIComponent<UIScrollbar>();
            newScrollbar.orientation = UIOrientation.Vertical;
            newScrollbar.pivot = UIPivotPoint.TopLeft;
            newScrollbar.minValue = 0;
            newScrollbar.value = 0;
            newScrollbar.incrementAmount = 50f;
            newScrollbar.autoHide = true;

            // Location and size.
            newScrollbar.width = 10f;
            newScrollbar.relativePosition = new Vector2(scrollPanel.relativePosition.x + scrollPanel.width, scrollPanel.relativePosition.y);
            newScrollbar.height = scrollPanel.height;

            // Tracking sprite.
            UISlicedSprite trackSprite = newScrollbar.AddUIComponent<UISlicedSprite>();
            trackSprite.relativePosition = Vector2.zero;
            trackSprite.autoSize = true;
            trackSprite.anchor = UIAnchorStyle.All;
            trackSprite.size = trackSprite.parent.size;
            trackSprite.fillDirection = UIFillDirection.Vertical;
            trackSprite.spriteName = "ScrollbarTrack";
            newScrollbar.trackObject = trackSprite;

            // Thumb sprite.
            UISlicedSprite thumbSprite = trackSprite.AddUIComponent<UISlicedSprite>();
            thumbSprite.relativePosition = Vector2.zero;
            thumbSprite.fillDirection = UIFillDirection.Vertical;
            thumbSprite.autoSize = true;
            thumbSprite.width = thumbSprite.parent.width;
            thumbSprite.spriteName = "ScrollbarThumb";
            newScrollbar.thumbObject = thumbSprite;

            // Event handler to handle resize of scroll panel.
            scrollPanel.eventSizeChanged += (component, newSize) =>
            {
                newScrollbar.relativePosition = new Vector2(scrollPanel.relativePosition.x + scrollPanel.width, scrollPanel.relativePosition.y);
                newScrollbar.height = scrollPanel.height;
            };

            // Attach to scroll panel.
            scrollPanel.verticalScrollbar = newScrollbar;

            return newScrollbar;
        }
    }
}