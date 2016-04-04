using UnityEngine;
using System.Collections;

public class Patrol : MonoBehaviour {

    public Vector3 StartPos;
    public Vector3 EndPos;
    public float Speed;

	// Use this for initialization
	void Start () {
        StartCoroutine(PatrolCoroutine());
	}
    IEnumerator PatrolCoroutine()
    {
        while (true)
        {
            float i = 0.0f;
            float rate = Speed / 10f;
            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(StartPos, EndPos, i);
                yield return null;
            }

            i = 0;
            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(EndPos, StartPos, i);
                yield return null;
            }
        }
    }
}
