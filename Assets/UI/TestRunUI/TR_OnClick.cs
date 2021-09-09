using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
public class TR_OnClick : MonoBehaviour
{
    Dropdown d_model;
    Dropdown d_map;

    Text m_ModelText;
    Text m_MapText;

    string config_fileName;

    void Awake(){

        d_model = GameObject.Find("ModelDropdown").GetComponent<Dropdown>();
        d_map = GameObject.Find("MapDropdown").GetComponent<Dropdown>();

        m_ModelText = GameObject.Find("ModelLabel").GetComponent<Text>();
        m_MapText = GameObject.Find("MapLabel").GetComponent<Text>();

        string path = Path.Combine(Application.dataPath, "../") + "/models/";
        List<string> m_modelDropOptions = new List<string>();

        m_modelDropOptions = GetBrainFiles();
        
        //path = Application.persistentDataPath;
        //List<string> m_mapDropOptions = new List<string>();
        /* temporarily disabled
        foreach(var f in Directory.GetFiles(path,"*.json")){
            var filename = new FileInfo(f).Name;
           m_mapDropOptions.Add(filename);
        }
        d_map.AddOptions(m_mapDropOptions);
        */
        if(m_modelDropOptions != null)
        {
            d_model.AddOptions(m_modelDropOptions);
        }
        

    }
    private List<string> GetBrainFiles()
    {
        string path = Directory.GetParent(Application.dataPath) + "/mainBuild/results/";
        string[] directories = Directory.GetDirectories(path);
        string[] fileNames;
        List<string> filePaths = new List<string>();

        if (directories.Length>0)
        {
            foreach(string dir in directories)
            {
                string[] files = Directory.GetFiles(dir, "*.onnx");
                if (files.Length != 0 && files != null)
                {
                    //only checking the 1st item because there's only 1 brain in the folder anyway
                    filePaths.Add(Directory.GetFiles(dir, "*.onnx")[0]); 
                }
            }
            if(filePaths.Count == 0)
            {
                Debug.Log("No brain/model files were found.");
            }
            else
            {
                return filePaths;
            }
        }
        else 
        {
            Debug.Log("No result directories found");
            
        }
        return null;

    }


    public void OnClickRun(){

        int modelVal = d_model.value;
        m_ModelText.text = d_model.options[modelVal].text;

        //int mapVal = d_map.value;
        //m_MapText.text = d_map.options[mapVal].text;

        DontDestroyOnLoad(m_ModelText);
        //DontDestroyOnLoad(m_MapText);


        //SceneManager.LoadScene("Training", LoadSceneMode.Single);
        string strCmdText;
        string envPath;
        if (!Application.isEditor)
            envPath = Path.Combine(Application.dataPath, "../../") + "/trainingScene/CarsML2021-main";
        else
            envPath = Path.Combine(Application.dataPath, "../") + "/trainingScene/CarsML2021-main";
        strCmdText = "/K mlagents-learn --inference" + " --env = " + envPath + " --width=1920 --height=1080" + " --mlagents-override-model=" + m_ModelText.text;

        Debug.Log(strCmdText);
        System.Diagnostics.Process.Start("CMD.exe", strCmdText); //Start cmd process
    }


    public void OnClickBack(){
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
