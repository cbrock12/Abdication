using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridMaker : MonoBehaviour
{

    // Holds the grid size
    public Vector2 gridSize;

    // Holds the grid's row and column
    public int gridX;
    public int gridY;

    // Holds the grid of nodes
    public Node[,] grid;

    // Holds the radius of each node
    public float radius;

    // Holds the path of nodes
    public List<Node> path;

    // Method called before first frame update
    public void Start()
    {

        // Calculate number of blocks in grid's rows and columns
        gridX = Mathf.RoundToInt(gridSize.x / (radius * 2));
        gridY = Mathf.RoundToInt(gridSize.y / (radius * 2));

        // Call CreateGrid method
        CreateGrid();
    }

    // Method for grid creation
    public void CreateGrid()
    {

        // Create new node grid
        grid = new Node[gridX, gridY];

        // Calculate bottom left position
        Vector2 bottomLeft = (Vector2) transform.position - Vector2.right * gridSize.x / 2 - Vector2.up * gridSize.y / 2;

        // Run through the rows
        for (int i = 0; i < gridX; i++)
        {

            // Run through the columns
            for (int j = 0; j < gridY; j++)
            {

                // Calculate location for the node
                Vector2 nodePoint = bottomLeft + Vector2.right * (i * (radius * 2) + radius) + Vector2.up * (j * (radius * 2) + radius);

                // Place the new node in its calculated position;
                grid[i, j] = new Node(nodePoint, i, j);
            }
        }
    }

    // Method for retrieving a node from the given position
    public Node NodeFromPosition(Vector2 position)
    {

        // Node's x and y ponts
        float xPoint = ((position.x + gridSize.x / 2) / gridSize.x);
        float yPoint = ((position.y + gridSize.y / 2) / gridSize.y);

        // Clamp said points
        xPoint = Mathf.Clamp01(xPoint);
        yPoint = Mathf.Clamp01(yPoint);

        // Round the points
        int x = Mathf.RoundToInt((gridX - 1) * xPoint);
        int y = Mathf.RoundToInt((gridY - 1) * yPoint);

        // return node within the grid
        return grid[x, y];
    }

    // Method for retrieving a list of neighboring nodes using the given node
    public List<Node> Neighbors(Node node)
    {

        // Create the node list
        List<Node> neighbors = new List<Node>();

        // Run horizontal checks
        for (int i = -1; i <= 1; i++)
        {

            // Run vertical checks
            for (int j = -1; j <= 1; j++)
            {

                // Skip past the given node
                if (i == 0 && j == 0)
                {

                    continue;
                }

                // Retrieve neighbor node position
                int x = node.gridX + i;
                int y = node.gridY + j;

                // Continue if node position is within bounds
                if (x >= 0 && x < gridX && y >= 0 && y < gridY)
                {

                    // Add the node from retrieved position to node list
                    neighbors.Add(grid[x, y]);
                }
            }
        }

        // Return the node list
        return neighbors;
    }
}
