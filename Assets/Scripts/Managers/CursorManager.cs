using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Managers
{
    public class CursorManager : MonoBehaviour
    {
        public Camera currentCamera;
        public GameObject cursorObj;

        public bool activeRaycast;

        public void Init()
        {
            cursorObj.SetActive(false);
            activeRaycast = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (activeRaycast)
            {
                //Get a raycast from Mouse Cursor -> Scene
                Ray ray = currentCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit, 1000))
                {
                    if (hit.transform.CompareTag("Floor"))
                    {
                        Vector3 newCursorPos = new Vector3()
                        {
                            x = Mathf.RoundToInt(hit.point.x),
                            y = hit.point.y,
                            z = Mathf.RoundToInt(hit.point.z),
                        };
                        //Debug.Log("Touch the Floor " + newCursorPos);
                        cursorObj.transform.position = newCursorPos;
                        cursorObj.SetActive(true);
                    }
                    else
                    {
                        cursorObj.SetActive(false);
                    }
                }
            }
        }
    }
}

