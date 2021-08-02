using UnityEngine;
using System.Collections.Generic;


public class MapEditorGrid{
    //Grid characteristics
    private int width;
    private int height;
    private float cellSize;
    private float lineSize = 0.1f;

    private GameObject gridFloor;

    //Grid Cells
    private int[,] grid;


    

    //Constructor, generates grid based on width*height*cellSize
    public MapEditorGrid(int width, int height, float cellSize,float lineSize){
        this.width = width;
        this.height = height;
        this.cellSize = cellSize;
        this.lineSize = lineSize;
        grid = new int[width, height];

        GenerateGrid();
    }

    private void GenerateGrid(){

        //Create gridFloor  
        gridFloor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        

        //list of all lines that make show the grid
        List<LineDrawer> lineDrawers = new List<LineDrawer>();
        //iterate through each cell
        for(int x = 0; x < grid.GetLength(0); x++){
            for(int y = 0; y < grid.GetLength(1); y++){
                grid[x,y] = 0; // cell value
                lineDrawers.Add(new LineDrawer(GetWorldPosition(x,y), GetWorldPosition(x, y + 1), Color.white,lineSize)); // draw rows
                lineDrawers.Add(new LineDrawer(GetWorldPosition(x,y), GetWorldPosition(x + 1, y), Color.white,lineSize)); //draw cols
            }
        }
        //add the final line for rows and cols
        lineDrawers.Add(new LineDrawer(GetWorldPosition(0,height), GetWorldPosition(width, height), Color.white,lineSize));
        lineDrawers.Add(new LineDrawer(GetWorldPosition(width, 0), GetWorldPosition(width, height), Color.white,lineSize));

        //offset half width, half height
        //size width * cell, height * cell
        gridFloor.transform.localScale = new Vector3(width * cellSize, 0.1f, height * cellSize);
        gridFloor.transform.localPosition = new Vector3(gridFloor.transform.position.x + (width*cellSize / 2), 
                                                        gridFloor.transform.position.y - 0.2f, 
                                                        gridFloor.transform.position.z + (height * cellSize/2));
        gridFloor.GetComponent<Renderer>().material = new Material(Shader.Find("Sprites/Diffuse"));
        Debug.Log("Created Grid" + ToString());
    }

    public void GetXY(Vector3 worldPosition, out int x, out int y){
        x = Mathf.FloorToInt(worldPosition.x / cellSize);
        y = Mathf.FloorToInt(worldPosition.z / cellSize);
    }

    public void SetValue(int x, int y){
        if(x >= 0 && y >= 0 && x < width && y < height){
            Debug.Log(x+","+y);
            Debug.Log(grid[x,y]);
            grid[x,y] = 1;
        } 
    }


    public void SetValue(Vector3 worldPosition){
        int x,y;
        GetXY(worldPosition, out x, out y);
        SetValue(x,y);
    }

    public int GetValue(Vector3 worldPosition){
        int x,y;
        GetXY(worldPosition,out x, out y);
        return GetValue(x,y);
    }

    public int GetValue(int x, int y){
        if(x >= 0 && y >= 0 && x < width && y < height){
            return grid[x,y] ;
        }
        return 0;
    }

    public Vector3 GetPointInWorld(Vector3 worldPosition){
        int x,y;
        GetXY(worldPosition, out x, out y);
        return new Vector3(x,0,y) * cellSize;
    }
    //Get the world position of each cell, we want it on the X-Z plane of unity
    private Vector3 GetWorldPosition(int x, int y){
        
        return new Vector3(x,0,y) * cellSize;
    }

    public override string ToString(){
        return "(" + this.width + "," + this.height + "," + this.cellSize + ")";
    }
}