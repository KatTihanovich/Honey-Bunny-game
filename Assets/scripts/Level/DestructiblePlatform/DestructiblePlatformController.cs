using UnityEngine;

public class DestructiblePlatformController : MonoBehaviour
{

    [SerializeField] private float time;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            Destroy(gameObject, time);
        }
    }
}
