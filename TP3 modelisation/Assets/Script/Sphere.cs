using TreeEditor;
using UnityEngine;

//Exercice 1 :
public class Sphere : MonoBehaviour
{
    public Vector3 center = Vector3.zero;
    public float radius = 5f;
    public int maxDepth = 4;

    public GameObject cubePrefab; 

    private Octree root;

    void Start()
    {
       
        float size = radius * 2f;
        Bounds rootBounds = new Bounds(center, new Vector3(size, size, size));
        root = new Octree(rootBounds);

        BuildOctree(root, 0);

        SpawnCubes(root);
    }

    void BuildOctree(Octree node, int depth)
    {
        Bounds b = node.bounds;

        if (IsCompletelyOutside(b))
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
        if (IsCompletelyInside(b))
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
                Instantiate(cubePrefab, node.bounds.center, Quaternion.identity,
                    this.transform).transform.localScale = node.bounds.size;
            }
            return;
        }

        for (int i = 0; i < 8; i++)
        {
            SpawnCubes(node.children[i]);
        }
    }

    bool IsCompletelyOutside(Bounds b)
    {
        Vector3 p = center; 
        Vector3 min = b.min;
        Vector3 max = b.max;

        float d2 = 0f;


        if (p.x < min.x)
        {
            float s = p.x - min.x;
            d2 += s * s;
        }
        else if (p.x > max.x)
        {
            float s = p.x - max.x;
            d2 += s * s;
        }


        if (p.y < min.y)
        {
            float s = p.y - min.y;
            d2 += s * s;
        }
        else if (p.y > max.y)
        {
            float s = p.y - max.y;
            d2 += s * s;
        }


        if (p.z < min.z)
        {
            float s = p.z - min.z;
            d2 += s * s;
        }
        else if (p.z > max.z)
        {
            float s = p.z - max.z;
            d2 += s * s;
        }

        return d2 > radius * radius;
    }

    bool IsCompletelyInside(Bounds b)
    {
        Vector3[] corners = new Vector3[8];
        Vector3 min = b.min;
        Vector3 max = b.max;

        corners[0] = new Vector3(min.x, min.y, min.z);
        corners[1] = new Vector3(min.x, min.y, max.z);
        corners[2] = new Vector3(min.x, max.y, min.z);
        corners[3] = new Vector3(min.x, max.y, max.z);
        corners[4] = new Vector3(max.x, min.y, min.z);
        corners[5] = new Vector3(max.x, min.y, max.z);
        corners[6] = new Vector3(max.x, max.y, min.z);
        corners[7] = new Vector3(max.x, max.y, max.z);

        float r2 = radius * radius;

        for (int i = 0; i < 8; i++)
        {
            if ((corners[i] - center).sqrMagnitude > r2)
                return false;
        }

        return true;
    }

}
