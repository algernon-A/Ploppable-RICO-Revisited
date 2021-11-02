using HarmonyLib;


namespace PloppableRICO
{
    /// <summary>
    /// Harmony patch to fix game bug where building incomes (tax revenue) can sometimes be negative for larger buildings.
    /// </summary>
    [HarmonyPatch(typeof(EconomyWrapper), nameof(EconomyWrapper.OnAddResource))]
    public static class EconomyWrapperPatch
    {
        /// <summary>
        /// Harmony Postfix patch to EcomonyWrapper.OnAddResource to fix any negative income 
        /// </summary>
        /// <param name="resource"></param>
        /// <param name="amount"></param>
        public static void Postfix(EconomyManager.Resource resource,  ref int amount, ItemClass.SubService subService)
        {
            if (resource == EconomyManager.Resource.PrivateIncome && amount < 0)
            {
                Logging.Message("fixed private building negative income of ", amount.ToString(), " for building with subService ", subService.ToString());
                amount = -amount;
            }
        }
    }
}
