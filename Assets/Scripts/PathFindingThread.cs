using UnityEngine;
using System.Collections;

public class PathFindingThread : MultiThread {

    public Vector3 startPosition;
    public Vector3 endPosition;
    public Vector3[] path;

    protected override void ThreadFunction()
    {
        Debug.Log(startPosition + " - " + endPosition);
        for (int i = 0; i < 5000; i++)
        {
            Debug.Log(Id + " - " + i);
        }
    }

    protected override void OnFinished()
    {

        Debug.Log(">>> Job_" + Id + " is done!");
    }


}
