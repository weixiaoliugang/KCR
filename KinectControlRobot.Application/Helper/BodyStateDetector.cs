using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;

namespace KinectControlRobot.Application.Helper
{
    public enum BodySide
    {
        Left,
        Right,
    }

    public static class BodyStateDetector
    {
        const int DeltaLength = 50;
        const int DeltaDepthAllowed = 30;

        public static bool IsHandOpen(DepthImageFrame depthFrame, DepthImagePoint mappedHand)
        {
            if (depthFrame == null)
            {
                return false;
            }

            var depthData = depthFrame.GetRawPixelData();

            var handPixelCount = 0;

            var handDepth = depthData[mappedHand.Y * depthFrame.Width + mappedHand.X].Depth;

            for (int yAxis = (mappedHand.Y - DeltaLength) > 0 ?
                mappedHand.Y - DeltaLength : 0;
                 yAxis < mappedHand.Y + DeltaLength && yAxis < depthFrame.Height; yAxis++)
            {
                for (int xAxis = (mappedHand.X - DeltaLength) > 0 ?
                    mappedHand.X - DeltaLength : 0;
                     xAxis < mappedHand.X + DeltaLength && xAxis < depthFrame.Width; xAxis++)
                {
                    if (Math.Abs(depthData[yAxis * depthFrame.Width + xAxis].Depth - handDepth)
                        < DeltaDepthAllowed)
                        handPixelCount++;
                }
            }

            return handPixelCount > DeltaLength * DeltaLength;
        }

        /// <summary>
        /// Gets the angle and rotation.
        /// The return list follows the following order
        /// wristRotation, elbowAngle, elbowRotation, shoulderRotationVertical, shoulderRotationHorizontal,
        /// hipRotationVertical, hipRotationHorizontal, kneeAngle,
        /// </summary>
        /// <param name="skeleton">The skeleton.</param>
        /// <param name="bodySide">The body side.</param>
        /// <returns></returns>
        public static List<double> GetAngleAndRotation(Skeleton skeleton, BodySide bodySide)
        {
            if (skeleton == null)
            {
                return null;
            }

            switch (bodySide)
            {
                case BodySide.Left:
                    return _calculateAngleAndRotation(new List<Vector3D>
                            {
                                skeleton.Joints[JointType.HandLeft].Position.ToVector3DAndScale(640,480,560),
                                skeleton.Joints[JointType.WristLeft].Position.ToVector3DAndScale(640,480,560),
                                skeleton.Joints[JointType.ElbowLeft].Position.ToVector3DAndScale(640,480,560),
                                skeleton.Joints[JointType.ShoulderLeft].Position.ToVector3DAndScale(640,480,560),
                                skeleton.Joints[JointType.HipLeft].Position.ToVector3DAndScale(640,480,560),
                                skeleton.Joints[JointType.KneeLeft].Position.ToVector3DAndScale(640,480,560),
                                skeleton.Joints[JointType.AnkleLeft].Position.ToVector3DAndScale(640,480,560),
                            });
                case BodySide.Right:
                    return _calculateAngleAndRotation(new List<Vector3D>
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

            return null;
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
                                           * SkeletonToVectorConverter.FactorToDegree;
            var elbowAngle = Math.Acos(Vector3D.DotProduct(elbow2Shoulder, elbow2Wrist)
                                       / (elbow2Shoulder.Length * elbow2Wrist.Length))
                             * SkeletonToVectorConverter.FactorToDegree;
            var elbowRotation = Math.Acos(Vector3D.DotProduct(crossProductBody, yAxis)
                                          / (crossProductBody.Length * yAxis.Length))
                                          * SkeletonToVectorConverter.FactorToDegree;
            var shoulderRotationVertical = Math.Atan2(-elbow2Shoulder.X, elbow2Shoulder.Y)
                                                      * SkeletonToVectorConverter.FactorToDegree;
            var shoulderRotationHorizontal = Math.Atan2(-elbow2Shoulder.X, elbow2Shoulder.Z)
                                                        * SkeletonToVectorConverter.FactorToDegree;

            var kneeAngle = Math.Acos(Vector3D.DotProduct(knee2Hip, knee2Ankle)
                                      / (knee2Hip.Length * knee2Ankle.Length))
                                      * SkeletonToVectorConverter.FactorToDegree;
            var hipRotationVertical = Math.Atan2(-knee2Hip.X, knee2Hip.Y)
                                                      * SkeletonToVectorConverter.FactorToDegree;
            var hipRotationHorizontal = Math.Atan2(-knee2Hip.X, knee2Hip.Z)
                                                        * SkeletonToVectorConverter.FactorToDegree;

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
