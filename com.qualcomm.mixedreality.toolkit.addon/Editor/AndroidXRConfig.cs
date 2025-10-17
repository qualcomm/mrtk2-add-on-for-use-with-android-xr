// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT 

#if UNITY_6000_0_OR_NEWER
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;
#endif

namespace Qualcomm.MixedReality.Toolkit.OpenXR.Editor
{
    internal class AndroidXRConfig
    {
#if UNITY_6000_0_OR_NEWER
        private static AddAndRemoveRequest request;

        [MenuItem("Mixed Reality/Toolkit/Utilities/Configure for Android XR...", priority = int.MaxValue)]
        public static void InstallPackages()
        {
            // Already a request in progress, so don't re-run
            if (request != null)
            {
                return;
            }

            Debug.Log("Adding com.unity.xr.androidxr-openxr and com.google.xr.extensions...");
            request = Client.AddAndRemove(new[] { "com.unity.xr.androidxr-openxr", "https://github.com/android/android-xr-unity-package.git" });
            EditorApplication.update += Progress;
        }

        private static void Progress()
        {
            if (request.IsCompleted)
            {
                Debug.Log($"Package install request complete ({request.Status})");
                EditorApplication.update -= Progress;
                request = null;
            }
        }
#endif
    }
}
