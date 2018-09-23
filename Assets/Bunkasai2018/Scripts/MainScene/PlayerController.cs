using System;
using Bunkasai2018;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace Bunkasai2018 {
    [RequireComponent (typeof (AudioListener))]
    public class PlayerController : MonoBehaviour {
        [SerializeField] private GameObject gameManagerObject;
        [SerializeField] public GameObject actionPointGroup;
        [SerializeField] private Camera frontCam;
        [SerializeField] private GameObject spotlight1;
        [SerializeField] private GameObject spotlight2;
        [SerializeField] public AnimationCurve lightCurve;
        [SerializeField] private int maxHp = 100;
        [HideInInspector] public bool isRunning = false;
        [SerializeField] private float walkSpeed = 5;
        [SerializeField] private float runSpeed = 10;
        [SerializeField] private bool gameOverEnabled = false;
        private int HP;
        private float speed;
        //private AudioSource m_AudioSource;
        public int actionPointIdNow = -1;

        public Vector3[] actionPointPositions;
        private Vector3[] actionPointRotations;
        private bool[] actionPointSlowCamera;
        private bool[] actionPointLinearMove;
        private bool[] actionPointLightFlicker;

        private AudioSource attackedSoundSource;

        // Use this for initialization
        void Start () {
            if (isRunning) {
                speed = runSpeed;
            } else {
                speed = walkSpeed;
            }

            HP = maxHp;
            int childNum = actionPointGroup.transform.childCount;

            actionPointPositions = new Vector3[childNum];
            actionPointRotations = new Vector3[childNum];
            actionPointSlowCamera = new bool[childNum];
            actionPointLinearMove = new bool[childNum];
            actionPointLightFlicker = new bool[childNum];

            for (int i = 0; i < childNum; i++) {
                actionPointPositions[i] = actionPointGroup.transform.GetChild (i).transform.position;
                actionPointRotations[i] = actionPointGroup.transform.GetChild (i).transform.rotation.eulerAngles;
                ActionPoint point = actionPointGroup.transform.GetChild (i).GetComponent<ActionPoint> ();
                actionPointSlowCamera[i] = point.slowCamera;
                actionPointLinearMove[i] = point.linearMove;
                actionPointLightFlicker[i] = point.lightFlicker;
            }

            attackedSoundSource = GetComponent<AudioSource> ();
        }

        // Update is called once per frame
        void Update () { }

        void OnTriggerEnter (Collider other) {
            if (other.gameObject.tag == "Enemy Bullet") {
                Attacked ();
                Destroy (other.gameObject, 0f);
            }
        }

        public int GetActionPointIdNow () {
            return actionPointIdNow;
        }

        public void MoveToNextActionPoint () {
            if (actionPointIdNow < actionPointPositions.Length) {
                actionPointIdNow++;
                Sequence sequence = DOTween.Sequence ();
                float duration = (actionPointPositions[actionPointIdNow] - transform.position).magnitude / speed;
                //TODO: 台形制御
                /* 
                if (duration > 1) {
                    sequence.Append (transform.DOMove ((actionPointPositions[actionPointIdNow] - transform.position).normalized * 0.25f + transform.position, 1 / speed).SetEase (Ease.InCubic));
                    sequence.Append (transform.DOMove (actionPointPositions[actionPointIdNow] - (actionPointPositions[actionPointIdNow] - transform.position).normalized * 0.25f, duration).SetEase (Ease.Linear));
                    sequence.Append (transform.DOMove (actionPointPositions[actionPointIdNow], 1 / speed).SetEase (Ease.OutCubic).OnComplete (() => {
                        actionPointGroup.transform.GetChild (actionPointIdNow).GetComponent<ActionPoint> ().OnMoveFinish ();
                    }));
                } else {
                    sequence.Append (transform.DOMove (actionPointPositions[actionPointIdNow], duration).SetEase (Ease.InOutSine).OnComplete (() => {
                        actionPointGroup.transform.GetChild (actionPointIdNow).GetComponent<ActionPoint> ().OnMoveFinish ();
                    }));
                }*/

                Ease moveEase = new Ease ();
                if (actionPointIdNow == 0) {
                    if (actionPointLinearMove[actionPointIdNow]) {
                        moveEase = Ease.InSine;
                    } else {
                        moveEase = Ease.InOutSine;
                    }
                } else {
                    if (actionPointLinearMove[actionPointIdNow - 1]) {
                        if (actionPointLinearMove[actionPointIdNow]) {
                            moveEase = Ease.Linear;
                        } else {
                            moveEase = Ease.OutSine;
                        }
                    } else {
                        if (actionPointLinearMove[actionPointIdNow]) {
                            moveEase = Ease.InSine;
                        } else {
                            moveEase = Ease.InOutSine;
                        }
                    }
                }

                sequence.Append (transform.DOMove (actionPointPositions[actionPointIdNow], duration).SetEase (moveEase).OnComplete (() => {
                    //Debug.Log (Time.time - HighScoreManager.thisGameScore.GetStartTime ());
                    actionPointGroup.transform.GetChild (actionPointIdNow).GetComponent<ActionPoint> ().OnMoveFinished ();
                }));
                int x = 0;
                sequence.Join (DOTween.To (() => x, num => x = num, 1, duration - 0.1f).OnComplete (() => {
                    actionPointGroup.transform.GetChild (actionPointIdNow).GetComponent<ActionPoint> ().OnMoveAlmostFinished ();
                }));
                if (actionPointSlowCamera[actionPointIdNow]) {
                    sequence.Join (transform.DORotate (actionPointRotations[actionPointIdNow], 2).SetEase (Ease.Linear));
                } else {
                    sequence.Join (transform.DORotate (actionPointRotations[actionPointIdNow], duration).SetEase (Ease.Linear));
                }
            }
        }

        public void Attacked (int atk = 1) {
            HP -= atk;
            if (HP <= 0 && gameOverEnabled) {
                gameManagerObject.GetComponent<GameManager> ().ChangeState (GameState.Over);
            }
            attackedSoundSource.PlayOneShot (attackedSoundSource.clip);
            //duration,strength,vibrato,randomness
            var sequence = DOTween.Sequence ();
            sequence.Append (frontCam.transform.DOShakePosition (0.6f, 0.9f, 7, 80));
            sequence.Append (frontCam.transform.DOLocalMove (new Vector3 (0, 0.8f, 0), 0.2f));
        }

        public void SetSpotlightIntensity (float intensity) {
            spotlight1.GetComponent<Light> ().intensity = intensity;
            spotlight2.GetComponent<Light> ().intensity = intensity;
        }

        public void StopGhosts () {
            foreach (Transform child in actionPointGroup.transform) {
                child.GetComponent<ActionPoint> ().KillAllGhosts ();
            }
        }
    }
}