// =================================
//
//	Calibration.cs
//	Created by Takuya Himeji
//
// =================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Calibration : MonoBehaviour
{
    #region Inspector Settings
    public string nextScene = "";
    [SerializeField] private MobileVRTrackerCalibration gvrTrackerCalibration;
    [SerializeField] private Transform floorObj;
    [SerializeField] private Transform mark;
    [SerializeField] private Image circleImage;
    [SerializeField] private Text heightText;
    [SerializeField] private Text noticeText;
    [SerializeField] private ParticleSystem effect;
    #endregion // Inspector Settings


    #region Member Field
    private float targetWaitTime = 1.5f;
    private float waitTimer = 0f;
    #endregion // Member Field


    #region MonoBehaviour Methods

    private void Awake()
    {

    }

    private void Start()
    {

    }

    private void Update()
    {

    }

    #endregion // MonoBehaviour Methods


    #region Member Methods
    public void CalibrationUpdate(CalibrationData data)
    {
        if (data != null)
        {
            if (!mark.gameObject.activeSelf)
            {
                mark.gameObject.SetActive(true);
            }
            mark.position = data.hitPlaneInfo.point;
            heightText.text = (int)(data.height * 100f) + "cm";


            if (waitTimer < targetWaitTime)
            {
                waitTimer += Time.deltaTime;
            }
            else
            {
                if (circleImage.fillAmount < 1f)
                {
                    circleImage.fillAmount += Time.deltaTime * 0.5f;
                }
                else
                {
                    SetFloor(data);
                    return;
                }
            }
        }
        else if (mark.gameObject.activeSelf)
        {
            // Planeから視線が外れた場合、初期化
            waitTimer = 0f;
            circleImage.fillAmount = 0f;
            mark.gameObject.SetActive(false);
        }
    }

    private void SetFloor(CalibrationData data)
    {
        // 目線の高さを設定
        gvrTrackerCalibration.SetEyeHeight();
        
        effect.Play();
        noticeText.text = "キャリブレーションが完了しました。\nそのまましばらくお待ちください。";

        floorObj.gameObject.SetActive(true);
        floorObj.position = new Vector3(floorObj.position.x, data.hitPlaneInfo.point.y, floorObj.position.z);

        mark.gameObject.SetActive(false);

        // 5秒待機後、シーン遷移
        StartCoroutine(WaitTimeCoroutine(5f, () =>
        {
            SceneManager.LoadScene(nextScene);
        }));
    }

    private IEnumerator WaitTimeCoroutine(float time, UnityEngine.Events.UnityAction callback)
    {
        // 待機
        yield return new WaitForSeconds(time);
        // コールバック
        callback();
    }
    #endregion // Member Methods
}