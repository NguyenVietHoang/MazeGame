using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Camera playerCam;

    public GameObject player;
    public float speed = 1;
    Vector3 target;
    Queue<Vector3> targets;
    // Start is called before the first frame update
    public void Init()
    {
        target = player.transform.position;
        player.SetActive(false);
        playerCam.gameObject.SetActive(false);
        targets = new Queue<Vector3>();
    }

    public void SetPath(Queue<Vector3> path)
    {
        if(path != null)
        {
            targets = new Queue<Vector3>(path);
            if (targets.Count > 0)
            {
                target = targets.Dequeue();
            }
        }
            
    }

    void MoveToPosition(Vector3 newPos)
    {
        target = new Vector3()
        {
            x = newPos.x,
            y = player.transform.position.y,
            z = newPos.z
        };
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 currentPos = player.transform.position;
        Vector3 currentCamPos = playerCam.transform.position;
        if (Vector3.Distance(target, currentPos) > 0.1f)
        {
            float step = speed * Time.deltaTime;
            player.transform.position = Vector3.MoveTowards(currentPos, target, step);
            currentCamPos.x = player.transform.position.x;
            currentCamPos.z = player.transform.position.z;
            playerCam.transform.position = currentCamPos;
        }
        else
        {
            if(targets.Count > 0)
            {
                target = targets.Dequeue();
            }
            else
            {
                player.transform.position = target;
            }
        }
    }
       
}
