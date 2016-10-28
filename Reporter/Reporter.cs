//unity before version 5 is old
//#define USE_OLD_UNITY 

using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Assets.Reporter;

[System.Serializable]
public class Images
{
    public Texture2D textureImage;
    public Texture2D materialImage;
    public Texture2D meshImage;
    public Texture2D audioImage;
    public Texture2D searchImage;

    public Texture2D closeImage;
    public Texture2D allImage;

    public Texture2D newObjectImage;
    public Texture2D remainObjectImage;
    public Texture2D removeObjectImage;

    public Texture2D normalImage;
    public Texture2D dublicateImage;
    public Texture2D leakImage;

    public Texture2D barImage;
    public Texture2D button_activeImage;
    public Texture2D even_logImage;
    public Texture2D odd_logImage;
    public Texture2D selectedImage;

    public GUISkin reporterScrollerSkin;
}
     
public class Reporter : MonoBehaviour
{
    public enum MemoryType
    {
        Normal,
        Leak,
        Dubliaction,
    }

    public class ObjectContents
    {
        public string name;
        public string detailInfo;
        public MemoryType logType;
    }

    private enum DetailView
    {
        New,
        Remain,
        Remove,
        All,
    }

    private enum ReportView
    {
        Texture,
        Material,
        Mesh,
        Audio,
    }

    private ReportView currentView = ReportView.Texture;
    private DetailView currentActiveTab = DetailView.All;

    private List<ObjectContents> currentObjectContents = new List<ObjectContents>();

    [HideInInspector]

    private bool show;
    private bool boolclear;
    private bool collapse;
    private bool clearOnNewSceneLoaded;
    private bool showTime;
    private bool showScene;
    private bool showMemory;
    private bool showFps;

    private bool showNewObj = true;
    private bool showRemainObj = true;
    private bool showRemoveObj = true;
    private bool showAllObj = true;

    static bool created = false;
    public bool Initialized = false;

    public Images images;
    // gui
    private GUIContent textureContent;
    private GUIContent materialContent;
    private GUIContent meshContent;
    private GUIContent audioContent;
    private GUIContent searchContent;
    private GUIContent closeContent;

    private GUIContent allContent;
    private GUIContent newObjectContent;
    private GUIContent remainObjectContent;
    private GUIContent removeObjectContent;

    private GUIContent normalContent;
    private GUIContent dublicateContent;
    private GUIContent leakContent;

    private GUIStyle barStyle;
    private GUIStyle buttonActiveStyle;

    private GUIStyle nonStyle;
    private GUIStyle whiteFontStyle;
    private GUIStyle lowerLeftFontStyle;
    private GUIStyle backStyle;
    private GUIStyle evenLogStyle;
    private GUIStyle oddLogStyle;
    private GUIStyle logButtonStyle;
    private GUIStyle selectedLogStyle;
    private GUIStyle selectedLogFontStyle;
    private GUIStyle detailsLabelStyle;
    private GUIStyle searchStyle;
    private GUIStyle sliderBackStyle;
    private GUIStyle sliderThumbStyle;
    private GUISkin toolbarScrollerSkin;
    private GUISkin logScrollerSkin;

    private Rect screenRect;
    private Rect toolBarRect;
    private Rect logsRect;
    private Rect detailsRect;
    private Rect buttomRect;

    private Vector2 detailsRectTopLeft;
    private Vector2 scrollPosition;
    private Vector2 scrollPosition2;
    private Vector2 toolbarScrollPosition;

    private ObjectContents selectedContents;

    private float toolbarOldDrag = 0;
    private float oldDrag;
    private float oldDrag2;
    private int startIndex;

    public Vector2 size = new Vector2(32, 32);
    public int numOfCircleToShow = 1;
    private string filterText = "";

    /*Texture*/
    private  TextureDetails TextureObj = new TextureDetails();
    /*Material*/
    private MaterialDetails MaterialObj = new MaterialDetails();
    /*Mesh*/
    private MeshDetails MeshObj = new MeshDetails();
    /*Audio*/
    private AudioDetails AudioObj = new AudioDetails();
   
    /*Texture Info <InstanceID, DetailInfo>*/
    private Dictionary<int, TextureDetails> newScanedTexture;
    private Dictionary<int, TextureDetails> oldScanedTexture = new Dictionary<int, TextureDetails>();

    private Dictionary<int, TextureDetails> removedTexture;
    private Dictionary<int, TextureDetails> remainedTexture;
    private Dictionary<int, TextureDetails> newTexture;

    private Dictionary<int, TextureDetails> realReamovedTexture = new Dictionary<int, TextureDetails>();
    private Dictionary<int, TextureDetails> realRemainedTexture = new Dictionary<int, TextureDetails>();
    private Dictionary<int, TextureDetails> realNewTexture = new Dictionary<int, TextureDetails>();
    private Dictionary<int, TextureDetails> realAllTexture = new Dictionary<int, TextureDetails>();

    /*Material Info <InstanceID, DetailInfo>*/
    private Dictionary<int, MaterialDetails> newScanedMaterial;
    private Dictionary<int, MaterialDetails> oldScanedMaterial = new Dictionary<int, MaterialDetails>();

    private Dictionary<int, MaterialDetails> removedMaterial;
    private Dictionary<int, MaterialDetails> remainedMaterial;
    private Dictionary<int, MaterialDetails> newMaterial;

    public Dictionary<int, MaterialDetails> realReamovedMaterial = new Dictionary<int, MaterialDetails>();
    public Dictionary<int, MaterialDetails> realRemainedMaterial = new Dictionary<int, MaterialDetails>();
    public Dictionary<int, MaterialDetails> realNewMaterial = new Dictionary<int, MaterialDetails>();
    public Dictionary<int, MaterialDetails> realAllMaterial = new Dictionary<int, MaterialDetails>();

    /*Mesh Info <InstanceID, DetailInfo>*/
    private Dictionary<int, MeshDetails> newScanedMesh;
    private Dictionary<int, MeshDetails> oldScanedMesh = new Dictionary<int, MeshDetails>();

    private Dictionary<int, MeshDetails> removedMesh;
    private Dictionary<int, MeshDetails> remainedMesh;
    private Dictionary<int, MeshDetails> newMesh;

    private Dictionary<int, MeshDetails> realReamovedMesh = new Dictionary<int, MeshDetails>();
    private Dictionary<int, MeshDetails> realRemainedMesh = new Dictionary<int, MeshDetails>();
    private Dictionary<int, MeshDetails> realNewMesh = new Dictionary<int, MeshDetails>();
    private Dictionary<int, MeshDetails> realAllMesh = new Dictionary<int, MeshDetails>();

    /*Audio Info <InstanceID, DetailInfo>*/
    private Dictionary<int, AudioDetails> newScanedAudio;
    private Dictionary<int, AudioDetails> oldScanedAudio = new Dictionary<int, AudioDetails>();

    private Dictionary<int, AudioDetails> removedAudio;
    private Dictionary<int, AudioDetails> remainedAudio;
    private Dictionary<int, AudioDetails> newAudio;

    private Dictionary<int, AudioDetails> realReamovedAudio = new Dictionary<int, AudioDetails>();
    private Dictionary<int, AudioDetails> realRemainedAudio = new Dictionary<int, AudioDetails>();
    private Dictionary<int, AudioDetails> realNewAudio = new Dictionary<int, AudioDetails>();
    private Dictionary<int, AudioDetails> realAllAudio = new Dictionary<int, AudioDetails>();

    //Objects Count Info
    private int allObjCount = 0;
    private int realNewObjCount = 0;
    private int realRemainObjCount = 0;
    private int realRemoveObjCount = 0;

    private int newObjCount = 0;
    private int removeObjcount = 0;

    private readonly string[] _ignoreTextures = new[] { "leak_icon", "normal_icon", "dublicate_icon","verticalslider","vertical scrollbar","button on hover", "button on","horizontal scrollbar thumb", "textfield hover", "vertival scrollbar", "box", "slider thumb", "slider thumb active", "slidert humb hover", "horizontalslider", "vertivalslider", "vertical scrollbar thumb", "textfield active", "textfield on", "textfield", "toggle on active", "toggle on hover", "toggle active","toggle hover", "vertical scrllbar", "horizontal scrollbar", "window","window on","UnityWatermark-dev", "","all", "newobject", "removeobject", "remainobject", "audio", "software", "selected", "search", "scroller_vertical_thumb", "scroller_vertical_back", "scroller_up_arraw", "scroller_right_arraw", "scroller_left_arraw", "scroller_horizental_thumb", "scroller_horizental_back", "scroller_down_arraw", "odd_log", "memory", "material", "mesh", "graphicCard", "texture", "even_log", "error_icon", "date", "ComputerIcon", "collapse", "close", "clearOnSceneLoaded", "clear", "chart", "back", "bar", "buildFrom", "button_active", "Font Texture", "Default-Particle", "button active", "button hover", "button", "d_SceneViewFx", "ClothRenderer Icon", "IN ObjectField", "cmd mid on", "Font Texture", "Font Texture", "Font Texture", "Font Texture", "Font Texture", "Font Texture", "TextMesh Icon", "d_WaitSpin02", "SpringJoint Icon", "AudioEchoFilter Icon", "d_MoveTool", "Mesh Icon", "d_WaitSpin11", "NetworkView Icon", "ConfigurableJoint Icon", "NavMeshAgent Icon", "MeshRenderer Icon", "WindZone Icon", "FolderEmpty Icon", "TextField", "Animator Icon", "d_PlayButtonProfile", "d_ParticleSystem Icon", "d_ToolHandleLocal", "cmd right", "ParticleEmitter Icon", "IN foldout", "d_WaitSpin06", "d_console.erroricon", "scroll vert up", "d_console.infoicon.sml", "Material Icon", "AudioHighPassFilter Icon", "Light Icon", "d_ToolHandleGlobal", "OcclusionPortal Icon", "GUILayer Icon", "d_ViewToolMove", "PrefabNormal Icon", "AudioSource Icon", "d_StepButton", "scroll vert", "AudioSource Gizmo", "GV gizmo pulldown", "FlareLayer Icon", "OffMeshLink Icon", "d_PauseButton", "d_RotateTool On", "ParticleAnimator Icon", "SphereCollider Icon", "ScriptableObject Icon", "AudioChorusFilter Icon", "d_SceneViewAudio", "d_console.infoicon", "Prefab Icon", "GameObject Icon", "ConstantForce Icon", "scroll vert down", "RigidBody Icon", "DefaultAsset Icon", "scroll vert thumb", "d_PauseButton On", "TrailRenderer Icon", "GUIText Icon", "Camera Gizmo", "AudioReverbZone Icon", "d_FilterByType", "console.infoicon", "d_WaitSpin05", "SkinnedCloth Icon", "OcclusionArea Icon", "Projector Icon", "unselected", "d_ToolHandlePivot", "AudioLowPassFilter Icon", "d_WaitSpin09", "d_ViewToolOrbit", "d_WaitSpin08", "Folder Icon", "d_ViewToolOrbit On", "d_UnityEditor.InspectorWindow", "d_FilterByLabel", "WorldParticleCollider Icon", "d_WaitSpin10", "d_PlayButtonProfile On", "d_WaitSpin00", "InteractiveCloth Icon", "btn left", "d_ScaleTool On", "cmd left on", "mini popup", "cmd mid", "IN foldout on", "btn right", "d_icon dropdown", "toggle", "pulldown", "cmd left", "toggle on", "cmd", "cn entrybackodd", "Transform Icon", "d_ToolHandleCenter", "LineRenderer Icon", "d_console.warnicon", "ParticleRenderer Icon", "d_PauseButton Anim", "d_ViewToolMove On", "MeshCollider Icon", "LightProbeGroup Icon", "AudioDistortionFilter Icon", "CharacterJoint Icon", "CharacterController Icon", "d_WaitSpin01", "d_ViewToolZoom On", "Toolbar.png", "dockarea overlay.png", "toolbar pulldown.png", "IN BigTitle.png", "toolbarsearchCancelButtonOff.png", "CN Box.png", "toolbar button on.png", "IN Header on.png", "pane options.png", "tabbar bright on f.png", "toolbar button act.png", "tabbar on.png", "toolbarsearchpopup.png", "IN LockButton.png", "gameviewbackground.png", "IN Header.png", "toolbar button.png", "tabbar on f.png", "toolbar back.png", "toolbarsearch.png", "toolbar popup.png", "dockarea back.png", "MeshFilter Icon", "d_PlayButton On", "d_PlayButton", "_Help", "CapsuleCollider Icon", "AudioListener Icon", "d_ScaleTool", "d_MoveTool on", "WheelCollider Icon", "LensFlare Icon", "d_StepButton Anim", "d_console.warnicon.sml", "d_PlayButton Anim", "AudioReverbFilter Icon", "SceneAsset Icon", "d_Project", "d_WaitSpin03", "d_UnityEditor.ConsoleWindow", "d_UnityEditor.SceneView", "Animation Icon", "BoxCollider Icon", "d__Popup", "d_SceneViewLighting", "d_StepButton On", "d_RotateTool", "GUITexture Icon", "d_ViewToolZoom", "d_UnityEditor.GameView", "PrefabModel Icon", "d_Favorite", "d_console.erroricon.sml", "LODGroup Icon", "Skybox Icon", "d_WaitSpin07", "d_UnityEditor.HierarchyWindow", "HingeJoint Icon", "Js Script Icon", "TerrainCollider Icon", "AnimationClip Icon", "NavMeshObstacle Icon", "cs Script Icon", "d_PlayButtonProfile Anim", "Fixedjoint Icon", "Halo Icon", "Camera Icon", "d_WaitSpin04", "ParticleSystem Gizmo", "MeshParticleEmitter Icon", };
    private readonly string[] _ignoreMaterials = new[] { "","Font Material", "[NGUI] Font Material", "Diffuse (Instance)"};
    private readonly string[] _ignoreMesh = new[] { "","Cube", "[NGUI] Font Material" };
    private readonly string[] _ignoreAudio = new[] { "" };

    void Awake()
    {
        if (!Initialized) Initialize();
    }  

    public void Initialize()
    {
        if (!created)
        {
            try
            {
                gameObject.SendMessage("OnPreStart");
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }

            DontDestroyOnLoad(gameObject);

            created = true;
        
        }
        else
        {
            Debug.LogWarning("tow manager is exists delete the second");
            DestroyImmediate(gameObject, true);
            return;
        }

        textureContent = new GUIContent("", images.textureImage, "Clear logs");
        materialContent = new GUIContent("", images.materialImage, "Collapse logs");
        meshContent = new GUIContent("", images.meshImage, "Clear logs on new scene loaded");
        audioContent = new GUIContent("", images.audioImage, "Show Hide Time");

        searchContent = new GUIContent("", images.searchImage, "Search for logs");
        closeContent = new GUIContent("", images.closeImage, "Hide logs");

        allContent = new GUIContent("", images.allImage, "Clear logs");
        newObjectContent = new GUIContent("", images.newObjectImage, "show or hide logs");
        remainObjectContent = new GUIContent("", images.remainObjectImage, "show or hide warnings");
        removeObjectContent = new GUIContent("", images.removeObjectImage, "show or hide errors");

        normalContent = new GUIContent("", images.normalImage, "show or hide normal");
        dublicateContent = new GUIContent("", images.dublicateImage, "show or hide dublicate");
        leakContent = new GUIContent("", images.leakImage, "show or hide leak");

        show = false;
        boolclear = true;
        collapse = false;
        clearOnNewSceneLoaded = false;
        showTime = false;
        showScene = false;
        showMemory = false;
        showFps = false;

        showNewObj = true;
        showRemainObj = false;
        showRemoveObj = false;
        showAllObj = false;

        filterText = "";
        size.x = size.y = PlayerPrefs.GetFloat("Reporter_size", 32);

        InitializeStyle();

        Initialized = true;

        if (show) DoShow();

    }

    void InitializeStyle()
    {
        int paddingX = (int)(size.x * 0.2f);
        int paddingY = (int)(size.y * 0.2f);

        nonStyle = new GUIStyle();
        nonStyle.clipping = TextClipping.Clip;
        nonStyle.border = new RectOffset(0, 0, 0, 0);
        nonStyle.normal.background = null;
        nonStyle.fontSize = (int)(size.y / 2);
        nonStyle.alignment = TextAnchor.MiddleCenter;

        whiteFontStyle = new GUIStyle();
        whiteFontStyle.clipping = TextClipping.Clip;
        whiteFontStyle.border = new RectOffset(0, 0, 0, 0);
        whiteFontStyle.normal.background = null;
        whiteFontStyle.fontSize = (int)(size.y / 2);
        whiteFontStyle.alignment = TextAnchor.MiddleCenter;
        whiteFontStyle.normal.textColor = Color.white;

        lowerLeftFontStyle = new GUIStyle();
        lowerLeftFontStyle.clipping = TextClipping.Clip;
        lowerLeftFontStyle.border = new RectOffset(0, 0, 0, 0);
        lowerLeftFontStyle.normal.background = null;
        lowerLeftFontStyle.fontSize = (int)(size.y / 2);
        lowerLeftFontStyle.fontStyle = FontStyle.Bold;
        lowerLeftFontStyle.alignment = TextAnchor.LowerLeft;

        barStyle = new GUIStyle();
        barStyle.border = new RectOffset(1, 1, 1, 1);
        barStyle.normal.background = images.barImage;
        barStyle.active.background = images.button_activeImage;
        barStyle.alignment = TextAnchor.MiddleCenter;
        barStyle.margin = new RectOffset(1, 1, 1, 1);

        barStyle.clipping = TextClipping.Clip;
        barStyle.fontSize = (int)(size.y / 2);

        buttonActiveStyle = new GUIStyle();
        buttonActiveStyle.border = new RectOffset(1, 1, 1, 1);
        buttonActiveStyle.normal.background = images.button_activeImage;
        buttonActiveStyle.alignment = TextAnchor.MiddleCenter;
        buttonActiveStyle.margin = new RectOffset(1, 1, 1, 1);
        buttonActiveStyle.fontSize = (int)(size.y / 2);

        backStyle = new GUIStyle();
        backStyle.normal.background = images.odd_logImage;
        backStyle.clipping = TextClipping.Clip;
        backStyle.fontSize = (int)(size.y / 2);

        evenLogStyle = new GUIStyle();
        evenLogStyle.normal.background = images.even_logImage;
        evenLogStyle.fixedHeight = size.y;
        evenLogStyle.clipping = TextClipping.Clip;
        evenLogStyle.alignment = TextAnchor.UpperLeft;
        evenLogStyle.imagePosition = ImagePosition.ImageLeft;
        evenLogStyle.fontSize = (int)(size.y / 2);

        oddLogStyle = new GUIStyle();
        oddLogStyle.normal.background = images.odd_logImage;
        oddLogStyle.fixedHeight = size.y;
        oddLogStyle.clipping = TextClipping.Clip;
        oddLogStyle.alignment = TextAnchor.UpperLeft;
        oddLogStyle.imagePosition = ImagePosition.ImageLeft;
        oddLogStyle.fontSize = (int)(size.y / 2);

        logButtonStyle = new GUIStyle();
        logButtonStyle.fixedHeight = size.y;
        logButtonStyle.clipping = TextClipping.Clip;
        logButtonStyle.alignment = TextAnchor.UpperLeft;
        logButtonStyle.fontSize = (int)(size.y / 2);
        logButtonStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        selectedLogStyle = new GUIStyle();
        selectedLogStyle.normal.background = images.selectedImage;
        selectedLogStyle.fixedHeight = size.y;
        selectedLogStyle.clipping = TextClipping.Clip;
        selectedLogStyle.alignment = TextAnchor.UpperLeft;
        selectedLogStyle.normal.textColor = Color.white;
        selectedLogStyle.fontSize = (int)(size.y / 2);

        selectedLogFontStyle = new GUIStyle();
        selectedLogFontStyle.normal.background = images.selectedImage;
        selectedLogFontStyle.fixedHeight = size.y;
        selectedLogFontStyle.clipping = TextClipping.Clip;
        selectedLogFontStyle.alignment = TextAnchor.UpperLeft;
        selectedLogFontStyle.normal.textColor = Color.white;
        selectedLogFontStyle.fontSize = (int)(size.y / 2);
        selectedLogFontStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);

        detailsLabelStyle = new GUIStyle();
        detailsLabelStyle.wordWrap = true;
        detailsLabelStyle.fontSize = (int)(size.y);
        detailsLabelStyle.padding = new RectOffset(paddingX, paddingX, paddingY, paddingY);


        searchStyle = new GUIStyle();
        searchStyle.clipping = TextClipping.Clip;
        searchStyle.alignment = TextAnchor.LowerCenter;
        searchStyle.fontSize = (int)(size.y / 2);
        searchStyle.wordWrap = true;

        sliderBackStyle = new GUIStyle();
        sliderBackStyle.normal.background = images.barImage;
        sliderBackStyle.fixedHeight = size.y;
        sliderBackStyle.border = new RectOffset(1, 1, 1, 1);

        sliderThumbStyle = new GUIStyle();
        sliderThumbStyle.normal.background = images.selectedImage;
        sliderThumbStyle.fixedWidth = size.x;

        GUISkin skin = images.reporterScrollerSkin;

        toolbarScrollerSkin = (GUISkin)GameObject.Instantiate(skin);
        toolbarScrollerSkin.verticalScrollbar.fixedWidth = 0f;
        toolbarScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        toolbarScrollerSkin.verticalScrollbarThumb.fixedWidth = 0f;
        toolbarScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

        logScrollerSkin = (GUISkin)GameObject.Instantiate(skin);
        logScrollerSkin.verticalScrollbar.fixedWidth = size.x * 2f;
        logScrollerSkin.horizontalScrollbar.fixedHeight = 0f;
        logScrollerSkin.verticalScrollbarThumb.fixedWidth = size.x * 2f;
        logScrollerSkin.horizontalScrollbarThumb.fixedHeight = 0f;

    }
   
   void FilteringLog<T>(bool filter, string _filterText, Dictionary<int, T> inputData) where T : class // n
    {
        foreach (KeyValuePair<int, T> kvp in inputData) 
        {
            ObjectContents log = new ObjectContents();
            System.Type type = kvp.Value.GetType();
            if (type == typeof(TextureDetails)) { log.detailInfo = TextureObj.GetLogContent(kvp.Value as TextureDetails); log.name = (kvp.Value as TextureDetails).objectName; }
            else if (type == typeof(MaterialDetails)) { log.detailInfo = MaterialObj.getLogContent(kvp.Value as MaterialDetails); log.name = (kvp.Value as MaterialDetails).objectName; }
            else if (type == typeof(MeshDetails)) { log.detailInfo = MeshObj.getLogContent(kvp.Value as MeshDetails); log.name = (kvp.Value as MeshDetails).objectName; } 
            else if (type == typeof(AudioDetails)) { log.detailInfo = AudioObj.getLogContent(kvp.Value as AudioDetails); log.name = (kvp.Value as AudioDetails).objectName;  } 
  
            if (filter)
            {
                if (log.detailInfo.ToLower().Contains(_filterText))
                    currentObjectContents.Add(log);
            }
            else
            {
                currentObjectContents.Add(log);
            }
        }
    }

    void CalculateCurrentContents()
    {
        bool filter = !string.IsNullOrEmpty(filterText);
        string _filterText = "";

        if (filter)
            _filterText = filterText.ToLower();
        currentObjectContents.Clear();

        if (currentView == ReportView.Texture)
        {
            if (currentActiveTab == DetailView.New) FilteringLog(filter, _filterText, realNewTexture);
            else if (currentActiveTab == DetailView.Remain) FilteringLog(filter, _filterText, realRemainedTexture);
            else if (currentActiveTab == DetailView.All) FilteringLog(filter, _filterText, realAllTexture);
            else FilteringLog(filter, _filterText, realReamovedTexture); 
        }
        else if (currentView == ReportView.Material)
        {
            if (currentActiveTab == DetailView.New) FilteringLog(filter, _filterText, realNewMaterial);
            else if (currentActiveTab == DetailView.Remain) FilteringLog(filter, _filterText, realRemainedMaterial);
            else if (currentActiveTab == DetailView.All) FilteringLog(filter, _filterText, realAllMaterial);
            else FilteringLog(filter, _filterText, realReamovedMaterial);
        }
        else if (currentView == ReportView.Mesh)
        {
            if (currentActiveTab == DetailView.New) FilteringLog(filter, _filterText, realNewMesh);
            else if (currentActiveTab == DetailView.Remain) FilteringLog(filter, _filterText, realRemainedMesh);
            else if (currentActiveTab == DetailView.All) FilteringLog(filter, _filterText, realAllMesh);
            else FilteringLog(filter, _filterText, realReamovedMesh);
        }
        else if (currentView == ReportView.Audio)
        {
            if (currentActiveTab == DetailView.New) FilteringLog(filter, _filterText, realNewAudio);
            else if (currentActiveTab == DetailView.Remain) FilteringLog(filter, _filterText, realRemainedAudio);
            else if (currentActiveTab == DetailView.All) FilteringLog(filter, _filterText, realAllAudio);
            else FilteringLog(filter, _filterText, realReamovedAudio);
        }

    }

    Rect countRect;
    void ClearButtonPressInfo(string input)
    {
        boolclear = false;
        collapse = false;
        clearOnNewSceneLoaded = false;
        showTime = false;
        showScene = false;
        showMemory = false;
        showFps = false;

        if (input == "boolclear")
            boolclear = true;
        else if (input == "collapse")
            collapse = true;
        else if (input == "clearOnNewSceneLoaded")
            clearOnNewSceneLoaded = true;
        else if (input == "showTime")
            showTime = true;
        else if (input == "showScene")
            showScene = true;
        else if (input == "showMemory")
            showMemory = true;
        else if (input == "showFps")
            showFps = true;
    }

    void ClearButtonPressDetailInfo(string input)
    {
        showNewObj = false;
        showRemainObj = false;
        showRemoveObj = false;
        showAllObj = false;

        if (input == "showNewObj")
            showNewObj = true;
        else if (input == "showRemainObj")
            showRemainObj = true;
        else if (input == "showRemoveObj")
            showRemoveObj = true;
        else if (input == "showAllObj")
            showAllObj = true;

    }

    void DrawDetailsView()
    {
        if (currentView == ReportView.Texture)
        {
            if (currentActiveTab == DetailView.New) SetTextureContents(realNewTexture);
            else if (currentActiveTab == DetailView.Remain) SetTextureContents(realRemainedTexture);
            else if (currentActiveTab == DetailView.All) SetTextureContents(realAllTexture);
            else SetTextureContents(realReamovedTexture);
        }
        else if (currentView == ReportView.Material)
        {
            if (currentActiveTab == DetailView.New) SetMaterialContents(realNewMaterial);
            else if (currentActiveTab == DetailView.Remain) SetMaterialContents(realRemainedMaterial);
            else if (currentActiveTab == DetailView.All) SetMaterialContents(realAllMaterial);
            else SetMaterialContents(realReamovedMaterial);
        }
        else if (currentView == ReportView.Mesh)
        {
            if (currentActiveTab == DetailView.New) SetMeshContents(realNewMesh);
            else if (currentActiveTab == DetailView.Remain) SetMeshContents(realRemainedMesh);
            else if (currentActiveTab == DetailView.All) SetMeshContents(realAllMesh);
            else SetMeshContents(realReamovedMesh);
        }
        else if (currentView == ReportView.Audio)
        {
            if (currentActiveTab == DetailView.New) SetAudioContents(realNewAudio);
            else if (currentActiveTab == DetailView.Remain) SetAudioContents(realRemainedAudio);
            else if (currentActiveTab == DetailView.All) SetAudioContents(realAllAudio);
            else SetAudioContents(realReamovedAudio);
        }
     
    }
    void DrawDetailVewCount()
    {
        if (currentView == ReportView.Texture)
        {
            realNewObjCount = realNewTexture.Count;
            realRemainObjCount = realRemainedTexture.Count;
            realRemoveObjCount = realReamovedTexture.Count;
            allObjCount = realAllTexture.Count;
            
        }
        else if (currentView == ReportView.Material)
        {
            realNewObjCount = realNewMaterial.Count;
            realRemainObjCount = realRemainedMaterial.Count;
            realRemoveObjCount = realReamovedMaterial.Count;
            allObjCount = realAllMaterial.Count;
        }
        else if (currentView == ReportView.Mesh)
        {
            realNewObjCount = realNewMesh.Count;
            realRemainObjCount = realRemainedMesh.Count;
            realRemoveObjCount = realReamovedMesh.Count;
            allObjCount = realAllMesh.Count;

        }
        else if (currentView == ReportView.Audio)
        {
            realNewObjCount = realNewAudio.Count;
            realRemainObjCount = realRemainedAudio.Count;
            realRemoveObjCount = realReamovedAudio.Count;
            allObjCount = realAllAudio.Count;
        }

    }

    void DrawContentsMenu()
    {
        toolBarRect.x = 0f;
        toolBarRect.y = 0f;
        toolBarRect.width = Screen.width;
        toolBarRect.height = size.y * 2f;

        GUI.skin = toolbarScrollerSkin;
        Vector2 drag = GetDrag();
        if ((drag.x != 0) && (downPos != Vector2.zero) && (downPos.y > Screen.height - size.y * 2f))
        {
            toolbarScrollPosition.x -= (drag.x - toolbarOldDrag);
        }

        toolbarOldDrag = drag.x;
        GUILayout.BeginArea(toolBarRect);
        toolbarScrollPosition = GUILayout.BeginScrollView(toolbarScrollPosition);
        GUILayout.BeginHorizontal(barStyle);

        if (GUILayout.Button(textureContent, (boolclear) ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressInfo("boolclear");
            currentView = ReportView.Texture;

            DrawDetailVewCount();

            if (currentActiveTab == DetailView.New) SetTextureContents(realNewTexture);
            else if (currentActiveTab == DetailView.Remain) SetTextureContents(realRemainedTexture);
            else if (currentActiveTab == DetailView.All) SetTextureContents(realAllTexture);
            else SetTextureContents(realReamovedTexture);

        }
        if (GUILayout.Button(materialContent, (collapse) ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressInfo("collapse");
            currentView = ReportView.Material;

            DrawDetailVewCount();

            if (currentActiveTab == DetailView.New) SetMaterialContents(realNewMaterial);
            else if (currentActiveTab == DetailView.Remain) SetMaterialContents(realRemainedMaterial);
            else if (currentActiveTab == DetailView.All) SetMaterialContents(realAllMaterial);
            else SetMaterialContents(realReamovedMaterial);

        }
        if (GUILayout.Button(meshContent, (clearOnNewSceneLoaded) ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressInfo("clearOnNewSceneLoaded");
            currentView = ReportView.Mesh;

            DrawDetailVewCount();

            if (currentActiveTab == DetailView.New) SetMeshContents(realNewMesh);
            else if (currentActiveTab == DetailView.Remain) SetMeshContents(realRemainedMesh);
            else if (currentActiveTab == DetailView.All) SetMeshContents(realAllMesh);
            else SetMeshContents(realReamovedMesh);
        }
        if (GUILayout.Button(audioContent, (showTime) ? buttonActiveStyle : barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressInfo("showTime");
            currentView = ReportView.Audio;

            DrawDetailVewCount();

            if (currentActiveTab == DetailView.New) SetAudioContents(realNewAudio);
            else if (currentActiveTab == DetailView.Remain) SetAudioContents(realRemainedAudio);
            else if (currentActiveTab == DetailView.All) SetAudioContents(realAllAudio);
            else SetAudioContents(realReamovedAudio);
        }

        GUILayout.Box(searchContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2));
        tempRect = GUILayoutUtility.GetLastRect();
        string newFilterText = GUI.TextField(tempRect, filterText, searchStyle);
        if (newFilterText != filterText)
        {
            filterText = newFilterText;
            CalculateCurrentContents();
        }

        GUILayout.FlexibleSpace();
    }
    void DrawDetailsMenu()
    {
        string logsText = " ";
        logsText = realNewObjCount.ToString();//+"/"+newObjCount.ToString();

        string logsWarningText = " ";
        logsWarningText = realRemainObjCount.ToString();

        string logsErrorText = " ";
        logsErrorText = realRemoveObjCount.ToString(); //+ "/" + removeObjcount.ToString(); ;

        string logsText2 = " ";
        logsText2 = allObjCount.ToString();

        GUILayout.BeginHorizontal((showAllObj) ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(allContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showAllObj");
            selectedContents = null;
            currentActiveTab = DetailView.All;
            DrawDetailsView();
        }
        if (GUILayout.Button(logsText2, whiteFontStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showAllObj");
            selectedContents = null;
            currentActiveTab = DetailView.All;
            DrawDetailsView();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal((showNewObj) ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(newObjectContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showNewObj");
            selectedContents = null;
            currentActiveTab = DetailView.New;
            DrawDetailsView();
        }
        if (GUILayout.Button(logsText, whiteFontStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showNewObj");
            selectedContents = null;
            currentActiveTab = DetailView.New;
            DrawDetailsView();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal((showRemainObj) ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(remainObjectContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showRemainObj");
            selectedContents = null;
            currentActiveTab = DetailView.Remain;
            DrawDetailsView();
        }
        if (GUILayout.Button(logsWarningText, whiteFontStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showRemainObj");
            selectedContents = null;
            currentActiveTab = DetailView.Remain;
            DrawDetailsView();
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal((showRemoveObj) ? buttonActiveStyle : barStyle);
        if (GUILayout.Button(removeObjectContent, nonStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showRemoveObj");
            selectedContents = null;
            currentActiveTab = DetailView.Remove;
            DrawDetailsView();
        }
        if (GUILayout.Button(logsErrorText, whiteFontStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            ClearButtonPressDetailInfo("showRemoveObj");
            selectedContents = null;
            currentActiveTab = DetailView.Remove;
            DrawDetailsView();
        }
        GUILayout.EndHorizontal();

        if (GUILayout.Button(closeContent, barStyle, GUILayout.Width(size.x * 2), GUILayout.Height(size.y * 2)))
        {
            show = false;
            selectedContents = null;
            ReporterGUI gui = gameObject.GetComponent<ReporterGUI>();
            DestroyImmediate(gui);

            try
            {
                gameObject.SendMessage("OnHideReporter");
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
            }
        }

        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();

        GUILayout.EndArea();
    }
    void DrawToolBar()
    {
        DrawContentsMenu();
        DrawDetailsMenu();
    }

    Rect tempRect;
    void DrawContents()
    {
        GUILayout.BeginArea(logsRect, backStyle);

        GUI.skin = logScrollerSkin;

        Vector2 drag = GetDrag();

        if (drag.y != 0 && logsRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
        {
            scrollPosition.y += (drag.y - oldDrag);
        }
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        oldDrag = drag.y;


        int totalVisibleCount = (int)(Screen.height * 0.75f / size.y);
        int totalCount = currentObjectContents.Count;

        totalVisibleCount = Mathf.Min(totalVisibleCount, totalCount - startIndex);
        int index = 0;
        int beforeHeight = (int)(startIndex * size.y);
     
        if (beforeHeight > 0)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(beforeHeight));
            GUILayout.Label("---");
            GUILayout.EndHorizontal();
        }

        int endIndex = startIndex + totalVisibleCount;
        endIndex = Mathf.Clamp(endIndex, 0, totalCount);
        bool scrollerVisible = (totalVisibleCount < totalCount);
        for (int i = startIndex; (startIndex + index) < endIndex; i++)
        {
            if (i >= currentObjectContents.Count)
                break;
            ObjectContents log = currentObjectContents[i];

            if (index >= totalVisibleCount) break;

            GUIContent content = null;
            if (log.logType == MemoryType.Dubliaction)
                content = dublicateContent;
            else if (log.logType == MemoryType.Leak)
                content = leakContent;
            else
                content = normalContent;

            GUIStyle currentLogStyle = ((startIndex + index) % 2 == 0) ? evenLogStyle : oddLogStyle;

            if (log == selectedContents) currentLogStyle = selectedLogStyle;
                       
            float w = 0f;

            countRect.x = Screen.width - w;
            countRect.y = size.y * i;
            if (beforeHeight > 0)
                countRect.y += 8;
            countRect.width = w;
            countRect.height = size.y;

            if (scrollerVisible)
                countRect.x -= size.x * 2;

            GUILayout.BeginHorizontal(currentLogStyle);
       
            if(currentActiveTab == DetailView.All && currentView == ReportView.Texture)
            {
                if (log == selectedContents)
                {
                    if (GUILayout.Button(content, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y))) { }
                    GUILayout.Label(log.detailInfo, selectedLogFontStyle);
                }
                else {
                    if (GUILayout.Button(content, nonStyle, GUILayout.Width(size.x), GUILayout.Height(size.y))) selectedContents = log;
                    if (GUILayout.Button(log.detailInfo, logButtonStyle)) selectedContents = log;
                }
            }
            else
            {
                if (log == selectedContents) GUILayout.Label(log.detailInfo, selectedLogFontStyle);
                else {
                    if (GUILayout.Button(log.detailInfo, logButtonStyle)) selectedContents = log;
                }
            }
            GUILayout.EndHorizontal();
            index++;
        }

        int afterHeight = (int)((totalCount - (startIndex + totalVisibleCount)) * size.y);
        if (afterHeight > 0)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(afterHeight));
            GUILayout.Label(" ");
            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
        GUILayout.EndArea();

        buttomRect.x = 0f;
        buttomRect.y = Screen.height - size.y;
        buttomRect.width = Screen.width;
        buttomRect.height = size.y;

        DrawDetails();
    }

    void DrawDetails()
    {
        if (selectedContents != null)
        {
            Vector2 drag = GetDrag();
            if (drag.y != 0 && detailsRect.Contains(new Vector2(downPos.x, Screen.height - downPos.y)))
            {
                scrollPosition2.y += drag.y - oldDrag2;
            }
            oldDrag2 = drag.y;

            GUILayout.BeginArea(detailsRect, backStyle);
            scrollPosition2 = GUILayout.BeginScrollView(scrollPosition2);

            GUILayout.BeginHorizontal();
            GUILayout.Label(selectedContents.detailInfo, detailsLabelStyle);
            GUILayout.EndHorizontal();

            GUILayout.Space(size.y * 0.25f);
            GUILayout.Space(size.y);

            GUILayout.EndScrollView();
            GUILayout.EndArea();

            if(currentView == ReportView.Texture)
            {
                Texture thumNailTexture = TextureObj.FindTexture(selectedContents.name);

                GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height * 0.75f, Screen.height - detailsRect.y, Screen.height - detailsRect.y));
                if (GUILayout.Button(thumNailTexture, nonStyle, GUILayout.Width(Screen.height - detailsRect.y), GUILayout.Height(Screen.height - detailsRect.y))) { }
                GUILayout.EndArea();

                thumNailTexture = null;
            }
            else if (currentView == ReportView.Material)
            {
                Texture thumNailTexture = TextureObj.FindTexture(selectedContents.name);
                if (thumNailTexture != null)
                {
                    GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height * 0.75f, Screen.height - detailsRect.y, Screen.height - detailsRect.y));
                    if (GUILayout.Button(thumNailTexture, nonStyle, GUILayout.Width(Screen.height - detailsRect.y), GUILayout.Height(Screen.height - detailsRect.y))) { }
                    GUILayout.EndArea();
                }
                thumNailTexture = null;
            }
            else if (currentView == ReportView.Mesh)
            {
                /*Thumnail Mesh Draw*/

                //Mesh mesh = FindMesh(selectedLog.name);
                //if(mesh != null)
                //{
                //    //MeshFilter mf = GetComponent<MeshFilter>();
                  
                    
                //    if(cameraObj == null)
                //    {
                //        cameraObj = new GameObject();
                //        cameraObj.AddComponent<Camera>();
                //        //cameraObj.transform.position = new Vector3(0.0f, 0.0f, -30.0f);
                //        cameraObj.GetComponent<Camera>().orthographic = true;
                //        //cameraObj.AddComponent<Light>();
                //        //cameraObj.light.type = LightType.Directional;
                //    }

                //    if (this.gameObject.GetComponent<MeshFilter>() == null)
                //    {
                //        meshFilter = this.gameObject.AddComponent<MeshFilter>();
                //        this.gameObject.AddComponent<MeshRenderer>();  //MeshRendererÄÄÆ÷³ÍÆ® Ãß°¡
                //        this.gameObject.GetComponent<MeshRenderer>().useLightProbes = true;

                //    }

                //    meshFilter.mesh = mesh;

                //    this.gameObject.renderer.material.color = Color.gray;


                //    PositionInView(meshFilter, cameraObj.GetComponent<Camera>());

                //    if (rdTex == null)
                //    {
                //        rdTex = new RenderTexture(128, 128, 16, RenderTextureFormat.ARGB32);
                //    }
                                   
                    
                //    cameraObj.GetComponent<Camera>().targetTexture = rdTex;
                //   // cameraObj.GetComponent<Camera>().Render();
                //    //RenderTexture.active = rdTex;
                //    //screenShot = GetRTPixels(cameraObj.GetComponent<Camera>().targetTexture);
                //    //screenShot.ReadPixels(new Rect(0, 0, 256, 256), 0, 0);

                   

                //    GUILayout.BeginArea(new Rect(Screen.width / 2, Screen.height * 0.75f, Screen.height - detailsRect.y, Screen.height - detailsRect.y));

                //    //Texture[] allTextures = Resources.LoadAll<Texture>(selectedLog.name);
                //    // GUIContent temp = new GUIContent("", thumNailTexture);
                //    if (GUILayout.Button(cameraObj.GetComponent<Camera>().targetTexture, nonStyle, GUILayout.Width(Screen.height - detailsRect.y), GUILayout.Height(Screen.height - detailsRect.y)))
                //    {

                //    }
                //    GUILayout.EndArea();
                //    //cameraObj.GetComponent<Camera>().targetTexture = null;
                //    //mf.mesh = mesh;
                //}

                //mesh = null;
            }
        }
        else
        {
            GUILayout.BeginArea(detailsRect, backStyle);
            GUILayout.EndArea();
        }

    }
   
    public void OnGUIDraw()
    {
        if (!show) return;

        screenRect.x = 0;
        screenRect.y = 0;
        screenRect.width = Screen.width;
        screenRect.height = Screen.height;

        GetDownPos();

        logsRect.x = 0f;
        logsRect.y = size.y * 2f;
        logsRect.width = Screen.width;
        logsRect.height = Screen.height * 0.75f - size.y * 2f;

        detailsRectTopLeft.x = 0f;
        detailsRect.x = 0f;
        detailsRectTopLeft.y = Screen.height * 0.75f;
        detailsRect.y = Screen.height * 0.75f;
        detailsRect.width = Screen.width;
        detailsRect.height = Screen.height * 0.25f;

        DrawToolBar();
        DrawContents();

    }

    List<Vector2> gestureDetector = new List<Vector2>();
    Vector2 gestureSum = Vector2.zero;
    float gestureLength = 0;
    int gestureCount = 0;

    bool IsGestureDone()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1)
            {
                gestureDetector.Clear();
                gestureCount = 0;
            }
            else
            {
                if (Input.touches[0].phase == TouchPhase.Canceled || Input.touches[0].phase == TouchPhase.Ended)
                    gestureDetector.Clear();
                else if (Input.touches[0].phase == TouchPhase.Moved)
                {
                    Vector2 p = Input.touches[0].position;
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }
        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                gestureDetector.Clear();
                gestureCount = 0;
            }
            else
            {
                if (Input.GetMouseButton(0))
                {
                    Vector2 p = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    if (gestureDetector.Count == 0 || (p - gestureDetector[gestureDetector.Count - 1]).magnitude > 10)
                        gestureDetector.Add(p);
                }
            }
        }

        if (gestureDetector.Count < 10)
            return false;

        gestureSum = Vector2.zero;
        gestureLength = 0;
        Vector2 prevDelta = Vector2.zero;
        for (int i = 0; i < gestureDetector.Count - 2; i++)
        {

            Vector2 delta = gestureDetector[i + 1] - gestureDetector[i];
            float deltaLength = delta.magnitude;
            gestureSum += delta;
            gestureLength += deltaLength;

            float dot = Vector2.Dot(delta, prevDelta);
            if (dot < 0f)
            {
                gestureDetector.Clear();
                gestureCount = 0;
                return false;
            }

            prevDelta = delta;
        }

        int gestureBase = (Screen.width + Screen.height) / 4;

        if (gestureLength > gestureBase && gestureSum.magnitude < gestureBase / 2)
        {
            gestureDetector.Clear();
            gestureCount++;
            if (gestureCount >= numOfCircleToShow)
                return true;
        }

        return false;
    }

    Vector2 downPos;
    Vector2 GetDownPos()
    {
        if (Application.platform == RuntimePlatform.Android ||
           Application.platform == RuntimePlatform.IPhonePlayer)
        {

            if (Input.touches.Length == 1 && Input.touches[0].phase == TouchPhase.Began)
            {
                downPos = Input.touches[0].position;
                return downPos;
            }
        }
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                downPos.x = Input.mousePosition.x;
                downPos.y = Input.mousePosition.y;
                return downPos;
            }
        }

        return Vector2.zero;
    }
    
    Vector2 mousePosition;
    Vector2 GetDrag()
    {

        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touches.Length != 1) return Vector2.zero;
            return Input.touches[0].position - downPos;
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                mousePosition = Input.mousePosition;
                return mousePosition - downPos;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }


    void CalculateStartIndex()
    {
        startIndex = (int)(scrollPosition.y / size.y);
        startIndex = Mathf.Clamp(startIndex, 0, currentObjectContents.Count);
    }

    void DoShow()
    {
        show = true;
        gameObject.AddComponent<ReporterGUI>();

        try
        {
            gameObject.SendMessage("OnShowReporter");
        }
        catch (System.Exception e)
        {
            Debug.LogException(e);
        }
    }
    void Update()
    {
        CalculateStartIndex();

        if (!show && IsGestureDone())
        {
            DoShow();
        }

    }

    private void ListLoadedTexture()
    {
        var allTextures = (IEnumerable<Texture>)Resources.FindObjectsOfTypeAll(typeof(Texture));
        allTextures = allTextures.Where(texture => !_ignoreTextures.Contains(texture.name));

        newScanedTexture = TextureObj.GetNewObjectInstanceIdList(allTextures);

        removedTexture = TextureObj.GetRemovedItems(oldScanedTexture, newScanedTexture); 
        remainedTexture = TextureObj.GetRemainingItems(oldScanedTexture, newScanedTexture); 
        newTexture = TextureObj.GetNewItems(oldScanedTexture, newScanedTexture); 

        realNewTexture = TextureObj.GetRealNewTexture(newTexture, removedTexture);
        realReamovedTexture = TextureObj.GetRealRemoveTextue(newTexture, removedTexture);
        realRemainedTexture = TextureObj.GetRealRemainItems(remainedTexture);
        realAllTexture = TextureObj.GetRealAllItems(newScanedTexture);

        /*´©¼ö Å½Áö*/
        //TextureObj.CheckDublicateValue(realAllTexture);
        //TextureObj.CheckLeak(realAllTexture);

        oldScanedTexture.Clear();
        foreach (KeyValuePair<int, TextureDetails> pair in newScanedTexture)
        {
            oldScanedTexture.Add(pair.Key, pair.Value);
        }
        newScanedTexture.Clear();

        removedTexture.Clear();
        remainedTexture.Clear();
        newTexture.Clear();
    }

    private void ListLoadedMaterail()
    {
        var allMaterial = (IEnumerable<Material>)Resources.FindObjectsOfTypeAll(typeof(Material));
        allMaterial = allMaterial.Where(materials => !_ignoreMaterials.Contains(materials.name));

        newScanedMaterial = MaterialObj.GetNewObjectInstanceIdList2(allMaterial);
            
        removedMaterial = MaterialObj.GetRemovedItems(oldScanedMaterial, newScanedMaterial); 
        remainedMaterial = MaterialObj.GetRemainingItems(oldScanedMaterial, newScanedMaterial); 
        newMaterial = MaterialObj.GetNewItems(oldScanedMaterial, newScanedMaterial); 

        realNewMaterial = MaterialObj.GetRealNewMaterial(newMaterial, removedMaterial);
        realReamovedMaterial = MaterialObj.GetRealRemoveMaterial(newMaterial, removedMaterial);
        realRemainedMaterial = MaterialObj.GetRealRemainItems(remainedMaterial);
        realAllMaterial = MaterialObj.GetRealAllItems(newScanedMaterial);

        oldScanedMaterial.Clear();
        foreach (KeyValuePair<int, MaterialDetails> pair in newScanedMaterial)
        {
            oldScanedMaterial.Add(pair.Key, pair.Value);
        }
        newScanedMaterial.Clear();

        removedMaterial.Clear();
        remainedMaterial.Clear();
        newMaterial.Clear();

    }
    private void ListLoadedMesh()
    {
        var allMesh = (IEnumerable<Mesh>)Resources.FindObjectsOfTypeAll(typeof(Mesh));
        allMesh = allMesh.Where(meshs => !_ignoreMesh.Contains(meshs.name));

        newScanedMesh = MeshObj.GetNewObjectInstanceIdList(allMesh);

        removedMesh = MeshObj.GetRemovedItems(oldScanedMesh, newScanedMesh); 
        remainedMesh = MeshObj.GetRemainingItems(oldScanedMesh, newScanedMesh);
        newMesh = MeshObj.GetNewItems(oldScanedMesh, newScanedMesh); 

        realNewMesh = MeshObj.GetRealNewMesh(newMesh, removedMesh);
        realReamovedMesh = MeshObj.GetRealRemoveMesh(newMesh, removedMesh);
        realRemainedMesh = MeshObj.GetRealRemainItems(remainedMesh);
        realAllMesh = MeshObj.GetRealAllItems(newScanedMesh);

        oldScanedMesh.Clear();
        foreach (KeyValuePair<int, MeshDetails> pair in newScanedMesh)
        {
            oldScanedMesh.Add(pair.Key, pair.Value);
        }
        newScanedMesh.Clear();

        removedMesh.Clear();
        remainedMesh.Clear();
        newMesh.Clear();
    }
  

    private void ListLoadedAudio()
    {
        var allAudio = (IEnumerable<AudioClip>)Resources.FindObjectsOfTypeAll(typeof(AudioClip));
        allAudio = allAudio.Where(audio => !_ignoreAudio.Contains(audio.name));

        newScanedAudio = AudioObj.GetNewObjectInstanceIdList(allAudio);

        removedAudio = AudioObj.GetRemovedItems(oldScanedAudio, newScanedAudio); 
        remainedAudio = AudioObj.GetRemainingItems(oldScanedAudio, newScanedAudio); 
        newAudio = AudioObj.GetNewItems(oldScanedAudio, newScanedAudio); 

        realNewAudio = AudioObj.GetRealNewAudio(newAudio, removedAudio);
        realReamovedAudio = AudioObj.GetRealRemoveAudio(newAudio, removedAudio);
        realRemainedAudio = AudioObj.GetRealRemainItems(remainedAudio);
        realAllAudio = AudioObj.GetRealAllItems(newScanedAudio);

        //AudioObj.CheckDublicateValue(realAllAudio);
        //AudioObj.CheckLeak(realAllAudio);

        oldScanedAudio.Clear();
        foreach (KeyValuePair<int, AudioDetails> pair in newScanedAudio)
        {
            oldScanedAudio.Add(pair.Key, pair.Value);
        }
        newScanedAudio.Clear();

        removedAudio.Clear();
        remainedAudio.Clear();
        newAudio.Clear();
    }
    private void Clear()
    {
        realNewTexture.Clear();
        realRemainedTexture.Clear();
        realReamovedTexture.Clear();
        realAllTexture.Clear();

        realNewMaterial.Clear();
        realRemainedMaterial.Clear();
        realReamovedMaterial.Clear();
        realAllMaterial.Clear();

        realNewMesh.Clear();
        realRemainedMesh.Clear();
        realReamovedMesh.Clear();
        realAllMesh.Clear();

        realNewAudio.Clear();
        realRemainedAudio.Clear();
        realReamovedAudio.Clear();
        realAllAudio.Clear();

    }
    void SetTextureContents(Dictionary<int, TextureDetails> setItem)
    {
        currentObjectContents.Clear();

        var sortedDic = (from dic in setItem orderby dic.Value.objectName ascending select dic);
        foreach (var item in sortedDic)
        {
            ObjectContents temp = new ObjectContents();
            temp.detailInfo = TextureObj.GetLogContent(item.Value);
            temp.name = item.Value.objectName;

            if (item.Value.dublicate) temp.logType = MemoryType.Dubliaction;
            else if (item.Value.leak) temp.logType = MemoryType.Leak;
            else temp.logType = MemoryType.Normal;

            currentObjectContents.Add(temp);
        }
    }

   
    void SetMaterialContents(Dictionary<int, MaterialDetails> setItem)
    {
        currentObjectContents.Clear();

        var sortedDic = (from dic in setItem orderby dic.Value.objectName ascending select dic);
        foreach (var item in sortedDic)
        {
            ObjectContents temp = new ObjectContents();

            temp.detailInfo = MaterialObj.getLogContent(item.Value);
            temp.name = item.Value.mainTextureName;
            currentObjectContents.Add(temp);
        }
    }

    void SetMeshContents(Dictionary<int, MeshDetails> setItem)
    {
        currentObjectContents.Clear();

        var sortedDic = (from dic in setItem orderby dic.Value.objectName ascending select dic);
        foreach (var item in sortedDic)
        {
            ObjectContents temp = new ObjectContents();

            temp.detailInfo = MeshObj.getLogContent(item.Value);
            temp.name = item.Value.objectName;
            currentObjectContents.Add(temp);
        }
    }

   

    void SetAudioContents(Dictionary<int, AudioDetails> setItem)
    {
        currentObjectContents.Clear();

        var sortedDic = (from dic in setItem orderby dic.Value.objectName ascending select dic);
        foreach (var item in sortedDic)
        {
            ObjectContents temp = new ObjectContents();

            temp.detailInfo = AudioObj.getLogContent(item.Value);
            temp.name = item.Value.objectName;
            currentObjectContents.Add(temp);
        }
    }
  
    void OnGUI()
    {
        GameObject UIobj = GameObject.Find("UI");
        if (UIobj != null && !show)
        {
            if (GUI.Button(new Rect(0, Screen.height - 50, Screen.width / 4, 50), "Snapshot"))
            {
                Clear();

                ListLoadedTexture();
                ListLoadedMaterail();
                ListLoadedMesh();
                ListLoadedAudio();

                DrawDetailVewCount();
                DrawDetailsView();
               
            }
        }
    }
}


