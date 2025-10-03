// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT 

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.Utilities;
using Microsoft.MixedReality.Toolkit.XRSDK.Input;
using System;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.OpenXR;

#if UNITY_ANDROID && !UNITY_EDITOR
using UnityEngine.Android;
#endif

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    [MixedRealityDataProvider(
        typeof(IMixedRealityInputSystem),
        (SupportedPlatforms)(-1),
        "OpenXR Device Manager",
        supportedUnityXRPipelines: SupportedUnityXRPipelines.XRSDK)]
    public class OpenXRDeviceManager : Microsoft.MixedReality.Toolkit.XRSDK.OpenXR.OpenXRDeviceManager
    {
        public OpenXRDeviceManager(
            IMixedRealityInputSystem inputSystem,
            string name = null,
            uint priority = 10,
            BaseMixedRealityProfile profile = null) : base(inputSystem, name, priority, profile)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            if (!Permission.HasUserAuthorizedPermission(HandTrackingPermission))
            {
                PermissionCallbacks callbacks = new();
                callbacks.PermissionDenied += OnPermissionDenied;
                callbacks.PermissionGranted += OnPermissionGranted;

                Permission.RequestUserPermission(HandTrackingPermission, callbacks);
                Debug.Log($"MRTK is requesting {HandTrackingPermission}.");
            }
            else
            {
                Debug.Log($"{HandTrackingPermission} already granted for MRTK.");
            }
        }

        private const string HandTrackingPermission = "android.permission.HAND_TRACKING";

        void OnPermissionDenied(string permission)
        {
            if (permission == HandTrackingPermission)
            {
                Debug.Log($"{HandTrackingPermission} denied or not needed on this runtime ({OpenXRRuntime.name}). MRTK hand tracking may not work as expected.");
            }
        }

        void OnPermissionGranted(string permission)
        {
            if (permission == HandTrackingPermission)
            {
                Debug.Log($"{HandTrackingPermission} newly granted for MRTK.");
            }
#endif // UNITY_ANDROID && !UNITY_EDITOR
        }

        protected override GenericXRSDKController GetOrAddController(InputDevice inputDevice)
        {
            // We want to ensure we're focused, as some runtimes continue reporting "tracked" devices while pose updates are paused.
            // This is allowed, per-spec, as a "should": "Runtimes should make input actions inactive while the application is unfocused,
            // and applications should react to an inactive input action by skipping rendering of that action's input avatar
            // (depictions of hands or other tracked objects controlled by the user)."
            if (!MRTKInputFocusFeature.XrSessionHasFocus.Value)
            {
                return null;
            }

            return base.GetOrAddController(inputDevice);
        }

        /// <inheritdoc />
        protected override Type GetControllerType(SupportedControllerType supportedControllerType)
        {
            Type controllerType =
                supportedControllerType == SupportedControllerType.ArticulatedHand && OpenXRRuntime.IsExtensionEnabled("XR_EXT_hand_interaction") ?
                typeof(OpenXRArticulatedHand) :
                base.GetControllerType(supportedControllerType);
            Debug.Log($"OpenXRDeviceManager.GetControllerType({supportedControllerType}) = {controllerType}");
            return controllerType;
        }
    }
}
