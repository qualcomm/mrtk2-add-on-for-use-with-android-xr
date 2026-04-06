// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.OpenXR;
using UnityEngine;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
using UnityEngine.XR.OpenXR;
#endif

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR Eye Gaze Provider",
        "Profiles/DefaultMixedRealityEyeTrackingProfile.asset", "MixedRealityToolkit.SDK",
        true,
        SupportedUnityXRPipelines.XRSDK)]
    public class OpenXREyeGazeProvider : OpenXREyeGazeDataProvider
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="inputSystem">The <see cref="Microsoft.MixedReality.Toolkit.Input.IMixedRealityInputSystem"/> instance that receives data from this provider.</param>
        /// <param name="name">Friendly name of the service.</param>
        /// <param name="priority">Service priority. Used to determine order of instantiation.</param>
        /// <param name="profile">The service's configuration profile.</param>
        public OpenXREyeGazeProvider(
            IMixedRealityInputSystem inputSystem,
            string name,
            uint priority,
            BaseMixedRealityProfile profile) : base(inputSystem, name, priority, profile) { }

        /// <inheritdoc />
        public override void Enable()
        {
            base.Enable();

            if (IsEnabled)
            {
                RequestPermission();
            }
        }

        private void RequestPermission()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!OpenXRRuntime.IsExtensionEnabled("XR_EXT_eye_gaze_interaction"))
            {
                // The corresponding OpenXR extension isn't enabled, so we don't need the permission
                return;
            }

            if (!Permission.HasUserAuthorizedPermission(EyeTrackingPermission))
            {
                PermissionCallbacks callbacks = new();
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionGranted += OnPermissionGranted;

                Permission.RequestUserPermission(EyeTrackingPermission, callbacks);
                Debug.Log($"MRTK is requesting {EyeTrackingPermission}.");
            }
            else
            {
                Debug.Log($"{EyeTrackingPermission} already granted for MRTK.");
            }
        }

        private const string EyeTrackingPermission = "android.permission.EYE_TRACKING_FINE";

        void OnPermissionDenied(string permission)
        {
            if (permission == EyeTrackingPermission)
            {
                Debug.Log($"{EyeTrackingPermission} denied or not needed on this runtime ({OpenXRRuntime.name}). MRTK eye tracking may not work as expected.");
            }
        }

        void OnPermissionGranted(string permission)
        {
            if (permission == EyeTrackingPermission)
            {
                Debug.Log($"{EyeTrackingPermission} newly granted for MRTK.");
            }
#endif // UNITY_ANDROID && !UNITY_EDITOR
        }
    }
}
