namespace PloppableRICO
{
    /// <summary>
    /// A simple UI panel to show any mod conflict notifications.
    /// </summary>
    class HarmonyNotification : ErrorNotification
    {
        /// <summary>
        /// Creates the panel object in-game.
        /// </summary>
        public override void Create()
        {
            base.Create();
            instance = uiGameObject.AddComponent<HarmonyNotification>();
        }


        /// <summary>
        /// Sets up the panel; called by Unity just before any of the Update methods is called for the first time..
        /// </summary>
        public override void Start()
        {
            headerText = "PRR_ERR_HAR0";
            messageText = Translations.Translate("PRR_ERR_HAR1") + "\r\n\r\n" + Translations.Translate("PRR_ERR_HAR2");
            listText = Translations.Translate("PRR_ERR_HAR3") + "\r\n\r\n" + Translations.Translate("PRR_ERR_HAR4");

            base.Start();
        }
    }
}