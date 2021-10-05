using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System.Text.RegularExpressions;

public class TR_OnClickDataCollect : MonoBehaviour
{

    Dropdown d_map;

    Text m_MapText;

    string options_path;
    string isTraining_path;
    string config_fileName;


    void Awake(){
        if (!Application.isEditor)
        {
            options_path = Path.Combine(Application.dataPath, "../") + "/InferenceOptions.json";
            isTraining_path = Path.Combine(Application.dataPath, "../") + "/isTraining.json";
        }
        else
        {
            options_path = Application.dataPath + "/InferenceOptions.json";
            isTraining_path = Application.dataPath  +"/isTraining.json";
        }

        Debug.Log(options_path);
        Debug.Log(isTraining_path);

        d_map = GameObject.Find("MapDropdown").GetComponent<Dropdown>();

        m_MapText = GameObject.Find("MapLabel").GetComponent<Text>();


        string path = Path.Combine(Application.dataPath, "../") + "/models/";



        path = Application.persistentDataPath;
        List<string> m_mapDropOptions = new List<string>();



        foreach(var f in Directory.GetFiles(path,"*.json")){
            var filename = new FileInfo(f).Name;
           m_mapDropOptions.Add(filename);
        }
        d_map.AddOptions(m_mapDropOptions);






    }


    public string CompileString(string filename)
    {
        Regex r = new Regex(@"\/([a-z]\w+)\\", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        MatchCollection matches = r.Matches(filename);

        return r.Match(filename).Groups[1].Value;
    }

    public void OnClickRun(){
             
        int mapVal = d_map.value;
        m_MapText.text = d_map.options[mapVal].text;

        bool heuristic = true;
        
        OverwriteOptions("false", "false", m_MapText.text, heuristic);
        string mapPath = Application.persistentDataPath;
        File.Copy(Path.Combine(mapPath, m_MapText.text), Path.Combine(mapPath, "selectedMap.json"), true);
        SceneManager.LoadScene("Training", LoadSceneMode.Single);
    }

    public void OnClickBack(){
        Application.Quit();
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


