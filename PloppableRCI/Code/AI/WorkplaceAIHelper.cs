// <copyright file="WorkplaceAIHelper.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using ColossalFramework.Math;

    /// <summary>
    /// Workplace AI helper utilities.
    /// </summary>
    internal static class WorkplaceAIHelper
    {
        /// <summary>
        /// Workplace level calculation interface.
        /// </summary>
        internal interface IWorkplaceLevelCalculator
        {
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
            void CalculateBaseWorkplaceCount(ItemClass.Level level, Randomizer r, int width, int length, out int level0, out int level1, out int level2, out int level3);
        }

        /// <summary>
        /// Calculates the workplaces for this building according to RICO settings.
        /// </summary>
        /// <param name="level">Building level.</param>
        /// <param name="ricoData">RICO building data.</param>
        /// <param name="workplaceLevelCalculator">Workplace level calculator to use.</param>
        /// <param name="r">Randomizer.</param>
        /// <param name="width">Building plot width (in cells).</param>
        /// <param name="length">Building plot length (in cells).</param>
        /// <param name="level0">The number of uneducated jobs.</param>
        /// <param name="level1">The number of educated jobs.</param>
        /// <param name="level2">The number of well-educated jobs.</param>
        /// <param name="level3">The number of highly-educated jobs.</param>
        internal static void CalculateWorkplaceCount(
            ItemClass.Level level,
            RICOBuilding ricoData,
            IWorkplaceLevelCalculator workplaceLevelCalculator,
            Randomizer r,
            int width,
            int length,
            out int level0,
            out int level1,
            out int level2,
            out int level3)
        {
            // If we've got valid rico data for this building, use those settings.
            if (ricoData != null)
            {
                // Are we using realistic population calculations?
                if (ricoData.UseReality)
                {
                    // Yes - use the realistic population calculations (by calling base method, which will be patched by real pop mod).
                    workplaceLevelCalculator.CalculateBaseWorkplaceCount(level, r, width, length, out level0, out level1, out level2, out level3);
                }
                else
                {
                    // No - use RICO settings.
                    level0 = ricoData.Workplaces[0];
                    level1 = ricoData.Workplaces[1];
                    level2 = ricoData.Workplaces[2];
                    level3 = ricoData.Workplaces[3];
                }
            }
            else
            {
                // No valid RICO data - set all workplaces to zero.
                level0 = 0;
                level1 = 0;
                level2 = 0;
                level3 = 0;
            }
        }

        /// <summary>
        /// Distributes total workplaces across education levels.
        /// </summary>
        /// <param name="workplaces">Total workplaces.</param>
        /// <param name="workplaceDistribution">Workplace distribution ratios.</param>
        /// <returns>Workplace distribution array (workplaces per education level).</returns>
        internal static int[] DistributeWorkplaceLevels(int workplaces, int[] workplaceDistribution)
        {
            int[] distributedJobs = new int[] { 0, 0, 0, 0 };

            // Null check.
            if (workplaceDistribution != null)
            {
                // Allocate jobs according to distribution.  Division after multiplication to reduce intermediate rounding errors.
                distributedJobs[1] = (workplaces * workplaceDistribution[1]) / workplaceDistribution[0];
                distributedJobs[2] = (workplaces * workplaceDistribution[2]) / workplaceDistribution[0];
                distributedJobs[3] = (workplaces * workplaceDistribution[3]) / workplaceDistribution[0];

                // Level 0 is the remainder.
                distributedJobs[0] = workplaces - distributedJobs[1] - distributedJobs[2] - distributedJobs[3];
            }

            return distributedJobs;
        }
    }
}
