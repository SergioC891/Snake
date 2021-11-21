using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Snake : MonoBehaviour
{
    [SerializeField] GameObject snakeHead;
    [SerializeField] GameObject snakeBody1;
    [SerializeField] GameObject snakeBody2;
    [SerializeField] GameObject snakeBody3;
    [SerializeField] GameObject snakeBody4;
    [SerializeField] GameObject snakeBody5;
    [SerializeField] GameObject snakeBody6;
    [SerializeField] GameObject snakeTail;

    [SerializeField] Text crystalsCounterText;
    [SerializeField] Text humansCounterText;
    [SerializeField] Text gameResultText;

    public float speed = 1.0f;
    public float moveXSpeed = 0.5f;
    public float rotSpeed = 0.3f;
    public Vector3 defaultLookAt = new Vector3(0.0f, -1.9f, 100.0f);
    public string borderName1 = "";
    public string borderName2 = "";
    public string levelFinishName  = "";
    public string littleHumanName = "";
    public string stoneName = "";
    public string crystalName = "";
    public string checkPointBoxName = "";
    public float destroyDelay = .1f;

    public int feverCrystalsCount = 3;
    public float feverSpeed = 3.0f;

    public float feverTime = 5.0f;
    public float delayForRestart = 5.0f;

    private float movePosX = 0.0f;
    private float updateEpsilon = 0.015f;
    private float moveX;
    private Vector3 _targetPos;
    private float offsetX = 0.1f;
    private float roadWidth = 0.9f;
    private bool levelFinishedFlag = false;
    private bool gameOverFlag = false;
    private GameObject[] snakeParts = new GameObject[8];
    private Dictionary<string, Color32> snakeColors = new Dictionary<string, Color32>();
    private string currentTargetColor = "";
    private int crystalsCounter = 0;
    private int humansCounter = 0;
    private string currentCollectedItem = "";
    private int currentCrystalCount = 0;
    private float tmpSpeed = 1.0f;
    private bool feverStartFlag = false;

    Camera _camera;

    // Start is called before the first frame update
    void Start()
    {
        _camera = Camera.main;

        snakeParts[0] = snakeHead;
        snakeParts[1] = snakeBody1;
        snakeParts[2] = snakeBody2;
        snakeParts[3] = snakeBody3;
        snakeParts[4] = snakeBody4;
        snakeParts[5] = snakeBody5;
        snakeParts[6] = snakeBody6;
        snakeParts[7] = snakeTail;

        snakeColors.Add("Pink", new Color32(255, 83, 177, 255));
        snakeColors.Add("Green", new Color32(64, 217, 82, 255));
        snakeColors.Add("Blue", new Color32(42, 231, 231, 0));
        snakeColors.Add("Yellow", new Color32(233, 231, 34, 0));
        snakeColors.Add("Red", new Color32(255, 0, 0, 0));
        snakeColors.Add("Lime", new Color32(184, 255, 42, 0));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if ((moveX != 0.0f)
            && checkUpdateEpsilon())
        {
            moveX = 0.0f;
            transform.LookAt(defaultLookAt);
        } 
        else if (!checkUpdateEpsilon())
        {
            if (transform.position.x > movePosX)
            {
                moveX = -moveXSpeed;
            }
            else if (transform.position.x < movePosX)
            {
                moveX = moveXSpeed;
            }
        }

        if (!levelFinishedFlag && !gameOverFlag)
        {
            transform.Translate(moveX * Time.deltaTime, 0, speed * Time.deltaTime);
        }

        if (!feverStartFlag && currentCrystalCount == feverCrystalsCount)
        {
            StartCoroutine(startFever());
        }
    }

    IEnumerator startFever()
    {
        tmpSpeed = speed;
        speed = feverSpeed;
        movePosX = 0;
        transform.LookAt(defaultLookAt);
        feverStartFlag = true;

        yield return new WaitForSeconds(feverTime);

        speed = tmpSpeed;
        currentCrystalCount = 0;
        crystalsCounter = 0;
        crystalsCounterText.text = crystalsCounter.ToString();
        feverStartFlag = false;
    }

    bool checkUpdateEpsilon()
    {
        return Mathf.Abs((Mathf.Abs(transform.position.x) - Mathf.Abs(movePosX))) < updateEpsilon;
    }

    public string getCurrentTargetColor()
    {
        return currentTargetColor;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == borderName1 || other.name == borderName2)
        {
            movePosX = (movePosX > 0) ? (transform.position.x - offsetX) : (transform.position.x + offsetX);
            moveX = 0.0f;
            transform.LookAt(defaultLookAt);
        }

        if (other.name == checkPointBoxName)
        {
            changeColor(other.tag);
        }

        if (other.name == levelFinishName)
        {
            levelFinishedFlag = true;
            StartCoroutine(levelCompleted());
        }

        if (other.name == littleHumanName)
        {
            if (currentTargetColor == other.tag || feverStartFlag)
            {
                humansCounter++;
                humansCounterText.text = humansCounter.ToString();
                setCollectedItem(littleHumanName);
                StartCoroutine(destroyWithDelay(other.gameObject));
            }
            else
            {   
                StartCoroutine(gameOverRoutine());
            }
        }

        if (other.name == stoneName)
        {
            if (!feverStartFlag)
            {
                StartCoroutine(gameOverRoutine());
            }
            else 
            {
                StartCoroutine(destroyWithDelay(other.gameObject));
            }
        }

        if (other.name == crystalName)
        {
            crystalsCounter++;
            crystalsCounterText.text = crystalsCounter.ToString();
            setCollectedItem(crystalName);
            StartCoroutine(destroyWithDelay(other.gameObject));
        }
    }

    IEnumerator gameOverRoutine()
    {
        gameOverFlag = true;
        gameResultText.text = "GAME OVER";

        yield return new WaitForSecondsRealtime(delayForRestart);

        SceneManager.LoadScene("Snake");
    }

    IEnumerator levelCompleted()
    {
        gameResultText.text = "LEVEL COMPLETED";

        yield return new WaitForSecondsRealtime(delayForRestart);

        SceneManager.LoadScene("Snake");
    }
    

    void setCollectedItem(string item)
    {
        currentCollectedItem = item;

        if (currentCollectedItem == crystalName)
        {
            currentCrystalCount++;
        }
        else
        {
            currentCrystalCount = 0;
        }
    }

    IEnumerator destroyWithDelay(GameObject destoyObject)
    {
        yield return new WaitForSeconds(destroyDelay);

        Destroy(destoyObject);
    }

    void changeColor(string color)
    {
        if (snakeColors.ContainsKey(color))
        {
            for (int i = 0; i < 8; i++)
            {
                snakeParts[i].GetComponent<Renderer>().material.SetColor("_Color", snakeColors[color]);
            }

            currentTargetColor = color;
        }
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButton(0) 
            && !levelFinishedFlag && !gameOverFlag 
            && !feverStartFlag)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);

            RaycastHit mouseHit;

            if (Physics.Raycast(ray, out mouseHit))
            {
                _targetPos = mouseHit.point;

                if (_targetPos.z > transform.position.z
                    && _targetPos.x > -roadWidth && _targetPos.x < roadWidth)
                {
                    movePosX = _targetPos.x;
                    transform.LookAt(new Vector3(_targetPos.x, transform.position.y, _targetPos.z));
                }
            }
        }
    }
}
