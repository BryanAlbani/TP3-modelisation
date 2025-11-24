using UnityEngine;

//Exercice 2 :
public class MoreSphere : MonoBehaviour
{
    [Header("Sphères")]
    public Vector3[] centers;   
    public float[] radii;       

    [Header("Octree")]
    public int maxDepth = 4;
    public GameObject cubePrefab;

    private Octree root;

    void Start()
    {
        if (centers == null || radii == null || centers.Length == 0 || radii.Length != centers.Length)
        {
            Debug.LogError("MoreSphere : configure centers et radii (le même nombre) dans l'inspecteur.");
            return;
        }


        Vector3 globalMin = centers[0] - Vector3.one * radii[0];
        Vector3 globalMax = centers[0] + Vector3.one * radii[0];

        for (int i = 1; i < centers.Length; i++)
        {
            Vector3 min = centers[i] - Vector3.one * radii[i];
            Vector3 max = centers[i] + Vector3.one * radii[i];

            globalMin = Vector3.Min(globalMin, min);
            globalMax = Vector3.Max(globalMax, max);
        }

        Vector3 size = globalMax - globalMin;
        Vector3 centerBox = (globalMin + globalMax) * 0.5f;

        Bounds rootBounds = new Bounds(centerBox, size);
        root = new Octree(rootBounds);


        BuildOctree(root, 0);
        SpawnCubes(root);
    }

    bool BoxOutsideAllSpheres(Bounds b)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;

        for (int i = 0; i < centers.Length; i++)
        {
            Vector3 c = centers[i];
            float r = radii[i];
            float r2 = r * r;

            float d2 = 0f;

            if (c.x < min.x)
            {
                float s = c.x - min.x;
                d2 += s * s;
            }
            else if (c.x > max.x)
            {
                float s = c.x - max.x;
                d2 += s * s;
            }

            if (c.y < min.y)
            {
                float s = c.y - min.y;
                d2 += s * s;
            }
            else if (c.y > max.y)
            {
                float s = c.y - max.y;
                d2 += s * s;
            }

            if (c.z < min.z)
            {
                float s = c.z - min.z;
                d2 += s * s;
            }
            else if (c.z > max.z)
            {
                float s = c.z - max.z;
                d2 += s * s;
            }

           
            if (d2 <= r2)
                return false; 
        }

        return true; 
    }

    void BuildOctree(Octree node, int depth)
    {
        Bounds b = node.bounds;

        if (BoxOutsideAllSpheres(b))
        {
            node.isLeaf = true;
            node.isFull = false;
            return;
        }

        if (depth >= maxDepth)
        {
            node.isLeaf = true;
            node.isFull = true;
            return;
        }

        node.Subdivide();
        for (int i = 0; i < 8; i++)
        {
            BuildOctree(node.children[i], depth + 1);
        }
    }

    void SpawnCubes(Octree node)
    {
        if (node.isLeaf)
        {
            if (node.isFull)
            {
                Instantiate(cubePrefab, node.bounds.center, Quaternion.identity, this.transform)
                    .transform.localScale = node.bounds.size;
            }
            return;
        }

        for (int i = 0; i < 8; i++)
            SpawnCubes(node.children[i]);
    }
}
