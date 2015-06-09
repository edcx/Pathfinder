using UnityEngine;
using System.Collections;

public class Patrol : MonoBehaviour {

    public Vector3 startPos;
    public Vector3 endPos;
    public float speed;

	// Use this for initialization
	void Start () {
        StartCoroutine(PatrolCoroutine());
	}
    IEnumerator PatrolCoroutine()
    {
        while (true)
        {
            float i = 0.0f;
            float rate = speed / 10f;
            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(startPos, endPos, i);
                yield return null;
            }

            i = 0;
            while (i < 1.0f)
            {
                i += Time.deltaTime * rate;
                transform.position = Vector3.Lerp(endPos, startPos, i);
                yield return null;
            }
        }
    }
}
