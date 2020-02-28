using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

namespace Models
{
    public class Maze : MonoBehaviour
    {
        MazeTree mazeMap;
        int width;
        int height;

        #region Assets for Buiding Maze
        public GameObject wallPrb;
        public GameObject floorPrb;
        #endregion

        IntPos rootPos;
        Dictionary<string, GameObject> wallList;
        GameObject floorObj;

        public bool MapReady = false;
        /// <summary>
        /// Call Build a Maze Object
        /// </summary>
        /// <param name="_width"></param>
        /// <param name="_height"></param>
        public void InitMaze(int _width, int _height)
        {
            width = _width;
            height = _height;

            mazeMap = new MazeTree();
            StartCoroutine(BuildStartMap(0, 0, 2));
        }

        /// <summary>
        /// Build a map with all bound
        /// </summary>
        /// <param name="_x"></param>
        /// <param name="_y"></param>
        /// <param name="_limit">Limit to convert Int POS to INDEX</param>
        IEnumerator BuildStartMap(int _x, int _y, ushort _limit)
        {
            int pow = Mathf.RoundToInt(Mathf.Pow(10, _limit));
            //The value is out of limit
            if(width / pow != 0 || height / pow != 0)
            {
                yield return null;
            }
            else
            {
                rootPos = new IntPos(_x, _y);

                if (wallList != null && wallList.Count > 0)
                {
                    foreach (var wall in wallList)
                    {
                        Destroy(wall.Value);
                    }
                }
                wallList = new Dictionary<string, GameObject>();

                if (floorObj != null)
                {
                    Destroy(floorObj);
                }

                Vector3 mapPos = new Vector3
                {
                    x = _x,
                    y = 0,
                    z = _y
                };
                transform.position = mapPos;

                //Build floor
                Vector3 newFloorScl = floorPrb.transform.localScale;
                newFloorScl.x = width;
                newFloorScl.z = height;
                Vector3 newFloorPos = mapPos;
                newFloorPos.x = ((float)width / 2f) - 0.5f;
                newFloorPos.z = ((float)height / 2f) - 0.5f;
                floorObj = BuildObject(floorPrb, gameObject, newFloorPos, newFloorScl, Vector3.zero, "Floor");

                //Build Bound
                Transform mazeTransform = gameObject.transform;
                Transform wallTransform = wallPrb.transform;

                for (int i = 0; i < width; i++)
                {
                    for (int j = 0; j < height; j++)
                    {
                        //Build extra bound on the west of the map
                        if (i == 0)
                        {
                            string boundname = "-1|" + (i * pow + j).ToString();
                            Vector3 newBoundPosEx = new Vector3(mazeTransform.position.x + i - 0.5f,
                                                mazeTransform.position.y,
                                                mazeTransform.position.z + j);
                            Vector3 newBoundRotEx = new Vector3(wallTransform.localEulerAngles.x + 0,
                                                            wallTransform.localEulerAngles.y + 90,
                                                            wallTransform.localEulerAngles.z + 0);
                            Vector3 newBoundSclEx = new Vector3(wallTransform.localScale.x,
                                                            wallTransform.localScale.y,
                                                            wallTransform.localScale.z);
                            wallList.Add(boundname,
                                BuildObject(wallPrb, gameObject, newBoundPosEx, newBoundSclEx, newBoundRotEx, boundname));
                            yield return new WaitForEndOfFrame();
                        }

                        //Build extra bound on the south of the map
                        if (j == 0)
                        {
                            string boundname = (i * pow + j).ToString() + "|-1";
                            Vector3 newBoundPosEx = new Vector3(mazeTransform.position.x + i,
                                                mazeTransform.position.y,
                                                mazeTransform.position.z + j - 0.5f);
                            Vector3 newBoundRotEx = new Vector3(wallTransform.localEulerAngles.x + 0,
                                                            wallTransform.localEulerAngles.y,
                                                            wallTransform.localEulerAngles.z + 0);
                            Vector3 newBoundSclEx = new Vector3(wallTransform.localScale.x,
                                                            wallTransform.localScale.y,
                                                            wallTransform.localScale.z);
                            wallList.Add(boundname,
                                BuildObject(wallPrb, gameObject, newBoundPosEx, newBoundSclEx, newBoundRotEx, boundname));
                            yield return new WaitForEndOfFrame();
                        }

                        //Build bound on the est of the map
                        string boundname0 = (i * pow + j).ToString() + "|" + ((i + 1) * pow + j).ToString();

                        Vector3 newBoundPos0 = new Vector3(mazeTransform.position.x + i + 0.5f,
                                                mazeTransform.position.y,
                                                mazeTransform.position.z + j);
                        Vector3 newBoundRot0 = new Vector3(wallTransform.localEulerAngles.x + 0,
                                                        wallTransform.localEulerAngles.y + 90,
                                                        wallTransform.localEulerAngles.z + 0);
                        Vector3 newBoundScl0 = new Vector3(wallTransform.localScale.x,
                                                        wallTransform.localScale.y,
                                                        wallTransform.localScale.z);
                        wallList.Add(boundname0,
                            BuildObject(wallPrb, gameObject, newBoundPos0, newBoundScl0, newBoundRot0, boundname0));
                        yield return new WaitForEndOfFrame();

                        //Build bound on the north of the map
                        string boundname1 = (i * pow + j).ToString() + "|" + (i * pow + (j + 1)).ToString();

                        Vector3 newBoundPos1 = new Vector3(mazeTransform.position.x + i,
                                                mazeTransform.position.y,
                                                mazeTransform.position.z + j + 0.5f);
                        Vector3 newBoundRot1 = new Vector3(wallTransform.localEulerAngles.x + 0,
                                                        wallTransform.localEulerAngles.y + 0,
                                                        wallTransform.localEulerAngles.z + 0);
                        Vector3 newBoundScl1 = new Vector3(wallTransform.localScale.x,
                                                        wallTransform.localScale.y,
                                                        wallTransform.localScale.z);
                        wallList.Add(boundname1,
                            BuildObject(wallPrb, gameObject, newBoundPos1, newBoundScl1, newBoundRot1, boundname1));
                        yield return new WaitForEndOfFrame();
                    }
                }

                StartCoroutine(BuildMaze());
            }
        }

        GameObject BuildObject(GameObject prefab, GameObject parent, Vector3 newPos, Vector3 newScale, Vector3 newRot, string name = "GameObject")
        {
            GameObject obj = Instantiate(prefab, parent.transform, true);
            Vector3 newObjScl = obj.transform.localScale;
            newObjScl.x = width;
            newObjScl.z = height;
            obj.transform.localScale = newScale;
            obj.transform.localPosition = newPos;
            obj.transform.localEulerAngles = newRot;
            obj.name = name;
            return obj;
        }

        /// <summary>
        /// Build the Maze path and show it on the Start Map
        /// </summary>
        IEnumerator BuildMaze()
        {
            Debug.Log("Start Building maze...");
            string[] dirs = new string[4] {"s", "w", "e", "n" };
            string[] oposites = new string[4] { "n", "e", "w", "s" };
            int[] randomIndex = new int[4] { 0, 2, 1, 3 };
            mazeMap = new MazeTree(rootPos);

            Queue<Node> nodesQueue = new Queue<Node>();
            int rootIndex = rootPos.GetIndex();
            Node currentNode = mazeMap.tree[rootIndex];
            // The root node was already added in the tree
            while (mazeMap.tree.Count < width*height)
            {
                currentNode = UpdateNeighbor(currentNode);

                if (currentNode.CheckAvailable())
                {
                    //Get a random available neighbor
                    string newNeighborDir = "";
                    string opositeDir = "";
                    
                    int[] newRIndex = randomIndex.OrderBy(x => System.Guid.NewGuid()).ToArray();
                    for (int n = 0; n < randomIndex.Length; n++)
                    {
                        newNeighborDir = dirs[newRIndex[n]];
                        opositeDir = oposites[newRIndex[n]];

                        //this direction is still available
                        if (currentNode.GetNeighbor(newNeighborDir) == -1)
                        {
                            break;
                        }
                    }

                    //Get the neighbor position
                    IntPos newNeiborPos = new IntPos(currentNode.position);
                    switch (newNeighborDir)
                    {
                        case "s":
                            newNeiborPos.y -= 1;
                            break;
                        case "w":
                            newNeiborPos.x -= 1;
                            break;
                        case "e":
                            newNeiborPos.x += 1;
                            break;
                        case "n":
                            newNeiborPos.y += 1;
                            break;
                    }

                    int newNeigborIndex = newNeiborPos.GetIndex();
                    currentNode.SetNeigbor(newNeigborIndex, newNeighborDir);
                    mazeMap.tree[currentNode.position.GetIndex()] = currentNode;
                    Node newNeighbor = new Node(newNeiborPos);
                    newNeighbor.SetNeigbor(currentNode.position.GetIndex(), opositeDir);

                    //show the Path on the map
                    string boundname1 = currentNode.position.GetIndex() + "|" + newNeigborIndex;
                    string boundname2 = newNeigborIndex + "|" + currentNode.position.GetIndex();

                    //Debug.Log("Current: " + currentNode.position.GetIndex() + ", New Neigbor: " + newNeighborDir + "/" + newNeigborIndex
                    //    + ", Wall name = " + boundname1 + "/" +boundname2);

                    if (wallList.ContainsKey(boundname1))
                    {
                        wallList[boundname1].SetActive(false);
                        //Debug.Log("Turn off wall:" + boundname1);
                    }
                    if (wallList.ContainsKey(boundname2))
                    {
                        wallList[boundname2].SetActive(false);
                        //Debug.Log("Turn off wall:" + boundname2);
                    }

                    mazeMap.tree.Add(newNeigborIndex, newNeighbor);
                    //If this node still has some free neighbor 
                    if (currentNode.CheckAvailable())
                    {
                        nodesQueue.Enqueue(currentNode);
                    }

                    //follow new node
                    currentNode = newNeighbor;
                }
                //Current node is full of neighbor, try another node in the queue
                else if(nodesQueue.Count > 0)
                {
                    currentNode = nodesQueue.Dequeue();
                    //Debug.Log("Take node from Queue: " + currentNode.position.GetIndex());
                }
                else
                {
                    //Debug.Log("Maze Build Finish");
                    break;
                }
                yield return new WaitForSeconds(0.1f);
            }

            MapReady = true;
        }

        /// <summary>
        /// Update all the invalid neighbor to -50
        /// </summary>
        /// <param name="node"></param>
        Node UpdateNeighbor(Node node)
        {
            //Debug.Log("Update neighbor: " + node.position.GetIndex() + "[" + node.position.x + "," + node.position.y + "]");
            //Node node = new Node(_node);
            IntPos nodePos = node.position;

            IntPos nNodePos = new IntPos()
            {
                x = nodePos.x,
                y = nodePos.y + 1
            };
            IntPos sNodePos = new IntPos(nodePos)
            {
                x = nodePos.x,
                y = nodePos.y - 1
            };
            IntPos eNodePos = new IntPos(nodePos)
            {
                x = nodePos.x + 1,
                y = nodePos.y
            };
            IntPos wNodePos = new IntPos(nodePos)
            {
                x = nodePos.x - 1,
                y = nodePos.y
            };

            int iNPos = nNodePos.GetIndex();
            int iSPos = sNodePos.GetIndex();
            int iEPos = eNodePos.GetIndex();
            int iWPos = wNodePos.GetIndex();

            //Debug.Log("NeighBor = " + iNPos + "/" + iSPos + "/" + iEPos + "/" + iWPos);

            //Update bound
            if (eNodePos.x >= width)
            {
                //Debug.Log("Update bound E");
                node.SetNeigbor(-50, "e");
            }

            if (wNodePos.x < 0)
            {
                //Debug.Log("Update bound W");
                node.SetNeigbor(-50, "w");
            }

            if (nNodePos.y >= height)
            {
                //Debug.Log("Update bound N");
                node.SetNeigbor(-50, "n");
            }

            if (sNodePos.y < 0)
            {
                //Debug.Log("Update bound S");
                node.SetNeigbor(-50, "s");
            }

            //Update existed neighbor
            if (mazeMap.tree.ContainsKey(iSPos)
                && node.GetNeighbor("s") == -1)
            {
                //Debug.Log("Update Neigbor S=" + iSPos);
                node.SetNeigbor(-50, "s");
            }
            if (mazeMap.tree.ContainsKey(iWPos)
                && node.GetNeighbor("w") == -1)
            {
                //Debug.Log("Update Neigbor W = " + iWPos);
                node.SetNeigbor(-50, "w");
            }
            if (mazeMap.tree.ContainsKey(iNPos) 
                && node.GetNeighbor("n") == -1)
            {
                //Debug.Log("Update Neigbor N = " + iNPos);
                node.SetNeigbor(-50, "n");
            }
            if (mazeMap.tree.ContainsKey(iEPos)
                && node.GetNeighbor("e") == -1)
            {
                //Debug.Log("Update Neigbor E = " + iEPos);
                node.SetNeigbor(-50, "e");
            }

            mazeMap.tree[node.position.GetIndex()] = node;
            //Debug.Log("End Update neighbor: " + node.position.GetIndex() + "[" + node.position.x + "," + node.position.y + "]");
            return node;
        }

        public Queue<Vector3> GetPath(Vector3 source, Vector3 dest)
        {
            List<int> pathIndex = mazeMap.GetPath(IntPos.GetRoundedPos(source).GetIndex(), IntPos.GetRoundedPos(dest).GetIndex());
            if(pathIndex == null)
            {
                return null;
            }

            Queue<Vector3> finalPath = new Queue<Vector3>();
            if(pathIndex.Count > 0)
            {
                for(int i = 0; i < pathIndex.Count; i++)
                {
                    IntPos tmp = IntPos.GetPos(pathIndex[i], 2);
                    finalPath.Enqueue(new Vector3(tmp.x, (source.y + dest.y) / 2f, tmp.y));
                }
            }
            return finalPath;
        }

        public void DestroyOldMap()
        {
            if (floorObj != null)
                Destroy(floorObj);

            if(wallList != null)
            {
                if (wallList != null && wallList.Count > 0)
                {
                    foreach (var wall in wallList)
                    {
                        Destroy(wall.Value);
                    }
                }
            }
        }
    }
}


