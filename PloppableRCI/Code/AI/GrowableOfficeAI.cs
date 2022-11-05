// <copyright file="GrowableOfficeAI.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using ColossalFramework.Math;
    using static WorkplaceAIHelper;

    /// <summary>
    /// Replacement for Office AI for growable RICO buildings.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Consistency with game member style")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1307:Accessible fields should begin with upper-case letter", Justification = "Consistency with game member style")]
    public class GrowableOfficeAI : OfficeBuildingAI, IWorkplaceLevelCalculator
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
        /// Number of workplaces in this building.
        /// Sets a reasonable default, but will be overwritten by ConvertPrefabs().
        /// </summary>
        public int m_workplaceCount = 1;

        /// <summary>
        /// Calculates the workplaces for this building according to RICO settings.
        /// </summary>
        /// <param name="level">Building level.</param>
        /// <param name="r">Randomizer.</param>
        /// <param name="width">Building plot width (in cells).</param>
        /// <param name="length">Building plot length (in cells).</param>
        /// <param name="level0">The number of uneducated jobs.</param>
        /// <param name="level1">The number of educated jobs.</param>
        /// <param name="level2">The number of well-educated jobs.</param>
        /// <param name="level3">The number of highly-educated jobs.</param>
        public override void CalculateWorkplaceCount(ItemClass.Level level, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3) =>
            WorkplaceAIHelper.CalculateWorkplaceCount(level, m_ricoData, this, r, width, length, out level0, out level1, out level2, out level3);

        /// <summary>
        /// Calculates the workplaces for this building according to base method (non-RICO settings).
        /// Called by WorkPlaceAIHelper to access the base game method; for implementing functionality of mods that have detoured/patched that method (e.g. Realistic Population mods).
        /// </summary>
        /// <param name="level">Building level.</param>
        /// <param name="r">Randomizer.</param>
        /// <param name="width">Building plot width (in cells).</param>
        /// <param name="length">Building plot length (in cells).</param>
        /// <param name="level0">The number of uneducated jobs.</param>
        /// <param name="level1">The number of educated jobs.</param>
        /// <param name="level2">The number of well-educated jobs.</param>
        /// <param name="level3">The number of highly-educated jobs.</param>
        public void CalculateBaseWorkplaceCount(ItemClass.Level level, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3)
        {
            base.CalculateWorkplaceCount(level, r, width, length, out level0, out level1, out level2, out level3);
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