﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public Camera playerCam;

    public GameObject player;
    public float speed = 1;
    Vector3 target;
    // Start is called before the first frame update
    void Start()
    {
        target = player.transform.position;
        player.SetActive(false);
    }

    public void MoveToPosition(Vector3 newPos)
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
        if (Vector3.Distance(target, currentPos) > 0.1f)
        {
            float step = speed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(currentPos, target, step);
        }
        else
        {
            player.transform.position = target;
        }
    }
       
}