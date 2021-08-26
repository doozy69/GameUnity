using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    private CubePos nowCube = new CubePos(0, 1, 0);
    public float cubeChangePlaceSpeed = .5f;
    public Transform cubeToPlace;

    public GameObject[] cubesToCreate;

    public GameObject allCubes, vfx;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRB;
    private float camMoveToYPosition;
    public float camMoveSpeed = 1.8f;
    private bool IsLose, firstCube;
    public Color[] bgcolors;
    private Color toCameraColor;
    public GameObject restartButton;
    public GameObject exitButton;

    public Text scoreTxt;

    private List<Vector3> allCubesPositions = new List<Vector3>
    {
        new Vector3 (0,0,0),
        new Vector3 (1,0,0),
        new Vector3 (-1,0,0),
        new Vector3 (0,1,0),
        new Vector3 (0,0,1),
        new Vector3 (0,0,-1),
        new Vector3 (1,0,1),
        new Vector3 (-1,0,-1),
        new Vector3 (-1,0,1),
        new Vector3 (1,0,-1),

    };
    private int prevCountMaxHor;
    private Transform mainCam;
    private Coroutine showCubePlace;

    private List<GameObject> posibleCubesToCreate = new List<GameObject>();
    
    private void Start()
    {
        posibleCubesToCreate.Add(cubesToCreate[0]);
        if (PlayerPrefs.GetInt("score") > 5) posibleCubesToCreate.Add(cubesToCreate[1]);
        if (PlayerPrefs.GetInt("score") > 10) posibleCubesToCreate.Add(cubesToCreate[2]);
        if (PlayerPrefs.GetInt("score") > 20) posibleCubesToCreate.Add(cubesToCreate[3]);
        if (PlayerPrefs.GetInt("score") > 30) posibleCubesToCreate.Add(cubesToCreate[4]);
        if (PlayerPrefs.GetInt("score") > 40) posibleCubesToCreate.Add(cubesToCreate[5]);
        if (PlayerPrefs.GetInt("score") > 40) posibleCubesToCreate.Add(cubesToCreate[8]);

        if (PlayerPrefs.GetInt("allCubes") > 1000) posibleCubesToCreate.Add(cubesToCreate[6]);
        if (PlayerPrefs.GetInt("allCubes") > 2000) posibleCubesToCreate.Add(cubesToCreate[7]);
        if (PlayerPrefs.GetInt("allCubes") > 3000) posibleCubesToCreate.Add(cubesToCreate[9]);
        if (PlayerPrefs.GetInt("allCubes") > 5555) posibleCubesToCreate.Add(cubesToCreate[10]);
        if (PlayerPrefs.GetInt("allCubes") > 10000) posibleCubesToCreate.Add(cubesToCreate[10]);

        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 6f + nowCube.y - 1f;
        allCubesRB = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());

        scoreTxt.text = "<size=22>BEST:</size>" + PlayerPrefs.GetInt("score") + " <size=22>NOW:</size>0\n<size=20>" + PlayerPrefs.GetInt("allCubes") + "</size>";
    }

    private void Update()
    {
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && cubeToPlace != null && allCubes != null && !EventSystem.current.IsPointerOverGameObject())
        {
#if !UNITY_EDITOR
            if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
#endif
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // Check if finger is over a UI element
                if (EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                {
                    return;
                }
            }


            if (!firstCube)
            {
                firstCube = true;
                foreach(GameObject obj in canvasStartPage)
                {
                    Destroy(obj);
                }
                exitButton.SetActive(true);
            }

            GameObject createCube = null;
            if (posibleCubesToCreate.Count == 1)
                createCube = posibleCubesToCreate[0];
            else
            {
                createCube = posibleCubesToCreate[UnityEngine.Random.Range(0, posibleCubesToCreate.Count)];
            }

            GameObject newCube = Instantiate(createCube, cubeToPlace.position, Quaternion.identity) as GameObject;

            newCube.transform.SetParent(allCubes.transform);

            nowCube.setVector(cubeToPlace.position);

            allCubesPositions.Add(cubeToPlace.position);

            allCubesRB.isKinematic = true;
            allCubesRB.isKinematic = false;

            if (PlayerPrefs.GetString("music") == "Yes") GetComponent<AudioSource>().Play();

            GameObject newVfx = Instantiate(vfx, cubeToPlace.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 1f);

            PlayerPrefs.SetInt("allCubes", PlayerPrefs.GetInt("allCubes") + 1);

            SpawnPositions();

            MoveCameraChangeBg();
        }

        if (!IsLose && allCubesRB != null && allCubesRB.velocity.magnitude > 0.1f)
        {
            IsLose = true;
            Destroy(cubeToPlace.gameObject);
            StopCoroutine(showCubePlace);
            restartButton.SetActive(true);
            
        }

        if (IsLose)
        {
            Camera.main.transform.localPosition = Vector3.MoveTowards(Camera.main.transform.localPosition, new Vector3(Camera.main.transform.localPosition.x, 6f, Camera.main.transform.localPosition.z-5f), 3f * Time.deltaTime);
        }
        else
        {
            mainCam.localPosition = Vector3.MoveTowards(mainCam.localPosition,
                                                    new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z),
                                                    camMoveSpeed * Time.deltaTime);
        }

        



        if (Camera.main.backgroundColor != toCameraColor)
        {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
        }
    }

    IEnumerator ShowCubePlace()
    {
        while (true)
        {
            SpawnPositions();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    private void SpawnPositions()
    {
        List<Vector3> positions = new List<Vector3>();
        if (IsPositionEmpty(new Vector3(nowCube.x+1, nowCube.y,nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));
        if (IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if (positions.Count > 1)
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        else if (positions.Count == 0)
            IsLose = true;
        else
            cubeToPlace.position = positions[0];
    }

    private bool IsPositionEmpty(Vector3 targetPos)
    {
        if (targetPos.y == 0) return false;
        
        foreach (Vector3 pos in allCubesPositions)
        {
            if (pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z)
            {
                return false;
            }
        }
        return true;
    }

    private void MoveCameraChangeBg()
    {
        int maxX = 0, maxY = 0, maxZ = 0, maxHor;
        foreach (Vector3 pos in allCubesPositions)
        {
            if (Mathf.Abs(Convert.ToInt32(pos.x)) > maxX) maxX = Mathf.Abs(Convert.ToInt32(pos.x));
            if (Mathf.Abs(Convert.ToInt32(pos.y)) > maxY) maxY = Mathf.Abs(Convert.ToInt32(pos.y));
            if (Mathf.Abs(Convert.ToInt32(pos.z)) > maxZ) maxZ = Mathf.Abs(Convert.ToInt32(pos.z));
        }

        if (PlayerPrefs.GetInt("score") < maxY-1) PlayerPrefs.SetInt("score", maxY-1);
        scoreTxt.text = "<size=22>BEST:</size>" + PlayerPrefs.GetInt("score") + " <size=22>NOW:</size>" + (maxY-1)+ "\n<size=20>"+PlayerPrefs.GetInt("allCubes")+"</size>";

        camMoveToYPosition = 6f + nowCube.y - 1f;

        maxHor = maxX > maxZ ? maxX : maxZ;

        if (maxHor % 2 == 0 && prevCountMaxHor != maxHor)
        {
            mainCam.localPosition -= new Vector3(0, 0, 3f);
            prevCountMaxHor = maxHor;
        }

        if (maxY >= 6) toCameraColor = bgcolors[2];
        else if (maxY >= 4) toCameraColor = bgcolors[1];
        else if (maxY >= 2) toCameraColor = bgcolors[0];

    }
}

struct CubePos
{
    public int x, y, z;

    public CubePos(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 getVector()
    {
        return new Vector3(x, y, z);
    }
    public void setVector(Vector3 pos)
    {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
}