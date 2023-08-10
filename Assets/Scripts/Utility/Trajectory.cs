using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public static class Trajectory
{

    public static void BuildLerpPoints(Vector3 start, Vector3 end, out Vector3 second, out Vector3 third, float mod)
    {
        second = start + (start - (start + Random.insideUnitSphere * mod));
        third = end + (end - (end + Random.insideUnitSphere * mod));
    }

    public static Vector3 BezierCurve(Vector3 start, Vector3 second, Vector3 third, Vector3 end, float speed, float distanceToEnemy, ref float lerpT)
    {
        lerpT += speed * Time.deltaTime / distanceToEnemy;
        Vector3 a = Vector3.Lerp(start, second, lerpT);
        Vector3 b = Vector3.Lerp(second, third, lerpT);
        Vector3 c = Vector3.Lerp(third, end, lerpT);

        Vector3 ab = Vector3.Lerp(a, b, lerpT);
        Vector3 bc = Vector3.Lerp(b, c, lerpT);

        return Vector3.Lerp(ab, bc, lerpT);
    }

    public static Vector3 SimpleTrajectory(Vector3 from, Vector3 to, float speed)
    {
        Vector3 moveDirection = Vector3.Normalize(to - from);
        return from + moveDirection * speed * Time.deltaTime;
    }

    public static Vector3 ArcTrajectory(Vector3 from, Vector3 to, Vector3 mainMovementVector, float speed)
    {
        float projectedMagnitude = Vector3.Dot(to-from, mainMovementVector)/mainMovementVector.magnitude;
        float distanceTravaledNormilized = projectedMagnitude / mainMovementVector.magnitude;
        Vector3 trajectoryPosition = from + mainMovementVector.normalized * speed * Time.deltaTime;

        trajectoryPosition += new Vector3(0, speed*Time.deltaTime*(distanceTravaledNormilized-0.61f)*3, 0);


        return trajectoryPosition;
    }

    public static Vector3 ArcTrajectoryMovingTarrget(Vector3 from, Vector3 to, Vector3 projectileSpawnPoint, float speed)
    {
        Vector3 trajectoryPosition = from + (to-from).normalized * speed * Time.deltaTime;

        return trajectoryPosition;
    }
}

public enum TrajectoryType
{
    Simple,
    BezierCurve,
    ArcTrajectory,
    AcrTrajectoryMovingTarget
}
