using UnityEngine;

public class DestructiblePlatformController : MonoBehaviour
{

    [SerializeField] private float time;
    //public AudioManager audioManager;
    // private void Awake()
    // {
    //     audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
    // }

    void Update(){
        
    }

    private void OnCollisionEnter2D(Collision2D coll)
    {
        if(coll.gameObject.tag == "Player")
        {
            //audioManager.PlaySFX(audioManager.destroyPlatform);
            Destroy(gameObject, time);
        }
    }
}
