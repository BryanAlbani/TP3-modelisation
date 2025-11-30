using UnityEngine;

//Exercice 3 :
public class Operator : MonoBehaviour
{
    public enum BoolOp { Union, Intersection }

    [Header("Spheres")]
    public Vector3[] centers;
    public float[] radii;

    [Header("Octree")]
    public int maxDepth = 4;
    public GameObject cubePrefab;

    [Header("Operateur booleen")]
    public BoolOp operation = BoolOp.Union;

    private Octree root;

    void Start()
    {
        if (centers == null || radii == null || centers.Length == 0 || radii.Length != centers.Length)
        {
            Debug.LogError("Operator : configure centers et radii (le meme nombre) dans l'inspecteur");
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

    bool BoxOutsideSphere(Bounds b, Vector3 c, float r)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;

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

        return d2 > r * r;
    }

    bool BoxInsideSphere(Bounds b, Vector3 c, float r)
    {
        Vector3 min = b.min;
        Vector3 max = b.max;

        Vector3[] corners = new Vector3[8];
        corners[0] = new Vector3(min.x, min.y, min.z);
        corners[1] = new Vector3(min.x, min.y, max.z);
        corners[2] = new Vector3(min.x, max.y, min.z);
        corners[3] = new Vector3(min.x, max.y, max.z);
        corners[4] = new Vector3(max.x, min.y, min.z);
        corners[5] = new Vector3(max.x, min.y, max.z);
        corners[6] = new Vector3(max.x, max.y, min.z);
        corners[7] = new Vector3(max.x, max.y, max.z);

        float r2 = r * r;

        for (int i = 0; i < 8; i++)
        {
            if ((corners[i] - c).sqrMagnitude > r2)
            {
                return false;
            }
                
        }

        return true;
    }


    void BuildOctree(Octree node, int depth)
    {
        Bounds b = node.bounds;


        if (operation == BoolOp.Union)
        {
            bool outsideAll = true;
            bool insideOne = false;

            for (int i = 0; i < centers.Length; i++)
            {
                if (!BoxOutsideSphere(b, centers[i], radii[i]))
                    outsideAll = false;

                if (BoxInsideSphere(b, centers[i], radii[i]))
                    insideOne = true;
            }

            if (outsideAll)
            {
                node.isLeaf = true;
                node.isFull = false;
                return;
            }

            if (insideOne && depth < maxDepth)
            {
                node.isLeaf = true;
                node.isFull = true;
                return;
            }
        }
        else 
        {
            for (int i = 0; i < centers.Length; i++)
            {
                if (BoxOutsideSphere(b, centers[i], radii[i]))
                {
                    node.isLeaf = true;
                    node.isFull = false;
                    return;
                }
            }

            bool insideAll = true;
            for (int i = 0; i < centers.Length; i++)
            {
                if (!BoxInsideSphere(b, centers[i], radii[i]))
                {
                    insideAll = false;
                    break;
                }
            }

            if (insideAll && depth < maxDepth)
            {
                node.isLeaf = true;
                node.isFull = true;
                return;
            }
        }

        if (depth >= maxDepth)
        {
            Vector3 centerBox = b.center;

            if (operation == BoolOp.Union)
            {
                bool inAny = false;
                for (int i = 0; i < centers.Length; i++)
                {
                    if ((centerBox - centers[i]).sqrMagnitude <= radii[i] * radii[i])
                    {
                        inAny = true;
                        break;
                    }
                }
                node.isLeaf = true;
                node.isFull = inAny;
            }
            else 
            {
                bool inAll = true;
                for (int i = 0; i < centers.Length; i++)
                {
                    if ((centerBox - centers[i]).sqrMagnitude > radii[i] * radii[i])
                    {
                        inAll = false;
                        break;
                    }
                }
                node.isLeaf = true;
                node.isFull = inAll;
            }

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
                Instantiate(cubePrefab, node.bounds.center, Quaternion.identity, this.transform).transform.localScale = node.bounds.size;
            }
            return;
        }

        for (int i = 0; i < 8; i++)
        {
            SpawnCubes(node.children[i]);
        }
            
    }
}
