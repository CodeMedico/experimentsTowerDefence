using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NovaTower : Tower
{
    private Nova nova;

    protected override void CreateAndSetProjectile()
    {
        foreach (ITarget enemyInTarget in enemiesInTarget)
        {
            if (enemyInTarget != null && nova == null)
            {
                nova = Instantiate(projectile as Nova, new Vector3(transform.position.x,0.02f,transform.position.z),
                    Quaternion.Euler(90,0,0), transform);
                nova.SetLayerMask(layerMaskEnemy);
                nova.transform.localScale = new Vector3(towerSO.TowerRange, towerSO.TowerRange, towerSO.TowerRange)*2f;
            }
            else
            {
                nova.gameObject.SetActive(true);
                nova.IsDamaged(false);
                nova.NovaSwitch(false);
            }
        }
    }

}
