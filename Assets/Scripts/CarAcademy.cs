using UnityEngine;

public class CarAcademy : MonoBehaviour
{
    [Header("Area Control")]
    public bool spawnAgents;
    [Tooltip("If 0, the number is controlled by individual areas.")]
    public int agentsPerArea;

    private TrainingArea[] areas;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    public void Start()
    {
        if (areas == null)
        {
            areas = GameObject.FindObjectsOfType<TrainingArea>();
        }
    }
    public void ResetAcademy()
    {
        foreach (TrainingArea area in areas)
        {
            area.ResetArea();
        }
    }
}
