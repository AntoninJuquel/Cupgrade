using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;
    private void Awake()
    {
        Instance = this;
    }
    public GameObject Create(string projectile,Vector3 position,Quaternion rotation, int damage, float maxDuration, float speed, float explosionRadius,float explosionForce, float rotateSpeed, float detectionRadius, int reflections)
    {
        GameObject p = ProjectilePool.Instance.SpawnFromPool(projectile, position, rotation);

        p.GetComponent<ProjectileCollision>().Setup(damage, maxDuration, explosionRadius,explosionForce, reflections);
        p.GetComponent<ProjectileMovement>().Setup(speed,rotateSpeed, detectionRadius);

        return p;
    }
}
