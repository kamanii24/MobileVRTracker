// =================================
//
//	MobileVRTracker.cs
//	Created by Takuya Himeji
//
// =================================

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;
using UnityEngine.XR.iOS;

public class MobileVRTracker : MonoBehaviour
{
    #region Singleton
    private static MobileVRTracker instance;

    public static MobileVRTracker Instance
    {
        get { return instance; }
    }
    #endregion // Singleton


    #region Inspector Settings
    [SerializeField] private Camera trackingCamera;  // 対象のカメラ
    #endregion // Inspector Settings


    #region Member Field
    private UnityARSessionNativeInterface session = null;
    private ARKitWorldTrackingSessionConfiguration config;
    private float eyeHeight = 0f;   // 床からの高さ(目線)
    #endregion // Member Field


    #region Properties
    public Transform TrackingCamera
    {
        get { return trackingCamera.transform; }
    }
    public float EyeHeight
    {
        get { return eyeHeight; }
        set { eyeHeight = value; }
    }
    #endregion // Properties


    #region MonoBehaviour Methods
    private void Awake()
    {
        instance = this;                    // インスタンス取得
        Application.targetFrameRate = 60;   // fpsセット
        DontDestroyOnLoad(gameObject);      // 永続化

        // ARKitセッションの初期化 ----
        session = UnityARSessionNativeInterface.GetARSessionNativeInterface();
        ARKitWorldTrackingSessionConfiguration config = new ARKitWorldTrackingSessionConfiguration();
        config.planeDetection = UnityARPlaneDetection.Horizontal;
        config.alignment = UnityARAlignment.UnityARAlignmentGravity;
        config.getPointCloudData = true;
        config.enableLightEstimation = false;
        session.RunWithConfig(config);  // セッション実行

        // Cameraがなければ取得
        if(trackingCamera == null) trackingCamera = Camera.main;
        // Cardboardの自動ヘッドトラッキング停止
        UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(trackingCamera, true);
        // シーン遷移時のイベント登録
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Update()
    {
        if (trackingCamera != null)
        {
            // 姿勢更新
            Matrix4x4 matrix = session.GetCameraPose();
            trackingCamera.transform.localPosition = UnityARMatrixOps.GetPosition(matrix);
            trackingCamera.transform.localRotation = UnityARMatrixOps.GetRotation(matrix);
        }
    }
    #endregion // MonoBehaviour Methodss


    #region Member Methods
    // シーン遷移時にコール
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(trackingCamera == null)
        {
            // カメラの更新(シーン遷移などでカメラが剥がれた時の処理)
            trackingCamera = Camera.main;    // Camera取得
            trackingCamera.transform.parent.localPosition = new Vector3(0f, eyeHeight, 0f);  // 目線の高さ設定
            // Cardboardの自動ヘッドトラッキング停止
            UnityEngine.XR.XRDevice.DisableAutoXRCameraTracking(trackingCamera, true);
        }
    }
    #endregion // Member Methods
}