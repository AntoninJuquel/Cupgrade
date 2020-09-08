using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    [System.Serializable]
    class PowerUp
    {
        public GameObject item;
        public float lootChance;
    }

    public static LootManager Instance;
    [SerializeField] PowerUp[] powerUps;
    [SerializeField] GameObject[] lootableWeapon;
    private void Awake()
    {
        Instance = this;
    }
    public void LootWeapon(Transform chest)
    {
        float chance = Random.value;
        GameObject gunToSpawn;
        if (chance < .25f)
            gunToSpawn = lootableWeapon[0];
        else if (chance < .5f)
            gunToSpawn = lootableWeapon[1];
        else if (chance < .75f)
            gunToSpawn = lootableWeapon[2];
        else
            gunToSpawn = lootableWeapon[3];
        

        Instantiate(gunToSpawn, chest.position, Quaternion.identity).name = gunToSpawn.name;
        StartCoroutine(CameraController.Instance.Shake(.1f, .1f));
        Destroy(chest.gameObject);
    }

    public void LootPowerUp(Vector3 position)
    {
        bool spawned = false;
        foreach (PowerUp powerUp in powerUps)
        {
            float chance = Random.value;
            if (spawned)
                return;
            if(chance < powerUp.lootChance)
            {
                Instantiate(powerUp.item, position, Quaternion.identity,LevelGenerator.Instance.GetLevelHolder());
                spawned = true;
            }    
        }
    }
}
