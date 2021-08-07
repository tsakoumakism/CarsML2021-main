using UnityEngine;
using UnityEngine.UI;
using System;  
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class TrainingArea : MonoBehaviour
{
    [Header("Training Area Settings")]
    public CarAgent agentPrefabPPO;
    public CarAgent agentPrefabSAC;


    public bool spawnAgents = false;
    //public int agentAmount;
    public int totalCheckpoints;
    public Transform finishLine;

    [Header("Reward/Penalty Settings")]
    public float successReward = 50f;
    public float checkpointReward = 10f;
    public float speedReward = 0.01f;
    public float collisionPenalty = -10f;
    public float idlePenalty = -10f;
    public float wrongCheckpointPenalty = -10f;

    //public TextMeshProUGUI monitorHUD;
    //public TextMeshProUGUI areaMonitor;
    private Vector3 startPosition;
    private Vector3 finishLinePos;
    private CarAgent[] agentList;
    private CarAcademy academy;

    //performance
    public float recordLap = Mathf.Infinity;

    public List<GameObject> roadBuildList = new List<GameObject>();

    string mapName;
    string modelName;

    int agentPPO;
    int agentSAC;
    int numOfAgents;
    [SerializeField] private BlockData bd;


    void Awake() {

        mapName = "selectedMap";
        LoadMap();

        academy = GameObject.Find("Academy").GetComponent<CarAcademy>();

        startPosition = GameObject.Find("Track").transform.GetChild(0).GetChild(0).position; // get the first road block of the track
       // float offsetY = startPosition.y + 10f;
       // startPosition = new Vector3(startPosition.x, offsetY, startPosition.z);
        EnableCheckPoints();

        //create finish line
        finishLinePos = new Vector3(startPosition.x, startPosition.y + 1.5f, startPosition.z-3f);
        Instantiate(finishLine, finishLinePos, Quaternion.Euler(0, 0, 0), transform.Find("Track"));

        Debug.Log("Training Start");
        
        int agentsToSpawn = numOfAgents;
        if (spawnAgents && academy.spawnAgents)
        {
            if (academy.agentsPerArea != 0)
            {
                Debug.Log("Agents (academy): " + academy.agentsPerArea);
                agentsToSpawn = academy.agentsPerArea;
            }
            else
            {
                Debug.Log("Agents (area): " + agentsToSpawn);
            }
            SpawnAgents(agentsToSpawn, startPosition, Quaternion.Euler(0, 0, 0));
        }
    }

    private void FixedUpdate()
    {
        
    }

    //spawn agents in an area around the specified position
    public void SpawnAgents(int agents, Vector3 _position, Quaternion _rotation)
    {
        ReadNumOfAgents();
        for (int i = 0; i < agents; i++)
        {
            Debug.Log(agents);
            Debug.Log(agentPPO);
            Debug.Log(agentSAC);
            if (agentPPO != 0 && agentSAC != 0)
            {
                Instantiate(agentPrefabPPO, _position, _rotation, transform.Find("Agents"));
                Instantiate(agentPrefabSAC, _position, _rotation, transform.Find("Agents"));
            }
            else if(agentPPO == 0)
            {
                Instantiate(agentPrefabSAC, _position, _rotation, transform.Find("Agents"));
            }
            else
            {
                Instantiate(agentPrefabPPO, _position, _rotation, transform.Find("Agents"));
            }
            
        }
    }

    public void UpdateMonitor(string text)
    {
        //areaMonitor.text = text;
        //monitorHUD.text = text;
    }

    public void UpdateStats(float laptime)
    {
        if (laptime < recordLap)
        {
            recordLap = laptime;
        }
        UpdateMonitor("Best Lap: " + recordLap);
    }

    //useless for now
    public void ResetArea()
    {
        if (agentList == null)
        {
            agentList = GameObject.FindObjectsOfType<CarAgent>();
        }

        foreach (CarAgent agent in agentList)
        {
            agent.OnEpisodeBegin();
        }
        ResetCheckpoints();
    }

    private void ResetCheckpoints()
    {
        //checkpoints are manual for now
    }


    private void LoadMap(){
        string fileName = mapName;
        using(StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + fileName + ".json")){
            string line;
            while((line = sr.ReadLine()) != null){ 
                bd = new BlockData();
                bd = JsonUtility.FromJson<BlockData>(line);
                foreach(GameObject item in roadBuildList){  
                    Debug.Log("script name" + bd.name + " || listname:" + item.name);
                    if(bd.name == item.name){
                        var go = Instantiate(item,bd.blockPosition,bd.blockRotation) as GameObject;
                        go.transform.SetParent(GameObject.Find("Track").transform);
                        go.tag = "wall";
                        go.layer = 10;
                        go.transform.GetChild(0).gameObject.layer = 10;
                        go.transform.GetChild(0).gameObject.tag = "wall";
                        go.transform.GetChild(1).gameObject.tag = "checkpoint";
                        go.transform.localPosition = bd.blockPosition;
                        go.transform.localRotation = bd.blockRotation;
                    }
                }
            }
        }
        Debug.Log("Loaded Successfully");
    }

    private void ReadNumOfAgents()
    {
        string line;
        string numOfAgentsString;
        string numofAgentsPPO;
        string numOfAgentsSAC;
        using (StreamReader sr = new StreamReader("../mainBuild/CarsML2021-main_Data/options.json"))
        {
            while ((line = sr.ReadLine()) != null)
            {  //is while necessary? probably not, it's probably making a lot of junk objects like this, but will i dare change it? nah.
                Options options = new Options();
                options = JsonUtility.FromJson<Options>(line);
                numOfAgentsString = options.numOfAgents;
                numofAgentsPPO = options.numOfAgentsPPO;
                numOfAgentsSAC = options.numOfAgentsSAC;

                numOfAgents = Int32.Parse(numOfAgentsString);
                agentPPO = Int32.Parse(numofAgentsPPO);
                agentSAC = Int32.Parse(numOfAgentsSAC);

            }

        }
        
    }
    public void EnableCheckPoints(){
        string checkDiff;
        using(StreamReader sr = new StreamReader(Path.Combine(Application.persistentDataPath,"checkpoints.txt"))){
            checkDiff = sr.ReadLine();
        }

    if(!isNumber(checkDiff)){
        Debug.LogError("Can only place numbers in textfield");
            checkDiff = "3";
    }
    int checkPointDiff = Int32.Parse(checkDiff);
    //int checkPointDiff = 3;
    int counter = 0;
    int cpCounter = 0;
    GameObject parentObj = GameObject.Find("Track");
    Debug.Log(parentObj);
    foreach(Transform child in parentObj.transform){
        Transform checkPointChild = child.transform.GetChild(1);
        if(counter % checkPointDiff == 0){
            checkPointChild.GetComponent<Checkpoint>().isEnabled = true;
            checkPointChild.GetComponent<Checkpoint>().checkpointNumber = cpCounter;
            cpCounter++;
        }
        counter++;
    }
}

    private bool isNumber(string inStr){
        Regex regex = new Regex(@"^\d+$");
        return regex.IsMatch(inStr);
    }

}
