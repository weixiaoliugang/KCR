using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.RightsManagement;
using System.Text;
using System.Windows.Media.Media3D;
using Microsoft.Kinect;
using System.Windows;

namespace KinectControlRobot.Application.Helper
{
    public static class SkeletonToVectorHelper
    {
        public const double FactorToDegree = 180 / Math.PI;

        public static Vector3D ToVector3D(this SkeletonPoint point)
        {
            return new Vector3D(point.X, point.Y, point.Z);
        }

        public static Vector3D ToVector3DAndScale(this SkeletonPoint point, float scaleX, 
            float scaleY, float scaleZ)
        {
            return new Vector3D(point.X * scaleX, point.Y * scaleY, point.Z * scaleZ);
        }

        public static Vector ToVector(this SkeletonPoint point)
        {
            return new Vector(point.X, point.Y);
        }

        public static Vector ToVectorAndScale(this SkeletonPoint point, float scaleX, float scaleY)
        {
            return new Vector(point.X * scaleX, point.Y * scaleY);
        }

        public static List<Vector> ToListOfVector(this JointCollection joints)
        {
            return joints.Select((j) => j.Position.ToVector()).ToList();
        }

        public static List<Vector3D> ToListOfVector3D(this JointCollection joints)
        {
            return joints.Select((j) => j.Position.ToVector3D()).ToList();
        }

        public static void GetSkeletons(this SkeletonFrame frame, ref Skeleton[] skeletons)
        {
            if (frame == null)
                return;

            if (skeletons == null || skeletons.Length != frame.SkeletonArrayLength)
            {
                skeletons = new Skeleton[frame.SkeletonArrayLength];
            }
            frame.CopySkeletonDataTo(skeletons);
        }

        public static Skeleton[] GetSkeletons(this SkeletonFrame frame)
        {
            if (frame == null)
                return new Skeleton[] { new Skeleton() { TrackingState = SkeletonTrackingState.NotTracked } };

            var skeletons = new Skeleton[frame.SkeletonArrayLength];
            frame.CopySkeletonDataTo(skeletons);

            return skeletons;
        }
    }
}
