using UnityEngine;

public class SliderMouseSensitivity : MonoBehaviour
{
    private bool init;

    private void OnSliderChange()
    {
        if (!this.init)
        {
            this.init = true;
            if (PlayerPrefs.HasKey("MouseSensitivity"))
            {
                gameObject.GetComponent<UISlider>().sliderValue = PlayerPrefs.GetFloat("MouseSensitivity");
            }
            else
            {
                PlayerPrefs.SetFloat("MouseSensitivity", gameObject.GetComponent<UISlider>().sliderValue);
            }
        }
        else
        {
            PlayerPrefs.SetFloat("MouseSensitivity", gameObject.GetComponent<UISlider>().sliderValue);
        }
        IN_GAME_MAIN_CAMERA.sensitivityMulti = PlayerPrefs.GetFloat("MouseSensitivity");
    }
}
