using UnityEngine;
using System.Collections.Generic;

public class Octree
{
    public Bounds bounds;
    public Octree[] children;
    public bool isLeaf;
    public bool isFull; 

    public Octree(Bounds b)
    {
        bounds = b;
        isLeaf = true;
        isFull = false;
        children = null;
    }

    public void Subdivide()
    {
        if (!isLeaf) return;

        children = new Octree[8];
        isLeaf = false;

        Vector3 size = bounds.size / 2f;
        Vector3 min = bounds.min;

        int index = 0;
        for (int x = 0; x < 2; x++)
        {
            for (int y = 0; y < 2; y++)
            {
                for (int z = 0; z < 2; z++)
                {
                    Vector3 childMin = min + Vector3.Scale(size, new Vector3(x, y, z));
                    Bounds childBounds = new Bounds(
                        childMin + size / 2f,
                        size
                    );
                    children[index++] = new Octree(childBounds);
                }
            }
        }
    }

}
