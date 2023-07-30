using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    Material originMaterial;
    Material activeMaterial;
    MeshRenderer[] meshRenderers;
    List<Material> materials = new List<Material>();

    void Start()
    {
        activeMaterial = Managers.Resource.Load<Material>("Prefabs/Active");
        originMaterial = Managers.Resource.Load<Material>("Prefabs/Crane");
        meshRenderers = GetComponentsInChildren<MeshRenderer>();
    }

    public void Hightlight(bool active)
    {
        materials.Clear();
        foreach (MeshRenderer renderer in meshRenderers)
        {
            if (materials.Count == 0)
            {
                materials.Add(renderer.material);
            }
            if (active)
            {
                materials.Add(activeMaterial);
                renderer.materials = materials.ToArray();
                // renderer.materials = activeMaterial;
            }
            else
            {
                renderer.materials = materials.ToArray();
            }
        }
    }
}
