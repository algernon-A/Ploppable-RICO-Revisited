// <copyright file="GrowableResidentialAI.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using ColossalFramework.Math;

    /// <summary>
    /// Replacement for Residential AI for growable RICO buildings.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Consistency with game member style")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Consistency with game member style")]
    public class GrowableResidentialAI : ResidentialBuildingAI
    {
        /// <summary>
        /// RICO data record.
        /// </summary>
        public RICOBuilding m_ricoData;

        /// <summary>
        /// Construction cost of this building.  Ignored for growables - having it here saves having extra checks in ConvertPrefabs().
        /// Sets a reasonable default, but will be overwritten by ConvertPrefabs() for ploppables.
        /// </summary>
        public int m_constructionCost = 10;

        /// <summary>
        /// Number of households in this building.
        /// Sets a reasonable default, but will be overwritten by ConvertPrefabs().
        /// </summary>
        public int m_homeCount = 1;

        /// <summary>
        /// Calculates the household count.
        /// </summary>
        /// <param name="level">Building level.</param>
        /// <param name="r">Randomizer.</param>
        /// <param name="width">Building plot width (in cells).</param>
        /// <param name="length">Building plot length (in cells).</param>
        /// <returns>The household count for the building.</returns>
        public override int CalculateHomeCount(ItemClass.Level level, Randomizer r, int width, int length)
        {
            // If we're using a Realistic Population mod, then just use the base method (will be patched by the mod).
            if (m_ricoData.UseReality)
            {
                return base.CalculateHomeCount(level, r, width, length);
            }

            // Otherwise, return the RICO homeCount.
            return m_homeCount;
        }

        /// <summary>
        /// Check to see if this building is unlocked (by progression level or other prerequisites).
        /// RICO buildings are always unlocked.
        /// </summary>
        /// <returns>Whether the building is currently unlocked (always true).</returns>
        public override bool CheckUnlocking()
        {
            return true;
        }
    }
}