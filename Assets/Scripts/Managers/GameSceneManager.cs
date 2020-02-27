using Models;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class GameSceneManager : MonoBehaviour
    {
        public Maze maze;
        public CursorManager cursorManager;
        public PlayerManager playerManager;

        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(StartMap());
        }

        IEnumerator StartMap()
        {
            int width = PlayerPrefs.GetInt("width");
            int height = PlayerPrefs.GetInt("height");
            maze.InitMaze(width, height);
            yield return new WaitUntil(() => maze.MapReady);

            cursorManager.activeRaycast = true;
            playerManager.MoveToPosition(Vector3.zero);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}

