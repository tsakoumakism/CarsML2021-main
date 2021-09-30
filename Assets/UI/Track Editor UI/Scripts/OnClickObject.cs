using UnityEngine;
using UnityEngine.UI;
using System;  
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;

public class OnClickObject : MonoBehaviour
{

public List<GameObject> roadBuildList = new List<GameObject>();
private GameObject selectedItem;
private int selectedIndex;
private GameObject parentObj;
[SerializeField] private BlockData bd;

private void Start(){
    GameObject obj = Instantiate(roadBuildList[0]);
    GameObject.Find("GridManager").GetComponent<MapEditorManager>().roadBuild = obj;
    parentObj = GameObject.Find("Map");
}

public void OnClickStraight()
{
    
    selectedIndex = 0;
    SwapBlocks();
}

public void OnClickTurnLeft()
{
    selectedIndex = 1;   
    SwapBlocks(); 
}

public void OnClickTurnRight()
{
    selectedIndex = 2;    

    SwapBlocks();
}

public void OnClickCheckPoint()
{
    selectedIndex = 3;  
    SwapBlocks();  
}

public void OnClickObstacle()
{
    selectedIndex = 4;    
    SwapBlocks();
}
public void OnClickStartPosition()
{
    selectedIndex = 5;    
    SwapBlocks();
    
}

public void OnClickSaveButton(){
    if(parentObj.transform.childCount > 0){
        var textField = GameObject.Find("FileNameInputField").transform.GetChild(2);
        string fileName = textField.GetComponent<Text>().text;
        string name = "";
        using(StreamWriter sw = new StreamWriter(Application.persistentDataPath + "/" + fileName + ".json")){
        foreach(Transform child in parentObj.transform){
            bd = new BlockData();
            bd.blockPosition = child.transform.localPosition;
            bd.blockRotation = child.transform.localRotation;
                    if (child.transform.name.EndsWith("(Clone)(Clone)"))
                        name = child.transform.name.Replace("(Clone)(Clone)", "");
                    else name = child.transform.name;
            bd.name = name;
            string block = JsonUtility.ToJson(bd);
            sw.WriteLine(block);
            }
        }
        Debug.Log("Saved Successfully in : " + Application.persistentDataPath);
    }else{

        Debug.Log("You have nothing to save");
    }
}

public void OnClickLoadButton(){
    //if(parentObj.transform.childCount > 0){  // we can ask if you want to overwrite the current map
    var textField = GameObject.Find("FileNameInputField").transform.GetChild(2);
    string fileName = textField.GetComponent<Text>().text;
        using(StreamReader sr = new StreamReader(Application.persistentDataPath + "/" + fileName + ".json")){
            string line;
            while((line = sr.ReadLine()) != null){ 
                bd = new BlockData();
                bd = JsonUtility.FromJson<BlockData>(line);
                foreach(GameObject item in roadBuildList){  
                    if(bd.name == item.name){
                        var go = Instantiate(item,bd.blockPosition,bd.blockRotation) as GameObject;
                        go.transform.SetParent(GameObject.Find("Map").transform);
                        go.transform.localPosition = bd.blockPosition;
                        go.transform.localRotation = bd.blockRotation;
                    }
                }
            }
        }
        Debug.Log("Loaded Successfully");
}

public void OnClickGenerateObstaclesAndCheckPoints(){
    var textField = GameObject.Find("InputField").transform.GetChild(2); //get the second child of InputField
    string checkDiff = textField.GetComponent<Text>().text;
    if(!isNumber(checkDiff)){
        Debug.LogError("Can only place numbers in textfield");
    }
    int checkPointDiff = Int32.Parse(checkDiff);
    int counter = 0;
    int cpCounter = 0;

    Debug.Log(parentObj);
    foreach(Transform child in parentObj.transform){
        Transform checkPointChild = child.transform.GetChild(0);
        if(counter % checkPointDiff == 0){
            checkPointChild.GetComponent<Checkpoint>().isEnabled = true;
            checkPointChild.GetComponent<Checkpoint>().checkpointNumber = cpCounter;
            cpCounter++;
        }
        counter++;
    }

}
public void OnClickBack(){
    SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
}

private bool isNumber(string inStr){
    Regex regex = new Regex(@"^\d+$");
    return regex.IsMatch(inStr);
}

public void LateUpdate(){
    
    // GameObject.Find("GridManager").GetComponent<MapEditorManager>().roadBuild = roadBuildList[selectedIndex];
    selectedItem = roadBuildList[selectedIndex];

    
}


public void SwapBlocks(){
    if(GameObject.Find("GridManager").GetComponent<MapEditorManager>().roadBuild != selectedItem){
        GameObject prevObj = GameObject.Find("GridManager").GetComponent<MapEditorManager>().roadBuild;
        GameObject obj = Instantiate(selectedItem);
        GameObject.Find("GridManager").GetComponent<MapEditorManager>().roadBuild = obj;
        Destroy(prevObj);
    }
}
}
