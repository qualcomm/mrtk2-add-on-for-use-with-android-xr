// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT

using System;
using System.Collections.Generic;
using Unity.XR.CoreUtils.Editor;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR;

namespace Qualcomm.MixedReality.Toolkit.OpenXR.Editor
{
    public class MRTKInputFocusFeatureValidation
    {
        [InitializeOnLoadMethod]
        private static void AddValidationCheck()
        {
            OpenXRFeatureAttribute attribute = Attribute.GetCustomAttribute(typeof(MRTKInputFocusFeature), typeof(OpenXRFeatureAttribute)) as OpenXRFeatureAttribute;

            foreach (BuildTargetGroup buildTargetGroup in attribute.BuildTargetGroups)
            {
                BuildValidator.AddRules(buildTargetGroup, GenerateFeatureRules(buildTargetGroup));
            }
        }

        private static IReadOnlyList<BuildValidationRule> GenerateFeatureRules(BuildTargetGroup buildTargetGroup)
        {
            return new List<BuildValidationRule>
            {
                new()
                {
                    Category = "MRTK2",
                    Message = "For MRTK2 input to work correctly on OpenXR, enable the MRTK2 Input Focus feature in the OpenXR Settings.",
                    CheckPredicate = () =>
                    {
                        OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
                        if (settings == null)
                        {
                            return false;
                        }

                        MRTKInputFocusFeature focusFeature = settings.GetFeature<MRTKInputFocusFeature>();
                        return focusFeature != null && focusFeature.enabled;
                    },
                    FixIt = () =>
                    {
                        OpenXRSettings settings = OpenXRSettings.GetSettingsForBuildTargetGroup(buildTargetGroup);
                        if (settings == null)
                        {
                            return;
                        }

                        MRTKInputFocusFeature focusFeature = settings.GetFeature<MRTKInputFocusFeature>();
                        if (focusFeature != null)
                        {
                            focusFeature.enabled = true;
                            EditorUtility.SetDirty(settings);
                        }
                    },
                    FixItMessage = $"Enable {nameof(MRTKInputFocusFeature)} in the OpenXR settings.",
                    Error = true
                }
            };
        }
    }
}
