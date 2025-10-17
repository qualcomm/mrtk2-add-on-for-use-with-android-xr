// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT

using Unity.XR.CoreUtils.Bindings.Variables;
using UnityEngine;
using UnityEngine.XR.OpenXR.Features;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    /// <summary>
    /// Provides focus data based on XrSession focus.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(
        UiName = "MRTK2 Input Focus",
        Desc = "Provides focus data based on XrSession focus.",
        Company = "Qualcomm",
        Version = "0.9.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA, BuildTargetGroup.Android },
        Category = FeatureCategory.Feature,
        FeatureId = "com.qualcomm.mixedreality.toolkit.openxr.mrtkinputfocus")]
#endif
    public sealed class MRTKInputFocusFeature : OpenXRFeature
    {
        /// <summary>
        /// Whether the current XrSession has focus or not, stored as a bindable variable that can be subscribed to for value changes.
        /// </summary>
        /// <remarks>Always <see langword="true"/> in the editor.</remarks>
        public static IReadOnlyBindableVariable<bool> XrSessionHasFocus => xrSessionHasFocus;
        private static readonly BindableVariable<bool> xrSessionHasFocus = new(Application.isEditor);

        /// <inheritdoc/>
        protected override void OnSessionStateChange(int oldState, int newState)
        {
            // If we've lost focus...
            // XR_SESSION_STATE_FOCUSED = 5
            if (oldState == 5)
            {
                xrSessionHasFocus.Value = false;
            }
            // ...or if we've gained focus
            // XR_SESSION_STATE_FOCUSED = 5
            else if (newState == 5)
            {
                xrSessionHasFocus.Value = true;
            }
        }
    }
}
