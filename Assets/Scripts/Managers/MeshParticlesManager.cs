using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshParticlesManager : MonoBehaviour
{
    public static MeshParticlesManager Instance;

    private void Awake()
    {
        Instance = this;
    }
}
