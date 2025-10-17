// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT 

using Microsoft.MixedReality.Toolkit.Input;

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    public interface IMixedRealityHandMeshProvider
    {
        /// <summary>
        /// Sets the <see cref="IMixedRealityInputSource"/> that represents the current hand for this mesh.
        /// </summary>
        /// <param name="inputSource">Implementation of the hand input source.</param>
        public void SetInputSource(IMixedRealityInputSource inputSource);

        /// <summary>
        /// Updates the hand mesh based on the current state of the hand.
        /// </summary>
        public void UpdateHandMesh();
    }
}
