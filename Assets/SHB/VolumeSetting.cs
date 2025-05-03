using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    public Slider volumeSlider;
    void Start()
    {
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        volumeSlider.value = AudioListener.volume;
    }

    void OnVolumeChanged(float value)
    {
        AudioListener.volume = value;
    }
}
