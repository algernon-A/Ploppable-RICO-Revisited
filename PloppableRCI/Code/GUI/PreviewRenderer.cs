// <copyright file="PreviewRenderer.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;
    using ColossalFramework;
    using UnityEngine;

    /// <summary>
    /// Render a 3d image of a given mesh.
    /// </summary>
    internal class PreviewRenderer : MonoBehaviour
    {
        // Rendering settings.
        private readonly Camera _renderCamera;
        private Mesh _currentMesh;
        private Bounds _currentBounds;
        private float _currentRotation;
        private float _currentZoom;
        private Material _material;

        // Rendering sub-components.
        private List<BuildingInfo.MeshInfo> _subMeshes;
        private List<BuildingInfo.SubInfo> _subBuildings;

        /// <summary>
        /// Initializes a new instance of the <see cref="PreviewRenderer"/> class.
        /// </summary>
        internal PreviewRenderer()
        {
            // Set up camera.
            _renderCamera = new GameObject("Camera").AddComponent<Camera>();
            _renderCamera.transform.SetParent(transform);
            _renderCamera.targetTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            _renderCamera.allowHDR = true;
            _renderCamera.enabled = false;

            // Basic defaults.
            _renderCamera.pixelRect = new Rect(0f, 0f, 512, 512);
            _renderCamera.backgroundColor = new Color(0, 0, 0, 0);
            _renderCamera.fieldOfView = 30f;
            _renderCamera.nearClipPlane = 1f;
            _renderCamera.farClipPlane = 1000f;
        }

        /// <summary>
        /// Sets material to render.
        /// </summary>
        internal Material Material { set => _material = value; }

        /// <summary>
        /// Gets or sets the preview image size.
        /// </summary>
        internal Vector2 Size
        {
            get => new Vector2(_renderCamera.targetTexture.width, _renderCamera.targetTexture.height);

            set
            {
                if (Size != value)
                {
                    // New size; set camera output sizes accordingly.
                    _renderCamera.targetTexture = new RenderTexture((int)value.x, (int)value.y, 24, RenderTextureFormat.ARGB32);
                    _renderCamera.pixelRect = new Rect(0f, 0f, value.x, value.y);
                }
            }
        }

        /// <summary>
        /// Gets or sets the currently rendered mesh.
        /// </summary>
        internal Mesh Mesh
        {
            get => _currentMesh;

            set => _currentMesh = value;
        }

        /// <summary>
        /// Gets the current building texture.
        /// </summary>
        internal RenderTexture Texture => _renderCamera.targetTexture;

        /// <summary>
        /// Gets or sets the preview camera rotation (in degrees).
        /// </summary>
        internal float CameraRotation
        {
            get => _currentRotation;
            set => _currentRotation = value % 360f;
        }

        /// <summary>
        /// Gets or sets the preview zoom level.
        /// </summary>
        internal float Zoom
        {
            get => _currentZoom;

            set
            {
                _currentZoom = Mathf.Clamp(value, 0.5f, 5f);
            }
        }

        /// <summary>
        /// Sets mesh and material from a BuildingInfo prefab.
        /// </summary>
        /// <param name="prefab">Prefab to render.</param>
        /// <returns>True if the target was valid (prefab or at least one subbuilding contains a valid material, and the prefab has at least one primary mesh, submesh, or subbuilding).</returns>
        internal bool SetTarget(BuildingInfo prefab)
        {
            // Assign main mesh and material.
            Mesh = prefab.m_mesh;
            _material = prefab.m_material;

            // Set up or clear submesh list.
            if (_subMeshes == null)
            {
                _subMeshes = new List<BuildingInfo.MeshInfo>();
            }
            else
            {
                _subMeshes.Clear();
            }

            // Add any submeshes to our submesh list.
            if (prefab.m_subMeshes != null && prefab.m_subMeshes.Length > 0)
            {
                for (int i = 0; i < prefab.m_subMeshes.Length; i++)
                {
                    _subMeshes.Add(prefab.m_subMeshes[i]);
                }
            }

            // Set up or clear sub-building list.
            if (_subBuildings == null)
            {
                _subBuildings = new List<BuildingInfo.SubInfo>();
            }
            else
            {
                _subBuildings.Clear();
            }

            if (prefab.m_subBuildings != null && prefab.m_subBuildings.Length > 0)
            {
                for (int i = 0; i < prefab.m_subBuildings.Length; i++)
                {
                    _subBuildings.Add(prefab.m_subBuildings[i]);

                    // If we don't already have a valid material, grab this one.
                    if (_material == null)
                    {
                        _material = prefab.m_subBuildings[i].m_buildingInfo.m_material;
                    }
                }
            }

            return _material != null && (_currentMesh != null || _subBuildings.Count > 0 || _subMeshes.Count > 0);
        }

        /// <summary>
        /// Render the current mesh.
        /// </summary>
        /// <param name="isThumb">True if this is a thumbnail render, false otherwise.</param>
        internal void Render(bool isThumb)
        {
            // Check to see if we have submeshes or sub-buildings.
            bool hasSubMeshes = _subMeshes != null && _subMeshes.Count > 0;
            bool hasSubBuildings = _subBuildings != null && _subBuildings.Count > 0;

            // If no primary mesh and no other meshes, don't do anything here.
            if (_currentMesh == null && !hasSubBuildings && !hasSubMeshes)
            {
                return;
            }

            // Set background - plain if this is a thumbnail and the 'skybox' option isn't selected.
            if (isThumb && ModSettings.thumbBacks != (byte)ModSettings.ThumbBackCats.Skybox)
            {
                // Is a thumbnail - user plain-colour background.
                _renderCamera.clearFlags = CameraClearFlags.Color;

                // Set dark sky-blue background colour if the default 'color' background is set
                if (ModSettings.thumbBacks == (byte)ModSettings.ThumbBackCats.Color)
                {
                    _renderCamera.backgroundColor = new Color32(33, 151, 199, 255);
                }
            }
            else
            {
                // Not a thumbnail - use skybox background.
                _renderCamera.clearFlags = CameraClearFlags.Skybox;
            }

            // Back up current game InfoManager mode.
            InfoManager infoManager = Singleton<InfoManager>.instance;
            InfoManager.InfoMode currentMode = infoManager.CurrentMode;
            InfoManager.SubInfoMode currentSubMode = infoManager.CurrentSubMode;

            // Set current game InfoManager to default (don't want to render with an overlay mode).
            infoManager.SetCurrentMode(InfoManager.InfoMode.None, InfoManager.SubInfoMode.Default);
            infoManager.UpdateInfoMode();

            // Backup current exposure and sky tint.
            float gameExposure = DayNightProperties.instance.m_Exposure;
            Color gameSkyTint = DayNightProperties.instance.m_SkyTint;

            // Backup current game lighting.
            Light gameMainLight = RenderManager.instance.MainLight;

            // Set exposure and sky tint for render.
            DayNightProperties.instance.m_Exposure = 0.5f;
            DayNightProperties.instance.m_SkyTint = new Color(0, 0, 0);
            DayNightProperties.instance.Refresh();

            // Set up our render lighting settings.
            Light renderLight = DayNightProperties.instance.sunLightSource;
            RenderManager.instance.MainLight = renderLight;

            // Reset the bounding box to be the smallest that can encapsulate all verticies of the new mesh.
            // That way the preview image is the largest size that fits cleanly inside the preview size.
            _currentBounds = new Bounds(Vector3.zero, Vector3.zero);
            Vector3[] vertices;

            // Set default model position.
            Vector3 modelPosition = new Vector3(0f, 0f, 0f);

            // Add our main mesh, if any (some are null, because they only 'appear' through subbuildings - e.g. Boston Residence Garage).
            if (_currentMesh != null && _material != null)
            {
                // Use separate verticies instance instead of accessing Mesh.vertices each time (which is slow).
                // >10x measured performance improvement by doing things this way instead.
                vertices = _currentMesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                    if (vertices[i].y > -2)
                    {
                        _currentBounds.Encapsulate(vertices[i]);
                    }
                }

                // Calculate rendering matrix and add mesh to scene.
                Matrix4x4 matrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(Vector3.zero), Vector3.one);
                Graphics.DrawMesh(_currentMesh, matrix, _material, 0, _renderCamera, 0, null, true, true);
            }

            // Render submeshes, if any.
            if (hasSubMeshes)
            {
                foreach (BuildingInfo.MeshInfo subMesh in _subMeshes)
                {
                    // Get local reference.
                    BuildingInfoBase subInfo = subMesh?.m_subInfo;

                    // Just in case.
                    if (subInfo?.m_mesh != null && subInfo?.m_material != null)
                    {
                        // Recalculate our matrix based on our submesh position and rotation.

                        // Calculate the relative rotation.
                        // We need to rotate the submesh before we apply the model rotation.
                        // Note that the order of multiplication (relative to the angle of operation) is reversed in the code, because of the way Unity overloads the multiplication operator.
                        // Note also that the submesh angle needs to be inverted to rotate correctly around the Y axis in our space.
                        Quaternion relativeRotation = Quaternion.AngleAxis(subMesh.m_angle * -1, Vector3.up);

                        // Calculate relative position of mesh given its starting position and our model rotation.
                        Vector3 relativePosition = subMesh.m_position;

                        // Put it all together into our rendering matrix.
                        Matrix4x4 matrix = Matrix4x4.TRS(relativePosition + modelPosition, relativeRotation, Vector3.one);

                        // Add submesh to scene.
                        Graphics.DrawMesh(subInfo.m_mesh, matrix, subInfo.m_material, 0, _renderCamera, 0, null, true, true);

                        // Expand our bounds to encapsulate the submesh.
                        vertices = subInfo.m_mesh.vertices;
                        for (int i = 0; i < vertices.Length; i++)
                        {
                            // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                            if (vertices[i].y + relativePosition.y > -2)
                            {
                                // Transform coordinates to our model rotation before encapsulating, otherwise we tend to cut off corners.
                                _currentBounds.Encapsulate(relativeRotation * (vertices[i] + subMesh.m_position));
                            }
                        }
                    }
                }
            }

            // Render subbuildings, if any.
            if (hasSubBuildings)
            {
                foreach (BuildingInfo.SubInfo subBuilding in _subBuildings)
                {
                    // Get local reference.
                    BuildingInfo subInfo = subBuilding?.m_buildingInfo;

                    // Just in case.
                    if (subInfo?.m_mesh != null && subInfo?.m_material != null)
                    {
                        // Calculate the relative rotation.
                        // We need to rotate the subbuilding before we apply the model rotation.
                        // Note that the order of multiplication (relative to the angle of operation) is reversed in the code, because of the way Unity overloads the multiplication operator.
                        Quaternion relativeRotation = Quaternion.AngleAxis(subBuilding.m_angle, Vector3.up);

                        // Recalculate our matrix based on our submesh position.
                        Vector3 relativePosition = subBuilding.m_position;
                        Matrix4x4 matrix = Matrix4x4.TRS(relativePosition + modelPosition, relativeRotation, Vector3.one);

                        // Add subbuilding to scene.
                        Graphics.DrawMesh(subInfo.m_mesh, matrix, subInfo.m_material, 0, _renderCamera, 0, null, true, true);

                        // Expand our bounds to encapsulate the submesh.
                        vertices = subInfo.m_mesh.vertices;
                        for (int i = 0; i < vertices.Length; i++)
                        {
                            // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                            if (vertices[i].y + relativePosition.y > -2)
                            {
                                _currentBounds.Encapsulate(vertices[i] + relativePosition);
                            }
                        }
                    }
                }
            }

            // Set zoom to encapsulate entire model.
            float magnitude = _currentBounds.extents.magnitude;
            float clipExtent = (magnitude + 16f) * 1.5f;
            float clipCenter = magnitude * _currentZoom;

            // Clip planes.
            _renderCamera.nearClipPlane = Mathf.Max(clipCenter - clipExtent, 0.01f);
            _renderCamera.farClipPlane = clipCenter + clipExtent;

            // Camera position and rotation - directly behind the model, facing forward.
            _renderCamera.transform.position = (-Vector3.forward * clipCenter) + _currentBounds.center;
            _renderCamera.transform.RotateAround(_currentBounds.center, Vector3.right, 20f);
            _renderCamera.transform.RotateAround(_currentBounds.center, Vector3.up, -_currentRotation);
            _renderCamera.transform.LookAt(_currentBounds.center);

            // If game is currently in nighttime, enable sun and disable moon lighting.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = true;
                DayNightProperties.instance.moonLightSource.enabled = false;
            }

            // Light settings.
            renderLight.transform.eulerAngles = new Vector3(55f, -_currentRotation - 20f, 0f);
            renderLight.intensity = 2f;
            renderLight.color = Color.white;

            // Render!
            _renderCamera.RenderWithShader(_material.shader, string.Empty);

            // Restore game lighting.
            RenderManager.instance.MainLight = gameMainLight;

            // Reset to moon lighting if the game is currently in nighttime.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = false;
                DayNightProperties.instance.moonLightSource.enabled = true;
            }

            // Restore game exposure and sky tint.
            DayNightProperties.instance.m_Exposure = gameExposure;
            DayNightProperties.instance.m_SkyTint = gameSkyTint;
            DayNightProperties.instance.Refresh();

            // Restore game InfoManager mode.
            infoManager.SetCurrentMode(currentMode, currentSubMode);
            infoManager.UpdateInfoMode();
        }
    }
}