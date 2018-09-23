using System.Collections;
using System.Collections.Generic;
using Bunkasai2018;
using UnityEngine;
using WiimoteApi;

namespace Bunkasai2018 {
    public class GunMechanism : MonoBehaviour {

        [SerializeField] private GameObject Display2ManagerObject;
        [SerializeField] private GameObject[] targetObjects;
        [SerializeField] private Camera orthoCamera;
        [SerializeField] private GameObject[] bulletPrefabs;
        [SerializeField] private float gunCooldownTime = 0.05f;
        private AudioSource gunshotSoundSource;
        private bool[] bButtonPressedTable = { false, false, false, false };
        private float[] timeChargeTable = { 0, 0, 0, 0 };
        private float[] timeReleaseTable = { 0, 0, 0, 0 };

        private Vector3 touchWorldPosition;
        private Vector3 touchScreenPosition;

        // Use this for initialization
        void Start () {
            gunshotSoundSource = GetComponents<AudioSource> () [0];
            touchWorldPosition = new Vector3 (0, 0, 0);
            touchScreenPosition = new Vector3 (0, 0, 10);
        }

        // Update is called once per frame
        void Update () {
            if (GameManager.debugMode) {
                Vector3 touchWorldPosition = orthoCamera.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10.0f));
                targetObjects[3].transform.position = touchWorldPosition;
                if (bButtonPressedTable[3] == false && Input.GetMouseButton (0) == true) {
                    if (Time.time > gunCooldownTime + timeReleaseTable[3]) {
                        StartCharge (3);
                    }
                } else if (bButtonPressedTable[3] == true && Input.GetMouseButton (0) == false) {
                    FireGunMouse (touchWorldPosition, 3, Mathf.Clamp (1 + (Time.time - timeChargeTable[3]) * 2.0f, 1, 5));
                } else if (bButtonPressedTable[3] == true && Input.GetMouseButton (0) == true) {
                    targetObjects[3].transform.localScale = Vector3.one * Mathf.Clamp (1 + (Time.time - timeChargeTable[3]) * 2.0f, 1, 5) / 10;
                }
            }

            if (Input.GetKey (KeyCode.A)) {
                WiimoteManager.FindWiimotes ();
            }

            if (Input.GetKey (KeyCode.B)) {
                foreach (Wiimote mote in WiimoteManager.Wiimotes) {
                    mote.SetupIRCamera (IRDataType.EXTENDED);
                    mote.RequestIdentifyWiiMotionPlus ();
                    mote.Accel.CalibrateAccel (AccelCalibrationStep.A_BUTTON_UP);
                }
            }

            if (!WiimoteManager.HasWiimote ()) { return; }

            /*if (Input.GetKeyDown ("0")) {
                foreach (Wiimote mote in WiimoteManager.Wiimotes) {
                    WiimoteManager.Cleanup (mote);
                }
            }*/

            /*
            int connectedWiimotes = 0;
            //接続してるwiiリモコンのhidapi_pathと設定したwiiリモコンのhidapi_pathを比較して設定した方が全て接続してるかどうか確かめる
            foreach (Wiimote mote in WiimoteManager.Wiimotes) {
                if (SettingsManager.wiimotePathTable.ContainsKey (mote.hidapi_path)) {
                    if (SettingsManager.wiimotePathTable[mote.hidapi_path] != 0) {
                        connectedWiimotes++;
                        int ret;
                        do {
                            ret = mote.ReadWiimoteData ();
                        } while (ret > 0);

                        Vector3 touchScreenPosition = new Vector3 ();
                        touchScreenPosition.x = mote.Ir.GetPointingPosition () [0] * Screen.width; //Math.Clamp?
                        touchScreenPosition.y = (mote.Ir.GetPointingPosition () [1] - 0.5f) * Screen.height;
                        touchScreenPosition.z = 11.0f;

                        Vector3 touchWorldPosition = gameCamera.ScreenToWorldPoint (touchScreenPosition);
                        targetObject[SettingsManager.wiimotePathTable[mote.hidapi_path] - 1].transform.position = touchWorldPosition;
                        if (bButtonPressedTable[SettingsManager.wiimotePathTable[mote.hidapi_path] - 1] == false && mote.Button.b == true) {
                            FireGun (touchWorldPosition, mote);
                            bButtonPressedTable[SettingsManager.wiimotePathTable[mote.hidapi_path] - 1] = true;
                        }
                        if (mote.Button.b == false) {
                            mote.RumbleOn = false;
                            bButtonPressedTable[SettingsManager.wiimotePathTable[mote.hidapi_path] - 1] = false;
                        }
                    }
                } else {

                }
            }

            if (connectedWiimotes < 4) {

            }*/
            //Debug.Log (WiimoteManager.Wiimotes.Count);

            for (int i = 0; i < WiimoteManager.Wiimotes.Count && i < targetObjects.Length; i++) {
                Wiimote mote = WiimoteManager.Wiimotes[i];
                int ret;
                do {
                    ret = mote.ReadWiimoteData ();
                } while (ret > 0);

                touchScreenPosition.x = mote.Ir.GetPointingPosition () [0] * Screen.width;
                touchScreenPosition.y = mote.Ir.GetPointingPosition () [1] * Screen.height;
                touchWorldPosition = orthoCamera.ScreenToWorldPoint (touchScreenPosition);
                targetObjects[i].transform.position = touchWorldPosition;
                if (bButtonPressedTable[i] == false && mote.Button.b == true) {
                    if (Time.time > gunCooldownTime + timeReleaseTable[i]) {
                        StartCharge (i);
                    }
                } else if (bButtonPressedTable[i] == true && mote.Button.b == false) {
                    FireGun (touchWorldPosition, mote, i, Mathf.Clamp (1 + (Time.time - timeChargeTable[i]) * 2.0f, 1, 5));
                } else if (bButtonPressedTable[i] == true && mote.Button.b == true) {
                    targetObjects[i].transform.localScale = Vector3.one * Mathf.Clamp (1 + (Time.time - timeChargeTable[i]) * 2.0f, 1, 5) / 10;
                }
            }
        }

        void OnApplicationQuit () {
            foreach (Wiimote mote in WiimoteManager.Wiimotes) {
                WiimoteManager.Cleanup (mote);
            }
        }

        void StartCharge (int number) {
            bButtonPressedTable[number] = true;
            timeChargeTable[number] = Time.time;
            //mote.RumbleOn = true;
            //mote.SendStatusInfoRequest ();
        }

        void FireGun (Vector3 worldPosition, Wiimote mote, int number, float strength) {
            GameObject playerBullet = Instantiate (bulletPrefabs[number], orthoCamera.transform.position + (worldPosition - orthoCamera.transform.position).normalized, Quaternion.identity);
            Bullet b = playerBullet.GetComponent<Bullet> ();
            b.targetDirection = (worldPosition - orthoCamera.transform.position).normalized * ((5f - strength) / 7 + 0.3f);
            b.strength = strength;
            b.owner = number;
            b.display2Manager = Display2ManagerObject.GetComponent<Display2Manager> ();
            Destroy (playerBullet, 3f);
            gunshotSoundSource.PlayOneShot (gunshotSoundSource.clip);
            bButtonPressedTable[number] = false;
            timeReleaseTable[number] = Time.time;
            targetObjects[number].transform.localScale = Vector3.one / 10;
            //mote.RumbleOn = false;
            //mote.SendStatusInfoRequest ();
        }

        void FireGunMouse (Vector3 worldPosition, int number, float strength) {
            GameObject playerBullet = Instantiate (bulletPrefabs[number], orthoCamera.transform.position + (worldPosition - orthoCamera.transform.position).normalized, Quaternion.identity);
            Bullet b = playerBullet.GetComponent<Bullet> ();
            b.targetDirection = (worldPosition - orthoCamera.transform.position).normalized * ((5f - strength) / 7 + 0.3f);
            b.strength = strength;
            b.owner = number;
            b.display2Manager = Display2ManagerObject.GetComponent<Display2Manager> ();
            Destroy (playerBullet, 3f);
            gunshotSoundSource.PlayOneShot (gunshotSoundSource.clip);
            bButtonPressedTable[number] = false;
            timeReleaseTable[number] = Time.time;
            targetObjects[number].transform.localScale = Vector3.one / 10;
        }

    }
}