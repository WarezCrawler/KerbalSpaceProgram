using UnityEngine;
using System;


namespace AutoRove
{
    /// <summary>
    /// helper functions for autoRove Plugin
    /// </summary>
    internal static class autoRoveUtils
    {
        private static string modTag = "[AutoRove] ";

        internal static void debugMessage(string message)
        {
            Debug.Log(modTag + message);
        }

        internal static void debugWarning(string message)
        {
            Debug.LogWarning(modTag + message);
        }

        internal static void debugError(string message)
        {
            Debug.LogError(modTag + message);
        }


        /// <summary>
        /// creates a line from start to finish with a color
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        internal static LineRenderer debugLine(Vector3d start, Vector3d end, Color color)
        {
            LineRenderer line = new GameObject("line").AddComponent<LineRenderer>();
            line.useWorldSpace = true;
            line.material = new Material(Shader.Find("Particles/Additive"));
            line.SetColors(color, color);
            line.SetWidth(1000, 1000);
            line.SetVertexCount(2);
            line.SetPosition(0, start);
            line.SetPosition(1, end);
            line.enabled = true;
            return line;
        }

        /// <summary>
        /// searches a ConfigNode recursevly for a Node named nodeName,
        /// that has a Value named valueName, which holds a string valueData
        /// </summary>
        /// <param name="parentNode"> the starting node </param>
        /// <param name="nodeName"> parameter under ConfigNode.name </param>
        /// <param name="valueName"> the name of the value </param>
        /// <param name="valueData"> the data stored on that value </param>
        /// <returns> the first hit or an empty ConfigNode</returns>
        internal static ConfigNode queryNode(ConfigNode parentNode, string nodeName, string valueName, string valueData)
        {
            if (parentNode == null)
            {
                return ConfigNode.Parse("");
            }
            else if (parentNode.name == nodeName && parentNode.HasValue(valueName) && parentNode.GetValue(valueName) == valueData)
            {
                return parentNode;
            }
            else if (parentNode.HasNode())
            {
                foreach (ConfigNode childNode in parentNode.nodes)
                {
                    ConfigNode nextNode = queryNode(childNode, nodeName, valueName, valueData);
                    if (nextNode.name == nodeName && nextNode.HasValue(valueName) && nextNode.GetValue(valueName) == valueData)
                    {
                        return nextNode;
                    }
                }
                return ConfigNode.Parse("");
            }
            else
            {
                return ConfigNode.Parse("");
            }
        }


        /// <summary>
        /// normalizes coordinates to something between +0 and +360
        /// </summary>
        /// <param name="coordinate"></param>
        /// <returns></returns>
        internal static double sanitizeDegrees(double coordinate)
        {
            double remainder = coordinate % 360;
            if (remainder < 0)
            {
                return remainder + 360;
            }
            else
            {
                return remainder;
            }
        }

        /// <summary>
        /// calculates the terain height at a position of a given celestial body
        /// </summary>
        /// <param name="latitude"> in degrees </param>
        /// <param name="longitude"> in degrees </param>
        /// <param name="body"></param>
        /// <returns> altitude in meters </returns>
        internal static double surfaceHeight(double latitude, double longitude, CelestialBody body)
        {
            //Vector3d pqsRadialVector = QuaternionD.AngleAxis(longitude, Vector3d.down) * QuaternionD.AngleAxis(latitude, Vector3d.forward) * Vector3d.right;
            //double altitude = body.pqsController.GetSurfaceHeight(pqsRadialVector) - body.pqsController.radius;

            //double altitude = ARsurfaceHeight(latitude, longitude, body);
            double altitude = WMTerrainHeight(latitude, longitude, body);

            //Debug.Log(
            //    "\nAutoRove surfaceHeight (altitude): " + altitude +
            //    "\nWaypoint Manager TerrainHeight (altitude): " + WMTerrainHeight(latitude, longitude, body)
            //    );

            return altitude;
        }

        internal static double ARsurfaceHeight(double latitude, double longitude, CelestialBody body)
        {
            Vector3d pqsRadialVector = QuaternionD.AngleAxis(longitude, Vector3d.down) * QuaternionD.AngleAxis(latitude, Vector3d.forward) * Vector3d.right;
            double altitude = body.pqsController.GetSurfaceHeight(pqsRadialVector) - body.pqsController.radius;


            //Debug.Log(
            //    "\nAutoRove surfaceHeight (altitude): " + altitude +
            //    "\nWaypoint Manager TerrainHeight (altitude): " + WMTerrainHeight(latitude, longitude, body)
            //    );

            return altitude;
        }

        internal static double WMTerrainHeight(double latitude, double longitude, CelestialBody body)
        {
            // Not sure when this happens - for Sun and Jool?
            if (body.pqsController == null)
            {
                return 0;
            }

            // Figure out the terrain height
            double latRads = Math.PI / 180.0 * latitude;
            double lonRads = Math.PI / 180.0 * longitude;
            Vector3d radialVector = new Vector3d(Math.Cos(latRads) * Math.Cos(lonRads), Math.Sin(latRads), Math.Cos(latRads) * Math.Sin(lonRads));
            return Math.Max(body.pqsController.GetSurfaceHeight(radialVector) - body.pqsController.radius, 0.0);
        }

        /// <summary>
        /// claculates the initial bearing between two points
        /// </summary>
        /// <param name="latStart"></param>
        /// <param name="longStart"></param>
        /// <param name="latEnd"></param>
        /// <param name="longEnd"></param>
        /// <returns>Bearing in degrees</returns>
        internal static double bearingDegrees(double latStart, double longStart, double latEnd, double longEnd)
        {
            double dLong = UtilMath.DegreesToRadians(longEnd - longStart);
            double dPhi = Math.Log(Math.Tan(UtilMath.DegreesToRadians(latEnd) / 2 + Math.PI / 4) / Math.Tan(UtilMath.DegreesToRadians((latStart)) / 2 + Math.PI / 4));
            if (Math.Abs(dLong) > Math.PI)
            {
                if (dLong > 0)
                {
                    dLong = -(2 * Math.PI - dLong);
                }
                else
                {
                    dLong = (2 * Math.PI + dLong);
                }
            }
            double brg = Math.Atan2(dLong, dPhi);
            brg = (UtilMath.RadiansToDegrees(brg) +360) % 360;
            return brg;
        }

        /// <summary>
        /// calculating the great circle distance for initial bearing
        /// </summary>
        /// <param name="latStart"> in degrees </param>
        /// <param name="longStart"> in degrees </param>
        /// <param name="bearing"> in degrees </param>
        /// <param name="distance"> in m </param>
        /// <param name="radius"> the radius of the body to travel in meters</param>
        /// <returns> the coordinate [lat, long] of the new position in degrees </returns>
        internal static double[] newPosition(double latStart, double longStart, double bearing, double distance, double radius)
        {
            latStart = UtilMath.DegreesToRadians(latStart);
            longStart = UtilMath.DegreesToRadians(longStart);
            bearing = UtilMath.DegreesToRadians(bearing);

            double newLatRad = Math.Asin( Math.Sin(latStart) * Math.Cos(distance/radius) + Math.Cos(latStart) * Math.Sin(distance/radius) * Math.Cos(bearing));
            double newLongRad = longStart + Math.Atan2(Math.Sin(bearing) * Math.Sin(distance / radius) * Math.Cos(latStart), Math.Cos(distance / radius) - Math.Sin(latStart) * Math.Sin(newLatRad));
            double newLatDegrees = UtilMath.RadiansToDegrees(newLatRad);
            double newLongDegrees = UtilMath.RadiansToDegrees(newLongRad);

            double[] finalCoordinate = new double[2] {sanitizeDegrees(newLatDegrees), sanitizeDegrees(newLongDegrees)};

            return finalCoordinate;
        }

        /// <summary>
        /// calculates the great circle distance between to points on a globe
        /// </summary>
        /// <param name="latStart"> in Degrees </param>
        /// <param name="longStart"> in Degrees </param>
        /// <param name="radius"> in m </param>
        /// <returns> the distance in m </returns>
        internal static double distanceBetweenPoints(double latStart, double longStart, double latEnd, double longEnd, double radius)
        {
            latStart = UtilMath.DegreesToRadians(latStart);
            longStart = UtilMath.DegreesToRadians(longStart);
            latEnd = UtilMath.DegreesToRadians(latEnd);
            longEnd = UtilMath.DegreesToRadians(longEnd);

            double a = Math.Sin((latEnd-latStart)/2) * Math.Sin((latEnd-latStart)/2) + (Math.Cos(latStart) * Math.Cos(latEnd) * Math.Sin((longEnd-longStart)/2)*Math.Sin((longEnd-longStart)/2));
            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1-a));
            double distance = radius * c;

            return distance;
        }

        /// <summary>Indicates whether the specified array is null or has a length of zero.</summary>
        /// <param name="array">The array to test.</param>
        /// <returns>true if the array parameter is null or has a length of zero; otherwise, false.</returns>
        public static bool IsNullOrEmpty(this Array array)
        {
            return (array == null || array.Length == 0);
        }

        // function to track the solar power output, results are way off...
        //internal static double sunPowerFactorAtPos(double latitude, double longitude, CelestialBody body)
        //{
        //    CelestialBody home = FlightGlobals.GetHomeBody();
        //    double now = Planetarium.GetUniversalTime();
        //    double homeSMA = home.orbit.semiMajorAxis;

        //    Vector3d sunPos = FlightGlobals.Bodies[0].position;
        //    Vector3d bodyPos = body.getPositionAtUT(now);

        //    Vector3d direction = sunPos - bodyPos;
        //    double sunDistance = direction.magnitude;
        //    direction = direction.normalized * body.Radius;  //Vector3.ClampMagnitude(direction, (float)body.Radius);
        //    double lat = body.GetLatitude(direction);
        //    double lon = body.GetLongitude(direction);

        //    double powerFactor = homeSMA / sunDistance * homeSMA / sunDistance;
        //    autoRoveUtils.debugWarning("hit at " + sanitizeDegrees(lat) + "/" + sanitizeDegrees(lon));
        //    if (latitude < lat - 90 || latitude > lat + 90)
        //    {
        //        autoRoveUtils.debugWarning("wrong latitude, no power!");
        //        powerFactor = 0;
        //        return powerFactor;
        //    }
        //    else if (longitude < lon - 90 || longitude > lon + 90)
        //    {
        //        autoRoveUtils.debugWarning("wrong longitude, no power!");
        //        powerFactor = 0;
        //        return powerFactor;
        //    }
        //    else
        //    {
        //        autoRoveUtils.debugWarning("everything ok, got power!");
        //        return powerFactor;
        //    }
        //}
    }
}
