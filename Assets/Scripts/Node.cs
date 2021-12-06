using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{

    // Holds the position of the node
    public Vector2 position;

    // Holds the node's row and column values
    public int gridX;
    public int gridY;

    // Holds the previous node of the current node
    public Node parent;

    // Holds the g and h costs
    public int g;
    public int h;

    // Finds the f cost
    public int F()
    {

        // returns calculation
        return g + h;
    }

    // Node constructor
    public Node(Vector2 pos, int x, int y)
    {

        // Node's position
        position = pos;

        // Node's row and column
        gridX = x;
        gridY = y;
    }
}
