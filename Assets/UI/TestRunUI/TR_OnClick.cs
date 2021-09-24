using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

public class TR_OnClick : MonoBehaviour
{
    Dropdown d_model_SAC;
    Dropdown d_model_PPO;
    Dropdown d_map;

    Text m_ModelTextSAC;
    Text m_ModelTextPPO;
    Text m_MapText;

    string config_fileName;

    List<string> filePathsSAC = new List<string>();
    List<string> filePathsPPO = new List<string>();

    void Awake(){

        d_model_SAC = GameObject.Find("ModelDropdownSAC").GetComponent<Dropdown>();
        d_model_PPO = GameObject.Find("ModelDropdownPPO").GetComponent<Dropdown>();
        d_map = GameObject.Find("MapDropdown").GetComponent<Dropdown>();

        m_ModelTextSAC = GameObject.Find("ModelLabelSAC").GetComponent<Text>();
        m_ModelTextPPO = GameObject.Find("ModelLabelPPO").GetComponent<Text>();
        m_MapText = GameObject.Find("MapLabel").GetComponent<Text>();

        string path = Path.Combine(Application.dataPath, "../") + "/models/";
        List<string> m_modelDropOptionsSAC = new List<string>(); 
        List<string> m_modelDropOptionsPPO = new List<string>();

        GetBrainFiles();

        m_modelDropOptionsSAC = filePathsSAC;
        m_modelDropOptionsPPO = filePathsPPO;

        //path = Application.persistentDataPath;
        //List<string> m_mapDropOptions = new List<string>();
        /* temporarily disabled
        foreach(var f in Directory.GetFiles(path,"*.json")){
            var filename = new FileInfo(f).Name;
           m_mapDropOptions.Add(filename);
        }
        d_map.AddOptions(m_mapDropOptions);
        */
        if (m_modelDropOptionsSAC != null)
        {
            d_model_SAC.AddOptions(m_modelDropOptionsSAC);
        }
        if (m_modelDropOptionsPPO != null)
        {
            d_model_PPO.AddOptions(m_modelDropOptionsPPO);
        }


    }
    private void GetBrainFiles()
    {
        string path;
        if (Application.isEditor)
        {
            path = Directory.GetParent(Application.dataPath) + "/mainbuild/results/";
        }
        else
        {
            path = Directory.GetParent(Application.dataPath) + "/results/";
        }
        
        string[] directories = Directory.GetDirectories(path);
        string[] fileNames;  //TODO make it so just names are displayed instead of paths, but the path is used as the value (prob with dictionary?)


        if (directories.Length>0)
        {
            foreach(string dir in directories)
            {
                string[] files = Directory.GetFiles(dir, "*.onnx");
                if (files.Length != 0 && files != null)
                {
                    //only checking the 1st item because there's only 1 brain per algo in the folder anyway
                    try
                    {
                        filePathsSAC.Add(Directory.GetFiles(dir, "*sac*.onnx")[0]);
                    }
                    catch { }
                    try {
                        filePathsPPO.Add(Directory.GetFiles(dir, "*ppo*.onnx")[0]);
                    }
                    catch { }
                    
                }
            }
            if(filePathsSAC.Count == 0 && filePathsPPO.Count == 0)
            {
                Debug.Log("No brain/model files were found.");
            }
        }
        else 
        {
            Debug.Log("No result directories found");
            
        }

    }


    public void OnClickRun(){

        int modelValSAC = d_model_SAC.value;
        m_ModelTextSAC.text = d_model_SAC.options[modelValSAC].text;

        int modelValPPO = d_model_PPO.value;
        m_ModelTextPPO.text = d_model_PPO.options[modelValPPO].text;

        //int mapVal = d_map.value;
        //m_MapText.text = d_map.options[mapVal].text;

        DontDestroyOnLoad(m_ModelTextSAC);
        DontDestroyOnLoad(m_ModelTextPPO);
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
        strCmdText = "/C " + sceneName + " --mlagents-override-model CarBrainSAC "+ m_ModelTextSAC.text + " CarBrainPPO " + m_ModelTextPPO.text;

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
}
