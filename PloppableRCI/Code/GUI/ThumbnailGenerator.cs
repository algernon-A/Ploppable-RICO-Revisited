// <copyright file="Thumbnails.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using AlgernonCommons;
    using ColossalFramework.UI;
    using UnityEngine;

    /// <summary>
    /// Creates thumbnail images.
    /// Inspired by Boogieman Sam's FindIt! UI.
    /// </summary>
    public class ThumbnailGenerator
    {
        // Renderer for thumbnail images.
        private readonly UIPreviewRenderer renderer;

        /// <summary>
        /// Constructor.
        /// </summary>
        public ThumbnailGenerator()
        {
            Logging.Message("creating thumbnail generator");

            // Get local reference from parent.
            renderer = ThumbnailManager.Renderer;

            // Size and setting for thumbnail images: 109 x 100, doubled for anti-aliasing.
            renderer.Size = new Vector2(109, 100) * 2f;
            renderer.CameraRotation = 210f;
        }

        /// <summary>
        /// Generates building thumbnail images (normal, focused, hovered, pressed and disabled) for the given building prefab.
        /// Thumbnails are no longer applied to the m_Thumbnail and m_Atlas fields of the prefab, but to the BuildingData record.
        /// </summary>
        /// <param name="building">RICO BuildingData record.</param>
        internal void CreateThumbnail(BuildingData building)
        {
            // Reset zoom.
            renderer.Zoom = 4f;

            // Don't do anything with null prefabs.
            if (building?.prefab == null)
            {
                return;
            }

            // Set mesh and material for render.
            if (!renderer.SetTarget(building.prefab))
            {
                // Something went wrong - this isn't a valid rendering target; exit.
                Logging.Message("no thumbnail generated for null mesh ", building.prefab.name);
                return;
            }

            // If the selected building has colour variations, temporarily set the colour to the default for rendering.
            if (building.prefab.m_useColorVariations)
            {
                Color originalColor = building.prefab.m_material.color;
                building.prefab.m_material.color = building.prefab.m_color0;
                renderer.Render(true);
                building.prefab.m_material.color = originalColor;
            }
            else
            {
                // No temporary colour change needed.
                renderer.Render(true);
            }

            // Back up game's current active texture.
            RenderTexture gameActiveTexture = RenderTexture.active;

            // Convert the render to a 2D texture.
            Texture2D thumbnailTexture = new Texture2D(renderer.Texture.width, renderer.Texture.height);
            RenderTexture.active = renderer.Texture;
            thumbnailTexture.ReadPixels(new Rect(0f, 0f, (float)renderer.Texture.width, (float)renderer.Texture.height), 0, 0);
            thumbnailTexture.Apply();

            // Temporary texture for resizing render to thumbnail size (109 x 100).
            RenderTexture resizingTexture = RenderTexture.GetTemporary(109, 100);

            // Resize 2D texture (to 109 x 100) using trilinear filtering.
            resizingTexture.filterMode = FilterMode.Trilinear;
            thumbnailTexture.filterMode = FilterMode.Trilinear;

            // Resize.
            Graphics.Blit(thumbnailTexture, resizingTexture);
            thumbnailTexture.Resize(109, 100);
            thumbnailTexture.ReadPixels(new Rect(0, 0, 109, 100), 0, 0);
            thumbnailTexture.Apply();

            // Release temporary texture.
            RenderTexture.ReleaseTemporary(resizingTexture);

            // Restore game's current active texture.
            RenderTexture.active = gameActiveTexture;

            // Thumbnail texture name is the same as the building's displayed name.
            thumbnailTexture.name = building.DisplayName;

            // Create new texture atlas with thumnails.
            UITextureAtlas thumbnailAtlas = ScriptableObject.CreateInstance<UITextureAtlas>();
            thumbnailAtlas.name = "RICOThumbnails_" + building.DisplayName;
            thumbnailAtlas.material = UnityEngine.Object.Instantiate<Material>(UIView.GetAView().defaultAtlas.material);
            thumbnailAtlas.material.mainTexture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            AddTexturesToAtlas(thumbnailAtlas, GenerateThumbnailVariants(thumbnailTexture));

            // Add atlas to our building data record.
            building.thumbnailAtlas = thumbnailAtlas;
        }

        /// <summary>
        /// Generates building thumbnail variants - focused, hovered, pressed, and disabled., 
        /// </summary>
        /// <param name="baseTexture">Base texture of the thumbnail.</param>
        /// <returns>2d variant icon textures.</returns>
        private Texture2D[] GenerateThumbnailVariants(Texture2D baseTexture)
        {
            Color32[] variantPixels = new Color32[baseTexture.width * baseTexture.height];
            Color32[] basePixels = baseTexture.GetPixels32();

            // Focused.
            ColorFilter(basePixels, variantPixels, 32, 64, 128, 2);
            Texture2D focusedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            focusedTexture.SetPixels32(variantPixels);
            focusedTexture.Apply(false);
            focusedTexture.name = baseTexture.name + "Focused";

            // Hovered.
            ColorFilter(basePixels, variantPixels, 128, 128, 128, 1);
            Texture2D hoveredTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            hoveredTexture.SetPixels32(variantPixels);
            hoveredTexture.Apply(false);
            hoveredTexture.name = baseTexture.name + "Hovered";

            // Pressed.
            ColorFilter(basePixels, variantPixels, 128, 128, 128, 2);
            Texture2D pressedTexture = new Texture2D(baseTexture.width, baseTexture.height, TextureFormat.ARGB32, false, false);
            pressedTexture.SetPixels32(variantPixels);
            pressedTexture.Apply(false);
            pressedTexture.name = baseTexture.name + "Pressed";

            // Don't bother with 'disabled' texture since we don't use it, and we save memory by not adding it.

            return new Texture2D[]
            {
                baseTexture,
                focusedTexture,
                hoveredTexture,
                pressedTexture
            };
        }

        /// <summary>
        /// Applies an RGB filter to a source colour, optionally reducing the intensity of the source colour before filtering (alpha is left unchanged).
        /// </summary>
        /// <param name="sourceColor">Source colour to filter.</param>
        /// <param name="resultColor">Result of filtering.</param>
        /// <param name="filterR">Red component of filter.</param>
        /// <param name="filterG">Green component of filter.</param>
        /// <param name="filterB">Blue component of filter.</param>
        /// <param name="filterStrength">Each channel (RGB) of the original colour is bitshifted right this number before filtering (to reduce its intensity).</param>
        private void ColorFilter(Color32[] sourceColor, Color32[] resultColor, byte filterR, byte filterG, byte filterB, byte filterStrength)
        {
            for (int i = 0; i < sourceColor.Length; i++)
            {
                // Rightshift the source channel by the required amount before adding the relevant filter channel.
                resultColor[i].r = (byte)((sourceColor[i].r >> filterStrength) + filterR);
                resultColor[i].g = (byte)((sourceColor[i].g >> filterStrength) + filterG);
                resultColor[i].b = (byte)((sourceColor[i].b >> filterStrength) + filterB);
                resultColor[i].a = sourceColor[i].a;
            }
        }

        /// <summary>
        /// Adds a collection of textures to an atlas.
        /// </summary>
        /// <param name="atlas">Atlas to add to.</param>
        /// <param name="newTextures">Textures to add.</param>
        private void AddTexturesToAtlas(UITextureAtlas atlas, Texture2D[] newTextures)
        {
            Texture2D[] textures = new Texture2D[atlas.count + newTextures.Length];

            // Populate textures with sprites from the atlas.
            for (int i = 0; i < atlas.count; i++)
            {
                textures[i] = atlas.sprites[i].texture;
                textures[i].name = atlas.sprites[i].name;
            }

            // Append new textures to our list.
            for (int i = 0; i < newTextures.Length; i++)
            {
                textures[atlas.count + i] = newTextures[i];
            }

            // Repack atlas with our new additions (regions are individual texture areas within the atlas).
            Rect[] regions = atlas.texture.PackTextures(textures, atlas.padding, 4096, false);

            // Clear original atlas sprites.
            atlas.sprites.Clear();

            // Iterate through our list, adding each sprite into the atlas.
            for (int i = 0; i < textures.Length; i++)
            {
                UITextureAtlas.SpriteInfo spriteInfo = atlas[textures[i].name];
                atlas.sprites.Add(new UITextureAtlas.SpriteInfo
                {
                    texture = textures[i],
                    name = textures[i].name,
                    border = (spriteInfo != null) ? spriteInfo.border : new RectOffset(),
                    region = regions[i]
                });
            }

            // Rebuild atlas indexes.
            atlas.RebuildIndexes();
        }
    }
}