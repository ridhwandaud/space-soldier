using UnityEngine;
using System.Collections.Generic;
using SpriteTile;
using Priority_Queue;

// TODO: This class needs heavy optimization - creates way too much garbage.
public class AStar : MonoBehaviour {
    public static int[,] world;

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

    void Awake () {
        //test();
    }

    static List<Int2> findNeighbors(Int2 point)
    {
        List<Int2> result = new List<Int2>();

        int aboveY = point.y - 1,
            belowY = point.y + 1,
            rightX = point.x + 1,
            leftX = point.x - 1;

        if (aboveY >= 0 && canWalkHere(point.x, aboveY))
        {
            result.Add(new Int2(point.x, aboveY));
            if (leftX >= 0 && canWalkHere(leftX, point.y) && canWalkHere(leftX, aboveY))
            {
                result.Add(new Int2(leftX, aboveY));
            }
            if (rightX < world.GetLength(1) && canWalkHere(rightX, point.y) && canWalkHere(rightX, aboveY))
            {
                result.Add(new Int2(rightX, aboveY));
            }
        }
        if (belowY < world.GetLength(0) && canWalkHere(point.x, belowY))
        {
            result.Add(new Int2(point.x, belowY));
            if (leftX >= 0 && canWalkHere(leftX, point.y) && canWalkHere(leftX, belowY))
            {
                result.Add(new Int2(leftX, belowY));
            }
            if (rightX < world.GetLength(1) && canWalkHere(rightX, point.y) && canWalkHere(rightX, belowY))
            {
                result.Add(new Int2(rightX, belowY));
            }
        }
        if (leftX >= 0 && canWalkHere(leftX, point.y))
        {
            result.Add(new Int2(leftX, point.y));
        }
        if (rightX < world.GetLength(1) && canWalkHere(rightX, point.y))
        {
            result.Add(new Int2(rightX, point.y));
        }

        return result;
    }

    static bool canWalkHere(int x, int y)
    {
        // y is row, x is column. think about it.
        // and of course, array is world[row, col]
        return LoadLevel.FloorIndices.Contains(world[y, x]);
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
        return point.x + (point.y * world.GetLength(1));
    }

    // Get rid of all of the news, since they are creating garbage. Reuse the objects.
    public static List<Node> calculatePath(Int2 start, Int2 end)
    {
        Node startNode = new Node(null, start, calculatePointIndex(start));
        Node targetNode = new Node(null, end, calculatePointIndex(end));

        Node[] visited = new Node[world.GetLength(0) * world.GetLength(1)];

        HeapPriorityQueue<Node> frontier = new HeapPriorityQueue<Node>(300);
        List<Node> result = new List<Node>();
        List<Int2> neighbors = null;
        frontier.Enqueue(startNode, 0); // dummy value for priority since it will be popped immediately.

        // Continue algorithm until there are no more open nodes.
        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            // If the popped node is the target node, then you are done.
            if (current.index == targetNode.index)
            {
                result.Clear();
                result.Add(current);

                Node nodeInShortestPath = current.parent;
                
                while (nodeInShortestPath != null)
                {
                    result.Add(nodeInShortestPath);
                    nodeInShortestPath = nodeInShortestPath.parent;
                }

                result.Reverse();
                break;
            }
            else
            {
                neighbors = findNeighbors(current.point);

                for (int i = 0; i < neighbors.Count; i++) {
                    Int2 neighbor = neighbors[i];
                    int pointIndex = calculatePointIndex(neighbor);

                    Node neighborNode = visited[pointIndex] != null ?
                        visited[pointIndex] : new Node(current, neighbor, pointIndex);
                    int newNeighborCost = current.g + manhattanDistance(neighbor, current.point);

                    if (visited[neighborNode.index] == null || neighborNode.g > newNeighborCost)
                    {
                        neighborNode.g = newNeighborCost;
                        neighborNode.f = neighborNode.g + manhattanDistance(neighbor, targetNode.point);
                        neighborNode.parent = current;

                        if (!frontier.Contains(neighborNode))
                        {
                            frontier.Enqueue(neighborNode, neighborNode.f);
                        }
                        else
                        {
                            frontier.UpdatePriority(neighborNode, neighborNode.f);
                        }
                        
                        visited[neighborNode.index] = neighborNode;
                    }
                }
            }
        }

        // If frontier is emptied out and the target hasn't been reached, then the path is blocked and no shortest path exists.
        return result;
    }

    static void test()
    {
        world = new int[9, 5] {
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
