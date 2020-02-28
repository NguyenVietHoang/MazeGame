using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public Camera globalViewCamera;

        public Maze maze;
        public CursorManager cursorManager;
        public PlayerManager playerManager;
        public PlayerManager aiManager;

        bool SceneReady;
        bool aiMoving;
        public GameObject finishCursor;

        public Canvas endgameCanvas;
        public TMPro.TextMeshProUGUI winTxt;
        public Button replayBtn;

        Vector3 playerStartPos;
        Vector3 aiStartPos;
        Vector3 FinishPos;
        // Start is called before the first frame update
        void Start()
        {
            //Freeze the game;
            Time.timeScale = 1f;

            replayBtn.onClick.RemoveAllListeners();
            replayBtn.onClick.AddListener(ReplayPlayBtnPress);
            replayBtn.gameObject.SetActive(false);
            winTxt.text = "Right Click to move the character\nPress Space to Zoom out";

            SceneReady = false;
            aiMoving = false;
            playerManager.Init();
            cursorManager.Init();
            StartCoroutine(StartMap());
        }

        IEnumerator StartMap()
        {
            int width = PlayerPrefs.GetInt("width");
            int height = PlayerPrefs.GetInt("height");

            playerStartPos = Vector3.zero;
            aiStartPos = new Vector3(0, 0, height - 1);
            FinishPos = new Vector3(width - 1, 0, Mathf.RoundToInt(height / 2));

            playerManager.SetPosition(playerStartPos);
            playerManager.player.gameObject.SetActive(true);

            aiManager.SetPosition(aiStartPos);
            aiManager.player.gameObject.SetActive(true);

            finishCursor.transform.position = FinishPos;

            //Switch to global Camera to view the Maze
            SwitchToGlobalCamera();

            Vector3 newFloorPos = globalViewCamera.gameObject.transform.position;
            newFloorPos.x = ((float)width / 2f) - 0.5f;
            newFloorPos.z = ((float)height / 2f) - 0.5f;
            globalViewCamera.transform.position = newFloorPos;

            cursorManager.currentCamera = globalViewCamera;

            maze.InitMaze(width, height);
            yield return new WaitUntil(() => maze.MapReady);

            //Switch to Player Camera
            playerManager.player.gameObject.SetActive(true);
            SwitchToPlayerCamera();

            cursorManager.activeRaycast = true;
            SceneReady = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(SceneReady)
            {
                //Start AI when the map is ready
                if(!aiMoving)
                {
                    Queue<Vector3> paths = maze.GetPath(aiStartPos, FinishPos);

                    if (paths != null)
                        aiManager.SetPath(paths);

                    aiMoving = true;
                }

                if(Input.GetMouseButtonDown(1))
                {
                    Queue<Vector3> paths = maze.GetPath(playerManager.player.transform.position, cursorManager.cursorObj.transform.position);
                    
                    if(paths != null)
                        playerManager.SetPath(paths);
                }

                if(Input.GetKeyDown(KeyCode.Space))
                {
                    SwitchToGlobalCamera();
                }
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    SwitchToPlayerCamera();
                }

                if (Vector3.Distance(FinishPos, playerManager.player.transform.position) < 0.5f)
                {
                    ShowEndGame("User Win!");
                }

                if (Vector3.Distance(FinishPos, aiManager.player.transform.position) < 0.5f)
                {
                    ShowEndGame("AI Win!");
                }
            }
        }

        void ShowEndGame(string msg)
        {
            cursorManager.activeRaycast = false;
            //Freeze the game;
            Time.timeScale = 0f;
            winTxt.text = msg;

            replayBtn.gameObject.SetActive(true);
        }

        void SwitchToGlobalCamera()
        {
            globalViewCamera.gameObject.SetActive(true);
            playerManager.playerCam.gameObject.SetActive(false);
            endgameCanvas.worldCamera = globalViewCamera;
            cursorManager.currentCamera = globalViewCamera;
        }

        void SwitchToPlayerCamera()
        {
            globalViewCamera.gameObject.SetActive(false);
            playerManager.playerCam.gameObject.SetActive(true);
            endgameCanvas.worldCamera = playerManager.playerCam;
            cursorManager.currentCamera = playerManager.playerCam;
        }

        void ReplayPlayBtnPress()
        {
            maze.DestroyOldMap();
            SceneManager.LoadScene("Loading");
        }
    }
}

