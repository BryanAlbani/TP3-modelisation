using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Voxel : MonoBehaviour
{
    public static float visibilityThreshold = 0.5f;

    public static List<Voxel> allVoxels = new List<Voxel>();

    [Range(0f, 1f)]
    public float potential = 1f;

    Renderer rend;

    void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    void OnEnable()
    {
        allVoxels.Add(this);
        UpdateVisibility();
    }

    void OnDisable()
    {
        allVoxels.Remove(this);
    }

    public void Init(float initialPotential)
    {
        potential = Mathf.Clamp01(initialPotential);
        UpdateVisibility();
    }

    public void AddPotential(float delta)
    {
        potential = Mathf.Clamp01(potential + delta);
        UpdateVisibility();
    }

    public void UpdateVisibility()
    {
        if (rend != null)
            rend.enabled = (potential >= visibilityThreshold);
    }

    public static void ModifyAllInRadius(Vector3 center, float radius, float delta)
    {
        float r2 = radius * radius;

        foreach (var v in allVoxels)
        {
            if (v == null) continue;

            if ((v.transform.position - center).sqrMagnitude <= r2)
            {
                v.AddPotential(delta);
            }
        }
    }
}
