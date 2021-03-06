﻿using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Text;
using System.IO;

public class fileManagerScript : MonoBehaviour
{

    public string OpenFilePanel()
    {
        string path = EditorUtility.OpenFilePanel(
                    "Read configuration data",
                    "",
                    "dat");

        return path;
    }

    public void LoadRobot()
    {
        string fileName = OpenFilePanel();

        try
        {
            StreamReader sReader = new StreamReader(fileName, Encoding.Default);
            int numOfRobots = 1;
            int numOfPolygons = 1;
            int numOfVertices = 3;
            int numOfControls = 2;
            using (sReader)
            {
                if (!System.Int32.TryParse(readToDataLine(sReader), out numOfRobots)) Debug.Log("Not number");
                //Debug.Log(numOfRobots);
                for (int i = 0; i < numOfRobots; i++)
                {
                    if (!System.Int32.TryParse(readToDataLine(sReader), out numOfPolygons)) Debug.Log("Not number");
                    Robot robot = new Robot();
                    //Debug.Log(numOfPolygons);
                    for (int j = 0; j < numOfPolygons; j++)
                    {
                        //vertices
                        if (!System.Int32.TryParse(readToDataLine(sReader), out numOfVertices)) Debug.Log("Not number");
                        //Debug.Log(numOfVertices);
                        Vector2[] vertices = new Vector2[numOfVertices];
                        for (int k = 0; k < numOfVertices; k++)
                        {
                            string[] xy = readToDataLine(sReader).Split(' ');
                            vertices[k] = new Vector2(System.Convert.ToSingle(xy[0]), System.Convert.ToSingle(xy[1]));
                            //Debug.Log(vertices[k]);
                        }
                        robot.addPolygon(new Polygon(numOfVertices, vertices));
                    }

                    //initial configuration
                    string[] initConfig = readToDataLine(sReader).Split(' ');
                    float[] initConfiguration = new float[] {System.Convert.ToSingle(initConfig[0]),
                        System.Convert.ToSingle(initConfig[1]), System.Convert.ToSingle(initConfig[2])};
                    robot.setConfiguration(initConfiguration);
                    //Debug.Log("(" + initConfiguration[0] + "," + initConfiguration[1] + "," + initConfiguration[2] + ")");

                    //goal configuration
                    string[] goalConfig = readToDataLine(sReader).Split(' ');
                    float[] goalConfiguration = new float[] {System.Convert.ToSingle(goalConfig[0]),
                        System.Convert.ToSingle(goalConfig[1]), System.Convert.ToSingle(goalConfig[2])};
                    GameObject.Find("Robot Manager").GetComponent<robotManagerScript>().addGoal(goalConfiguration);
                    //Debug.Log("(" + goalConfiguration[0] + "," + goalConfiguration[1] + "," + goalConfiguration[2] + ")");

                    //control points
                    if (!System.Int32.TryParse(readToDataLine(sReader), out numOfControls)) Debug.Log("Not number");
                    //Debug.Log(numOfControls);
                    for (int j = 0; j < numOfControls; j++)
                    {
                        string[] xy = readToDataLine(sReader).Split(' ');
                        Vector2 point = new Vector2(System.Convert.ToSingle(xy[0]), System.Convert.ToSingle(xy[1]));
                        //Debug.Log(point);
                        robot.addControlPoint(point);
                    }
                    GameObject.Find("Robot Manager").GetComponent<robotManagerScript>().addRobot(robot);
                }
                GameObject.Find("Robot Manager").GetComponent<robotManagerScript>().drawRobots();
                sReader.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    public void LoadObstacle()
    {
        string fileName = OpenFilePanel();

        try
        {
            StreamReader sReader = new StreamReader(fileName, Encoding.Default);
            int numOfObstacle = 1;
            int numOfPolygons = 1;
            int numOfVertices = 3;
            using (sReader)
            {
                if (!System.Int32.TryParse(readToDataLine(sReader), out numOfObstacle)) Debug.Log("Not number");
                //Debug.Log(numOfObstacle);
                for (int i = 0; i < numOfObstacle; i++)
                {
                    if (!System.Int32.TryParse(readToDataLine(sReader), out numOfPolygons)) Debug.Log("Not number");
                    //Debug.Log(numOfPolygons);
                    Obstacle obs = new Obstacle();

                    for (int j = 0; j < numOfPolygons; j++)
                    {
                        //vertices
                        if (!System.Int32.TryParse(readToDataLine(sReader), out numOfVertices)) Debug.Log("Not number");
                        //Debug.Log(numOfVertices);
                        Vector2[] vertices = new Vector2[numOfVertices];
                        for (int k = 0; k < numOfVertices; k++)
                        {
                            string[] xy = readToDataLine(sReader).Split(' ');
                            vertices[k] = new Vector2(System.Convert.ToSingle(xy[0]), System.Convert.ToSingle(xy[1]));
                            //Debug.Log(vertices[k]);
                        }

                        obs.addPolygon(new Polygon(numOfVertices, vertices));
                    }

                    //initial configuration
                    string[] initConfig = readToDataLine(sReader).Split(' ');
                    float[] initConfiguration = new float[]{System.Convert.ToSingle(initConfig[0]),
                        System.Convert.ToSingle(initConfig[1]), System.Convert.ToSingle(initConfig[2])};
                    //Debug.Log("(" + initConfiguration[0] + "," + initConfiguration[1] + "," + initConfiguration[2] + ")");
                    obs.configuration = initConfiguration;

                    //add the obstacle to obstacleManager
                    GameObject.Find("Obstacle Manager").GetComponent<obstacleManagerScript>().addObstacle(obs);
                }
                GameObject.Find("Obstacle Manager").GetComponent<obstacleManagerScript>().drawObstacles();

                sReader.Close();
            }
        }
        catch (System.Exception e)
        {
            Debug.Log(e.Message);
        }
    }

    private string readToDataLine(StreamReader sr)
    {
        string line;
        while ((line = sr.ReadLine()) != null && line[0] == '#');

        return line;
    }

    public void SaveFile()
    {

    }
}
