using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class TR_OnClick : MonoBehaviour
{
    Dropdown d_model_SAC;
    Dropdown d_model_PPO;
    Dropdown d_map;
    Toggle t_heuristic;

    Text m_ModelTextSAC;
    Text m_ModelTextPPO;
    Text m_MapText;

    string options_path;
    string isTraining_path;
    string config_fileName;

    List<string> filePathsSAC = new List<string>();
    List<string> filePathsPPO = new List<string>();
    List<string> fileNameSAC = new List<string>();
    List<string> fileNamePPO = new List<string>();

    void Awake(){

        options_path = Application.dataPath + "/InferenceOptions.json";
        isTraining_path = Application.dataPath + "/isTraining.json";

        d_model_SAC = GameObject.Find("ModelDropdownSAC").GetComponent<Dropdown>();
        d_model_PPO = GameObject.Find("ModelDropdownPPO").GetComponent<Dropdown>();
        d_map = GameObject.Find("MapDropdown").GetComponent<Dropdown>();

        m_ModelTextSAC = GameObject.Find("ModelLabelSAC").GetComponent<Text>();
        m_ModelTextPPO = GameObject.Find("ModelLabelPPO").GetComponent<Text>();
        m_MapText = GameObject.Find("MapLabel").GetComponent<Text>();

        t_heuristic = GameObject.Find("HeuristicToggle").GetComponent<Toggle>();

        string path = Path.Combine(Application.dataPath, "../") + "/models/";


        GetBrainFiles();



        path = Application.persistentDataPath;
        List<string> m_mapDropOptions = new List<string>();
        //List<string> m_modelDropOptionsSAC = new List<string>();
        //List<string> m_modelDropOptionsPPO = new List<string>();


        foreach(var f in Directory.GetFiles(path,"*.json")){
            var filename = new FileInfo(f).Name;
           m_mapDropOptions.Add(filename);
        }
        d_map.AddOptions(m_mapDropOptions);



        if (filePathsSAC != null)
        {
            d_model_SAC.AddOptions(fileNameSAC);
        }
        if (filePathsPPO != null)
        {
            d_model_PPO.AddOptions(fileNamePPO);
        }


    }

    private void GetBrainFiles()
    {


        string path;
        if (Application.isEditor)
        {
            path = Directory.GetParent(Application.dataPath) + "/mainBuild/results/";
        }
        else
        {
            path = Directory.GetParent(Application.dataPath) + "/results/";
        }

        string[] directories = Directory.GetDirectories(path);
        string[] fileNames;


        if (directories.Length > 0)
        {
            foreach (string dir in directories)
            {
                string[] files = Directory.GetFiles(dir, "*.onnx");
                if (files.Length != 0 && files != null)
                {
                    //only checking the 1st item because there's only 1 brain per algo in the folder anyway
                    try
                    {
                        string filename = Directory.GetFiles(dir, "*sac*.onnx")[0];
                        filename = CompileString(filename);

                        filePathsSAC.Add(Directory.GetFiles(dir, "*sac*.onnx")[0]);
                        fileNameSAC.Add(filename);
                    }
                    catch { }
                    try
                    {
                        string filename = Directory.GetFiles(dir, "*ppo*.onnx")[0];
                        filename = CompileString(filename);

                        filePathsPPO.Add(Directory.GetFiles(dir, "*ppo*.onnx")[0]);
                        fileNamePPO.Add(filename);

                    }
                    catch { }

                }
            }
            if (filePathsSAC.Count == 0 && filePathsPPO.Count == 0)
            {
                Debug.Log("No brain/model files were found.");
            }
        }
        else
        {
            Debug.Log("No result directories found");

        }

    }

    public string CompileString(string filename)
    {
        Regex r = new Regex(@"\/([a-z]\w+)\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        MatchCollection matches = r.Matches(filename);

        return r.Match(filename).Groups[1].Value;
    }

    public void OnClickRun(){

        int modelValSAC = d_model_SAC.value;
        string modelSac;
        if (modelValSAC != 0)
            modelSac = filePathsSAC[modelValSAC - 1];
        else modelSac = "none";


        string modelPpo;
        int modelValPPO = d_model_PPO.value;
        if (modelValPPO != 0)
            modelPpo = filePathsPPO[modelValPPO - 1];
        else modelPpo = "none";
        
        
        int mapVal = d_map.value;
        m_MapText.text = d_map.options[mapVal].text;

        bool heuristic = t_heuristic.isOn;

        OverwriteOptions(modelPpo, modelSac, m_MapText.text, heuristic);
        

        //DontDestroyOnLoad(m_ModelTextSAC);
        //DontDestroyOnLoad(m_ModelTextPPO);
        //DontDestroyOnLoad(m_MapText);


        //SceneManager.LoadScene("Training", LoadSceneMode.Single);

        //launch the player for testing the selected model
        string strCmdText;
        string envPath;
        string sceneName = "CarsML2021-main.exe";
        if (!Application.isEditor)
            envPath = Path.Combine(Application.dataPath, "../../") + "/trainingScene/";
        else
            envPath = Path.Combine(Application.dataPath, "../") + "/trainingScene/";
        //strCmdText = "/K mlagents-learn --inference" + " --env = " + envPath + sceneName + " --width=1920 --height=1080" + " --mlagents-override-model=" + m_ModelText.text;
        strCmdText = "/K " + sceneName + " --mlagents-override-model CarBrainSAC "+ modelSac + " CarBrainPPO " + modelPpo;

        Debug.Log(strCmdText);
        var proc = new System.Diagnostics.ProcessStartInfo();
        proc.FileName = "cmd.exe";
        proc.WorkingDirectory = envPath;
        proc.Arguments = @strCmdText;
        proc.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden; 
        System.Diagnostics.Process.Start(proc); //Start cmd process
        Debug.Log(proc);
    }

    public void OnClickBack(){
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }



    public void OverwriteOptions(string hasPPO, string hasSAC, string mapSelected,bool heuristic)
    {
        using (StreamWriter sw = new StreamWriter(options_path))
        {
            //write all options in JSON format
            InferenceOptions options = new InferenceOptions();
            options.testPPO = hasPPO;
            options.testSAC = hasSAC;
            options.selectedMap = mapSelected;
            options.heuristic = (heuristic) ? "true" : "false";
            string opt = JsonUtility.ToJson(options);
            sw.WriteLine(opt);
        }

        using (StreamWriter sw = new StreamWriter(isTraining_path))
        {
            //write all options in JSON format
            TrainingType options = new TrainingType();
            options.training = "false";
            string opt = JsonUtility.ToJson(options);
            sw.WriteLine(opt);
        }

    }
}


