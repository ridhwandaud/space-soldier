using UnityEngine;
using System.Collections.Generic;
using SpriteTile;
using Priority_Queue;

public class AStar : MonoBehaviour {
    private static int[,] World;
    private static Node[] Nodes;
    private static Node[] Visited;
    private static HeapPriorityQueue<Node> Frontier;
    private static List<Int2> Neighbors;
    private static List<Node> Result;

    // Struct would be better. Structs are treated like primitives and are created on the stack.
    public class Node : PriorityQueueNode
    {
        public Node parent;
        public int index;
        public int f;
        public int g;
        public Int2 point;

        public Node(Node parent, Int2 point, int index)
        {
            g = 0;
            this.parent = parent;
            this.point = point;
            this.index = index;
        }
    }

    public static void Init(int[,] world)
    {
        World = world;
        InitNodes();
        Frontier = new HeapPriorityQueue<Node>(300);
        Result = new List<Node>(300);
        Neighbors = new List<Int2>(10);
        Visited = new Node[World.GetLength(0) * World.GetLength(1)];
    }

    static void findNeighbors(Int2 point)
    {
        Neighbors.Clear();

        int aboveY = point.y - 1,
            belowY = point.y + 1,
            rightX = point.x + 1,
            leftX = point.x - 1;

        if (aboveY >= 0 && canWalkHere(point.x, aboveY))
        {
            Neighbors.Add(new Int2(point.x, aboveY));
            if (leftX >= 0 && canWalkHere(leftX, point.y) && canWalkHere(leftX, aboveY))
            {
                Neighbors.Add(new Int2(leftX, aboveY));
            }
            if (rightX < World.GetLength(1) && canWalkHere(rightX, point.y) && canWalkHere(rightX, aboveY))
            {
                Neighbors.Add(new Int2(rightX, aboveY));
            }
        }
        if (belowY < World.GetLength(0) && canWalkHere(point.x, belowY))
        {
            Neighbors.Add(new Int2(point.x, belowY));
            if (leftX >= 0 && canWalkHere(leftX, point.y) && canWalkHere(leftX, belowY))
            {
                Neighbors.Add(new Int2(leftX, belowY));
            }
            if (rightX < World.GetLength(1) && canWalkHere(rightX, point.y) && canWalkHere(rightX, belowY))
            {
                Neighbors.Add(new Int2(rightX, belowY));
            }
        }
        if (leftX >= 0 && canWalkHere(leftX, point.y))
        {
            Neighbors.Add(new Int2(leftX, point.y));
        }
        if (rightX < World.GetLength(1) && canWalkHere(rightX, point.y))
        {
            Neighbors.Add(new Int2(rightX, point.y));
        }
    }

    static bool canWalkHere(int x, int y)
    {
        // y is row, x is column. think about it.
        // and of course, array is world[row, col]
        return LoadLevel.FloorIndices.Contains(World[y, x]);
    }

    static int manhattanDistance(Int2 point, Int2 goal)
    {
        return Mathf.Abs(goal.x - point.x) + Mathf.Abs(goal.y - point.y);
    }

    public static int vectorManhattanDistance(Vector2 point, Vector2 goal)
    {
        return (int)(Mathf.Abs(goal.x - point.x) + Mathf.Abs(goal.y - point.y));
    }

    static int calculatePointIndex(Int2 point) {
        return point.x + (point.y * World.GetLength(1));
    }

    static void InitNodes ()
    {
        Nodes = new Node[World.GetLength(0) * World.GetLength(1)];

        for (int i = 0; i < Nodes.Length; i++)
        {
            Nodes[i] = new Node(null, Int2.zero, i);
        }
    }

    static Node createNode (Node parent, Int2 point, int index)
    {
        Node node = Nodes[index];
        node.parent = parent;
        node.point = point;
        node.f = 0;
        node.g = 0;
        return node;
    }

    static void ResetState()
    {
        for (int i = 0; i < Visited.Length; i++) {
            Visited[i] = null;
        }

        Frontier.Clear();
        Result.Clear();
        Neighbors.Clear();

    }

    public static List<Node> calculatePath(Int2 start, Int2 end)
    {
        ResetState();

        Node startNode = createNode(null, start, calculatePointIndex(start));
        Node targetNode = createNode(null, end, calculatePointIndex(end));

        Visited[startNode.index] = startNode;

        Frontier.Enqueue(startNode, 0); // dummy value for priority since it will be popped immediately.

        // Continue algorithm until there are no more open nodes.
        while (Frontier.Count > 0)
        {
            Node current = Frontier.Dequeue();

            // If the popped node is the target node, then you are done.
            if (current.index == targetNode.index)
            {
                Result.Clear();
                Result.Add(current);

                Node nodeInShortestPath = current.parent;
                
                while (nodeInShortestPath != null)
                {
                    Result.Add(nodeInShortestPath);
                    nodeInShortestPath = nodeInShortestPath.parent;
                }

                Result.Reverse();
                break;
            }
            else
            {
                findNeighbors(current.point);

                for (int i = 0; i < Neighbors.Count; i++) {
                    Int2 neighbor = Neighbors[i];
                    int pointIndex = calculatePointIndex(neighbor);

                    Node neighborNode = Visited[pointIndex] != null ?
                        Visited[pointIndex] : createNode(null, neighbor, pointIndex);
                    int newNeighborCost = current.g + manhattanDistance(neighbor, current.point);

                    if (Visited[neighborNode.index] == null || neighborNode.g > newNeighborCost)
                    {
                        neighborNode.g = newNeighborCost;
                        neighborNode.f = neighborNode.g + manhattanDistance(neighbor, targetNode.point);
                        neighborNode.parent = current;

                        if (!Frontier.Contains(neighborNode))
                        {
                            Frontier.Enqueue(neighborNode, neighborNode.f);
                        }
                        else
                        {
                            Frontier.UpdatePriority(neighborNode, neighborNode.f);
                        }
                        
                        Visited[neighborNode.index] = neighborNode;
                    }
                }
            }
        }

        // If frontier is emptied out and the target hasn't been reached, then the path is blocked and no shortest path exists.
        return Result;
    }

    static void test()
    {
        World = new int[9, 5] {
        {0, 0, 0, 0, 0} ,
        {0, 0, 1, 1, 0}, 
        {0, 1, 1, 0, 0}, 
        {0, 1, 1, 0, 0}, 
        {0, 0, 1, 0, 0}, 
        {0, 0, 1, 0, 0}, 
        {0, 0, 1, 0, 0}, 
        {0, 0, 1, 0, 0}, 
        {0, 0, 0, 0, 0}
        };

        List<Node> path = calculatePath(new Int2(2, 1), new Int2(2, 5));

        if (path == null)
        {
            Debug.Log("path is null.");
        }
        else
        {
            foreach (Node node in path)
            {
                Debug.Log(node.point.ToString());
            }
        }
    }

    public static Int2 positionToArrayIndices(Vector2 position)
    {
        // Need to add half of the tile size because the map origin is at the center of tile 0, 0.
        return new Int2((int)(position.x + GameSettings.TileSize / 2f), (int)(position.y + GameSettings.TileSize / 2f));
    }

    public static Vector2 positionToArrayIndicesVector(Vector2 position)
    {
        return new Vector2((int)(position.x + GameSettings.TileSize / 2f), (int)(position.y + GameSettings.TileSize / 2f));
    }

    public static Vector2 arrayIndicesToPosition(Int2 point)
    {
        return new Vector2(point.x, point.y);
    }
}
