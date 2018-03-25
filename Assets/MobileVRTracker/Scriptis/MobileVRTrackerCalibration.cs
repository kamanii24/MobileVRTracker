// =================================
//
//	MobileVRTrackerCalibration.cs
//	Created by Takuya Himeji
//
// =================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.iOS;

// キャリブレーションの更新データ管理クラス
[System.Serializable]
public class CalibrationData
{
    public RaycastHit hitPlaneInfo;
    public float height;
}

// キャリブレーションの更新イベント
[System.Serializable]
public class CalibrationUpdateEvent : UnityEvent<CalibrationData> { }

public class MobileVRTrackerCalibration : MonoBehaviour
{
    #region Inspector Settings
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private ParticleSystem pointCloudParticlePrefab;
    [SerializeField] private int maxPointsToShow = 10000;
    [SerializeField] private float particleSize = 0.01f;
	// UnityEvents
    public CalibrationUpdateEvent onCalibrationUpdate;
    #endregion // Inspector Settings

    #region Member Field
    private MobileVRTracker vrTacker;
    private CalibrationData calibData;
    private UnityARAnchorManager anchorManager = null;
    private Vector3[] pointCloudData;
    private ParticleSystem particleBase;
    private ParticleSystem.Particle[] particles;
    private RaycastHit hit;
    private bool isHit = false;
    private bool frameUpdated = false;
    private bool calibrated = false;
    #endregion // Member Field


    #region MonoBehaviour Methods
    private void Awake()
    {

    }

    private void Start()
    {
        // GvrTrackerインスタンス取得
        vrTacker = MobileVRTracker.Instance;
        calibData = new CalibrationData();

        // PointCloudパーティクル初期化
        particleBase = Instantiate(pointCloudParticlePrefab);
        frameUpdated = false;
        UnityARSessionNativeInterface.ARFrameUpdatedEvent += ARFrameUpdated;

        // Plane生成用Anchor
        anchorManager = new UnityARAnchorManager();
        UnityARUtility.InitializePlanePrefab(planePrefab);
    }

    private void Update()
    {
        if (calibrated) return;

		// 生成されたPlaneに対するRaycast処理 --------
        Vector3 fwd = vrTacker.TrackingCamera.TransformDirection(Vector3.forward);
        isHit = Physics.Raycast(vrTacker.TrackingCamera.position, fwd, out hit, 100);
        if (isHit)
        {
			// データ更新
            calibData.hitPlaneInfo = hit;
            calibData.height = vrTacker.TrackingCamera.position.y - hit.point.y;
            onCalibrationUpdate.Invoke(calibData);	// イベント通知
        }
        else
        {
            // Planeから視点が外れた場合はリセット
            calibData.hitPlaneInfo = default(RaycastHit);
            calibData.height = 0f;
            onCalibrationUpdate.Invoke(null);		// イベント通知
        }


        // PointCloudParticleの更新 --------
        if (frameUpdated)
        {
            if (pointCloudData != null && pointCloudData.Length > 0)
            {
                int numParticles = Mathf.Min(pointCloudData.Length, maxPointsToShow);
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[numParticles];
                int index = 0;
                foreach (Vector3 currentPoint in pointCloudData)
                {
                    particles[index].position = currentPoint;
                    particles[index].startColor = new Color(1.0f, 1.0f, 1.0f);
                    particles[index].startSize = particleSize;
                    index++;
                }
                particleBase.SetParticles(particles, numParticles);
            }
            else
            {
                ParticleSystem.Particle[] particles = new ParticleSystem.Particle[1];
                particles[0].startSize = 0.0f;
                particleBase.SetParticles(particles, 1);
            }
            frameUpdated = false;
        }
    }

    private void OnDestroy()
    {
        // delegate解除
        UnityARSessionNativeInterface.ARFrameUpdatedEvent -= ARFrameUpdated;
        // AnchorManager破棄
        if (anchorManager != null)
            anchorManager.Destroy();
    }
    #endregion // MonoBehaviour Methods


    #region Member Methods
	// ARKitによるフレーム更新処理
    private void ARFrameUpdated(UnityARCamera camera)
    {
        // PointCloud情報の取得
        pointCloudData = camera.pointCloudData;
        frameUpdated = true;
    }

    // 目線の高さを設定する
    public void SetEyeHeight()
    {
        // 目線位置の確定
        vrTacker.EyeHeight = vrTacker.TrackingCamera.parent.position.y - hit.point.y;
		calibrated = true;          // キャリブレーション完了
        anchorManager.Destroy();    // 生成したPlaneを全て削除する
    }
    #endregion // Member Methods
}