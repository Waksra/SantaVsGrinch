using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineBasicMultiChannelPerlin noise;
    
    [Header("Trauma")]
    [SerializeField] private float traumaDecay = 1f;
    [Header("Amplitude")]
    [SerializeField] private float amplitudeMultiplier = 1f;

    private static float trauma = 0f;

    public static void AddTrauma(float value) => trauma = Mathf.Clamp01(trauma + value);

    private void Awake()
    {
        virtualCamera = GetComponent<CinemachineVirtualCamera>();
        noise = virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }
    
    private void FixedUpdate()
    {
        if (trauma <= 0f)
        {
            if (noise.m_AmplitudeGain < 0f)
                noise.m_AmplitudeGain = 0f;
            return;
        }
        trauma -= Time.fixedDeltaTime * traumaDecay;
        noise.m_AmplitudeGain = trauma * amplitudeMultiplier;
    }
}
