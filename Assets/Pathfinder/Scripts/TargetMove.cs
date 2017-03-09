using UnityEngine;

namespace Assets.Pathfinder.Scripts
{
    public class TargetMove : MonoBehaviour
    {

        public float MoveSpeed = 5;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update ()
        {

            float x = Input.GetAxisRaw("Horizontal");
            float z = Input.GetAxisRaw("Vertical");

            Vector3 dir = new Vector3(x, 0 ,z);

            transform.Translate(dir.normalized*MoveSpeed);
        }
    }
}
