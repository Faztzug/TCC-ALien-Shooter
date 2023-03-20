using UnityEngine;

public class FrameRate : MonoBehaviour
{
    [SerializeField] private int targetFrameRate = 60;
    void Start()
    {
        Application.targetFrameRate = targetFrameRate;
    }
}
