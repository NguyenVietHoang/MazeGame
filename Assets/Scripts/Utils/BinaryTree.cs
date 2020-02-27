using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Utils
{
    public class IntPos
    {
        public int x;
        public int y;

        ushort limit;
        public IntPos(): this(0, 0)
        {
        }

        public IntPos(IntPos pos)
        {
            x = pos.x;
            y = pos.y;
            limit = pos.limit;
        }

        public IntPos(int _x, int _y, ushort _limit = 2)
        {
            x = _x;
            y = _y;

            limit = _limit;
        }

        /// <summary>
        /// Convert vector 2 to IntPos
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public IntPos GetRoundedPos(Vector2 pos)
        {
            return new IntPos(Mathf.RoundToInt(pos.x), Mathf.RoundToInt(pos.y));
        }

        /// <summary>
        /// Get the position of this Pos in INT format
        /// </summary>
        /// <returns></returns>
        public int GetIndex()
        {
            int pow = Mathf.RoundToInt(Mathf.Pow(10, limit));

            //The position number is too long
            if (x / pow != 0 || y / pow != 0)
            {
                return -1;
            }
            else
                return (x % pow) * pow + (y % pow);
        }

        /// <summary>
        /// Convert back int format to INTPOS
        /// </summary>
        /// <param name="_index"></param>
        /// <returns></returns>
        public IntPos GetPos(int _index)
        {
            int pow = Mathf.RoundToInt(Mathf.Pow(10, limit));
            return new IntPos(_index / pow, _index % pow);
        }
    }

    public class Node
    {
        public IntPos position { get; private set; }
        
        //Index of the neighbors
        int wNeighbor;
        int eNeighbor;
        int nNeighbor;
        int sNeighbor;

        //>4 => not available, 1 node can only have 4 neighbors
        int available;
        //short position
        //public int index { get; private set; }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="_pos">Position of this node</param>
        /// <param name="_limit">
        /// Limit of index.
        /// Eg: _limit = 2 ~> index will be XXYY
        /// ~> Max index will be 9999
        /// </param>
        public Node(IntPos _pos)
        {
            position = new IntPos(_pos);
            //index = position.GetIndex();
            available = 0;
            wNeighbor = -1;
            eNeighbor = -1;
            nNeighbor = -1;
            sNeighbor = -1;
        }

        public Node(Node node)
        {
            position = new IntPos(node.position);
            //index = position.GetIndex();
            available = node.available;
            wNeighbor = node.GetNeighbor("w");
            eNeighbor = node.GetNeighbor("e");
            nNeighbor = node.GetNeighbor("n");
            sNeighbor = node.GetNeighbor("s");
        }

        /// <summary>
        /// Check if there are any neighbor is still available
        /// </summary>
        /// <returns></returns>
        public bool CheckAvailable()
        {
            return available < 4;
        }

        /// <summary>
        /// Set the neighbor position
        /// </summary>
        /// <param name="_pos">New position of neighbor (index format)</param>
        /// <param name="dir">
        /// s: south Neighbor.
        /// n: north Neighbor.
        /// w: west Neighbor.
        /// e: est Neighbor.
        /// </param>
        /// <returns>
        /// true if dir is correct and the neighbor data is (-1,-1)
        /// </returns>
        public bool SetNeigbor(int _pos, string dir)
        {
            switch (dir)
            {
                case "s":
                    if(sNeighbor == -1)
                    {
                        sNeighbor = _pos;
                        available++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "n":
                    if (nNeighbor == -1)
                    {
                        nNeighbor = _pos;
                        available++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "e":
                    if (eNeighbor== -1)
                    {
                        eNeighbor = _pos;
                        available++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                case "w":
                    if (wNeighbor == -1)
                    {
                        wNeighbor = _pos;
                        available++;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                default:
                    return false;
            }
        }

        /// <summary>
        /// Get the neighbor position
        /// </summary>
        /// <param name="dir">
        /// s: south Neighbor.
        /// n: north Neighbor.
        /// w: west Neighbor.
        /// e: est Neighbor.
        /// </param>
        /// <returns>
        /// != -99 if dir is correct
        /// </returns>
        public int GetNeighbor(string dir)
        {
            switch (dir)
            {
                case "s":
                    return sNeighbor;
                case "n":
                    return nNeighbor;
                case "e":
                    return eNeighbor;
                case "w":
                    return wNeighbor;
                default:
                    return -99;
            }
        }
    }

    public class MazeTree
    {
        public Dictionary<int, Node> tree;

        public MazeTree() : this(new IntPos())
        {
            
        }

        public MazeTree(IntPos rootPos)
        {
            tree = new Dictionary<int, Node>();
            Node root = new Node(rootPos);
            tree.Add(root.position.GetIndex(), root);
        }

        /// <summary>
        /// Add new Node to the tree.
        /// This one does not include adding neigbor, you should add it manually.
        /// </summary>
        /// <param name="_node"></param>
        /// <returns></returns>
        public bool AddNode(Node _node)
        {
            if(tree.ContainsKey(_node.position.GetIndex()))
            {
                return false;
            }
            else
            {
                tree.Add(_node.position.GetIndex(), _node);
                return true;
            }
        }

        /// <summary>
        /// Get the path from source to dest.
        /// This algorithm will not find the shortest path from 2 vertex.
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        /// <returns></returns>
        public List<int> GetPath(int src, int dest)
        {
            List<int> path = new List<int>();

            //Only find path if the dest and src exists in the tree
            if(tree.ContainsKey(src) && tree.ContainsKey(dest))
            {
                //Clone a list of index for DFS
                //Key = node Index; Value = Prec Node Index
                Dictionary<int, int> cloneTree = new Dictionary<int, int>();
                foreach(var node in tree)
                {
                    cloneTree.Add(node.Key, -1);
                }

                int currentNode = src;
                Queue<int> currentTree = new Queue<int>();
                while(currentNode != dest)
                {
                    int tmp = tree[currentNode].GetNeighbor("s");
                    if(tmp != -1 && cloneTree[tmp] != -1)
                    {
                        currentTree.Enqueue(tmp);
                        //Update Predecessor
                        cloneTree[tmp] = currentNode;
                    }

                    tmp = tree[currentNode].GetNeighbor("w");
                    if (tmp != -1 && cloneTree[tmp] != -1)
                    {
                        currentTree.Enqueue(tmp);
                        //Update Predecessor
                        cloneTree[tmp] = currentNode;
                    }

                    tmp = tree[currentNode].GetNeighbor("n");
                    if (tmp != -1 && cloneTree[tmp] != -1)
                    {
                        currentTree.Enqueue(tmp);
                        //Update Predecessor
                        cloneTree[tmp] = currentNode;
                    }

                    tmp = tree[currentNode].GetNeighbor("e");
                    if (tmp != -1 && cloneTree[tmp] != -1)
                    {
                        currentTree.Enqueue(tmp);
                        //Update Predecessor
                        cloneTree[tmp] = currentNode;
                    }

                    //The source and dest is not connect each other
                    if(currentTree.Count <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        currentNode = currentTree.Dequeue();
                    }
                }

                //Build the Path
                currentNode = dest;
                while(currentNode != src)
                {
                    int tmp = cloneTree[currentNode];
                    path.Add(tmp);
                    currentNode = tmp;
                }
            }

            return path;
        }
    }

}
