using UnityEngine;

public class Traumatiser : MonoBehaviour
{
    public void AddTrauma(float trauma)
    {
        CameraController.AddTrauma(trauma);
    }
}
