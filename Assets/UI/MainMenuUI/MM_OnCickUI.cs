 using UnityEngine;
 using UnityEngine.SceneManagement;
 using UnityEngine.UI;
 using System;  
 using System.Collections.Generic;
 using System.Collections;
 using System.IO;
 using System.Text.RegularExpressions;

public class MM_OnCickUI : MonoBehaviour
{
    public void OnClickTrainModel(){
        SceneManager.LoadScene("TrainMenu", LoadSceneMode.Single);
    }
    
    public void OnClickViewLiveStats(){
        System.Diagnostics.Process.Start("CMD.exe","/C tensorboard --logdir results --port 6006"); //Start cmd process
        Application.OpenURL("http://localhost:6006/");
    }

    public void OnClickMapEditor(){
        SceneManager.LoadScene("Map Editor", LoadSceneMode.Single);
    }

    public void OnClickTestModel(){
        SceneManager.LoadScene("TestModel", LoadSceneMode.Single);
    }
}
