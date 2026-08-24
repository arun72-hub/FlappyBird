using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{
    void Awake()
    {
        Canvas canvas = GetComponent<Canvas>();
        Camera uiCamera = canvas.worldCamera;
        bool isTablet = uiCamera.aspect > (9f / 16f);
        CanvasScaler canvasScaler = GetComponent<CanvasScaler>();
        canvasScaler.matchWidthOrHeight = isTablet ? 1 : 0;
    }
}