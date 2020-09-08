using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleManager : MonoBehaviour
{
    public static ParticleManager Instance;

    private void Awake()
    {
        Instance = this;
    }

    public void SpawnParticle(string name,Vector3 position,Quaternion rotation)
    {
        GameObject explosion = ParticlePool.Instance.SpawnFromPool(name, position, rotation);
        StartCoroutine(DestroyParticle(explosion, .1f));
    }
    IEnumerator DestroyParticle(GameObject particle, float time)
    {
        yield return new WaitForSeconds(time);
        particle.SetActive(false);
    }
}
