﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class robotManagerScript : MonoBehaviour
{
    private int numOfRobot = 0;
    private List<Robot> robotList = new List<Robot>();
    public List<float[]> goalConfigList = new List<float[]>();
    public List<GameObject> goalGameobjects = new List<GameObject>();
    private byte[,] bitmap = new byte[130, 130];
    private float UNIT = 8.0f / 128.0f;

    // Update is called once per frame
    void Update()
    {

    }

    public void drawRobots()
    {
        for (int i = 0; i < numOfRobot; i++)
        {
            drawRobot(i);
            drawGoal(i);
        }
    }

    private void drawRobot(int index)
    {
        robotList[index].Draw3D(index);
    }

    private void drawGoal(int index)
    {
        goalGameobjects.Add(GameObject.Instantiate(robotList[index].gameobject));
        goalGameobjects[index].name = "Robot " + index + " Goal";
        goalGameobjects[index].transform.parent = this.gameObject.transform;

        float[] configuration = goalConfigList[index];
        goalGameobjects[index].transform.position = new Vector3(configuration[0] * UNIT, 0, configuration[1] * UNIT);
        goalGameobjects[index].transform.Rotate(Vector3.up * -configuration[2]);

        foreach (MeshRenderer render in goalGameobjects[index].GetComponentsInChildren<MeshRenderer>())
            render.material.color = Color.blue;
    }

    public void addRobot(Robot newRobot)
    {
        numOfRobot++;
        robotList.Add(newRobot);
        newRobot.gameobject.transform.parent = this.gameObject.transform;
    }

    public void addGoal(float[] newGoal)
    {
        if (newGoal.Length == 3)
            goalConfigList.Add(newGoal);
    }

    public void initBitmap(int index)
    {
        for (int i = 0; i < 130; i++)
        {
            for (int j = 0; j < 130; j++)
            {
                RaycastHit hit;
                if (i == 0 | j == 0 | i == 129 | j == 129)
                    bitmap[i, j] = 255;
                else if (Physics.Raycast(new Vector3(UNIT / 2.0f + (j - 1) * UNIT, 2.0f, UNIT / 2.0f + (i - 1) * UNIT),
                     Vector3.down, out hit, 1.5f))
                {
                    if (hit.collider.tag == "Obstacle")
                    {
                        bitmap[i, j] = 255;
                        print("Hit " + hit.collider.name + " at" + i + "," + j);
                    }
                }
                else
                {
                    bitmap[i, j] = 254;
                    //Debug.Log("No Obstacle");
                }

            }
        }

        float[] goal = goalConfigList[index];
        bitmap[(int)(goal[0] / UNIT), (int)(goal[1] / UNIT)] = 0;
        //Debug.Log("Complete");
    }

    public void resolvePotential()
    {

    }
}

public class Robot
{
    private int numOfPolygon = 0;
    private List<Polygon> polygonList = new List<Polygon>();

    private int numOfControl = 0;
    private List<Vector2> controlList = new List<Vector2>();

    public float[] configuration = new float[3] { 0.0f, 0.0f, 0.0f };

    public GameObject gameobject = new GameObject();
    private float UNIT = 8.0f / 128.0f;

    public Robot()
    {
        setupGameobject();
    }

    public Robot(int nPolygons, Polygon[] polygons)
    {
        setupGameobject();
        for (int i = 0; i < nPolygons; i++)
            addPolygon(polygons[i]);
    }

    private void setupGameobject()
    {
        gameobject.name = "Robot";
        gameobject.tag = "Robot";
        gameobject.layer = LayerMask.NameToLayer("Robot"); ;
        gameobject.AddComponent<objectEditor>();
    }

    public void addPolygon(Polygon newPolygon)
    {
        numOfPolygon++;
        polygonList.Add(newPolygon);
        newPolygon.gameobject.transform.parent = this.gameobject.transform;
        newPolygon.gameobject.transform.localScale = new Vector3(1f, 1f, 1f);
    }

    public void updateConfiguration()
    {
        configuration[0] = gameobject.transform.position.x / UNIT;
        configuration[1] = gameobject.transform.position.y / UNIT;
        configuration[2] = 360 - gameobject.transform.rotation.y;
    }

    public void addControlPoint(Vector2 newPoint)
    {
        numOfControl++;
        controlList.Add(newPoint);
    }

    public void setControlPoint(int index, Vector2 newPoint)
    {
        controlList[index] = newPoint;
    }

    public Vector2 getControlPoint(int index)
    {
        return controlList[index];
    }

    public void modifyPolygon(int index, Polygon newPolygon)
    {
        if (index >= 0 && index < numOfPolygon)
            polygonList[index] = newPolygon;
    }

    public void applyTransform()
    {
        gameobject.transform.localScale = new Vector3(UNIT, 1, UNIT);
        gameobject.transform.position = new Vector3(configuration[0] * UNIT, 0, configuration[1] * UNIT);
        gameobject.transform.Rotate(Vector3.up * -configuration[2]);
    }

    private void ChangeLayers(Transform trans)
    {
        trans.gameObject.layer = LayerMask.NameToLayer("Robot");
        foreach (Transform child in trans)
        {
            ChangeLayers(child);
        }
    }

    private void setupRigidbody()
    {
        Rigidbody rigid = gameobject.AddComponent<Rigidbody>();
        rigid.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
    }

    private void setupCollider()
    {
        MeshCollider collider = this.gameobject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        MeshFilter[] meshFilters = this.gameobject.GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.worldToLocalMatrix;
            i++;
        }
        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        collider.sharedMesh = mesh;
    }

    public void Draw3D(int index)
    {
        for (int i = 0; i < numOfPolygon; i++)
            polygonList[i].threeDMesh(Color.red);

        gameobject.name = "Robot " + index.ToString();
        ChangeLayers(gameobject.transform);
        setupRigidbody();
        applyTransform();
    }

    public void Draw2D(int index)
    {
        for (int i = 0; i < numOfPolygon; i++)
            polygonList[i].updateMesh(Color.blue);

        gameobject.name = "Robot " + index.ToString();
        setupCollider();
        applyTransform();
    }
}