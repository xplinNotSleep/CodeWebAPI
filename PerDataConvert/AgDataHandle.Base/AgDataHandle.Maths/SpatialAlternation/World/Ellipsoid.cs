using AgDataHandle.Maths;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AgDataHandle.Maths.SpatialAlternation
{
    // https://github.com/AnalyticalGraphicsInc/cesium
    /// <summary>
    /// A quadratic surface defined in Cartesian coordinates by the equation
    /// <code>(x / a)^2 + (y / b)^2 + (z / c)^2 = 1</code>.  Primarily used
    /// by Cesium to represent the shape of planetary bodies.
    /// </summary>
    public class Ellipsoid
    {

        public Vector3d Radii { get; }

        public Vector3d RadiiSquared { get; }

        public Vector3d RadiiToTheFourth { get; }

        public Vector3d OneOverRadii { get; }

        public Vector3d OneOverRadiiSquared { get; }

        public double MinimumRadius { get; }

        public double MaximumRadius { get; }

        public double CenterToleranceSquared { get; }

        public double SquaredXOverSquaredZ { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">The radius in the x direction.</param>
        /// <param name="y">The radius in the y direction.</param>
        /// <param name="z">The radius in the z direction.</param>
        public Ellipsoid(double x, double y, double z)
        {
            Radii = new Vector3d(x, y, z);
            RadiiSquared = new Vector3d(x * x, y * y, z * z);
            RadiiToTheFourth = new Vector3d(x * x * x * x, y * y * y * y, z * z * z * z);
            OneOverRadii = new Vector3d(
                x == 0.0 ? 0.0 : 1.0 / x,
                y == 0.0 ? 0.0 : 1.0 / y,
                z == 0.0 ? 0.0 : 1.0 / z
            );
            OneOverRadiiSquared = new Vector3d(
                x == 0.0 ? 0.0 : 1.0 / (x * x),
                y == 0.0 ? 0.0 : 1.0 / (y * y),
                z == 0.0 ? 0.0 : 1.0 / (z * z)
            );
            MinimumRadius = (new[] { x, y, z }).Min();
            MaximumRadius = (new[] { x, y, z }).Max();
            CenterToleranceSquared = 0.1;

            if (RadiiSquared.Z != 0.0)
            {
                SquaredXOverSquaredZ = RadiiSquared.X / RadiiSquared.Z;
            }
        }

        public Vector3 GeodeticSurfaceNormal(Vector3 pnt)
        {
            //var res = Vector3.Multiply(pnt, OneOverRadiiSquared);
            var res = new Vector3(pnt.X * (float)OneOverRadiiSquared.X, pnt.Y * (float)OneOverRadiiSquared.Y, pnt.Z * (float)OneOverRadiiSquared.Z);
            return res.Normalize<Vector3>();
        }
        public Vector3d GeodeticSurfaceNormalDouble(Vector3d pnt)
        {
            return new Vector3d(pnt.X * OneOverRadiiSquared.X, pnt.Y * OneOverRadiiSquared.Y, pnt.Z * OneOverRadiiSquared.Z).Normalize<Vector3d>();
        }
        /// <summary>
        /// Converts the provided cartesian to cartographic representation.
        /// The cartesian is undefined at the center of the ellipsoid.
        /// </summary>
        /// <param name="xyz"></param>
        /// <returns></returns>
        public Vector3d CartesianToCartographic(Vector3d xyz)
        {
            Vector3d p = ScaleToGeodeticSurface(xyz, OneOverRadii, OneOverRadiiSquared, CenterToleranceSquared);
            if (p == null) return p;
            Vector3d n = GeodeticSurfaceNormalDouble(p);
            Vector3d h = xyz.MinusToOther(p);

            double longitude = Math.Atan2(n.Y, n.X);
            double latitude = Math.Asin(n.Z);

            double heitDot = h.Dot(xyz);
            double heitSign = 0.0;
            if (heitDot > 0)
            {
                heitSign = 1.0;
            }
            else if (heitDot < 0)
            {
                heitSign = -1.0;
            }
            double height = heitSign * h.Length();

            return new Vector3d(longitude, latitude, height);
        }

        /// <summary>
        /// Scales the provided Cartesian position along the geodetic surface normal
        /// so that it is on the surface of this ellipsoid.If the position is
        /// at the center of the ellipsoid, this function returns undefined.
        /// </summary>
        /// <param name="cartesian">The Cartesian position to scale.</param>
        /// <param name="oneOverRadii">One over radii of the ellipsoid.</param>
        /// <param name="oneOverRadiiSquared">One over radii squared of the ellipsoid.</param>
        /// <param name="centerToleranceSquared">Tolerance for closeness to the center.</param>
        /// <returns></returns>
        private static Vector3d ScaleToGeodeticSurface(Vector3d cartesian,
            Vector3d oneOverRadii, Vector3d oneOverRadiiSquared,
            double centerToleranceSquared)
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
            Vector3d intersection = cartesian.MultiplyToOther<Vector3d>(ratio);

            // If the position is near the center, the iteration will not converge.
            if (squaredNorm < centerToleranceSquared)
            {
                return double.IsInfinity(ratio) ? null : intersection;
            }

            double oneOverRadiiSquaredX = oneOverRadiiSquared.X;
            double oneOverRadiiSquaredY = oneOverRadiiSquared.Y;
            double oneOverRadiiSquaredZ = oneOverRadiiSquared.Z;

            // Use the gradient at the intersection point in place of the true unit normal.
            // The difference in magnitude will be absorbed in the multiplier.
            Vector3d gradient = new Vector3d(
                intersection.X * oneOverRadiiSquaredX * 2.0,
                intersection.Y * oneOverRadiiSquaredY * 2.0,
                intersection.Z * oneOverRadiiSquaredZ * 2.0
                );

            // Compute the initial guess at the normal vector multiplier, lambda.
            double lambda = (1.0 - ratio) * cartesian.Length() / (0.5 * gradient.Length());
            double correction = 0.0;

            double xMultiplier;
            double yMultiplier;
            double zMultiplier;
            double func;
            do
            {
                lambda -= correction;

                xMultiplier = 1.0 / (1.0 + lambda * oneOverRadiiSquaredX);
                yMultiplier = 1.0 / (1.0 + lambda * oneOverRadiiSquaredY);
                zMultiplier = 1.0 / (1.0 + lambda * oneOverRadiiSquaredZ);

                double xMultiplier2 = xMultiplier * xMultiplier;
                double yMultiplier2 = yMultiplier * yMultiplier;
                double zMultiplier2 = zMultiplier * zMultiplier;

                double xMultiplier3 = xMultiplier2 * xMultiplier;
                double yMultiplier3 = yMultiplier2 * yMultiplier;
                double zMultiplier3 = zMultiplier2 * zMultiplier;

                func = x2 * xMultiplier2 + y2 * yMultiplier2 + z2 * zMultiplier2 - 1.0;

                // "denominator" here refers to the use of this expression in the velocity and acceleration
                // computations in the sections to follow.
                double denominator = x2 * xMultiplier3 * oneOverRadiiSquaredX + y2 * yMultiplier3 * oneOverRadiiSquaredY + z2 * zMultiplier3 * oneOverRadiiSquaredZ;

                double derivative = -2.0 * denominator;

                correction = func / derivative;
            } while (Math.Abs(func) > 1e-12); // CesiumMath.EPSILON12

            return new Vector3d(positionX * xMultiplier, positionY * yMultiplier, positionZ * zMultiplier);

        }

        public static Ellipsoid Wgs84 { get; } = new Ellipsoid(
            6378137.0f, 6378137.0f, 6356752.3142451793f);
    }
}
