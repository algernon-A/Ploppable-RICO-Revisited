// <copyright file="WhatsNewMessageListing.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the Apache license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using AlgernonCommons.Notifications;

    /// <summary>
    /// "What's new" update messages.
    /// </summary>
    internal class WhatsNewMessageListing
    {
        /// <summary>
        /// Gets the list of versions and associated update message lines (as translation keys).
        /// </summary>
        internal WhatsNewMessage[] Messages => new WhatsNewMessage[]
        {
            new WhatsNewMessage
            {
                Version = new Version("2.5"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_25_0"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.4.3.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_243_0"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.4.2.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_242_0"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.4.1.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_241_0"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.4.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_240_0",
                    "PRR_UPD_240_1",
                    "PRR_UPD_240_2",
                    "PRR_UPD_240_3"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.3.7.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_237_0"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.3.6.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_236_0"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.3.5.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_235_0",
                    "PRR_UPD_235_1",
                    "PRR_UPD_235_2"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.3.4.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_234_0",
                    "PRR_UPD_234_1",
                    "PRR_UPD_234_2"
                }
            },
            new WhatsNewMessage
            {
                Version = new Version("2.3.0"),
                MessagesAreKeys = true,
                Messages = new string[]
                {
                    "PRR_UPD_23_2",
                    "PRR_UPD_23_3",
                    "PRR_UPD_23_4",
                    "PRR_UPD_23_5"
                }
            }
        };
    }
}