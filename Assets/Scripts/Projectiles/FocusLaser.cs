using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusLaser : Laser
{
    public float beamLenght;
    private float focusModifier;
    protected override void LaserHit()
    {
        Vector3 directionToTarget = target.GetPosition() - laserBody.GetPosition(0);
        Vector3 focusPoint = Vector3.Normalize(directionToTarget) * beamLenght + laserBody.GetPosition(0);
        laserBody.SetPosition(1, Vector3.Lerp(laserBody.GetPosition(1), focusPoint, .3f));
        focusModifier = (beamLenght - Vector3.Distance(focusPoint, target.GetPosition()))/beamLenght;
    }

    public override float LaserDamage(float damage)
    {
        return damage * focusModifier;
    }
}
