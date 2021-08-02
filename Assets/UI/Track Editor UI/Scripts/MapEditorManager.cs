using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEditorManager : MonoBehaviour
{
    public int gridWidth,gridHeight;
    public float cellSize,lineSize;
    public GameObject roadBuild;
    private Vector3 truePosition;
    private int value;


    private MapEditorGrid mapEditorGrid;

    private RaycastHit hit;
    private Camera mainCamera;
    private Ray ray;
    private int layerMask = 1 << 9;
    Vector3 point;

    private GameObject parent;

    private void Start()
    {
        //set to collide to everything other than our layerMask
        layerMask = ~layerMask;
        mapEditorGrid = new MapEditorGrid(gridWidth, gridHeight, cellSize,lineSize); 
        mainCamera = Camera.main;
        //Cursor.lockState = CursorLockMode.Confined;
        parent = GameObject.Find("Map");
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
         RaycastHit hit;
         if (Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask)){
            point = hit.point;
            //gets the x,z values * cellSize to offset the movement correctly
            point = mapEditorGrid.GetPointInWorld(point);   
            //pass the movement, offset by cellSize to match the grid
            roadBuild.transform.position = point + (cellSize * new Vector3(1f, 0, 1f));

            if(Input.GetMouseButton(0)){

                Debug.Log(mapEditorGrid.GetValue(point));
                if(mapEditorGrid.GetValue(point) == 1) return;
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "road") return;
                
                var go = Instantiate(roadBuild,roadBuild.transform.position,roadBuild.transform.localRotation) as GameObject;   
                go.transform.SetParent(parent.transform);
                go.tag = "road";
                go.layer = 0;
                mapEditorGrid.SetValue(point);
                //SpawnObject();
            }

            
        if (Physics.Raycast(ray, out hit, Mathf.Infinity,layerMask)){
            if(Input.GetMouseButtonDown(1)){
                Debug.Log(hit.collider.tag);
                if(hit.collider.tag == "road"){
                    Destroy(hit.collider.gameObject);
                }
            }
        }

    }

        if(Input.GetAxis("Mouse ScrollWheel") != 0){
            roadBuild.transform.Rotate(new Vector3( 0, 0 ,transform.localRotation.y + 90f  * GetMouseWheel()), Space.Self);
            
        }
                        
    }

    IEnumerator RayCorountine(){
            
            yield return new WaitForSeconds(5);
        }

    void SpawnObject(){
        var go = Instantiate(roadBuild,roadBuild.transform.position,roadBuild.transform.localRotation) as GameObject;   
            go.transform.SetParent(parent.transform);
            go.tag = "road";
            go.layer = 0;
    }

    private float GetMouseWheel(){
        return (Input.GetAxis("Mouse ScrollWheel") > 0) ? 1f : -1f;
    }

    private void GenerateTrack(){

    }
}
