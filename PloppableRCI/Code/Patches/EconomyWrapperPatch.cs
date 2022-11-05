// <copyright file="EconomyWrapperPatch.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons;
    using HarmonyLib;

    /// <summary>
    /// Harmony patch to fix game bug where building incomes (tax revenue) can sometimes be negative for larger buildings.
    /// </summary>
    [HarmonyPatch(typeof(EconomyWrapper), nameof(EconomyWrapper.OnAddResource))]
    public static class EconomyWrapperPatch
    {
        /// <summary>
        /// Harmony Postfix patch to EcomonyWrapper.OnAddResource to fix any negative income.
        /// </summary>
        /// <param name="resource">Resource being added.</param>
        /// <param name="amount">Amount being added.</param>
        /// <param name="subService">Building SubService.</param>
        public static void Postfix(EconomyManager.Resource resource,  ref int amount, ItemClass.SubService subService)
        {
            if (resource == EconomyManager.Resource.PrivateIncome && amount < 0)
            {
                Logging.Message("fixed private building negative income of ", amount, " for building with subService ", subService);
                amount = -amount;
            }
        }
    }
}
