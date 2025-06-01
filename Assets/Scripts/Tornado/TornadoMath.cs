using UnityEngine;
using System.Collections;

public static class TornadoMath 
{
    public static float GetAngle(float x, float y)
    {
        float angle = (Mathf.Atan2(y, x) / Mathf.PI) * 180;

        if (angle < 0f) {
            angle += 360f;
        }

        return angle;
    }


    public static Vector3 GetParticlePos(Vector3 centerPos, float angle, float radius, float rotationSpeed)
    {
        angle += rotationSpeed * Time.deltaTime;

        float angleInRad = angle * Mathf.Deg2Rad;
        float newX = centerPos.x + radius * Mathf.Cos(angleInRad);
        float newZ = centerPos.z + radius * Mathf.Sin(angleInRad);

        Vector3 newPos = new Vector3(newX, 0f, newZ);

        return newPos;
    }



    public static Vector3 GetParticleVel(float angle, float radius, float rotationSpeed)
    {
        angle += rotationSpeed * Time.deltaTime;

        float angleInRad = angle * Mathf.Deg2Rad;
        float newX = Mathf.Cos(angleInRad) - radius * Mathf.Sin(angleInRad);
        float newZ = Mathf.Sin(angleInRad) + radius * Mathf.Cos(angleInRad);

        Vector3 newVel = new Vector3(newX, 0f, newZ);

        return newVel;
    }


    public static float CalculateProgress(Vector3 from, Vector3 to, Vector3 current)
    {
        Vector3 a = from;
        Vector3 b = to;
        Vector3 c = current;


        Vector3 ab = b - a;
        Vector3 ac = c - a;

        float progress = Vector3.Dot(ac, ab) / ab.sqrMagnitude;

        return progress;
    }


    public static Vector3 CheckRadius(Vector3 particlePos, Vector3 centerPos, float wantedRadius)
    {
        particlePos.y = 0f;
        centerPos.y = 0f;

        float currentRadius = (particlePos - centerPos).magnitude;

        Vector3 dir = (particlePos - centerPos).normalized;

        if (currentRadius < wantedRadius) {
            particlePos += dir * (wantedRadius - currentRadius);
        }

        return particlePos;
    }
}
