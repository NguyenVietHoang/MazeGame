using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public Camera globalViewCamera;

        public Maze maze;
        public CursorManager cursorManager;
        public PlayerManager playerManager;

        bool SceneReady;

        // Start is called before the first frame update
        void Start()
        {
            SceneReady = false;
            playerManager.Init();
            cursorManager.Init();
            StartCoroutine(StartMap());
        }

        IEnumerator StartMap()
        {
            //int width = PlayerPrefs.GetInt("width");
            //int height = PlayerPrefs.GetInt("height");

            int width = 10;
            int height = 5;

            //Switch to global Camera to view the Maze
            globalViewCamera.gameObject.SetActive(true);
            cursorManager.currentCamera = globalViewCamera;

            maze.InitMaze(width, height);
            yield return new WaitUntil(() => maze.MapReady);

            //Switch to Player Camera
            playerManager.player.gameObject.SetActive(true);
            cursorManager.currentCamera = playerManager.playerCam;
            globalViewCamera.gameObject.SetActive(false);
            playerManager.playerCam.gameObject.SetActive(true);

            cursorManager.activeRaycast = true;
            SceneReady = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(SceneReady)
            {
                if(Input.GetMouseButtonDown(1))
                {
                    Queue<Vector3> paths = maze.GetPath(playerManager.player.transform.position, cursorManager.cursorObj.transform.position);
                    
                    if(paths != null)
                        playerManager.SetPath(paths);
                }
            }
        }
    }
}

