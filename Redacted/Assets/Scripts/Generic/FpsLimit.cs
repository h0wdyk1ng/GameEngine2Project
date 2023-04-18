using UnityEngine;

public class FpsLimit : MonoBehaviour
{
    [SerializeField] private bool isTesting = true;
    [SerializeField] private float maxFps = 90;
    // Start is called before the first frame update
    void Start()
    {
        if (isTesting)
            Application.targetFrameRate = (int)maxFps;
        else
            Application.targetFrameRate = Screen.currentResolution.refreshRate;
    }
}
