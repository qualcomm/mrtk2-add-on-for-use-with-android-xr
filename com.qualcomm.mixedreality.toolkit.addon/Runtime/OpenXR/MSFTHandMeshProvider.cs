// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT 

using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.XRSDK.OpenXR;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    /// <summary>
    /// Provides a hand mesh from the <c>XR_MSFT_hand_tracking_mesh</c> OpenXR extension.
    /// </summary>
    public class MSFTHandMeshProvider : IMixedRealityHandMeshProvider
    {
        /// <summary>
        /// The OpenXR extension string representing the feature this provider reads the mesh from.
        /// </summary>
        public const string OpenXRExtension = "XR_MSFT_hand_tracking_mesh";

        /// <summary>
        /// The user's left hand.
        /// </summary>
        public static MSFTHandMeshProvider Left { get; } = new MSFTHandMeshProvider(Handedness.Left);

        /// <summary>
        /// The user's right hand.
        /// </summary>
        public static MSFTHandMeshProvider Right { get; } = new MSFTHandMeshProvider(Handedness.Right);

        private MSFTHandMeshProvider(Handedness handedness)
        {
            meshProvider = handedness == Handedness.Left ? OpenXRHandMeshProvider.Left : OpenXRHandMeshProvider.Right;
        }

        private readonly OpenXRHandMeshProvider meshProvider;

        void IMixedRealityHandMeshProvider.SetInputSource(IMixedRealityInputSource inputSource)
        {
            meshProvider?.SetInputSource(inputSource);
        }

        void IMixedRealityHandMeshProvider.UpdateHandMesh()
        {
            meshProvider?.UpdateHandMesh();
        }
    }
}
