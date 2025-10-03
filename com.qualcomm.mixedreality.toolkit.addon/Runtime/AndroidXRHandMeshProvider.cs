// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using System.Collections.Generic;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.XR;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    /// <summary>
    /// Provides a hand mesh from the <c>XR_ANDROID_hand_mesh</c> OpenXR extension.
    /// </summary>
    internal class AndroidXRHandMeshProvider : IMixedRealityHandMeshProvider
    {
        /// <summary>
        /// The OpenXR extension string representing the feature this provider reads the mesh from.
        /// </summary>
        public const string OpenXRExtension = "XR_ANDROID_hand_mesh";

        /// <summary>
        /// The user's left hand.
        /// </summary>
        public static AndroidXRHandMeshProvider Left { get; } = new AndroidXRHandMeshProvider(Handedness.Left);

        /// <summary>
        /// The user's right hand.
        /// </summary>
        public static AndroidXRHandMeshProvider Right { get; } = new AndroidXRHandMeshProvider(Handedness.Right);

        private AndroidXRHandMeshProvider(Handedness handedness)
        {
            this.handedness = handedness;
            mesh = new Mesh();

            List<XRMeshSubsystem> meshSubsystems = new List<XRMeshSubsystem>();
            SubsystemManager.GetSubsystems(meshSubsystems);
            foreach (XRMeshSubsystem subsystem in meshSubsystems)
            {
                if (subsystem.subsystemDescriptor.id == "AndroidXRHandMeshProvider"
                    || subsystem.subsystemDescriptor.id == "AndroidXRMeshProvider")
                {
                    meshSubsystem = subsystem;
                    break;
                }
            }
        }

        private readonly XRMeshSubsystem meshSubsystem = null;
        private readonly Handedness handedness;
        private readonly Mesh mesh;

        private readonly List<Vector3> vertices = new();
        private readonly List<Vector3> normals = new();
        private readonly List<int> triangles = new();

        private IMixedRealityInputSource inputSource = null;

        /// <inheritdoc/>
        void IMixedRealityHandMeshProvider.SetInputSource(IMixedRealityInputSource inputSource) => this.inputSource = inputSource;

        private static readonly ProfilerMarker UpdateHandMeshPerfMarker = new ProfilerMarker($"[MRTK] {nameof(AndroidXRHandMeshProvider)}.UpdateHandMesh");

        /// <inheritdoc/>
        void IMixedRealityHandMeshProvider.UpdateHandMesh()
        {
            using (UpdateHandMeshPerfMarker.Auto())
            {
                MixedRealityInputSystemProfile inputSystemProfile = CoreServices.InputSystem?.InputSystemProfile;
                MixedRealityHandTrackingProfile handTrackingProfile = inputSystemProfile != null ? inputSystemProfile.HandTrackingProfile : null;

                if (handTrackingProfile == null || !handTrackingProfile.EnableHandMeshVisualization || meshSubsystem == null)
                {
                    // If hand mesh visualization is disabled make sure to clean up if we've already initialized
                    if (triangles.Count > 0)
                    {
                        // Notify that hand mesh has been updated (cleared)
                        CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, new HandMeshInfo());
                        triangles.Clear();
                    }
                    return;
                }

                List<MeshInfo> meshInfos = new List<MeshInfo>();
                if (meshSubsystem.TryGetMeshInfos(meshInfos))
                {
                    int handMeshIndex = handedness == Handedness.Left ? 0 : 1;

                    if (meshInfos[handMeshIndex].ChangeState == MeshChangeState.Added
                        || meshInfos[handMeshIndex].ChangeState == MeshChangeState.Updated)
                    {
                        meshSubsystem.GenerateMeshAsync(meshInfos[handMeshIndex].MeshId, mesh,
                            null, MeshVertexAttributes.Normals, result =>
                            {
                                if (result.Status != MeshGenerationStatus.Success)
                                {
                                    Debug.LogWarning($"Hand mesh not properly generated: {result.Status}");
                                    return;
                                }

                                result.Mesh.GetVertices(vertices);
                                result.Mesh.GetNormals(normals);
                                result.Mesh.GetTriangles(triangles, 0);

                                // The bounds center gives us the translation from the mesh's local origin
                                // to the mesh itself. We remove this translation here in order to get a
                                // zeroed local space hand mesh, since we'll put the mesh into world space
                                // with the position value sent via HandMeshInfo below.
                                Matrix4x4 matrix = Matrix4x4.Translate(result.Mesh.bounds.center * -1);
                                for (int i = 0; i < vertices.Count; i++)
                                {
                                    vertices[i] = matrix.MultiplyPoint3x4(vertices[i]);
                                }

                                HandMeshInfo handMeshInfo = new HandMeshInfo
                                {
                                    vertices = vertices.ToArray(),
                                    normals = normals.ToArray(),
                                    triangles = triangles.ToArray(),
                                    position = MixedRealityPlayspace.TransformPoint(result.Position + result.Mesh.bounds.center),
                                    rotation = MixedRealityPlayspace.Rotation * result.Rotation,
                                };

                                CoreServices.InputSystem?.RaiseHandMeshUpdated(inputSource, handedness, handMeshInfo);
                            });
                    }
                }
            }
        }
    }
}
