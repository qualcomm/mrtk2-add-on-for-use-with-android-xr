// Copyright (c) Qualcomm Technologies, Inc. and/or its subsidiaries.
// SPDX-License-Identifier: MIT 

using Microsoft.MixedReality.Toolkit;
using Microsoft.MixedReality.Toolkit.Utilities;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Hands;
using Handedness = Microsoft.MixedReality.Toolkit.Utilities.Handedness;

namespace Qualcomm.MixedReality.Toolkit.OpenXR
{
    internal class UnityXRHandJointProvider
    {
        public UnityXRHandJointProvider(Handedness handedness)
        {
            this.handedness = handedness;
        }

        private readonly Handedness handedness;
        private static XRHandSubsystem handSubsystem = null;
        private static readonly List<XRHandSubsystem> XRHandSubsystems = new List<XRHandSubsystem>();

        public void UpdateHandJoints(ref MixedRealityPose[] jointPoses)
        {
            XRHand xrHand = GetTrackedHand();

            /// <summary>
            /// Obtains a reference to the actual XRHand object representing the tracked hand
            /// functionality present on HandNode.
            /// </summary>
            XRHand GetTrackedHand()
            {
                if (handSubsystem == null || !handSubsystem.running)
                {
                    handSubsystem = null;
                    SubsystemManager.GetSubsystems(XRHandSubsystems);
                    foreach (XRHandSubsystem xrHandSubsystem in XRHandSubsystems)
                    {
                        if (xrHandSubsystem.running)
                        {
                            handSubsystem = xrHandSubsystem;
                            break;
                        }
                    }

                    if (handSubsystem == null)
                    {
                        // No hand subsystem running this frame.
                        return default;
                    }
                }

                return handedness.HasFlag(Handedness.Left) ? handSubsystem.leftHand : handSubsystem.rightHand;
            }

            if (xrHand.isTracked)
            {
                for (int i = (int)XRHandJointID.BeginMarker; i < (int)XRHandJointID.EndMarker; i++)
                {
                    if (xrHand.GetJoint((XRHandJointID)i).TryGetPose(out Pose pose))
                    {
                        jointPoses ??= new MixedRealityPose[ArticulatedHandPose.JointCount];

                        // We want input sources to follow the playspace, so fold in the playspace transform here to
                        // put the pose into world space.
                        Vector3 position = MixedRealityPlayspace.TransformPoint(pose.position);
                        Quaternion rotation = MixedRealityPlayspace.Rotation * pose.rotation;

                        jointPoses[i] = new MixedRealityPose(position, rotation);
                    }
                }
            }
        }
    }
}
