using System;
using System.Collections.Generic;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace KinectControlRobot.Application.Helper
{
    public static class BodyStateDetector
    {
        /// <summary>
        /// Gets the state of the hand.
        /// </summary>
        /// <param name="depthData">The depth data.</param>
        /// <param name="mappedHandLeft">The mapped hand left.</param>
        /// <param name="mappedHandRight">The mapped hand right.</param>
        /// <param name="isLeftHandOpen">if set to <c>true</c> [is left hand open].</param>
        /// <param name="isRightHandOpen">if set to <c>true</c> [is right hand open].</param>
        public static void GetHandState(DepthImagePixel[] depthData, DepthImagePoint mappedHandLeft,
            DepthImagePoint mappedHandRight, out bool isLeftHandOpen, out bool isRightHandOpen)
        {
            isLeftHandOpen = _isHandOpen(depthData, mappedHandLeft);
            isRightHandOpen = _isHandOpen(depthData, mappedHandRight);
        }

        private static bool _isHandOpen(DepthImagePixel[] depthData, DepthImagePoint mappedHand)
        {
            const int deltaLength = 50;
            const int deltaDepthAllowed = 30;

            var handPixelCount = 0;

            var handDepth = depthData[mappedHand.Y * 640 + mappedHand.X].Depth;

            for (int yAxis = (mappedHand.Y - deltaLength) > 0 ?
                mappedHand.Y - deltaLength : 0;
                 yAxis < mappedHand.Y + deltaLength && yAxis < 480; yAxis++)
            {
                for (int xAxis = (mappedHand.X - deltaLength) > 0 ?
                    mappedHand.X - deltaLength : 0;
                     xAxis < mappedHand.X + deltaLength && xAxis < 640; xAxis++)
                {
                    if (Math.Abs(depthData[yAxis * 640 + xAxis].Depth - handDepth) < deltaDepthAllowed)
                        handPixelCount++;
                }
            }

            return handPixelCount > deltaLength * deltaLength;
        }

        /// <summary>
        /// Gets the angle and rotation.
        /// The return list follows the following order
        /// 
        ///     wristRotation, elbowAngle, elbowRotation, shoulderRotationVertical, shoulderRotationHorizontal,
        ///     hipRotationVertical, hipRotationHorizontal, kneeAngle,
        /// 
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="leftBodyAngleAndRotation">The left body angle and rotation.</param>
        /// <param name="rightBodyAngleAndRotation">The right body angle and rotation.</param>
        public static void GetAngleAndRotation(Skeleton skeleton,
            out List<double> leftBodyAngleAndRotation, out List<double> rightBodyAngleAndRotation)
        {
            leftBodyAngleAndRotation = _calculateAngleAndRotation(new List<Vector3D>
            {
                skeleton.Joints[JointType.HandLeft].Position.ToVector3DAndScale(640,480,560),
                skeleton.Joints[JointType.WristLeft].Position.ToVector3DAndScale(640,480,560),
                skeleton.Joints[JointType.ElbowLeft].Position.ToVector3DAndScale(640,480,560),
                skeleton.Joints[JointType.ShoulderLeft].Position.ToVector3DAndScale(640,480,560),
                skeleton.Joints[JointType.HipLeft].Position.ToVector3DAndScale(640,480,560),
                skeleton.Joints[JointType.KneeLeft].Position.ToVector3DAndScale(640,480,560),
                skeleton.Joints[JointType.AnkleLeft].Position.ToVector3DAndScale(640,480,560),
            });

            rightBodyAngleAndRotation = _calculateAngleAndRotation(new List<Vector3D>
            {
                skeleton.Joints[JointType.HandRight].Position.ToVector3DAndScale(640, 480, 560),
                skeleton.Joints[JointType.WristRight].Position.ToVector3DAndScale(640, 480, 560),
                skeleton.Joints[JointType.ElbowRight].Position.ToVector3DAndScale(640, 480, 560),
                skeleton.Joints[JointType.ShoulderRight].Position.ToVector3DAndScale(640, 480, 560),
                skeleton.Joints[JointType.HipRight].Position.ToVector3DAndScale(640, 480, 560),
                skeleton.Joints[JointType.KneeRight].Position.ToVector3DAndScale(640, 480, 560),
                skeleton.Joints[JointType.AnkleRight].Position.ToVector3DAndScale(640, 480, 560),
            });
        }

        private static List<double> _calculateAngleAndRotation(List<Vector3D> skeletonJoints)
        {
            var yAxis = new Vector3D(0, 480, 0);

            var hand = skeletonJoints[0];
            var wrist = skeletonJoints[1];
            var elbow = skeletonJoints[2];
            var shoulder = skeletonJoints[3];
            var hip = skeletonJoints[4];
            var knee = skeletonJoints[5];
            var ankle = skeletonJoints[6];

            var wrist2Hand = hand - wrist;
            var elbow2Shoulder = shoulder - elbow;
            var elbow2Wrist = wrist - elbow;
            var knee2Hip = hip - knee;
            var knee2Ankle = ankle - knee;

            var crossProductBody = Vector3D.CrossProduct(elbow2Wrist, elbow2Shoulder);

            var wristRotation = Math.Atan2(wrist2Hand.X, -wrist2Hand.Y)
                                           * SkeletonToVectorHelper.FactorToDegree;
            var elbowAngle = Math.Acos(Vector3D.DotProduct(elbow2Shoulder, elbow2Wrist)
                                       / (elbow2Shoulder.Length * elbow2Wrist.Length))
                             * SkeletonToVectorHelper.FactorToDegree;
            var elbowRotation = Math.Acos(Vector3D.DotProduct(crossProductBody, yAxis)
                                          / (crossProductBody.Length * yAxis.Length))
                                          * SkeletonToVectorHelper.FactorToDegree;
            var shoulderRotationVertical = Math.Atan2(-elbow2Shoulder.X, elbow2Shoulder.Y)
                                                      * SkeletonToVectorHelper.FactorToDegree;
            var shoulderRotationHorizontal = Math.Atan2(-elbow2Shoulder.X, elbow2Shoulder.Z)
                                                        * SkeletonToVectorHelper.FactorToDegree;

            var kneeAngle = Math.Acos(Vector3D.DotProduct(knee2Hip, knee2Ankle)
                                      / (knee2Hip.Length * knee2Ankle.Length))
                                      * SkeletonToVectorHelper.FactorToDegree;
            var hipRotationVertical = Math.Atan2(-knee2Hip.X, knee2Hip.Y)
                                                      * SkeletonToVectorHelper.FactorToDegree;
            var hipRotationHorizontal = Math.Atan2(-knee2Hip.X, knee2Hip.Z)
                                                        * SkeletonToVectorHelper.FactorToDegree;

            return new List<double>
            {
                wristRotation,
                elbowAngle,
                elbowRotation,
                shoulderRotationVertical,
                shoulderRotationHorizontal,
                hipRotationVertical,
                hipRotationHorizontal,
                kneeAngle,
            };
        }
    }
}
