using AgDataHandle.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgDataHandle.Maths.SpatialAlternation {
    /// <summary>
    /// GIS helpers
    /// </summary>
    public static class GisUtil {
        public static readonly Vector3 Wgs84RadiiSquared = new Vector3(6378137.0f * 6378137.0f,
                6378137.0f * 6378137.0f, 6356752.3142451793f * 6356752.3142451793f);

        private static IMatrix4x4 EastNorthUpToFixedFrame(Vector3 origin, Ellipsoid ellipsoid) {
            var firstAxis = "east";
            var secondAxis = "north";
            return LocalFrameToFixedFrame(origin, ellipsoid, firstAxis, secondAxis);
        }

        private static Dictionary<string, Dictionary<string, string>> VectorProductLocalFrame
             = new Dictionary<string, Dictionary<string, string>>
             {
                 {"up", new Dictionary<string, string>{
                     { "south", "east"}, {"north", "west"},
                     { "west", "south"}, { "east", "north"} } },
                 { "down", new Dictionary<string, string>{
                     { "south", "west" }, { "north", "east" },
                     { "west", "north" }, { "east", "south" }  } },

                 { "south", new Dictionary<string, string>
                 {
                     { "up", "west" }, { "down", "east"},
                     { "west", "down" }, { "east", "up"}
                 } },
                 { "north", new Dictionary<string, string>
                 {
                     {"up", "east" }, {"down", "west"},
                     { "west", "up" }, { "east", "down" }
                 } },

                 { "west", new Dictionary<string, string>
                 {
                     { "up", "north" }, { "down", "south" },
                     { "north", "down" }, { "south", "up" }
                 } },
                 { "east", new Dictionary<string, string>
                 {
                     { "up", "south" }, { "down", "north" },
                     { "north", "up" }, { "south", "down" }
                 } }
             };

        private static Dictionary<string, Vector3> DegeneratePositionLocalFrame =
            new Dictionary<string, Vector3>
            {
                { "north", new Vector3(-1, 0, 0) },
                { "east",  new Vector3( 0, 1, 0) },
                { "up",    new Vector3( 0, 0, 1) },
                { "south", new Vector3( 1, 0, 0) },
                { "west",  new Vector3( 0,-1, 0) },
                { "down",  new Vector3( 0, 0,-1) }
            };

        //这是参照Cesium的方法而使用的静态变量写法，但是经测试，有概率会造成多线程冲突，从而导致错误的结果。所以注释了。
        /*private static Dictionary<string, Vector3> ScratchCalculateCartesian =
            new Dictionary<string, Vector3>
            {
                { "north", new Vector3() },
                { "east", new Vector3() },
                { "up", new Vector3() },
                { "south", new Vector3() },
                { "west", new Vector3() },
                { "down", new Vector3() }
            };*/

        /// <summary>
        /// computes a 4x4 transformation matrix from a reference frame
        /// centered at the provided origin to the provided ellipsoid's fixed reference frame.
        /// </summary>
        /// <param name="firstAxis"></param>
        /// <param name="secondAxis"></param>
        /// <param name="origin"></param>
        /// <param name="ellipsoid"></param>
        /// <returns></returns>
        private static IMatrix4x4 LocalFrameToFixedFrame(
            Vector3 origin, Ellipsoid ellipsoid, string firstAxis, string secondAxis) {
            var thirdAxis = VectorProductLocalFrame[firstAxis][secondAxis];
            Vector3 scratchFirstCartesian, scratchSecondCartesian, scratchThirdCartesian;
            if (Math.Abs(origin.X) < 1e-14 && Math.Abs(origin.Y) < 1e-14)
            {
                // almost zero
                var sign = 0;
                if (origin.Z > 0) sign = 1;
                else if (origin.Z < 0) sign = -1;
                if (!DegeneratePositionLocalFrame.ContainsKey(firstAxis))
                {
                    throw new ArgumentException("no firstAxis", nameof(firstAxis));
                }
                if (!DegeneratePositionLocalFrame.ContainsKey(secondAxis))
                {
                    throw new ArgumentException("no secondAxis", nameof(secondAxis));
                }
                scratchFirstCartesian = DegeneratePositionLocalFrame[firstAxis];
                if (firstAxis != "east" && firstAxis != "west")
                {
                    scratchFirstCartesian = scratchFirstCartesian.Multiply<Vector3>(sign);
                }
                scratchSecondCartesian = DegeneratePositionLocalFrame[secondAxis];
                if (secondAxis != "east" && secondAxis != "west")
                {
                    scratchSecondCartesian = scratchSecondCartesian.Multiply<Vector3>(sign);
                }
                scratchThirdCartesian = DegeneratePositionLocalFrame[thirdAxis];
                if (thirdAxis != "east" && thirdAxis != "west")
                {
                    scratchThirdCartesian = scratchThirdCartesian.Multiply<Vector3>(sign);
                }
            }
            else
            {
                if (ellipsoid == null)
                {
                    ellipsoid = Ellipsoid.Wgs84;
                }

                //这是参照Cesium的方法而使用的静态变量写法，但是经测试，有概率会造成多线程冲突，从而导致错误的结果。所以注释了。
                /*ScratchCalculateCartesian["up"] = ellipsoid.GeodeticSurfaceNormal(origin);

                ScratchCalculateCartesian["east"] =
                    (new Vector3(-origin.Y, origin.X, 0.0f)).Normalize<Vector3>();
                ScratchCalculateCartesian["north"] =
                    ScratchCalculateCartesian["up"].CrossToOther(ScratchCalculateCartesian["east"]);

                ScratchCalculateCartesian["down"] =
                    ScratchCalculateCartesian["up"].MultiplyToOther<Vector3>(-1);
                ScratchCalculateCartesian["west"] =
                    ScratchCalculateCartesian["east"].MultiplyToOther<Vector3>(-1);
                ScratchCalculateCartesian["south"] =
                    ScratchCalculateCartesian["north"].MultiplyToOther<Vector3>(-1);

                scratchFirstCartesian = ScratchCalculateCartesian[firstAxis];
                scratchSecondCartesian = ScratchCalculateCartesian[secondAxis];
                scratchThirdCartesian = ScratchCalculateCartesian[thirdAxis];*/

                //以下为防止多线程冲突而创建的实例
                Dictionary<string, Vector3> scratchCalculateCartesian = new Dictionary<string, Vector3>
                {
                    { "north", new Vector3() },
                    { "east", new Vector3() },
                    { "up", new Vector3() },
                    { "south", new Vector3() },
                    { "west", new Vector3() },
                    { "down", new Vector3() }
                };
                scratchCalculateCartesian["up"] = ellipsoid.GeodeticSurfaceNormal(origin);

                scratchCalculateCartesian["east"] = (new Vector3(-origin.Y, origin.X, 0.0f)).Normalize<Vector3>();
                scratchCalculateCartesian["north"] = scratchCalculateCartesian["up"].CrossToOther(scratchCalculateCartesian["east"]);

                scratchCalculateCartesian["down"] = scratchCalculateCartesian["up"].MultiplyToOther<Vector3>(-1);
                scratchCalculateCartesian["west"] = scratchCalculateCartesian["east"].MultiplyToOther<Vector3>(-1);
                scratchCalculateCartesian["south"] = scratchCalculateCartesian["north"].MultiplyToOther<Vector3>(-1);

                scratchFirstCartesian = scratchCalculateCartesian[firstAxis];
                scratchSecondCartesian = scratchCalculateCartesian[secondAxis];
                scratchThirdCartesian = scratchCalculateCartesian[thirdAxis];
            }

            var result = new float[16];
            result[0] = scratchFirstCartesian.X;
            result[1] = scratchFirstCartesian.Y;
            result[2] = scratchFirstCartesian.Z;
            result[3] = 0.0f;
            result[4] = scratchSecondCartesian.X;
            result[5] = scratchSecondCartesian.Y;
            result[6] = scratchSecondCartesian.Z;
            result[7] = 0.0f;
            result[8] = scratchThirdCartesian.X;
            result[9] = scratchThirdCartesian.Y;
            result[10] = scratchThirdCartesian.Z;
            result[11] = 0.0f;
            result[12] = origin.X;
            result[13] = origin.Y;
            result[14] = origin.Z;
            result[15] = 1.0f;

            return new Matrix4x4(result);
        }

        public static IMatrix4x4 HeadingPitchRollToFixedFrame(Vector3 origin, EulerAngle hpr) {
            var hprQuaternion = hpr.ToQuaternion<Quaternion>();
            var scratchScale = new Vector3(1, 1, 1);
            var hprMatrix = Matrix4x4.FromTranslationQuaternionRotationScale<IMatrix4x4>(
                new Vector3(), hprQuaternion, scratchScale);

            var fixedFrameTransform = EastNorthUpToFixedFrame(origin, Ellipsoid.Wgs84);
            return fixedFrameTransform.Multiply(hprMatrix);
        }


        public static Vector3 CesiumFromRadians(float longitude, float latitude, float height, Vector3 radiiSquared = null) {
            if (radiiSquared == null) radiiSquared = Wgs84RadiiSquared;
            float cosLatitude = (float)Math.Cos(latitude);
            Vector3 scratchN = (new Vector3((float)(cosLatitude * Math.Cos(longitude)), (float)(cosLatitude * Math.Sin(longitude)), (float)Math.Sin(latitude))).Normalize<Vector3>();
            Vector3 scratchK = radiiSquared.MultiplyToOther(scratchN);
            var dot = scratchN.Dot(scratchK);
            float gamma = (float)Math.Sqrt(dot);
            scratchK = scratchK.Divide<Vector3>((float)gamma);
            scratchN = scratchN.Multiply<Vector3>((float)height);
            return scratchK.Add(scratchN);
        }
        /// <summary>
        /// 与Cesium.Transforms.eastNorthUpToFixedFrame方法等效
        /// </summary>
        /// <param name="longitude">longitude in radian</param>
        /// <param name="latitude">latitude in radian</param>
        /// <param name="height">height in meter</param>
        /// <returns></returns>
        public static IMatrix4x4 Wgs84Transform(float longitude, float latitude, float height, float rotate = 0) {
            if (Math.Abs(longitude) > Math.PI || Math.Abs(latitude) > Math.PI)
            {
                longitude = (float)Math.PI * longitude / 180f;
                latitude = (float)Math.PI * latitude / 180f;
            }
            Vector3 pnt = CesiumFromRadians(longitude, latitude, height, Wgs84RadiiSquared);

            return HeadingPitchRollToFixedFrame(pnt, new EulerAngle(0f, 0f, rotate));
        }
    }

    /// <summary>
    /// GIS helpers
    /// </summary>
    public static class GisUtilDouble {
        public static readonly Vector3d Wgs84RadiiSquared = new Vector3d(6378137.0 * 6378137.0,
                6378137.0 * 6378137.0, 6356752.3142451793 * 6356752.3142451793);

        private static IMatrix4x4D EastNorthUpToFixedFrame(Vector3d origin, Ellipsoid ellipsoid) {
            var firstAxis = "east";
            var secondAxis = "north";
            return LocalFrameToFixedFrame(origin, ellipsoid, firstAxis, secondAxis);
        }

        private static Dictionary<string, Dictionary<string, string>> VectorProductLocalFrame
             = new Dictionary<string, Dictionary<string, string>>
             {
                 {"up", new Dictionary<string, string>{
                     { "south", "east"}, {"north", "west"},
                     { "west", "south"}, { "east", "north"} } },
                 { "down", new Dictionary<string, string>{
                     { "south", "west" }, { "north", "east" },
                     { "west", "north" }, { "east", "south" }  } },

                 { "south", new Dictionary<string, string>
                 {
                     { "up", "west" }, { "down", "east"},
                     { "west", "down" }, { "east", "up"}
                 } },
                 { "north", new Dictionary<string, string>
                 {
                     {"up", "east" }, {"down", "west"},
                     { "west", "up" }, { "east", "down" }
                 } },

                 { "west", new Dictionary<string, string>
                 {
                     { "up", "north" }, { "down", "south" },
                     { "north", "down" }, { "south", "up" }
                 } },
                 { "east", new Dictionary<string, string>
                 {
                     { "up", "south" }, { "down", "north" },
                     { "north", "up" }, { "south", "down" }
                 } }
             };

        private static Dictionary<string, Vector3d> DegeneratePositionLocalFrame =
            new Dictionary<string, Vector3d>
            {
                { "north", new Vector3d(-1, 0, 0) },
                { "east",  new Vector3d( 0, 1, 0) },
                { "up",    new Vector3d( 0, 0, 1) },
                { "south", new Vector3d( 1, 0, 0) },
                { "west",  new Vector3d( 0,-1, 0) },
                { "down",  new Vector3d( 0, 0,-1) }
            };

        //这是参照Cesium的方法而使用的静态变量写法，但是经测试，有概率会造成多线程冲突，从而导致错误的结果。所以注释了。
        /*private static Dictionary<string, Vector3d> ScratchCalculateCartesian =
            new Dictionary<string, Vector3d>
            {
                { "north", new Vector3d() },
                { "east", new Vector3d() },
                { "up", new Vector3d() },
                { "south", new Vector3d() },
                { "west", new Vector3d() },
                { "down", new Vector3d() }
            };*/

        /// <summary>
        /// computes a 4x4 transformation matrix from a reference frame
        /// centered at the provided origin to the provided ellipsoid's fixed reference frame.
        /// </summary>
        /// <param name="firstAxis"></param>
        /// <param name="secondAxis"></param>
        /// <param name="origin"></param>
        /// <param name="ellipsoid"></param>
        /// <returns></returns>
        private static IMatrix4x4D LocalFrameToFixedFrame(Vector3d origin, Ellipsoid ellipsoid, string firstAxis, string secondAxis) 
        {
            var thirdAxis = VectorProductLocalFrame[firstAxis][secondAxis];
            Vector3d scratchFirstCartesian, scratchSecondCartesian, scratchThirdCartesian;
            if (Math.Abs(origin.X) < 1e-14 && Math.Abs(origin.Y) < 1e-14)
            {
                // almost zero
                var sign = 0;
                if (origin.Z > 0) sign = 1;
                else if (origin.Z < 0) sign = -1;
                if (!DegeneratePositionLocalFrame.ContainsKey(firstAxis))
                {
                    throw new ArgumentException("no firstAxis", nameof(firstAxis));
                }
                if (!DegeneratePositionLocalFrame.ContainsKey(secondAxis))
                {
                    throw new ArgumentException("no secondAxis", nameof(secondAxis));
                }
                scratchFirstCartesian = DegeneratePositionLocalFrame[firstAxis];
                if (firstAxis != "east" && firstAxis != "west")
                {
                    scratchFirstCartesian = scratchFirstCartesian.Multiply<Vector3d>(sign);
                }
                scratchSecondCartesian = DegeneratePositionLocalFrame[secondAxis];
                if (secondAxis != "east" && secondAxis != "west")
                {
                    scratchSecondCartesian = scratchSecondCartesian.Multiply<Vector3d>(sign);
                }
                scratchThirdCartesian = DegeneratePositionLocalFrame[thirdAxis];
                if (thirdAxis != "east" && thirdAxis != "west")
                {
                    scratchThirdCartesian = scratchThirdCartesian.Multiply<Vector3d>(sign);
                }
            }
            else
            {
                if (ellipsoid == null)
                {
                    ellipsoid = Ellipsoid.Wgs84;
                }

                //这是参照Cesium的方法而使用的静态变量写法，但是经测试，有概率会造成多线程冲突，从而导致错误的结果。所以注释了。
                /*ScratchCalculateCartesian["up"] = ellipsoid.GeodeticSurfaceNormalDouble(origin);

                ScratchCalculateCartesian["east"] = (new Vector3d(-origin.Y, origin.X, 0.0f)).Normalize<Vector3d>();
                ScratchCalculateCartesian["north"] = ScratchCalculateCartesian["up"].CrossToOther(ScratchCalculateCartesian["east"]);

                ScratchCalculateCartesian["down"] = ScratchCalculateCartesian["up"].MultiplyToOther<Vector3d>(-1);
                ScratchCalculateCartesian["west"] = ScratchCalculateCartesian["east"].MultiplyToOther<Vector3d>(-1);
                ScratchCalculateCartesian["south"] = ScratchCalculateCartesian["north"].MultiplyToOther<Vector3d>(-1);

                scratchFirstCartesian = ScratchCalculateCartesian[firstAxis];
                scratchSecondCartesian = ScratchCalculateCartesian[secondAxis];
                scratchThirdCartesian = ScratchCalculateCartesian[thirdAxis];*/

                //以下为防止多线程冲突而创建的实例
                Dictionary<string, Vector3d> scratchCalculateCartesian = new Dictionary<string, Vector3d>
                {
                    { "north", new Vector3d() },
                    { "east", new Vector3d() },
                    { "up", new Vector3d() },
                    { "south", new Vector3d() },
                    { "west", new Vector3d() },
                    { "down", new Vector3d() }
                };
                scratchCalculateCartesian["up"] = ellipsoid.GeodeticSurfaceNormalDouble(origin);

                scratchCalculateCartesian["east"] = (new Vector3d(-origin.Y, origin.X, 0.0f)).Normalize<Vector3d>();
                scratchCalculateCartesian["north"] = scratchCalculateCartesian["up"].CrossToOther(scratchCalculateCartesian["east"]);

                scratchCalculateCartesian["down"] = scratchCalculateCartesian["up"].MultiplyToOther<Vector3d>(-1);
                scratchCalculateCartesian["west"] = scratchCalculateCartesian["east"].MultiplyToOther<Vector3d>(-1);
                scratchCalculateCartesian["south"] = scratchCalculateCartesian["north"].MultiplyToOther<Vector3d>(-1);

                scratchFirstCartesian = scratchCalculateCartesian[firstAxis];
                scratchSecondCartesian = scratchCalculateCartesian[secondAxis];
                scratchThirdCartesian = scratchCalculateCartesian[thirdAxis];
            }

            var result = new double[16];
            result[0] = scratchFirstCartesian.X;
            result[1] = scratchFirstCartesian.Y;
            result[2] = scratchFirstCartesian.Z;
            result[3] = 0.0f;
            result[4] = scratchSecondCartesian.X;
            result[5] = scratchSecondCartesian.Y;
            result[6] = scratchSecondCartesian.Z;
            result[7] = 0.0f;
            result[8] = scratchThirdCartesian.X;
            result[9] = scratchThirdCartesian.Y;
            result[10] = scratchThirdCartesian.Z;
            result[11] = 0.0f;
            result[12] = origin.X;
            result[13] = origin.Y;
            result[14] = origin.Z;
            result[15] = 1.0f;

            return new Matrix4x4D(result);
        }

        public static IMatrix4x4D HeadingPitchRollToFixedFrame(Vector3d origin, IEulerAngleD hpr) {
            var hprQuaternion = hpr.ToQuaternion<IQuaternionD>();
            var scratchScale = new Vector3d(1, 1, 1);
            var hprMatrix = Matrix4x4D.FromTranslationQuaternionRotationScale<IMatrix4x4D>(
                new Vector3d(), hprQuaternion, scratchScale);

            var fixedFrameTransform = EastNorthUpToFixedFrame(origin, Ellipsoid.Wgs84);
            return fixedFrameTransform.Multiply(hprMatrix);
        }

        public static Vector3d CesiumFromRadians(double longitude, double latitude, double height, Vector3d radiiSquared = null) {
            if (radiiSquared == null) radiiSquared = Wgs84RadiiSquared;
            double cosLatitude = Math.Cos(latitude);
            Vector3d scratchN = (new Vector3d((cosLatitude * Math.Cos(longitude)), (cosLatitude * Math.Sin(longitude)), Math.Sin(latitude))).Normalize<Vector3d>();
            Vector3d scratchK = radiiSquared.MultiplyToOther(scratchN);
            double dot = scratchN.Dot(scratchK);
            double gamma = Math.Sqrt(dot);
            scratchK = scratchK.Divide<Vector3d>(gamma);
            scratchN = scratchN.Multiply<Vector3d>(height);
            return scratchK.Add(scratchN);
        }

        /// <summary>
        /// 与Cesium.Transforms.eastNorthUpToFixedFrame方法等效
        /// </summary>
        /// <param name="longitude">longitude in radian</param>
        /// <param name="latitude">latitude in radian</param>
        /// <param name="height">height in meter</param>
        /// <returns></returns>
        public static IMatrix4x4D Wgs84Transform(double longitude, double latitude, double height, double rotate = 0) {
            if (Math.Abs(longitude) > Math.PI || Math.Abs(latitude) > Math.PI)
            {
                longitude = Math.PI * longitude / 180;
                latitude = Math.PI * latitude / 180;
            }
            Vector3d pnt = CesiumFromRadians(longitude, latitude, height, Wgs84RadiiSquared);

            return HeadingPitchRollToFixedFrame(pnt, new EulerAngleD(0, 0, rotate));
        }

        public static readonly Vector3d wgs84OneOverRadii = new Vector3d(1.0 / 6378137.0, 1.0 / 6378137.0, 1.0 / 6356752.3142451793);
        public static readonly Vector3d wgs84OneOverRadiiSquared = new Vector3d(1.0 / (6378137.0 * 6378137.0), 1.0 / (6378137.0 * 6378137.0), 1.0 / (6356752.3142451793 * 6356752.3142451793));
        public static Vector3d CesiumFromCartesian(Vector3d cartesian)
        {
            Vector3d oneOverRadii = wgs84OneOverRadii;
            Vector3d oneOverRadiiSquared = wgs84OneOverRadiiSquared;
            double centerToleranceSquared = 0.1;
            //`cartesian is required.` is thrown from scaleToGeodeticSurface
            Vector3d p = scaleToGeodeticSurface(cartesian, oneOverRadii, oneOverRadiiSquared, centerToleranceSquared);
            if (p == null)
            {
                return null;
            }
            Vector3d n = p * oneOverRadiiSquared;
            n = n.Normalize<Vector3d>();
            Vector3d h = cartesian - p;
            double longitude = Math.Atan2(n.Y, n.X);
            double latitude = Math.Asin(n.Z);
            double height = Sign(h.Dot(cartesian)) * h.Length();
            return new Vector3d(longitude, latitude, height);
        }
        public static Vector3d scaleToGeodeticSurface(Vector3d cartesian, Vector3d oneOverRadii, Vector3d oneOverRadiiSquared, double centerToleranceSquared)
        {
            double positionX = cartesian.X;
            double positionY = cartesian.Y;
            double positionZ = cartesian.Z;

            double oneOverRadiiX = oneOverRadii.X;
            double oneOverRadiiY = oneOverRadii.Y;
            double oneOverRadiiZ = oneOverRadii.Z;

            double x2 = positionX * positionX * oneOverRadiiX * oneOverRadiiX;
            double y2 = positionY * positionY * oneOverRadiiY * oneOverRadiiY;
            double z2 = positionZ * positionZ * oneOverRadiiZ * oneOverRadiiZ;

            // Compute the squared ellipsoid norm.
            double squaredNorm = x2 + y2 + z2;
            double ratio = Math.Sqrt(1.0 / squaredNorm);

            // As an initial approximation, assume that the radial intersection is the projection point.
            Vector3d intersection = cartesian * ratio;

            // If the position is near the center, the iteration will not converge.
            if (squaredNorm < centerToleranceSquared)
            {
                if (double.IsInfinity(ratio))
                    return null;
            }

            double oneOverRadiiSquaredX = oneOverRadiiSquared.X;
            double oneOverRadiiSquaredY = oneOverRadiiSquared.Y;
            double oneOverRadiiSquaredZ = oneOverRadiiSquared.Z;

            // Use the gradient at the intersection point in place of the true unit normal.
            // The difference in magnitude will be absorbed in the multiplier.
            Vector3d gradient = new Vector3d();
            gradient.X = intersection.X * oneOverRadiiSquaredX * 2.0;
            gradient.Y = intersection.Y * oneOverRadiiSquaredY * 2.0;
            gradient.Z = intersection.Z * oneOverRadiiSquaredZ * 2.0;

            // Compute the initial guess at the normal vector multiplier, lambda.
            double lambda = (1.0 - ratio) * cartesian.Length() / (0.5 * gradient.Length());
            double correction = 0.0;

            double func;
            double denominator;
            double xMultiplier;
            double yMultiplier;
            double zMultiplier;
            double xMultiplier2;
            double yMultiplier2;
            double zMultiplier2;
            double xMultiplier3;
            double yMultiplier3;
            double zMultiplier3;

            do
            {
                lambda -= correction;

                xMultiplier = 1.0 / (1.0 + lambda * oneOverRadiiSquaredX);
                yMultiplier = 1.0 / (1.0 + lambda * oneOverRadiiSquaredY);
                zMultiplier = 1.0 / (1.0 + lambda * oneOverRadiiSquaredZ);

                xMultiplier2 = xMultiplier * xMultiplier;
                yMultiplier2 = yMultiplier * yMultiplier;
                zMultiplier2 = zMultiplier * zMultiplier;

                xMultiplier3 = xMultiplier2 * xMultiplier;
                yMultiplier3 = yMultiplier2 * yMultiplier;
                zMultiplier3 = zMultiplier2 * zMultiplier;

                func = x2 * xMultiplier2 + y2 * yMultiplier2 + z2 * zMultiplier2 - 1.0;

                // "denominator" here refers to the use of this expression in the velocity and acceleration
                // computations in the sections to follow.
                denominator = x2 * xMultiplier3 * oneOverRadiiSquaredX + y2 * yMultiplier3 * oneOverRadiiSquaredY + z2 * zMultiplier3 * oneOverRadiiSquaredZ;
                double derivative = -2.0 * denominator;
                correction = func / derivative;
            } while (Math.Abs(func) > 1e-12);

            return new Vector3d(positionX * xMultiplier, positionY * yMultiplier, positionZ * zMultiplier);
        }

        public static int Sign(double value)
        {
            if (value == 0)
                return 0;
            return value > 0 ? 1 : -1;
        }
    }
}
