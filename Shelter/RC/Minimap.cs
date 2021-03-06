using System;
using System.Collections;
using Mod;
using Mod.Managers;
using Mod.Modules;
using RC;
using UnityEngine;
using UnityEngine.UI;
using LogType = Mod.Logging.LogType;
using Object = UnityEngine.Object;

[DisallowMultipleComponent]
// ReSharper disable once CheckNamespace
public class Minimap : MonoBehaviour, IDisposable
{
    private GameObject[] _interface;
    
    private bool assetsInitialized = false;
    private static Sprite borderSprite;
    private RectTransform borderT;
    private Canvas canvas;
    private Vector2 cornerPosition;
    private float cornerSizeRatio;
    private Preset initialPreset;
    public static Minimap instance;
    private bool isEnabled;
    private bool isEnabledTemp;
    private Vector3 lastMinimapCenter;
    private float lastMinimapOrthoSize;
    private Camera lastUsedCamera;
    private bool maximized = false;
    private RectTransform minimap;
    private float MINIMAP_CORNER_SIZE;
    public float MINIMAP_CORNER_SIZE_SCALED;
    private Vector2 MINIMAP_ICON_SIZE;
    private float MINIMAP_POINTER_DIST;
    private float MINIMAP_POINTER_SIZE;
    private int MINIMAP_SIZE;
    private Vector2 MINIMAP_SUPPLY_SIZE;
    private MinimapIcon[] minimapIcons;
    private bool minimapIsCreated = false;
    private RectTransform minimapMaskT;
    private Bounds minimapOrthographicBounds;
    public RenderTexture minimapRT;
    public Camera myCam;
    private static Sprite pointerSprite;
    private CanvasScaler scaler;
    private static Sprite supplySprite;
    private static Sprite whiteIconSprite;
    
    private void AddBorderToTexture(ref Texture2D texture, Color borderColor, int borderPixelSize)
    {
        int num = texture.width * borderPixelSize;
        Color[] colors = new Color[num];
        for (int i = 0; i < num; i++)
        {
            colors[i] = borderColor;
        }
        texture.SetPixels(0, texture.height - borderPixelSize, texture.width - 1, borderPixelSize, colors);
        texture.SetPixels(0, 0, texture.width, borderPixelSize, colors);
        texture.SetPixels(0, 0, borderPixelSize, texture.height, colors);
        texture.SetPixels(texture.width - borderPixelSize, 0, borderPixelSize, texture.height, colors);
        texture.Apply();
    }

    private void AutomaticSetCameraProperties(Camera cam)
    {
        Renderer[] rendererArray = FindObjectsOfType<Renderer>();
        if (rendererArray.Length > 0)
        {
            this.minimapOrthographicBounds = new Bounds(rendererArray[0].transform.position, Vector3.zero);
            for (int i = 0; i < rendererArray.Length; i++)
            {
                if (rendererArray[i].gameObject.layer == 9)
                {
                    this.minimapOrthographicBounds.Encapsulate(rendererArray[i].bounds);
                }
            }
        }
        Vector3 size = this.minimapOrthographicBounds.size;
        float num2 = size.x > size.z ? size.x : size.z;
        size.z = size.x = num2;
        this.minimapOrthographicBounds.size = size;
        cam.orthographic = true;
        cam.orthographicSize = num2 * 0.5f;
        Vector3 center = this.minimapOrthographicBounds.center;
        center.y = cam.farClipPlane * 0.5f;
        Transform transform = cam.transform;
        transform.position = center;
        transform.eulerAngles = new Vector3(90f, 0f, 0f);
        cam.aspect = 1f;
        this.lastMinimapCenter = center;
        this.lastMinimapOrthoSize = cam.orthographicSize;
    }

    private void AutomaticSetOrthoBounds()
    {
        Renderer[] rendererArray = FindObjectsOfType<Renderer>();
        if (rendererArray.Length > 0)
        {
            this.minimapOrthographicBounds = new Bounds(rendererArray[0].transform.position, Vector3.zero);
            for (int i = 0; i < rendererArray.Length; i++)
            {
                this.minimapOrthographicBounds.Encapsulate(rendererArray[i].bounds);
            }
        }
        Vector3 size = this.minimapOrthographicBounds.size;
        float num2 = size.x > size.z ? size.x : size.z;
        size.z = size.x = num2;
        this.minimapOrthographicBounds.size = size;
        this.lastMinimapCenter = this.minimapOrthographicBounds.center;
        this.lastMinimapOrthoSize = num2 * 0.5f;
    }

    private void Awake()
    {
        instance = this;
    }

    private Texture2D CaptureMinimap(Camera cam)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;
        cam.Render();
        Texture2D textured = new Texture2D(cam.targetTexture.width, cam.targetTexture.height, TextureFormat.RGB24, false)
        {
            filterMode = FilterMode.Bilinear
        };
        textured.ReadPixels(new Rect(0f, 0f, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        textured.Apply();
        RenderTexture.active = active;
        return textured;
    }

    private void CaptureMinimapRT(Camera cam)
    {
        RenderTexture active = RenderTexture.active;
        RenderTexture.active = this.minimapRT;
        cam.targetTexture = this.minimapRT;
        cam.Render();
        RenderTexture.active = active;
    }

    private void CheckUserInput()
    {
        if (!ModuleManager.Enabled(nameof(ModuleShowMap)) || !GameManager.settings.IsMapAllowed)
            Dispose();
    }

    public void CreateMinimap(Camera cam, int minimapResolution = 512, float cornerSize = 0.3f, Preset mapPreset = null)
    {
        this.isEnabled = true;
        this.lastUsedCamera = cam;
        if (!this.assetsInitialized)
        {
            this.Initialize();
        }
        GameObject obj2 = GameObject.Find("mainLight");
        Light component = null;
        Quaternion identity = Quaternion.identity;
        LightShadows none = LightShadows.None;
        Color clear = Color.clear;
        float intensity = 0f;
        float nearClipPlane = cam.nearClipPlane;
        float farClipPlane = cam.farClipPlane;
        int cullingMask = cam.cullingMask;
        if (obj2 != null)
        {
            component = obj2.GetComponent<Light>();
            identity = component.transform.rotation;
            none = component.shadows;
            intensity = component.intensity;
            clear = component.color;
            component.shadows = LightShadows.None;
            component.color = Color.white;
            component.intensity = 0.5f;
            component.transform.eulerAngles = new Vector3(90f, 0f, 0f);
        }
        cam.nearClipPlane = 0.3f;
        cam.farClipPlane = 1000f;
        cam.cullingMask = 512;
        cam.clearFlags = CameraClearFlags.Color;
        this.MINIMAP_SIZE = minimapResolution;
        this.MINIMAP_CORNER_SIZE = this.MINIMAP_SIZE * cornerSize;
        this.cornerSizeRatio = cornerSize;
        this.CreateMinimapRT(cam, minimapResolution);
        if (mapPreset != null)
        {
            this.initialPreset = mapPreset;
            this.ManualSetCameraProperties(cam, mapPreset.center, mapPreset.orthographicSize);
        }
        else
        {
            this.AutomaticSetCameraProperties(cam);
        }
        this.CaptureMinimapRT(cam);
        if (obj2 != null)
        {
            component.shadows = none;
            component.transform.rotation = identity;
            component.color = clear;
            component.intensity = intensity;
        }
        cam.nearClipPlane = nearClipPlane;
        cam.farClipPlane = farClipPlane;
        cam.cullingMask = cullingMask;
        cam.orthographic = false;
        cam.clearFlags = CameraClearFlags.Skybox;
        this.CreateUnityUIRT(minimapResolution);
        this.minimapIsCreated = true;
        StartCoroutine(this.HackRoutine());
    }

    private void CreateMinimapRT(Camera cam, int pixelSize)
    {
        if (this.minimapRT == null)
        {
            bool flag2 = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB565);
            RenderTextureFormat format = flag2 ? RenderTextureFormat.RGB565 : RenderTextureFormat.Default;
            this.minimapRT = new RenderTexture(pixelSize, pixelSize, 16, format);
            if (!flag2)
                Shelter.LogBoth("{0} ({1}) does not support RGB565 format, the minimap will have transparency issues on certain maps", LogType.Error, 
                    SystemInfo.graphicsDeviceName, SystemInfo.graphicsDeviceVendor);
        }
        cam.targetTexture = this.minimapRT;
    }

    private void CreateUnityUIRT(int minimapResolution)
    {
        _interface = new GameObject[4];
        
        _interface[0] = new GameObject("Canvas");
        _interface[0].AddComponent<RectTransform>();
        this.canvas = _interface[0].AddComponent<Canvas>();
        this.canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        this.scaler = _interface[0].AddComponent<CanvasScaler>();
        this.scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        this.scaler.referenceResolution = new Vector2(800f, 600f);
        this.scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        this.scaler.matchWidthOrHeight = 1f;
        _interface[1] = new GameObject("Mask");
        _interface[1].transform.SetParent(_interface[0].transform, false);
        this.minimapMaskT = _interface[1].AddComponent<RectTransform>();
        _interface[1].AddComponent<CanvasRenderer>();
        this.minimapMaskT.anchorMin = this.minimapMaskT.anchorMax = Vector2.one;
        float num = this.MINIMAP_CORNER_SIZE * 0.5f;
        this.cornerPosition = new Vector2(-(num + 5f), -(num + 70f));
        this.minimapMaskT.anchoredPosition = this.cornerPosition;
        this.minimapMaskT.sizeDelta = new Vector2(this.MINIMAP_CORNER_SIZE, this.MINIMAP_CORNER_SIZE);
        _interface[2] = new GameObject("MapBorder");
        _interface[2].transform.SetParent(this.minimapMaskT, false);
        this.borderT = _interface[2].AddComponent<RectTransform>();
        this.borderT.anchorMin = this.borderT.anchorMax = new Vector2(0.5f, 0.5f);
        this.borderT.sizeDelta = this.minimapMaskT.sizeDelta;
        _interface[2].AddComponent<CanvasRenderer>();
        Image image = _interface[2].AddComponent<Image>();
        image.sprite = borderSprite;
        image.type = Image.Type.Sliced;
        _interface[3] = new GameObject("Minimap");
        _interface[3].transform.SetParent(this.minimapMaskT, false);
        this.minimap = _interface[3].AddComponent<RectTransform>();
        this.minimap.SetAsFirstSibling();
        _interface[3].AddComponent<CanvasRenderer>();
        this.minimap.anchorMin = this.minimap.anchorMax = new Vector2(0.5f, 0.5f);
        this.minimap.anchoredPosition = Vector2.zero;
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        RawImage image2 = _interface[3].AddComponent<RawImage>();
        image2.texture = this.minimapRT;
        image2.maskable = true;
        _interface[3].AddComponent<Mask>().showMaskGraphic = true;
    }

    private Vector2 GetSizeForStyle(IconStyle style)
    {
        if (style == IconStyle.CIRCLE)
        {
            return this.MINIMAP_ICON_SIZE;
        }
        if (style == IconStyle.SUPPLY)
        {
            return this.MINIMAP_SUPPLY_SIZE;
        }
        return Vector2.zero;
    }

    private static Sprite GetSpriteForStyle(IconStyle style)
    {
        if (style == IconStyle.CIRCLE)
        {
            return whiteIconSprite;
        }
        if (style == IconStyle.SUPPLY)
        {
            return supplySprite;
        }
        return null;
    }

    private IEnumerator HackRoutine()
    {
        yield return new WaitForEndOfFrame();
        this.RecaptureMinimap(this.lastUsedCamera, this.lastMinimapCenter, this.lastMinimapOrthoSize);
    }

    private void Initialize()
    {
        Vector3 pivot = new Vector3(0.5f, 0.5f);
        Texture2D texture = (Texture2D)GameManager.RCassets.Load("icon");
        Rect rect = new Rect(0f, 0f, texture.width, texture.height);
        whiteIconSprite = Sprite.Create(texture, rect, pivot);
        texture = (Texture2D)GameManager.RCassets.Load("iconpointer");
        rect = new Rect(0f, 0f, texture.width, texture.height);
        pointerSprite = Sprite.Create(texture, rect, pivot);
        texture = (Texture2D)GameManager.RCassets.Load("supplyicon");
        rect = new Rect(0f, 0f, texture.width, texture.height);
        supplySprite = Sprite.Create(texture, rect, pivot);
        texture = (Texture2D)GameManager.RCassets.Load("mapborder");
        rect = new Rect(0f, 0f, texture.width, texture.height);
        Vector4 border = new Vector4(5f, 5f, 5f, 5f);
        borderSprite = Sprite.Create(texture, rect, pivot, 100f, 1, SpriteMeshType.FullRect, border);
        this.MINIMAP_ICON_SIZE = new Vector2(whiteIconSprite.texture.width, whiteIconSprite.texture.height);
        this.MINIMAP_POINTER_SIZE = (pointerSprite.texture.width + pointerSprite.texture.height) / 2f;
        this.MINIMAP_POINTER_DIST = (this.MINIMAP_ICON_SIZE.x + this.MINIMAP_ICON_SIZE.y) * 0.25f;
        this.MINIMAP_SUPPLY_SIZE = new Vector2(supplySprite.texture.width, supplySprite.texture.height);
        this.assetsInitialized = true;
    }

    private void ManualSetCameraProperties(Camera cam, Vector3 centerPoint, float orthoSize)
    {
        Transform transform = cam.transform;
        centerPoint.y = cam.farClipPlane * 0.5f;
        transform.position = centerPoint;
        transform.eulerAngles = new Vector3(90f, 0f, 0f);
        cam.orthographic = true;
        cam.orthographicSize = orthoSize;
        float x = orthoSize * 2f;
        this.minimapOrthographicBounds = new Bounds(centerPoint, new Vector3(x, 0f, x));
        this.lastMinimapCenter = centerPoint;
        this.lastMinimapOrthoSize = orthoSize;
    }

    private void ManualSetOrthoBounds(Vector3 centerPoint, float orthoSize)
    {
        float x = orthoSize * 2f;
        this.minimapOrthographicBounds = new Bounds(centerPoint, new Vector3(x, 0f, x));
        this.lastMinimapCenter = centerPoint;
        this.lastMinimapOrthoSize = orthoSize;
    }

    public void Maximize()
    {
        this.isEnabledTemp = true;
        if (!this.isEnabled)
        {
            this.SetEnabledTemp(true);
        }
        this.minimapMaskT.anchorMin = this.minimapMaskT.anchorMax = new Vector2(0.5f, 0.5f);
        this.minimapMaskT.anchoredPosition = Vector2.zero;
        this.minimapMaskT.sizeDelta = new Vector2(this.MINIMAP_SIZE, this.MINIMAP_SIZE);
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        this.borderT.sizeDelta = this.minimapMaskT.sizeDelta;
        if (this.minimapIcons != null)
        {
            foreach (MinimapIcon icon in minimapIcons)
            {
                if (icon != null)
                {
                    icon.SetSize(GetSizeForStyle(icon.style));
                    if (icon.rotation)
                    {
                        icon.SetPointerSize(MINIMAP_POINTER_SIZE, MINIMAP_POINTER_DIST);
                    }
                }
            }
        }
        this.maximized = true;
    }

    public void Minimize()
    {
        this.isEnabledTemp = false;
        if (!this.isEnabled)
        {
            this.SetEnabledTemp(false);
        }
        this.minimapMaskT.anchorMin = this.minimapMaskT.anchorMax = Vector2.one;
        this.minimapMaskT.anchoredPosition = this.cornerPosition;
        this.minimapMaskT.sizeDelta = new Vector2(this.MINIMAP_CORNER_SIZE, this.MINIMAP_CORNER_SIZE);
        this.minimap.sizeDelta = this.minimapMaskT.sizeDelta;
        this.borderT.sizeDelta = this.minimapMaskT.sizeDelta;
        if (this.minimapIcons != null)
        {
            float num = 1f - (this.MINIMAP_SIZE - this.MINIMAP_CORNER_SIZE) / this.MINIMAP_SIZE;
            float a = this.MINIMAP_POINTER_SIZE * num;
            a = Mathf.Max(a, this.MINIMAP_POINTER_SIZE * 0.5f);
            float originDistance = (this.MINIMAP_POINTER_SIZE - a) / this.MINIMAP_POINTER_SIZE;
            originDistance = this.MINIMAP_POINTER_DIST * originDistance;
            for (int i = 0; i < this.minimapIcons.Length; i++)
            {
                MinimapIcon icon = this.minimapIcons[i];
                if (icon != null)
                {
                    Vector2 sizeForStyle = this.GetSizeForStyle(icon.style);
                    sizeForStyle.x = Mathf.Max(sizeForStyle.x * num, sizeForStyle.x * 0.5f);
                    sizeForStyle.y = Mathf.Max(sizeForStyle.y * num, sizeForStyle.y * 0.5f);
                    icon.SetSize(sizeForStyle);
                    if (icon.rotation)
                    {
                        icon.SetPointerSize(a, originDistance);
                    }
                }
            }
        }
        this.maximized = false;
    }

    public static void OnScreenResolutionChanged()
    {
        if (instance != null)
            instance.StartCoroutine(instance.ScreenResolutionChangedRoutine());
    }

    private void RecaptureMinimap()
    {
        if (this.lastUsedCamera != null)
        {
            this.RecaptureMinimap(this.lastUsedCamera, this.lastMinimapCenter, this.lastMinimapOrthoSize);
        }
    }

    private void RecaptureMinimap(Camera cam, Vector3 centerPosition, float orthoSize)
    {
        if (this.minimap != null)
        {
            GameObject obj2 = GameObject.Find("mainLight");
            Light component = null;
            Quaternion identity = Quaternion.identity;
            LightShadows none = LightShadows.None;
            Color clear = Color.clear;
            float intensity = 0f;
            float nearClipPlane = cam.nearClipPlane;
            float farClipPlane = cam.farClipPlane;
            int cullingMask = cam.cullingMask;
            if (obj2 != null)
            {
                component = obj2.GetComponent<Light>();
                identity = component.transform.rotation;
                none = component.shadows;
                clear = component.color;
                intensity = component.intensity;
                component.shadows = LightShadows.None;
                component.color = Color.white;
                component.intensity = 0.5f;
                component.transform.eulerAngles = new Vector3(90f, 0f, 0f);
            }
            cam.nearClipPlane = 0.3f;
            cam.farClipPlane = 1000f;
            cam.clearFlags = CameraClearFlags.Color;
            cam.cullingMask = 512;
            this.CreateMinimapRT(cam, this.MINIMAP_SIZE);
            this.ManualSetCameraProperties(cam, centerPosition, orthoSize);
            this.CaptureMinimapRT(cam);
            if (obj2 != null)
            {
                component.shadows = none;
                component.transform.rotation = identity;
                component.color = clear;
                component.intensity = intensity;
            }
            cam.nearClipPlane = nearClipPlane;
            cam.farClipPlane = farClipPlane;
            cam.cullingMask = cullingMask;
            cam.orthographic = false;
            cam.clearFlags = CameraClearFlags.Skybox;
        }
    }

    private IEnumerator ScreenResolutionChangedRoutine()
    {
        yield return 0;
        this.RecaptureMinimap();
    }

    public void SetEnabled(bool enabled)
    {
        this.isEnabled = enabled;
        if (this.canvas != null)
        {
            this.canvas.gameObject.SetActive(enabled);
        }
    }

    public void SetEnabledTemp(bool enabled)
    {
        if (this.canvas != null)
        {
            this.canvas.gameObject.SetActive(enabled);
        }
    }

    public void TrackGameObjectOnMinimap(GameObject objToTrack, Color iconColor, bool trackOrientation, bool depthAboveAll = false, IconStyle iconStyle = 0)
    {
        if (this.minimap != null)
        {
            MinimapIcon icon;
            if (trackOrientation)
            {
                icon = MinimapIcon.CreateWithRotation(this.minimap, objToTrack, iconStyle, this.MINIMAP_POINTER_DIST);
            }
            else
            {
                icon = MinimapIcon.Create(this.minimap, objToTrack, iconStyle);
            }
            icon.SetColor(iconColor);
            icon.SetDepth(depthAboveAll);
            Vector2 sizeForStyle = this.GetSizeForStyle(iconStyle);
            if (this.maximized)
            {
                icon.SetSize(sizeForStyle);
                if (icon.rotation)
                {
                    icon.SetPointerSize(this.MINIMAP_POINTER_SIZE, this.MINIMAP_POINTER_DIST);
                }
            }
            else
            {
                float num = 1f - (this.MINIMAP_SIZE - this.MINIMAP_CORNER_SIZE) / this.MINIMAP_SIZE;
                sizeForStyle.x = Mathf.Max(sizeForStyle.x * num, sizeForStyle.x * 0.5f);
                sizeForStyle.y = Mathf.Max(sizeForStyle.y * num, sizeForStyle.y * 0.5f);
                icon.SetSize(sizeForStyle);
                if (icon.rotation)
                {
                    float a = this.MINIMAP_POINTER_SIZE * num;
                    a = Mathf.Max(a, this.MINIMAP_POINTER_SIZE * 0.5f);
                    float originDistance = (this.MINIMAP_POINTER_SIZE - a) / this.MINIMAP_POINTER_SIZE;
                    originDistance = this.MINIMAP_POINTER_DIST * originDistance;
                    icon.SetPointerSize(a, originDistance);
                }
            }
            if (this.minimapIcons == null)
            {
                this.minimapIcons = new MinimapIcon[] { icon };
            }
            else
            {
                MinimapIcon[] iconArray2 = new MinimapIcon[this.minimapIcons.Length + 1];
                for (int i = 0; i < this.minimapIcons.Length; i++)
                {
                    iconArray2[i] = this.minimapIcons[i];
                }
                iconArray2[iconArray2.Length - 1] = icon;
                this.minimapIcons = iconArray2;
            }
        }
    }

    public static void TryRecaptureInstance()
    {
        if (instance != null)
        {
            instance.RecaptureMinimap();
        }
    }

    public IEnumerator TryRecaptureInstanceE(float time)
    {
        yield return new WaitForSeconds(time);
        TryRecaptureInstance();
    }

    private void Update()
    {
        this.CheckUserInput();
        if ((this.isEnabled || this.isEnabledTemp) && this.minimapIsCreated && this.minimapIcons != null)
        {
            for (int i = 0; i < this.minimapIcons.Length; i++)
            {
                MinimapIcon icon = this.minimapIcons[i];
                if (icon == null)
                {
                    RCextensions.RemoveAt<MinimapIcon>(ref this.minimapIcons, i);
                }
                else if (!icon.UpdateUI(this.minimapOrthographicBounds, this.maximized ? this.MINIMAP_SIZE : this.MINIMAP_CORNER_SIZE))
                {
                    icon.Destroy();
                    RCextensions.RemoveAt<MinimapIcon>(ref this.minimapIcons, i);
                }
            }
        }
    }

    public static void WaitAndTryRecaptureInstance(float time)
    {
        instance.StartCoroutine(instance.TryRecaptureInstanceE(time));
    }

    public enum IconStyle
    {
        CIRCLE,
        SUPPLY
    }

    public class MinimapIcon
    {
        private Transform obj;
        private RectTransform pointerRect;
        public readonly bool rotation;
        public readonly IconStyle style;
        private RectTransform uiRect;

        public MinimapIcon(GameObject trackedObject, GameObject uiElement, IconStyle style)
        {
            this.rotation = false;
            this.style = style;
            this.obj = trackedObject.transform;
            this.uiRect = uiElement.GetComponent<RectTransform>();
            CatchDestroy component = this.obj.GetComponent<CatchDestroy>();
            if (component == null)
            {
                this.obj.gameObject.AddComponent<CatchDestroy>().target = uiElement;
            }
            else if (component.target != null && component.target != uiElement)
            {
                Object.Destroy(component.target);
            }
            else
            {
                component.target = uiElement;
            }
        }

        public MinimapIcon(GameObject trackedObject, GameObject uiElement, GameObject uiPointer, IconStyle style)
        {
            this.rotation = true;
            this.style = style;
            this.obj = trackedObject.transform;
            this.uiRect = uiElement.GetComponent<RectTransform>();
            this.pointerRect = uiPointer.GetComponent<RectTransform>();
            CatchDestroy component = this.obj.GetComponent<CatchDestroy>();
            if (component == null)
            {
                this.obj.gameObject.AddComponent<CatchDestroy>().target = uiElement;
            }
            else if (component.target != null && component.target != uiElement)
            {
                Object.Destroy(component.target);
            }
            else
            {
                component.target = uiElement;
            }
        }

        public static MinimapIcon Create(RectTransform parent, GameObject trackedObject, IconStyle style)
        {
            Sprite spriteForStyle = GetSpriteForStyle(style);
            GameObject uiElement = new GameObject("MinimapIcon");
            RectTransform transform = uiElement.AddComponent<RectTransform>();
            transform.anchorMin = transform.anchorMax = new Vector3(0.5f, 0.5f);
            transform.sizeDelta = new Vector2(spriteForStyle.texture.width, spriteForStyle.texture.height);
            Image image = uiElement.AddComponent<Image>();
            image.sprite = spriteForStyle;
            image.type = Image.Type.Simple;
            uiElement.transform.SetParent(parent, false);
            return new MinimapIcon(trackedObject, uiElement, style);
        }

        public static MinimapIcon CreateWithRotation(RectTransform parent, GameObject trackedObject, IconStyle style, float pointerDist)
        {
            Sprite spriteForStyle = GetSpriteForStyle(style);
            GameObject uiElement = new GameObject("MinimapIcon");
            RectTransform transform = uiElement.AddComponent<RectTransform>();
            transform.anchorMin = transform.anchorMax = new Vector3(0.5f, 0.5f);
            transform.sizeDelta = new Vector2(spriteForStyle.texture.width, spriteForStyle.texture.height);
            Image image = uiElement.AddComponent<Image>();
            image.sprite = spriteForStyle;
            image.type = Image.Type.Simple;
            uiElement.transform.SetParent(parent, false);
            GameObject uiPointer = new GameObject("IconPointer");
            RectTransform transform2 = uiPointer.AddComponent<RectTransform>();
            transform2.anchorMin = transform2.anchorMax = transform.anchorMin;
            transform2.sizeDelta = new Vector2(pointerSprite.texture.width, pointerSprite.texture.height);
            Image image2 = uiPointer.AddComponent<Image>();
            image2.sprite = pointerSprite;
            image2.type = Image.Type.Simple;
            uiPointer.transform.SetParent(transform, false);
            transform2.anchoredPosition = new Vector2(0f, pointerDist);
            return new MinimapIcon(trackedObject, uiElement, uiPointer, style);
        }

        public void Destroy()
        {
            if (this.uiRect != null)
            {
                Object.Destroy(this.uiRect.gameObject);
            }
        }

        public void SetColor(Color color)
        {
            if (this.uiRect != null)
            {
                this.uiRect.GetComponent<Image>().color = color;
            }
        }

        public void SetDepth(bool aboveAll)
        {
            if (this.uiRect != null)
            {
                if (aboveAll)
                {
                    this.uiRect.SetAsLastSibling();
                }
                else
                {
                    this.uiRect.SetAsFirstSibling();
                }
            }
        }

        public void SetPointerSize(float size, float originDistance)
        {
            if (this.pointerRect != null)
            {
                this.pointerRect.sizeDelta = new Vector2(size, size);
                this.pointerRect.anchoredPosition = new Vector2(0f, originDistance);
            }
        }

        public void SetSize(Vector2 size)
        {
            if (this.uiRect != null)
            {
                this.uiRect.sizeDelta = size;
            }
        }

        public bool UpdateUI(Bounds worldBounds, float minimapSize)
        {
            if (this.obj == null)
            {
                return false;
            }
            float x = worldBounds.size.x;
            Vector3 vector = this.obj.position - worldBounds.center;
            vector.y = vector.z;
            vector.z = 0f;
            float num2 = Mathf.Abs(vector.x) / x;
            vector.x = vector.x < 0f ? -num2 : num2;
            float num3 = Mathf.Abs(vector.y) / x;
            vector.y = vector.y < 0f ? -num3 : num3;
            Vector2 vector2 = vector * minimapSize;
            this.uiRect.anchoredPosition = vector2;
            if (this.rotation)
            {
                float z = Mathf.Atan2(this.obj.forward.z, this.obj.forward.x) * 57.29578f - 90f;
                this.uiRect.eulerAngles = new Vector3(0f, 0f, z);
            }
            return true;
        }
    }

    public class Preset
    {
        public readonly Vector3 center;
        public readonly float orthographicSize;

        public Preset(Vector3 center, float orthographicSize)
        {
            this.center = center;
            this.orthographicSize = orthographicSize;
        }
    }

    public void Dispose()
    {
        foreach (var obj in _interface)
            Destroy(obj);
        Destroy(this);
    }
}

