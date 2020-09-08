using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionRendererSorter : MonoBehaviour
{
    [SerializeField] int sortingOrderBase = 5000;
    [SerializeField] int offset = 0;
    [SerializeField] bool runOnlyOnce = false;

    Renderer m_renderer;

    private void Awake()
    {
        m_renderer = GetComponentInChildren<Renderer>();
    }

    private void LateUpdate()
    {
        m_renderer.sortingOrder = (int)(sortingOrderBase - transform.position.y - offset);
        if (runOnlyOnce)
            Destroy(this);
    }
}
