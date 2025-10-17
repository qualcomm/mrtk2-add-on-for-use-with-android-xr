// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT 

using System.IO;
using System.Text;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.UnityLinker;
using UnityEngine;

namespace Qualcomm.MixedReality.Toolkit.OpenXR.Editor
{
    public class MRTKPreserveSettings : IUnityLinkerProcessor
    {
        int IOrderedCallback.callbackOrder => 0;

        string IUnityLinkerProcessor.GenerateAdditionalLinkXmlFile(BuildReport report, UnityLinkerBuildPipelineData data)
        {
            StringBuilder sb = new("<linker>\n");
            sb.AppendLine($"  <assembly fullname=\"{typeof(OpenXRDeviceManager).Assembly.GetName().Name}\"/>");
            sb.AppendLine("</linker>");

            string linkXmlPath = Path.Combine(Application.dataPath, "..", "Temp", "MRTKAXRLink.xml");
            File.WriteAllText(linkXmlPath, sb.ToString());
            return linkXmlPath;
        }
    }
}
