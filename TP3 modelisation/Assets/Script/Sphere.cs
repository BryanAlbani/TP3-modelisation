using TreeEditor;
using UnityEngine;

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
       
        Vector3 c = node.bounds.center;
        float dist2 = (c - center).sqrMagnitude;

        if (dist2 <= radius * radius)
        {
            
            node.isFull = true;
        }

        if (depth >= maxDepth)
        {
          
            node.isLeaf = true;
            return;
        }

       
        float halfDiag = node.bounds.extents.magnitude;
        if (dist2 > (radius + halfDiag) * (radius + halfDiag))
        {
          
            node.isLeaf = true;
            node.isFull = false;
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
}
