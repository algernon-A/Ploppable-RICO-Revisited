// <copyright file="OptionsPanel.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons.UI;

    /// <summary>
    /// The mod's settings options panel.
    /// </summary>
    public class OptionsPanel : OptionsPanelBase
    {
        /// <summary>
        /// Performs on-demand panel setup.
        /// </summary>
        protected override void Setup()
        {
            // Add tabstrip.
            AutoTabstrip tabstrip = AutoTabstrip.AddTabstrip(this, 0f, 0f, OptionsPanelManager<OptionsPanel>.PanelWidth, OptionsPanelManager<OptionsPanel>.PanelHeight, out _, tabHeight: 50f);

            // Add tabs and panels.
            new ModOptions(tabstrip, 0);
            new GrowableOptions(tabstrip, 1);
            new PloppableOptions(tabstrip, 2);
            new ComplaintOptions(tabstrip, 3);

            // Force panel refresh.
            tabstrip.selectedIndex = -1;
            tabstrip.selectedIndex = 0;
        }
    }
}