using UnityEngine;

public class AspectRatioUtility : MonoBehaviour
{
  void Start()
  {
    Adjust();
  }

  public void Adjust()
  {
    float targetaspect = 16.0f / 9.0f;

    float windowaspect = (float)Screen.width / (float)Screen.height;

    float scaleheight = windowaspect / targetaspect;

    Camera camera = GetComponent<Camera>();

    if (scaleheight < 1.0f)
    {
      Rect rect = camera.rect;

      rect.width = 1.0f;
      rect.height = scaleheight;
      rect.x = 0;
      rect.y = (1.0f - scaleheight) / 2.0f;

      camera.rect = rect;
    }
    else
    {
      float scalewidth = 1.0f / scaleheight;

      Rect rect = camera.rect;

      rect.width = scalewidth;
      rect.height = 1.0f;
      rect.x = (1.0f - scalewidth) / 2.0f;
      rect.y = 0;

      camera.rect = rect;
    }

  }
}

/*using UnityEngine;


[DisallowMultipleComponent]
[RequireComponent(typeof(Camera))]
public class AspectRatioUtility : MonoBehaviour
{
    public enum FitMode { ExpandToFit, Letterbox }

    public float targetWidth = 16f;
    public float targetHeight = 9f;

    public float baseOrthoSize = 5f;

    [Range(1f, 179f)]
    public float baseVerticalFOV = 60f;

    public FitMode fitMode = FitMode.ExpandToFit;

    public bool expandPerspectiveFOV = true;

    Camera cam;
    int lastW, lastH;

    void Awake()
    {
        cam = GetComponent<Camera>();
        Apply();
    }

    void OnEnable()
    {
        Apply();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        if (!cam) cam = GetComponent<Camera>();
        Apply();
    }
#endif

    void Update()
    {
        if (Screen.width != lastW || Screen.height != lastH)
        {
            ApplyNextFrame();
        }
    }

    void ApplyNextFrame()
    {
        lastW = Screen.width; lastH = Screen.height;
        StartCoroutine(_ApplyNextFrame());
    }

    System.Collections.IEnumerator _ApplyNextFrame()
    {
        yield return null;
        Apply();
        Canvas.ForceUpdateCanvases();
    }

    public void Apply()
    {
        if (!cam) cam = GetComponent<Camera>();

        float targetAspect = targetWidth / Mathf.Max(0.0001f, targetHeight);
        float windowAspect = (float)Screen.width / Mathf.Max(1, (float)Screen.height);

        if (fitMode == FitMode.ExpandToFit)
        {
            cam.rect = new Rect(0f, 0f, 1f, 1f);

            if (cam.orthographic)
            {
                if (windowAspect < targetAspect)
                {
                    float scale = targetAspect / windowAspect; 
                    cam.orthographicSize = baseOrthoSize * scale;
                }
                else
                {
                    cam.orthographicSize = baseOrthoSize;
                }
            }
            else
            {
                if (expandPerspectiveFOV && windowAspect < targetAspect)
                {
                    float baseRad = baseVerticalFOV * Mathf.Deg2Rad * 0.5f;
                    float newRad = Mathf.Atan(Mathf.Tan(baseRad) * (targetAspect / windowAspect));
                    cam.fieldOfView = Mathf.Clamp(newRad * 2f * Mathf.Rad2Deg, 1f, 179f);
                }
                else
                {
                    cam.fieldOfView = baseVerticalFOV;
                }

                cam.rect = new Rect(0f, 0f, 1f, 1f);
                cam.ResetAspect(); 
            }
        }
        else 
        {
            float scaleHeight = windowAspect / targetAspect;
            if (scaleHeight < 1f)
            {
                cam.rect = new Rect(0f, (1f - scaleHeight) * 0.5f, 1f, scaleHeight);
            }
            else
            {
                float scaleWidth = 1f / scaleHeight;
                cam.rect = new Rect((1f - scaleWidth) * 0.5f, 0f, scaleWidth, 1f);
            }

            if (cam.orthographic) cam.orthographicSize = baseOrthoSize;
            else cam.fieldOfView = baseVerticalFOV;
        }
    }
}*/

