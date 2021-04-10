using System.Collections.Generic;
using UnityEngine;
using ColossalFramework;


namespace PloppableRICO
{
    /// <summary>
    /// Render a 3d image of a given mesh.
    /// </summary>
    public class UIPreviewRenderer : MonoBehaviour
    {
        // Rendering settings.
        private readonly Camera renderCamera;
        private Mesh currentMesh;
        private Bounds currentBounds;
        private float currentRotation;
        private float currentZoom;
        private Material _material;

        // Rendering sub-components.
        private List<BuildingInfo.MeshInfo> subMeshes;
        private List<BuildingInfo.SubInfo> subBuildings;


        /// <summary>
        /// Sets material to render.
        /// </summary>
        public Material Material { set => _material = value; }


        /// <summary>
        /// Initialise the new renderer object.
        /// </summary>
        public UIPreviewRenderer()
        {
            // Set up camera.
            renderCamera = new GameObject("Camera").AddComponent<Camera>();
            renderCamera.transform.SetParent(transform);
            renderCamera.targetTexture = new RenderTexture(512, 512, 24, RenderTextureFormat.ARGB32);
            renderCamera.allowHDR = true;
            renderCamera.enabled = false;

            // Basic defaults.
            renderCamera.pixelRect = new Rect(0f, 0f, 512, 512);
            renderCamera.backgroundColor = new Color(0, 0, 0, 0);
            renderCamera.fieldOfView = 30f;
            renderCamera.nearClipPlane = 1f;
            renderCamera.farClipPlane = 1000f;
        }


        /// <summary>
        /// Image size.
        /// </summary>
        public Vector2 Size
        {
            get => new Vector2(renderCamera.targetTexture.width, renderCamera.targetTexture.height);

            set
            {
                if (Size != value)
                {
                    // New size; set camera output sizes accordingly.
                    renderCamera.targetTexture = new RenderTexture((int)value.x, (int)value.y, 24, RenderTextureFormat.ARGB32);
                    renderCamera.pixelRect = new Rect(0f, 0f, value.x, value.y);
                }
            }
        }

        /// <summary>
        /// Sets mesh and material from a BuildingInfo prefab.
        /// </summary>
        /// <param name="prefab">Prefab to render</param>
        /// <returns>True if the target was valid (prefab or at least one subbuilding contains a valid material, and the prefab has at least one primary mesh, submesh, or subbuilding)</returns>
        public bool SetTarget(BuildingInfo prefab)
        {
            // Assign main mesh and material.
            Mesh = prefab.m_mesh;
            _material = prefab.m_material;

            // Set up or clear submesh list.
            if (subMeshes == null)
            {
                subMeshes = new List<BuildingInfo.MeshInfo>();
            }
            else
            {
                subMeshes.Clear();
            }

            // Add any submeshes to our submesh list.
            if (prefab.m_subMeshes != null && prefab.m_subMeshes.Length > 0)
            {
                for (int i = 0; i < prefab.m_subMeshes.Length; i++)
                {
                    subMeshes.Add(prefab.m_subMeshes[i]);
                }
            }

            // Set up or clear sub-building list.
            if (subBuildings == null)
            {
                subBuildings = new List<BuildingInfo.SubInfo>();
            }
            else
            {
                subBuildings.Clear();
            }

            if (prefab.m_subBuildings != null && prefab.m_subBuildings.Length > 0)
            {
                for (int i = 0; i < prefab.m_subBuildings.Length; i++)
                {
                    subBuildings.Add(prefab.m_subBuildings[i]);

                    // If we don't already have a valid material, grab this one.
                    if (_material == null)
                    {
                        _material = prefab.m_subBuildings[i].m_buildingInfo.m_material;
                    }
                }
            }

            return _material != null && (currentMesh != null || subBuildings.Count > 0 || subMeshes.Count > 0);
        }


        /// <summary>
        /// Currently rendered mesh.
        /// </summary>
        public Mesh Mesh
        {
            get => currentMesh;

            set => currentMesh = value;
        }


        /// <summary>
        /// Current building texture.
        /// </summary>
        public RenderTexture Texture
        {
            get => renderCamera.targetTexture;
        }


        /// <summary>
        /// Preview camera rotation (degrees).
        /// </summary>
        public float CameraRotation
        {
            get { return currentRotation; }
            set { currentRotation = value % 360f; }
        }


        /// <summary>
        /// Zoom level.
        /// </summary>
        public float Zoom
        {
            get { return currentZoom; }
            set
            {
                currentZoom = Mathf.Clamp(value, 0.5f, 5f);
            }
        }


        /// <summary>
        /// Render the current mesh.
        /// </summary>
        /// <param name="isThumb">True if this is a thumbnail render, false otherwise</param>
        public void Render(bool isThumb)
        {
            // Check to see if we have submeshes or sub-buildings.
            bool hasSubMeshes = subMeshes != null && subMeshes.Count > 0;
            bool hasSubBuildings = subBuildings != null && subBuildings.Count > 0;

            // If no primary mesh and no other meshes, don't do anything here.
            if (currentMesh == null && !hasSubBuildings && !hasSubMeshes)
            {
                return;
            }

            // Set background - plain if this is a thumbnail and the 'skybox' option isn't selected.
            if (isThumb && ModSettings.thumbBacks != (byte)ModSettings.ThumbBackCats.skybox)
            {
                // Is a thumbnail - user plain-colour background.
                renderCamera.clearFlags = CameraClearFlags.Color;

                // Set dark sky-blue background colour if the default 'color' background is set
                if (ModSettings.thumbBacks == (byte)ModSettings.ThumbBackCats.color)
                {
                    renderCamera.backgroundColor = new Color32(33, 151, 199, 255);
                }
            }
            else
            {
                // Not a thumbnail - use skybox background.
                renderCamera.clearFlags = CameraClearFlags.Skybox;
            }

            // Back up current game InfoManager mode.
            InfoManager infoManager = Singleton<InfoManager>.instance;
            InfoManager.InfoMode currentMode = infoManager.CurrentMode;
            InfoManager.SubInfoMode currentSubMode = infoManager.CurrentSubMode; ;

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
            currentBounds = new Bounds(Vector3.zero, Vector3.zero);
            Vector3[] vertices;

            // Set default model position.
            Vector3 modelPosition = new Vector3(0f, 0f, 0f);

            // Add our main mesh, if any (some are null, because they only 'appear' through subbuildings - e.g. Boston Residence Garage).
            if (currentMesh != null && _material != null)
            {
                // Use separate verticies instance instead of accessing Mesh.vertices each time (which is slow).
                // >10x measured performance improvement by doing things this way instead.
                vertices = currentMesh.vertices;
                for (int i = 0; i < vertices.Length; i++)
                {
                    // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                    if (vertices[i].y > -2)
                    {
                        currentBounds.Encapsulate(vertices[i]);
                    }
                }

                // Calculate rendering matrix and add mesh to scene.
                Matrix4x4 matrix = Matrix4x4.TRS(modelPosition, Quaternion.Euler(Vector3.zero), Vector3.one);
                Graphics.DrawMesh(currentMesh, matrix, _material, 0, renderCamera, 0, null, true, true);
            }

            // Render submeshes, if any.
            if (hasSubMeshes)
            {
                foreach (BuildingInfo.MeshInfo subMesh in subMeshes)
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
                        Quaternion relativeRotation = Quaternion.AngleAxis((subMesh.m_angle * -1), Vector3.up);

                        // Calculate relative position of mesh given its starting position and our model rotation.
                        Vector3 relativePosition = subMesh.m_position;

                        // Put it all together into our rendering matrix.
                        Matrix4x4 matrix = Matrix4x4.TRS(relativePosition + modelPosition, relativeRotation, Vector3.one);

                        // Add submesh to scene.
                        Graphics.DrawMesh(subInfo.m_mesh, matrix, subInfo.m_material, 0, renderCamera, 0, null, true, true);

                        // Expand our bounds to encapsulate the submesh.
                        vertices = subInfo.m_mesh.vertices;
                        for (int i = 0; i < vertices.Length; i++)
                        {
                            // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                            if (vertices[i].y + relativePosition.y > -2)
                            {
                                // Transform coordinates to our model rotation before encapsulating, otherwise we tend to cut off corners.
                                currentBounds.Encapsulate(relativeRotation * (vertices[i] + subMesh.m_position));
                            }
                        }
                    }
                }
            }

            // Render subbuildings, if any.
            if (hasSubBuildings)
            {
                foreach (BuildingInfo.SubInfo subBuilding in subBuildings)
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
                        Graphics.DrawMesh(subInfo.m_mesh, matrix, subInfo.m_material, 0, renderCamera, 0, null, true, true);

                        // Expand our bounds to encapsulate the submesh.
                        vertices = subInfo.m_mesh.vertices;
                        for (int i = 0; i < vertices.Length; i++)
                        {
                            // Exclude vertices with large negative Y values (underground) from our bounds (e.g. SoCal Laguna houses), otherwise the result doesn't look very good.
                            if (vertices[i].y + relativePosition.y > -2)
                            {
                                currentBounds.Encapsulate(vertices[i] + relativePosition);
                            }
                        }
                    }
                }
            }

            // Set zoom to encapsulate entire model.
            float magnitude = currentBounds.extents.magnitude;
            float clipExtent = (magnitude + 16f) * 1.5f;
            float clipCenter = magnitude * currentZoom;

            // Clip planes.
            renderCamera.nearClipPlane = Mathf.Max(clipCenter - clipExtent, 0.01f);
            renderCamera.farClipPlane = clipCenter + clipExtent;

            // Camera position and rotation - directly behind the model, facing forward.
            renderCamera.transform.position = (-Vector3.forward * clipCenter) + currentBounds.center;
            renderCamera.transform.RotateAround(currentBounds.center, Vector3.right, 20f);
            renderCamera.transform.RotateAround(currentBounds.center, Vector3.up, -currentRotation);
            renderCamera.transform.LookAt(currentBounds.center);

            // If game is currently in nighttime, enable sun and disable moon lighting.
            if (gameMainLight == DayNightProperties.instance.moonLightSource)
            {
                DayNightProperties.instance.sunLightSource.enabled = true;
                DayNightProperties.instance.moonLightSource.enabled = false;
            }

            // Light settings.
            renderLight.transform.eulerAngles = new Vector3(55f, -currentRotation - 20f, 0f);
            renderLight.intensity = 2f;
            renderLight.color = Color.white;

            // Render!
            renderCamera.RenderWithShader(_material.shader, "");

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