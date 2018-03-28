# MobileVRTracker(Beta)
ARKitを使ったモバイルVRポジショントラッキングを可能にしたスクリプトです。

# 使い方
#### 初期設定
UnityARKitPluginが別途必要です。<br>
https://bitbucket.org/Unity-Technologies/unity-arkit-plugin<br>
**※現在ARKitバージョン1.5が公開されていますが、当スクリプトは1.5に対応していません。旧バージョンをご利用ください。**
<br>

#### シーン構成
- **Calibration**<br>
ポジショントラッキングのキャリブレーションを行うシーンです。<br>
自身が立っている床面を認識させることで、VR空間上でも自分の目線と同じ高さでプレイすることが可能です。<br>

- **WalkingDemo**<br>
実際にポジショントラッキングを使用して空間内を移動できるデモです。<br>
このシーンへはCalibrationシーンで正確な目線の高さを設定してから遷移されることを想定されているため、WalkingDemoシーン単体では正常にトラッキングされないことに注意してください。<br>
WalkingDemoシーン単体でポジショントラッキングを行うためには、*Prefabs/VRMobileVRTracker*プレハブをシーンへ追加し、トラッキング対象のCameraをInspectorへ設定します。ただし、この場合はキャリブレーションを行わないため実際の目線の高さとは異なり、シーン内に配置してある*CameraRig*の位置からトラッキングが開始されます。<br>

[![IMAGE ALT TEXT HERE](http://img.youtube.com/vi/RCv8Dw0A0wA/0.jpg)](http://www.youtube.com/watch?v=RCv8Dw0A0wA)
<br>


## 対応デバイス
ARKitに対応したiPhoneのみ。
<br>


## ビルド環境<br>
Unity 2017.3.1f1<br>
macOS High Sierra 10.13.3
