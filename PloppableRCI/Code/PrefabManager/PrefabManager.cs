// <copyright file="RICOPrefabManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;

    /// <summary>
    /// Keeps track of prefabs and provides the link between prefabs and RICO settings.
    /// </summary>
    public static class PrefabManager
    {
        /// <summary>
        /// Gets the dictionary of active prefabs.
        /// </summary>
        internal static Dictionary<BuildingInfo, BuildingData> PrefabDictionary { get; } = new Dictionary<BuildingInfo, BuildingData>();

    }
}