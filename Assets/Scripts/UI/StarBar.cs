using UnityEngine;
using UnityEngine.UI;

public class StarBar : MonoBehaviour
{
    private Slider slider;
    public float FillSpeed = 0.5f;
    private float targetProgress = 0;

    private void Awake(){
        slider = gameObject.GetComponent<Slider>();
    }
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (slider.value < targetProgress)
            slider.value += FillSpeed * Time.deltaTime;
    }

    public void IncrementProgress(float newProgress){
        targetProgress = slider.value + newProgress;
    }

    public void ResetProgress()
    {
        slider.value = 0f;
        targetProgress = 0f;
    }

}
