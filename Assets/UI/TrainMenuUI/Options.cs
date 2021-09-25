using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Options 
{
public string numOfAgents, cpReward,  speedReward,  collisionPenalty,  idlePenalty,  wrongCheckPenalty, timescale,numOfAgentsPPO, numOfAgentsSAC, brainOverride;

}

[System.Serializable]
public class InferenceOptions
{
    public string testPPO, testSAC, heuristic, selectedMap;
}

[System.Serializable]
public class TrainingType
{
    public string training;
}