using UnityEngine;
using UnityEngine.UI;
using System;  
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using Unity.Barracuda;

public class TrainingArea : MonoBehaviour
{
    public bool isBuildForDatacollection = false;
    [Header("Training Area Settings")]
    public CarAgent agentPrefabPPO;
    public CarAgent agentPrefabSAC;
    public CarAgent agentPrefabHeuristic;

    public string mapName = "selectedMap";

    public bool isTraining;
    public bool spawnAgents = false;
    public bool agentPPO;
    public bool agentSAC;
    public bool heuristic;
    public bool inferPPO, inferSAC;
    public int numOfAgents;
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

    public Text trainingHUD;
    public Text drivingHUD;
    public Text performanceHUD;
    public Vector3 startPosition;
    private Vector3 finishLinePos;
    private CarAgent[] agentList;
    private CarAcademy academy;
    private GameObject[] cameraList;

    public List<Checkpoint> checkpointList = new List<Checkpoint>();
    public List<Checkpoint> checkpointPassingList = new List<Checkpoint>();
    private int currentCam;

    
    //performance
    public float recordLap = Mathf.Infinity;

    public List<GameObject> roadBuildList = new List<GameObject>();

    string modelName;

    [SerializeField] private BlockData bd;


    private string options_path, inference_options_path, training_type_path;

    const string k_CommandLineModelOverrideFlag = "--mlagents-override-model";
    public Dictionary<string, string> m_BehaviorNameOverrides = new Dictionary<string, string>();
    public Dictionary<string, NNModel> m_CachedModels = new Dictionary<string, NNModel>();

    void Awake()
    {
        
        if (!Application.isEditor)
        {
            if(!isBuildForDatacollection)
            training_type_path = Path.Combine(Application.dataPath, "../../mainBuild/CarsML2021-main_Data") + "/isTraining.json";
            else training_type_path = Path.Combine(Application.dataPath, "../") + "/isTraining.json";
            CheckTraining();
            if (isTraining) {
                options_path = Path.Combine(Application.dataPath, "../../mainBuild/CarsML2021-main_Data") + "/options.json";
            }
            else
            {
                if(!isBuildForDatacollection)
                inference_options_path = Path.Combine(Application.dataPath, "../../mainBuild/CarsML2021-main_Data") + "/InferenceOptions.json";
                else inference_options_path = Path.Combine(Application.dataPath, "../") + "/InferenceOptions.json";
            }
            
            //training_type_path = Path.Combine(Application.dataPath, "../../mainBuild/CarsML2021-main_Data") + "/isTraining.json";
        }
        else
        {
            //if we are in editor we use our inspector values
            //training_type_path = Application.dataPath + "/isTraining.json";
           // CheckTraining();
           //Just use the editor values if you are in editor.
            if (isTraining) 
            options_path = Application.dataPath + "/options.json";
            else
            inference_options_path = Application.dataPath + "/InferenceOptions.json";
            //training_type_path = Application.dataPath + "/isTraining.json";
        }

        //read args to load models, make sure it's training and in the app
        if(!isTraining && !Application.isEditor)
        GetAssetPathFromCommandLine();

        LoadMap();

        Debug.Log("Continue");
        if(isTraining)
        LoadOptionsForTraining();



        academy = GameObject.Find("Academy").GetComponent<CarAcademy>();

        startPosition = GameObject.Find("Track").transform.GetChild(0).GetChild(0).position; // get the first road block of the track
                                                                                             // float offsetY = startPosition.y + 10f;
                                                                                             // startPosition = new Vector3(startPosition.x, offsetY, startPosition.z);

        EnableCheckPoints();
        


        //create finish line
        finishLinePos = new Vector3(startPosition.x, startPosition.y + 1.5f, startPosition.z - 3f);
        Instantiate(finishLine, finishLinePos, Quaternion.Euler(0, 0, 0), transform.Find("Track"));
        finishLine.GetChild(0).transform.localPosition = new Vector3(0,-1.54f, 0);
        finishLine.GetChild(0).transform.localScale = new Vector3(9,0.1f,3);

        //initialize cameras (create list, find active one and store index)
        cameraList = GameObject.FindGameObjectsWithTag("camera");
        for (int i = 0; i < cameraList.Length; i++)
        {
            cameraList[i].SetActive(false);
        }
        

        if (!heuristic)
        {
            currentCam = 0;
            cameraList[0].SetActive(true);
        }
        else
        {
            currentCam = 1;
            cameraList[1].SetActive(true);
        }
        

         Debug.Log("Training Start");
         int agentsToSpawn;
         if (!Application.isEditor)
         {
            if (isTraining)
            {
                 ReadNumOfAgents();
                 agentsToSpawn = numOfAgents;
    
            }
            else
            {
                agentsToSpawn = 1;
                CheckInferneceOptions();

            }

           }
            else
            {
                if (isTraining)
                {
                    //when in editor, values are controlled from the editor
                    agentsToSpawn = numOfAgents;
                    Debug.Log(numOfAgents);
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
                    }
                

                }
                else
                {
                    agentsToSpawn = 1;

                }

        }
        SpawnAgents(agentsToSpawn, startPosition, Quaternion.Euler(0, 0, 0));

    }


    private void Start()
    {
        Debug.Log("IsTraining " + isTraining + "\nSpawn Agents " + spawnAgents + "\nMapName " + mapName + "\nAgentPPO " + agentPPO + "\nAgent SAC " + agentSAC + "\nHeuristic " + heuristic
            + "\nInfer PPO " + inferPPO + "\nInfer Sac " + inferSAC + "\nnum of agents " + numOfAgents + "\nSuccessReward " + successReward + "\nSpeedReward " + speedReward + 
            "\ncollisionPenalty  " + collisionPenalty);
    }

    void GetAssetPathFromCommandLine()
    {
        m_BehaviorNameOverrides.Clear();


        var args = Environment.GetCommandLineArgs();
        for (var i = 0; i < args.Length - 1; i++)
        {
            if (args[i] == k_CommandLineModelOverrideFlag && i < args.Length - 2)
            {
                var key = args[i + 1].Trim();
                var value = args[i + 2].Trim();
                m_BehaviorNameOverrides[key] = value;
            }

        }


        Debug.Log("args: ");
        foreach (string a in args)
        {
            Debug.Log(a);

        }
        Debug.Log("behaviours: ");
        foreach (KeyValuePair<string, string> pair in m_BehaviorNameOverrides)
        {
            Debug.Log("Behaviour: " + pair.Key + " -- " + pair.Value);
        }

    }

    public Checkpoint getNextCheckpoint()
    {
        if (checkpointPassingList.Count <= 0)
            ResetCheckpoints();
        return checkpointPassingList[0];
    }

    public void PopCheckpoint(Checkpoint current)
    {
        foreach(Checkpoint cp in checkpointPassingList)
        {
            if(current == cp)
            {
                    checkpointPassingList.Remove(current);
                
            }
        }
    }

    public void ResetCheckpoints()
    {
        checkpointPassingList.Clear();
        foreach (Checkpoint cp in checkpointList)
        {
            cp.isPassed = false;
            checkpointPassingList.Add(cp);
        }
    
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            ToggleView();
        }
      
    }
    

    //spawn agents in an area around the specified position
    public void SpawnAgents(int agents, Vector3 _position, Quaternion _rotation)
    {
        if (heuristic)
        {
            Instantiate(agentPrefabHeuristic, _position, _rotation, transform.Find("Agents"));
        }
        else if(isTraining)
        {
            Debug.Log("Spawning " + agents + " agents at " + _position + " and rotation " + _rotation);
            for (int i = 0; i < agents; i++)
            {
                if (agentPPO)
                    Instantiate(agentPrefabPPO, _position, _rotation, transform.Find("Agents"));
                if (agentSAC)
                    Instantiate(agentPrefabSAC, _position, _rotation, transform.Find("Agents"));
            }
        }
        else
        {
            if (inferPPO)
            {
                Instantiate(agentPrefabPPO, _position, _rotation, transform.Find("Agents"));
            }
            if (inferSAC)
            {
                Instantiate(agentPrefabSAC, _position, _rotation, transform.Find("Agents"));
            }
        }
  
            



    }

    public void UpdateMonitor(string trainingText, string drivingText, string performanceText)
    {
        //areaMonitor.text = text;
        trainingHUD.text = trainingText;
        drivingHUD.text = drivingText;
        performanceHUD.text = performanceText;
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

 


    private void LoadMap(){
        string fileName = mapName;
        using(StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + fileName + ".json")){
            string line;
            while((line = sr.ReadLine()) != null){ 
                bd = new BlockData();
                bd = JsonUtility.FromJson<BlockData>(line);
                foreach(GameObject item in roadBuildList){
                    //Debug.Log("script name" + bd.name + " || listname:" + item.name);
                    if (bd.name == item.name)
                    {
                        var go = Instantiate(item, bd.blockPosition, bd.blockRotation) as GameObject;
                        go.transform.SetParent(GameObject.Find("Track").transform);
                        go.tag = "wall";
                        go.layer = 10;
                        if (go.transform.childCount > 0) { 
                            go.transform.GetChild(0).gameObject.layer = 10;
                            go.transform.GetChild(0).gameObject.tag = "wall";
                            go.transform.GetChild(1).gameObject.tag = "checkpoint";
                        }
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
        StreamReader sr;

        sr = new StreamReader(options_path);
        using (sr)
        {
            while ((line = sr.ReadLine()) != null)
            {  //is while necessary? probably not, it's probably making a lot of junk objects like this, but will i dare change it? nah.
                Options options = new Options();
                options = JsonUtility.FromJson<Options>(line);
                numOfAgentsString = options.numOfAgents;
                numofAgentsPPO = options.numOfAgentsPPO;
                numOfAgentsSAC = options.numOfAgentsSAC;

                numOfAgents = Int32.Parse(numOfAgentsString);
                agentPPO = Int32.Parse(numofAgentsPPO) > 0 ? true : false;
                agentSAC = Int32.Parse(numOfAgentsSAC) > 0 ? true : false;
            }
        }       
    }

    private void LoadOptionsForTraining()
    {
        string line;
        StreamReader sr;

        sr = new StreamReader(options_path);
        using (sr)
        {
            while ((line = sr.ReadLine()) != null)
            {  //is while necessary? probably not, it's probably making a lot of junk objects like this, but will i dare change it? nah.
                Options options = new Options();
                options = JsonUtility.FromJson<Options>(line);
                successReward = float.Parse(options.successReward);
                checkpointReward = float.Parse(options.cpReward);
                speedReward = float.Parse(options.speedReward);
                collisionPenalty = (-1) * float.Parse(options.collisionPenalty);
                idlePenalty = (-1) * float.Parse(options.idlePenalty);
                wrongCheckpointPenalty = (-1) * float.Parse(options.wrongCheckPenalty);
            }
        }
    }

    private void CheckTraining()
    {
        string line;
        StreamReader sr;

        sr = new StreamReader(training_type_path);
        using (sr) {
            line = sr.ReadLine();
            TrainingType train = new TrainingType();
            train = JsonUtility.FromJson<TrainingType>(line);
            isTraining = (train.training == "true") ? true : false;
        }
    }

    private void CheckInferneceOptions()
    {
        string line;
        StreamReader sr;

        sr = new StreamReader(inference_options_path);
        using (sr)
        {
            line = sr.ReadLine();
            InferenceOptions infer = new InferenceOptions();
            infer = JsonUtility.FromJson<InferenceOptions>(line);

            inferPPO = (infer.testPPO != "none") ? true : false;
            inferSAC = (infer.testSAC != "none") ? true : false;
            heuristic = (infer.heuristic == "true") ? true : false;
            
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
        if(child.transform.childCount > 0)
            {
                for(int i = 1; i < child.transform.childCount; i++)
                {
                    Transform checkPointChild = child.transform.GetChild(i);
                    if (counter % checkPointDiff == 0)
                    {
                        checkPointChild.GetComponent<Checkpoint>().isEnabled = true;
                        checkPointChild.GetComponent<Checkpoint>().checkpointNumber = cpCounter;
                        checkpointList.Add(checkPointChild.GetComponent<Checkpoint>());
                        cpCounter++;
                    }
                    counter++;
                }
                
            }
    }
}

    private bool isNumber(string inStr){
        Regex regex = new Regex(@"^\d+$");
        return regex.IsMatch(inStr);
    }

    // changes camera views (currently between top down and follow camera (random agent)
    public void ToggleView()
    {
        cameraList[currentCam].SetActive(false);
        int nextCam = (currentCam + 1) % cameraList.Length;
        cameraList[nextCam].SetActive(true); //cyclically iterate through the array
        currentCam = nextCam;
    }
}
