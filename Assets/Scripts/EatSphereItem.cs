using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EatSphereItem : MonoBehaviour
{
    public GameObject snakeHead;
    public string littleHumanName = "";
    private string currentTargetColor = "";

    void OnTriggerEnter(Collider other)
    {
        if (other.name == littleHumanName)
        {
            currentTargetColor = snakeHead.GetComponent<Snake>().getCurrentTargetColor();
            if (currentTargetColor == other.tag)
            {
                other.GetComponent<LittleHuman>().moveToSnakeHead();
            }
        }
    }
}
