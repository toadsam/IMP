using UnityEngine;
using UnityEngine.UI;

public class VolumeSetting : MonoBehaviour
{
    // Reference to the UI slider used to control volume
    public Slider volumeSlider;

    void Start()
    {
        // Link the OnVolumeChanged method to the slider's value change event
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);

        // Initialize slider's value to match current audio listener volume
        volumeSlider.value = AudioListener.volume;
    }

    // Called whenever the slider value changes
    void OnVolumeChanged(float value)
    {
        // Apply the new volume to the global AudioListener
        AudioListener.volume = value;
    }
}
