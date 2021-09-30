using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using System.Text;
using System.Text.RegularExpressions;

public class CarAgent : Agent
{
    [Header("Car Agent Settings")]
    public float maxIdleTime = 10;
    public bool monitorInfo = false;

    private CarController car;
    private Engine carEngine;
    private Rigidbody agentRigidbody;
    private TrainingArea trainingArea;
    private int checkpointsPassed = 0;
    private Vector3 startingPosition;
    private Quaternion startingRotation;
    private float idleMeter;
    private float maxReward = 0;
    private float maxLaps = 3;

    private bool respawned = false;

    [HideInInspector] Vector3 localVelocity;
    [HideInInspector] public float speed;

    //performance variables
    [Header("Timers and Performance")]
    public int lapsCompleted;
    public float lapTime;
    public float prevLapTime;
    public float bestLap;
    public int mostLaps;
    //public float totalTime;
    private float spawnTime;
    private float meanLapTime;

    private bool dejaVu;



    List<float> bestLapTimes = new List<float>();
    List<float> meanLapTimes = new List<float>();
    List<int> errorCounts = new List<int>();
    int setsCompleted;
    int errorCount;

    public override void Initialize()
    {
        base.Initialize();
        
        gameObject.layer = 8;
        car = GetComponent<CarController>();
        carEngine = GetComponent<Engine>();
        agentRigidbody = GetComponent<Rigidbody>();
        trainingArea = transform.parent.GetComponentInParent<TrainingArea>();

        startingPosition = transform.localPosition;
        startingRotation = transform.rotation;

        agentRigidbody.centerOfMass = GameObject.Find("CenterOfMass").transform.localPosition;
        idleMeter = maxIdleTime;
        car.SetGear(1);
    }

    public void Start()
    {
        //setsCompleted = 0;
        //if (!Application.isEditor)
        //{
        //    if (trainingArea.m_BehaviorNameOverrides.ContainsKey(GetComponent<BehaviorParameters>().BehaviorName))
        //    {
        //        Debug.Log("Overriding brain...");
        //        OverrideModel();

        //    }
        //    if (GetComponent<BehaviorParameters>().Model == null)
        //        Debug.Log("Model Name After : " + "null");
        //    else
        //    {
        //        Debug.Log("Model Name After : " + GetComponent<BehaviorParameters>().Model.ToString());
        //    }
        //}


    }

    public void FixedUpdate()
    {
        spawnTime += Time.deltaTime;
        //brake and freeze position if freshly respawned
        //this is to avoid some issues with unity physics and entity spawning
        if (respawned)
        {
            car.HandBrake();
            agentRigidbody.isKinematic = true;
            respawned = false;
            spawnTime = 0f;
        }
        else
        {
            agentRigidbody.isKinematic = false;
        }

        //determine slip (temp)
        float driftAngle = Vector3.Angle(agentRigidbody.velocity, transform.forward);
        if (driftAngle > 5f)
        {
            dejaVu = true;
        }
        else
        {
            dejaVu = false;
        }
        
       
            if (maxReward < GetCumulativeReward())
            {
                maxReward = GetCumulativeReward();
            }
            string hudTextTraining = "Reward: " + GetCumulativeReward().ToString("0.00") +
                        "\nMax Reward: " + maxReward.ToString("0.00") +
                        "\nStep Count: " + StepCount;
            string hudTextDriving = "\nSpeed: " + speed.ToString("0.00") +
                        "\nTorque: " + car.carEngine.Torque.ToString("0") +
                        "\nRPM: " + car.carEngine.Rpm.ToString("0") +
                        "\nTireRPM: " + car.frontDriverW.rpm.ToString("0") +
                        "\nDrift Angle: " + driftAngle.ToString("0.0") +
                        "\nGas: " + car.GasPedalPosition.ToString("P") +
                        "\nBrake: " + car.BrakePedalPosition.ToString("P") +
                        "\nSteering Angle: " + car.SteeringAngle.ToString("0.00") +
                        "\nGear: " + car.GetGear();
            string hudTextPerformance = "\nSets: " + setsCompleted + "\nLaps: " + lapsCompleted +
                        "\nBest Lap " + bestLap + "\nErrors: " + errorCount;
        if(monitorInfo)
           trainingArea.UpdateMonitor(hudTextTraining, hudTextDriving, hudTextPerformance);
        
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //car vision
        //handled by the RayPerceptionSensor component

        //car velocity
        localVelocity = transform.InverseTransformDirection(agentRigidbody.velocity);
        sensor.AddObservation(localVelocity.magnitude);

        //car rotation
        //sensor.AddObservation(transform.localRotation);

        //engine state
        //sensor.AddObservation(car.GetGear());
        //sensor.AddObservation(carEngine.Rpm);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //set steering amount
        car.WheelPosition = actionBuffers.ContinuousActions[1];

        //set gear
        //car.SetGear(actionBuffers.DiscreteActions[0]);
        //todo: mask the action of setting the same gear again

        //determine acceleration and brake
        if (actionBuffers.ContinuousActions[0] < 0)
        {
            car.GasPedalPosition = 0f;
            car.BrakePedalPosition = -actionBuffers.ContinuousActions[0];
        }
        else
        {
            car.BrakePedalPosition = 0f;
            car.GasPedalPosition = actionBuffers.ContinuousActions[0];
        }

        //reward forward speed to encourage movement
        //speed = Mathf.Sqrt(Mathf.Pow(localVelocity.x, 2) + Mathf.Pow(localVelocity.z, 2));
        speed = localVelocity.magnitude;
        AddReward(speed * trainingArea.speedReward);

        //Give up and reset if agent stays idle for too long
        if (idleMeter <= 0)
        {
            AddReward(trainingArea.idlePenalty);
            errorCount++;
            EndEpisode();
        }
        else
        {
            //reduce idle meter every step
            idleMeter -= 0.01f;
        }
    }

    public override void OnEpisodeBegin()
    {
        agentRigidbody.velocity = Vector3.zero;
        agentRigidbody.angularVelocity = Vector3.zero;
        transform.localPosition = startingPosition;
        transform.rotation = startingRotation;
        idleMeter = maxIdleTime;
        respawned = true; //flag as "just respawned" to help reset speed and position

        spawnTime = 0f;
        prevLapTime = 0f;
        bestLap = Mathf.Infinity;
        meanLapTime = 0;
        checkpointsPassed = 0;
        lapsCompleted = 0;
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("wall"))
        {
            //Give up and reset
            AddReward(trainingArea.collisionPenalty);
            errorCount++;
            EndEpisode();
        }
    }
    
    private void OnTriggerEnter(Collider col)
    {
        //check if a checkpoint is passed
        if (col.gameObject.CompareTag("checkpoint"))
        {
            //check if agent passed the correct checkpoint (ie. didnt go backwards)
            int checkpointNumber = col.gameObject.GetComponent<Checkpoint>().checkpointNumber;

            //if (checkpointsPassed % trainingArea.totalCheckpoints == checkpointNumber - 1)
            if (true)
            {
                checkpointsPassed++;
                /************disabled for infinite laps********************
                //check it's the last checkpoint(ie the finish line)
                if (checkpointsPassed == trainingArea.totalCheckpoints)
                {
                    AddReward(trainingArea.successReward);
                    Debug.Log("Success!");
                    Done();
                }
                else
                {
                    AddReward(trainingArea.checkpointReward);
                    idleMeter = maxIdleTime;
                }
                **********************************************************/
                AddReward(trainingArea.checkpointReward);
                idleMeter = maxIdleTime;
            }
            else
            {
                //Give up and reset if agent went backwards
                //SetReward(trainingArea.wrongCheckpointPenalty);
                errorCount++;
                EndEpisode();
            }
        }

        //passing the finish line
        if (col.gameObject.CompareTag("finish"))
        {
            lapTime = spawnTime - prevLapTime;
            prevLapTime = lapTime;
            meanLapTime += lapTime;
            if(bestLap > lapTime)
            {
                bestLap = lapTime;
            }
            lapsCompleted++;
            if (lapsCompleted == maxLaps)
            {
                AddReward(trainingArea.successReward);
                bestLapTimes.Add(bestLap);
                meanLapTimes.Add(meanLapTime / 3.0f);
                errorCounts.Add(errorCount);
                errorCount = 0;
                bestLap = Mathf.Infinity;
                meanLapTime = 0f;
                setsCompleted++;
                Debug.Log("Success!");
                EndEpisode();
            }
        }
    }

    //statistics tracker
    public void StatTracker()
    {
        //probably useless
    }

    //select "Heuristic Only" in Behaviour Parameters to control agent manually
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;
        continuousActionsOut[0] = Input.GetAxis("Vertical");
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        if (Input.GetKey("1"))
        {
            discreteActionsOut[0] = 1;
            Debug.Log("Setting gear: " + discreteActionsOut[0]);
        }
        else if (Input.GetKey("2"))
        {
            discreteActionsOut[0] = 2;
            Debug.Log("Setting gear: " + discreteActionsOut[0]);
        }
        else if (Input.GetKey("3"))
        {
            discreteActionsOut[0] = 3;
            Debug.Log("Setting gear: " + discreteActionsOut[0]);
        }
        else if (Input.GetKey("4"))
        {
            discreteActionsOut[0] = 4;
            Debug.Log("Setting gear: " + discreteActionsOut[0]);
        }
        else if (Input.GetKey("0"))
        {
            discreteActionsOut[0] = 0;
            Debug.Log("Setting gear: " + discreteActionsOut[0]);
        }
    }
    
    void OverrideModel()
    {
        var bp = GetComponent<BehaviorParameters>();

        var nnModel = GetModelForBehaviorName(bp.BehaviorName);
        if (trainingArea.m_BehaviorNameOverrides.ContainsKey(bp.BehaviorName))
        {
            Debug.Log($"Overriding behavior {bp.BehaviorName} for agent with model {nnModel?.name}");
            // This might give a null model; that's better because we'll fall back to the Heuristic
            SetModel($"Override_{bp.BehaviorName}", nnModel);
            Debug.Log("Model Set: " + GetComponent<BehaviorParameters>().Model.name);
        }
        

    }
    NNModel GetModelForBehaviorName(string behaviorName)
    {
        
        if (trainingArea.m_CachedModels.ContainsKey(behaviorName))
        {
            return trainingArea.m_CachedModels[behaviorName];
        }

        if (!trainingArea.m_BehaviorNameOverrides.ContainsKey(behaviorName))
        {
            Debug.Log($"No override for behaviorName {behaviorName}");
            return null;
        }

        var assetPath = trainingArea.m_BehaviorNameOverrides[behaviorName];
        Debug.Log("Behaviour Name " + behaviorName + " Asset Path " + assetPath);
        byte[] model = null;
        try
        {
            model = File.ReadAllBytes(assetPath);
        }
        catch (IOException)
        {
            Debug.Log($"Couldn't load file {assetPath}", this);
            // Cache the null so we don't repeatedly try to load a missing file
            trainingArea.m_CachedModels[behaviorName] = null;
            return null;
        }

        var asset = ScriptableObject.CreateInstance<NNModel>();
        asset.modelData = ScriptableObject.CreateInstance<NNModelData>();
        asset.modelData.Value = model;

        asset.name = "Override - " + Path.GetFileName(assetPath);
        trainingArea.m_CachedModels[behaviorName] = asset;
        return asset;
    }

    void OnApplicationQuit()
    {
        Debug.Log("Im Quiting");
        //if we are testing models write their performance in csv files
        if (!trainingArea.isTraining && !trainingArea.heuristic)
        {
            var csv = new StringBuilder();

            for (int i = 0; i < bestLapTimes.Count; i++)
            {
                csv.AppendLine(bestLapTimes[i].ToString());
            }

            File.WriteAllText(Application.persistentDataPath + "/" + gameObject.name + "-BestLapTimes.csv", csv.ToString());

            csv = new StringBuilder();

            for(int i = 0; i < meanLapTimes.Count; i++)
            {
                csv.AppendLine(meanLapTimes[i].ToString());
            }

            File.WriteAllText(Application.persistentDataPath + "/" + gameObject.name + "-MeanLapTimes.csv", csv.ToString());

            csv = new StringBuilder();
            for (int i = 0; i < errorCounts.Count; i++)
            {
                csv.AppendLine(errorCounts[i].ToString());
            };
            File.WriteAllText(Application.persistentDataPath + "/" + gameObject.name + "-ErrorCount.csv", csv.ToString());
        }
        else if (trainingArea.heuristic) //it would be testing anyway
        {
            string path = Application.persistentDataPath + "/Heuristic-" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss");
            DirectoryInfo di = Directory.CreateDirectory(Application.persistentDataPath + "/Heuristic-" + DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH'-'mm'-'ss"));
            Console.WriteLine("The directory was created successfully at {0}.", Directory.GetCreationTime(path));

            var csv = new StringBuilder();

            for (int i = 0; i < bestLapTimes.Count; i++)
            {
                csv.AppendLine(bestLapTimes[i].ToString());
            }

            File.WriteAllText(path + "/" + gameObject.name + "-BestLapTimes.csv", csv.ToString());

            csv = new StringBuilder();

            for (int i = 0; i < meanLapTimes.Count; i++)
            {
                csv.AppendLine(meanLapTimes[i].ToString());
            }

            File.WriteAllText(path + "/" + gameObject.name + "-MeanLapTimes.csv", csv.ToString());

        }

    }

}

    //void GetAssetPathFromCommandLine()
    //{
    //    m_BehaviorNameOverrides.Clear();


    //    var args = Environment.GetCommandLineArgs();
    //    string arg = "";
    //    foreach(string a in args)
    //    {
    //        arg += a + " ";
    //    }

    //    Debug.Log("ARGUMENT STRING" + arg);

    //    if (!Application.isEditor)
    //    {

    //        /*
    //         *    Argument Looks like this : 
    //         * /UnityProjects/GitHub/CarsML2021-main/mainBuild/CarsML2021-main_Data\../..//trainingScene/CarsML2021-main.exe 
    //         * --mlagents-override-model CarBrainSAC E:\UnityProjects\GitHub\CarsML2021-main\mainBuild/results/pposac2\CarBrainSAC.onnx 
    //         * --mlagents-override-model CarBrainPPO E:\UnityProjects\GitHub\CarsML2021-main\mainBuild/results/ppo1\CarBrainPPO.onnx  
    //         */

    //        Regex r = new Regex(@"--mlagents-override-model (\w+) ([^ ]+CarBrainSAC.onnx) --mlagents-override-model (\w+) ([^ ]+CarBrainPPO.onnx)", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    //        MatchCollection matches = r.Matches(arg);

    //        GroupCollection groups = matches[0].Groups;
    //        var sacName = groups[1].Value;
    //        var pathSAC = @"" + groups[2].Value;

    //        var ppoName = groups[3].Value;
    //        var pathPPO = @"" + groups[4].Value;

    //        Debug.Log("SHOW ME KEYS" + sacName + " " + ppoName);
    //        Debug.Log("SHOW ME PATHS" + pathSAC + " || " + pathPPO);

    //        if(GetComponent<BehaviorParameters>().BehaviorName == sacName) //check if this agent is ppo or sac
    //            NewOverrideModel(sacName, pathSAC);
    //        else
    //            NewOverrideModel(ppoName, pathPPO);


    //    }

    //    else
    //    {
    //        var value1 = @""+ Path.Combine(Application.dataPath, "../../mainBuild/results/sactest1/CarBrainSAC.onnx");
    //        Debug.Log(value1);
    //        var key1 = "CarBrainSAC";
    //        m_BehaviorNameOverrides[key1] = value1;

    //        var value2 = @"" + Path.Combine(Application.dataPath, "../../mainBuild/results/ppo1/CarBrainPPO.onnx");
    //        var key2 = "CarBrainPPO";
    //        Debug.Log(value2);
    //        m_BehaviorNameOverrides[key2] = value2;
    //    }
    //}



    //void NewOverrideModel(string behaviorName, string path)
    //{
    //    Debug.Log("Overriding... " + path + " " + behaviorName);

    //    this.LazyInitialize();


    //    var nnModel = CustomGetModelForBehaviorName(behaviorName, path);
    //    SetModel(behaviorName, nnModel);
    //    Debug.Log("IM DONE SETTING AND : " + GetComponent<BehaviorParameters>().BehaviorName + " " + GetComponent<BehaviorParameters>().Model.name);
    //}

    //NNModel CustomGetModelForBehaviorName(string behaviorName,string path){


    //    byte[] model = null;
        


    //    try
    //    {
    //        model = File.ReadAllBytes(path);
    //    }
    //    catch (IOException)
    //    {
    //        Debug.Log($"Couldn't Load file {path}", this);
    //        return null;
    //    }

    //    var asset = ScriptableObject.CreateInstance<NNModel>();
    //    asset.modelData = ScriptableObject.CreateInstance<NNModelData>();
    //    asset.modelData.Value = model;

    //    asset.name = Path.GetFileName(path);
    //    Debug.Log("THIS IS THE NEW MODEL? : " + asset.name + " " + asset.modelData.name + "  " + asset.modelData.Value +  " ");
    //    return asset;

    //}

