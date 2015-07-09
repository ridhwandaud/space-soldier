using UnityEngine;
using System.Collections.Generic;
using System;
using SpriteTile;
using Priority_Queue;

public class AStar : MonoBehaviour {
    public static int[,] world;

    class Node : PriorityQueueNode
    {
        public Node parent;
        public int index;
        public int f;
        public int g;
        public Int2 point;

        public Node(Node parent, Int2 point, int index)
        {
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
        }
        if (belowY < world.GetLength(1) && canWalkHere(point.x, belowY))
        {
            result.Add(new Int2(point.x, belowY));
        }
        if (leftX >= 0 && canWalkHere(leftX, point.y))
        {
            result.Add(new Int2(leftX, point.y));
        }
        if (rightX < world.GetLength(0) && canWalkHere(rightX, point.y))
        {
            result.Add(new Int2(rightX, point.y));
        }

        return result;
    }

    static bool canWalkHere(int x, int y)
    {
        return world[x, y] == 0;
    }

    static int manhattanDistance(Int2 point, Int2 goal)
    {
        return Mathf.Abs(goal.x - point.x) + Mathf.Abs(goal.y - point.y);
    }

    static int calculatePointIndex(Int2 point) {
        return point.x + (point.y * world.GetLength(0));
    }

    static List<Node> calculatePath(Int2 start, Int2 end)
    {
        Node startNode = new Node(null, start, calculatePointIndex(start));
        Node targetNode = new Node(null, end, calculatePointIndex(end));

        bool[] visited = new bool[world.GetLength(0) * world.GetLength(1)];

        HeapPriorityQueue<Node> frontier = new HeapPriorityQueue<Node>(100);
        frontier.Enqueue(startNode, 0); // dummy value for priority since it will be popped immediately.

        // Continue algorithm until there are no more open nodes.
        while (frontier.Count > 0)
        {
            Node current = frontier.Dequeue();

            // If the popped node is the target node, then you are done.
            if (current.index == targetNode.index)
            {
                List<Node> result = new List<Node>();
                result.Add(current);

                Node nodeInShortestPath = current.parent;
                
                while (nodeInShortestPath != null)
                {
                    result.Add(nodeInShortestPath);
                    nodeInShortestPath = nodeInShortestPath.parent;
                }

                result.Reverse();
                return result;
            }
            else
            {
                List<Int2> neighbors = findNeighbors(current.point);
                foreach (Int2 neighbor in neighbors) {
                    Node neighborNode = new Node(current, neighbor, calculatePointIndex(neighbor));
                    int newNeighborCost = current.g + current.g + manhattanDistance(neighbor, current.point);
                    if (!visited[neighborNode.index] || neighborNode.g > newNeighborCost)
                    {
                        neighborNode.g = newNeighborCost;
                        neighborNode.f = neighborNode.g + manhattanDistance(neighbor, targetNode.point);
                        frontier.Enqueue(neighborNode, neighborNode.f);
                        visited[neighborNode.index] = true;
                    }
                }
            }
        }

        // If frontier is emptied out and the target hasn't been reached, then the path is blocked and no shortest path exists.
        return null;
    }

    static void test()
    {
        world = new int[5, 10] {
            {1, 1, 0, 1, 1, 0, 0, 0, 0, 1},
            {0, 0, 0, 1, 1, 0, 0, 1, 1 ,1},
            {1, 0, 0, 0, 0, 0, 0, 1, 1, 0},
            {1, 1, 0, 0, 1, 1, 0, 0, 1, 1},
            {1, 1, 1, 0, 1, 0, 0, 1, 1, 1}
        };

        List<Node> path = calculatePath(new Int2(1, 0), new Int2(4, 6));

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
}
