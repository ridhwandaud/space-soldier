using UnityEngine;
using System.Collections;

public class SporeEffect : MonoBehaviour {
    [SerializeField]
    private Material sporeEffectMaterial;
    [SerializeField]
    private bool isHallucinating;
    [SerializeField]
    private SporeEffectVariable timeMultiplierSettings;
    [SerializeField]
    private SporeEffectVariable amplitudeSettings;
    [SerializeField]
    private SporeEffectVariable waveCountMultiplierSettings;

    private float timeMultiplierVal;
    private float amplitudeVal;
    private float waveCountMultiplierVal;
    private float hallucinationEndTime;

    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        timeMultiplierVal = Mathf.Max(timeMultiplierSettings.min, timeMultiplierVal
            - Time.deltaTime * timeMultiplierSettings.decrement);
        amplitudeVal = Mathf.Max(amplitudeSettings.min, amplitudeVal
            - Time.deltaTime * amplitudeSettings.decrement);
        waveCountMultiplierVal = Mathf.Max(waveCountMultiplierSettings.min, waveCountMultiplierVal
            - Time.deltaTime * waveCountMultiplierSettings.decrement);

        if (amplitudeVal <= 0 || waveCountMultiplierVal <= 0)
        {
            isHallucinating = false;
        }

        if (isHallucinating)
        {
            float camWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
            sporeEffectMaterial.SetFloat("_CameraWidth", camWidth);
            sporeEffectMaterial.SetFloat("_TimeMultiplier", timeMultiplierVal);
            sporeEffectMaterial.SetFloat("_Amplitude", amplitudeVal);
            sporeEffectMaterial.SetFloat("_WaveCountMultiplier", waveCountMultiplierVal);
            Graphics.Blit(source, destination, sporeEffectMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }

    public void ProcessSpore()
    {
        if (!isHallucinating)
        {
            isHallucinating = true;
            timeMultiplierVal = timeMultiplierSettings.initial;
            amplitudeVal = amplitudeSettings.initial;
            waveCountMultiplierVal = waveCountMultiplierSettings.initial;
        } else
        {
            timeMultiplierVal = Mathf.Min(timeMultiplierSettings.max, timeMultiplierVal + timeMultiplierSettings.increment);
            amplitudeVal = Mathf.Min(amplitudeSettings.max, amplitudeVal + amplitudeSettings.increment);
            waveCountMultiplierVal = Mathf.Min(waveCountMultiplierSettings.max, waveCountMultiplierVal
                + waveCountMultiplierSettings.increment);
        }
    }

    [System.Serializable]
    struct SporeEffectVariable
    {
        public float max;
        public float min;
        public float initial;
        public float increment;
        public float decrement;
    }
}
