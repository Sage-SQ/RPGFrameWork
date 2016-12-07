using System;

using UnityEngine;

 

public static class UEx

{

    /// <summary>

    /// gets the square distance between two vector3 positions. this is much faster that Vector3.distance.

    /// </summary>

    /// <param name="first">first point</param>

    /// <param name="second">second point</param>

    /// <returns>squared distance</returns>

    public static float SqrDistance(Vector3 first, Vector3 second)

    {

        return Vector3.SqrMagnitude(first - second);

    }

 

    public static Vector3 MidPoint(Vector3 first, Vector3 second)

    {

        return new Vector3((first.x + second.x) * 0.5f, (first.y + second.y) * 0.5f, (first.z + second.z) * 0.5f);

    }

    /// <summary>

    /// get the square distance from a point to a line segment.

    /// </summary>

    /// <param name="lineP1">line segment start point</param>

    /// <param name="lineP2">line segment end point</param>

    /// <param name="point">point to get distance to</param>

    /// <param name="closestPoint">set to either 1, 2, or 4, determining which end the point is closest to (p1, p2, or the middle)</param>

    /// <returns></returns>

    public static float SqrLineDistance(Vector3 lineP1, Vector3 lineP2, Vector3 point, out int closestPoint)

    {

 

        Vector3 v = lineP2 - lineP1;

        Vector3 w = point - lineP1;

 

        float c1 = Vector3.Dot(w, v);

 

        if (c1 <= 0) //closest point is p1

        {

            closestPoint = 1;

            return SqrDistance(point, lineP1);

        }

 

        float c2 = Vector3.Dot(v, v);

        if (c2 <= c1) //closest point is p2

        {

            closestPoint = 2;

            return SqrDistance(point, lineP2);

        }

 

 

        float b = c1 / c2;

 

        Vector3 pb = lineP1 + b * v;

        {

            closestPoint = 4;

            return SqrDistance(point, pb);

        }

    }

}