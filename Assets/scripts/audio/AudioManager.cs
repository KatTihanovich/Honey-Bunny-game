using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public void SoundControl(){
        if(AudioListener.pause == true){
            AudioListener.pause = false;
        } else {
            AudioListener.pause = true;
        }
    }
}
