using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LittleHuman : MonoBehaviour
{
    public GameObject snakeHead;
    public float speedX = .75f;
    private bool startMoveToHead = false;

    // Update is called once per frame
    void Update()
    {
        if (startMoveToHead)
        {
            transform.LookAt(snakeHead.transform);
            transform.Translate(0, 0, speedX * Time.deltaTime);
        }
    }

    public void moveToSnakeHead()
    {
        startMoveToHead = true;
    }
}
