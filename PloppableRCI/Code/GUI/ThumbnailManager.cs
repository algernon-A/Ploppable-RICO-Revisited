// <copyright file="ThumbnailManager.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System;
    using AlgernonCommons;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Static class to coordinate thumbnail generation.
    /// </summary>
    internal static class ThumbnailManager
    {
        // Instances.
        private static GameObject s_gameObject;
        private static ThumbnailGenerator s_generator;
        private static PreviewRenderer s_renderer;

        /// <summary>
        /// Gets the renderer instance.
        /// </summary>
        internal static PreviewRenderer Renderer => s_renderer;

        /// <summary>
        /// Forces immediate rendering of a thumbnail.
        /// </summary>
        /// <param name="buildingData">RICO building data record.</param>
        internal static void CreateThumbnail(BuildingData buildingData)
        {
            if (s_gameObject == null)
            {
                Create();
            }

            s_generator.CreateThumbnail(buildingData);
        }

        /// <summary>
        /// Creates our renderer GameObject.
        /// </summary>
        internal static void Create()
        {
            try
            {
                // If no instance already set, create one.
                if (s_gameObject == null)
                {
                    // Give it a unique name for easy finding with ModTools.
                    s_gameObject = new GameObject("RICOThumbnailRenderer");
                    s_gameObject.transform.parent = UIView.GetAView().transform;

                    // Add our queue manager and renderer directly to the gameobject.
                    s_renderer = s_gameObject.AddComponent<PreviewRenderer>();
                    s_generator = new ThumbnailGenerator();
                }
            }
            catch (Exception e)
            {
                Logging.LogException(e, "exception creating renderer");
            }
        }

        /// <summary>
        /// Cleans up when finished.
        /// </summary>
        internal static void Close()
        {
            if (s_gameObject != null)
            {
                // Destroy gameobject components.
                GameObject.Destroy(s_renderer);
                GameObject.Destroy(s_gameObject);

                // Let the garbage collector cleanup.
                s_generator = null;
                s_renderer = null;
                s_gameObject = null;
            }
        }
    }
}