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
        
        //m_modelDropOptions = Resources.FindObjectsOfAllType
        // foreach(var d in Directory.GetDirectories(path)){
        //     var dirName = new DirectoryInfo(d).Name;
        //     m_modelDropOptions.Add(dirName);
        // }
        
        path = Application.persistentDataPath;
        List<string> m_mapDropOptions = new List<string>();
        /* temporarily disabled
        foreach(var f in Directory.GetFiles(path,"*.json")){
            var filename = new FileInfo(f).Name;
           m_mapDropOptions.Add(filename);
        }

        d_model.AddOptions(m_modelDropOptions);
        d_map.AddOptions(m_mapDropOptions);
        */
        
    }


    public void OnClickRun(){
        /* temporarily disabled
        int modelVal = d_model.value;
        m_ModelText.text = d_model.options[modelVal].text;

        int mapVal = d_map.value;
        m_MapText.text = d_map.options[mapVal].text;

        DontDestroyOnLoad(m_ModelText);
        DontDestroyOnLoad(m_MapText);
        */

        SceneManager.LoadScene("Training", LoadSceneMode.Single);
    }


    public void OnClickBack(){
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }
}
