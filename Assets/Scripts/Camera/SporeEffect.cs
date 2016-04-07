using UnityEngine;
using System.Collections;

public class SporeEffect : MonoBehaviour {
    [SerializeField]
    private Material sporeEffectMaterial;
    [SerializeField]
    private bool isHallucinating;
    [SerializeField]
    private SporeEffectVariable amplitudeSettings;

    public float amplitudeVal;
    private float hallucinationEndTime;

    void OnRenderImage (RenderTexture source, RenderTexture destination)
    {
        amplitudeVal = Mathf.Max(amplitudeSettings.min, amplitudeVal
            - Time.deltaTime * amplitudeSettings.decrement);

        if (amplitudeVal <= 0)
        {
            isHallucinating = false;
        }

        if (isHallucinating)
        {
            float camWidth = Camera.main.orthographicSize * Camera.main.aspect * 2;
            sporeEffectMaterial.SetFloat("_CameraWidth", camWidth);
            sporeEffectMaterial.SetFloat("_Amplitude", amplitudeVal);
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
            amplitudeVal = amplitudeSettings.initial;
        } else
        {
            amplitudeVal = Mathf.Min(amplitudeSettings.max, amplitudeVal + amplitudeSettings.increment);
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
