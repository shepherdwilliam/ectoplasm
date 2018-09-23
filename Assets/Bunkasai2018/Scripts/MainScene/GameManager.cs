using System;
using Bunkasai2018;
using DG.Tweening;
using DigitalRuby.RainMaker;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Bunkasai2018 {
	public enum GameState {
		Title,
		Practice,
		Idle,
		Game,
		Over,
		Clear,
		Ending
	}

	public class GameManager : StatefulObjectBase<GameManager, GameState> {

		public static bool debugMode = false;
		[SerializeField] private GameObject bgmManager;
		[SerializeField] private GameObject gunManager;
		[SerializeField] private GameObject display2Manager;
		[SerializeField] public GameObject rainObject;
		[SerializeField] public GameObject player;
		[SerializeField] private GameObject tutorialPoint;
		[SerializeField] private GameObject mainPoint;
		[SerializeField] private GameObject tutorialGhost;
		[SerializeField] private GameObject spotlight;
		[SerializeField] private UnityEngine.UI.Image logoImage;
		[SerializeField] private UnityEngine.UI.Image panel;
		[SerializeField] public UnityEngine.UI.Image dialogImage;
		[SerializeField] public UnityEngine.UI.Image[] bombImages;
		[SerializeField] private UnityEngine.UI.Text introLabel;
		[SerializeField] private UnityEngine.UI.Text endingLabel;
		[SerializeField] public UnityEngine.UI.Text tutorialLabel;
		[SerializeField] private float timeLimit = 300;
		[SerializeField] private AnimationCurve ghostCurve;
		[SerializeField] private AudioClip[] communications;
		[SerializeField] public AudioClip dialogOpenClip;
		[SerializeField] public AudioClip dialogCloseClip;
		[SerializeField] public AudioSource communicationsAudioSource;
		[SerializeField] public AudioSource openCloseAudioSource;
		// in seconds
		private float gameStartTime;
		private int tutorialState = 0;

		// Use this for initialization
		void Start () {
			// ゲームステート初期化
			stateList.Add (new StateTitle (this));
			stateList.Add (new StatePractice (this));
			stateList.Add (new StateIdle (this));
			stateList.Add (new StateGame (this));
			stateList.Add (new StateOver (this));
			stateList.Add (new StateClear (this));
			stateList.Add (new StateEnding (this));
			stateMachine = new StateMachine<GameManager> ();
			ChangeState (GameState.Title);
			display2Manager.GetComponent<Display2Manager> ().RankingUpdate ();

			// 衝突を無視するように設定
			//Physics.IgnoreLayerCollision (LayerMask.NameToLayer ("Default"), LayerMask.NameToLayer ("Ghost"));
		}

		private class StateTitle : State<GameManager> {

			public StateTitle (GameManager owner) : base (owner) { }

			private bool stateChange = false;

			public override void Enter () {
				Time.timeScale = 1;
				Sequence sequence = DOTween.Sequence ();
				sequence.Append (owner.logoImage.DOFade (1, 0.7f));
				owner.player.transform.position = owner.tutorialPoint.transform.position;
				owner.player.transform.rotation = owner.tutorialPoint.transform.rotation;
			}

			public override void Execute () {
				if (Input.GetKeyDown (KeyCode.Space) && !stateChange) {
					stateChange = true;
					Sequence sequence = DOTween.Sequence ();
					sequence.Append (owner.logoImage.DOFade (0, 1f).OnComplete (() => {
						owner.bgmManager.GetComponent<BGMManager> ().FadeOutBGM ();
						owner.ChangeState (GameState.Practice);
					}));
				}
			}

			public override void Exit () { }
		}

		private class StatePractice : State<GameManager> {

			public StatePractice (GameManager owner) : base (owner) { }

			/*string[] SAMPLEtutorialTexts = {
								"これより、訓練を開始する。",
								"何か異常があれば、すぐに指令に伝えること。",
								"まず君たちの乗車している車両であるが、バッテリー駆動のためおおよそ５分程度しか任務に当たることができない。",
								"速やかに任務を遂行してくれ。",
								"次に、 銃の撃ち方であるが、 トリガーを引くと照準に向けて弾が発射される。",
								"トリガーを引き続けることでより強力な弾を放つことも可能だ。",
								"一度発射して見てくれ。",
								"最後にectoplasmの倒し方だ。",
								"ectoplasmを発見したら、 照準を合わせ、 トリガーを引いてくれ。 数発当てることで消失させることが可能だ。",
								"確実に消滅したことを確認するまで打ち続けてくれ。",
								"また、 中には攻撃を仕掛けてくる個体もいる。",
								"攻撃を受けると可視化装置や銃にノイズが発生してしまう。",
								"弾を撃つことで相殺できるので、 積極的に撃ち落としてくれ。",
								"今から我々が捕獲し、 弱らせた個体を数匹放出するので、 銃の操作を確認してくれ。（ 撃ち落とした後）",
								"これにて訓練は終了する。 各々の持てる力を最大限発揮し、 任務に挑んでくれ。（ 最後の機械のところ）",
								"あれが博士の開発したectoplasm発生装置だ！ これを破壊すればectoplasmはすべて消滅する！",
								"銃で撃って破壊してくれ！ "
							};*/

			/* 音声
			string[] tutorialTextsA = {
				"これより、訓練を開始する。",
				"君たちの乗車している車両はバッテリー駆動のため５分程度しか稼働できない。速やかに任務を遂行してくれ。",
				"敵、ectoplasm(エクトプラズム)を倒すには、照準を合わせてトリガーを引いてくれ。エネルギー弾が発射され、数発当てることで消滅するだろう。",
				"トリガーを引き続けてエネルギーを貯めることでより強力な弾を放ったり、車両の中央にあるボタンを押すことで、3回までだが全体攻撃のできる爆弾を使用する事ができる。",
				"中には攻撃を仕掛けてくる個体もいる。相手の弾にこちらの弾を当てることで相殺できるので、積極的に撃ち落としてくれ。",
				"今から我々が捕獲して弱らせた個体を放出するので、銃の操作を確認してくれ。"
			};

			string[] tutorialTextsB = {
				"これにて訓練は終了する。各々の持てる力を最大限発揮し、任務に挑んでくれ。"
			};

			トリガーを引き続けてエネルギーを貯めることでより強力な弾を放ったり、車両の中央にあるボタンを押すことで、3回までだが全体攻撃のできる爆弾を使用する事ができる。

			敵の本拠地は生徒棟202Rにあるとの情報が入った。

			一部通路が謎の力により封鎖されており、少し遠回りになるが、管理棟一階を通り、二階に上り渡り廊下を渡って生徒棟三階の202Rに向かう。
			
			あれが博士の開発したectoplasm発生装置だ！これを破壊すればectoplasmはすべて消滅する！

			*/

			//ゲーム内テキスト
			string[] tutorialTextsA = {
				"ー訓練開始ー",
				"車両はバッテリー駆動のため５分しか稼働できない。",
				"敵、ectoplasmを倒すには、照準を合わせてトリガーを引く。\nエネルギー弾が発射され、敵に数発当たると消滅する。",
				"トリガーを引き続けると強力な弾を放つ事ができる。\n車両の中央のボタンで爆弾を使用する事ができる。",
				"攻撃された時は、相手の弾にこちらの弾を当てる事で相殺できる。",
				"ー操作確認ー"
			};

			string[] tutorialTextsB = {
				"ー訓練終了ー"
			};

			public override void Enter () {
				HighScoreManager.thisGameScore = new Score ();
				owner.display2Manager.GetComponent<Display2Manager> ().GameStart ();
				owner.tutorialGhost.GetComponent<Ghost> ().PopThisUp (owner.tutorialPoint.transform.position, 2, true);
				owner.spotlight.GetComponent<Light> ().enabled = true;
				Sequence sequence = DOTween.Sequence ();
				sequence.Append (owner.panel.DOFade (0, 0.3f));
				sequence.Append (owner.dialogImage.DOFade (1, 0.2f).OnStart (() => {
					owner.openCloseAudioSource.PlayOneShot (owner.dialogOpenClip);
				}));
				sequence.Append (owner.dialogImage.DOFade (1, 0.5f));
				foreach (string subtext in tutorialTextsA) {
					sequence.Append (owner.tutorialLabel.DOFade (1, 0.0f).OnStart (() => {
						owner.tutorialLabel.text = subtext;
					}));
					if (subtext == tutorialTextsA[0]) {
						sequence.Append (owner.tutorialLabel.DOFade (1, 3f).OnStart (() => {
							owner.communicationsAudioSource.PlayOneShot (owner.communications[0]);
						}));
					} else if (subtext == tutorialTextsA[1]) {
						sequence.Append (owner.tutorialLabel.DOFade (1, 7.3f).OnStart (() => {
							owner.communicationsAudioSource.PlayOneShot (owner.communications[1]);
						}));
					} else if (subtext == tutorialTextsA[2]) {
						sequence.Append (owner.tutorialLabel.DOFade (1, 8.5f).OnStart (() => {
							owner.communicationsAudioSource.PlayOneShot (owner.communications[2]);
						}));
					} else if (subtext == tutorialTextsA[3]) {
						sequence.Append (owner.tutorialLabel.DOFade (1, 11f).OnStart (() => {
							owner.communicationsAudioSource.PlayOneShot (owner.communications[3]);
						}));
					} else if (subtext == tutorialTextsA[4]) {
						sequence.Append (owner.tutorialLabel.DOFade (1, 8f).OnStart (() => {
							owner.communicationsAudioSource.PlayOneShot (owner.communications[4]);
						}));
					} else {
						sequence.Append (owner.tutorialLabel.DOFade (1, 5f).OnStart (() => {
							owner.communicationsAudioSource.PlayOneShot (owner.communications[5]);
						}));
					}
				}
				sequence.Append (owner.tutorialLabel.DOFade (0, 0.0f).OnStart (() => {
					owner.tutorialState = 1;
					owner.gunManager.GetComponent<GunMechanism> ().enabled = true;
				}));
				sequence.Join (owner.tutorialGhost.transform.DOMove (new Vector3 (0, 4, 0), 3f).SetRelative ());
				sequence.Append (owner.tutorialGhost.transform.DOMoveY (1, 2f).SetEase (owner.ghostCurve).SetLoops (100));
			}

			public override void Execute () {
				if (owner.tutorialState == 2) {
					Sequence sequence = DOTween.Sequence ();
					foreach (string subtext in tutorialTextsB) {
						sequence.Append (owner.tutorialLabel.DOFade (1, 0.0f).OnStart (() => {
							owner.tutorialLabel.text = subtext;
						}));
						sequence.Append (owner.tutorialLabel.DOFade (1, 5f));
					}
					sequence.Append (owner.tutorialLabel.DOFade (0, 0.0f));
					sequence.Append (owner.dialogImage.DOFade (0, 0.2f).OnStart (() => {
						owner.openCloseAudioSource.PlayOneShot (owner.dialogCloseClip);
					}));
					sequence.Append (owner.panel.DOFade (1, 0.3f)).OnComplete (() => {
						owner.gunManager.GetComponent<GunMechanism> ().enabled = false;
						owner.player.transform.position = owner.mainPoint.transform.position;
						owner.player.transform.rotation = owner.mainPoint.transform.rotation;
						owner.ChangeState (GameState.Idle);
					});
					owner.tutorialState = 3;
				}
			}

			public override void Exit () {
				owner.spotlight.GetComponent<Light> ().enabled = false;
			}
		}

		private class StateIdle : State<GameManager> {

			public StateIdle (GameManager owner) : base (owner) { }

			public override void Enter () {
				//Time.timeScale = 0f;
				Sequence sequence = DOTween.Sequence ();
				sequence.Append (owner.introLabel.DOFade (1, 0.3f));
				sequence.Append (owner.panel.DOFade (1, 2f));
				sequence.Append (owner.introLabel.DOFade (0, 0.5f));
				sequence.Join (owner.panel.DOFade (0, 1f));
				sequence.Append (owner.panel.DOFade (0, 1f).OnComplete (() => {
					owner.ChangeState (GameState.Game);
				}));
				//Debug.Log ("enter");
			}

			public override void Execute () { }

			public override void Exit () { }
		}

		private class StateGame : State<GameManager> {

			public StateGame (GameManager owner) : base (owner) { }

			public override void Enter () {
				//Debug.Log ("game");
				//Time.timeScale = 1f;
				//owner.firstActionPoint.GetComponent<ActionPoint> ().DoAction ();
				HighScoreManager.thisGameScore.SetStartTime ();
				owner.gameStartTime = Time.time;
				owner.gunManager.GetComponent<GunMechanism> ().enabled = true;
				foreach (UnityEngine.UI.Image image in owner.bombImages) {
					image.DOFade (1, 0f);
				}
				owner.player.GetComponent<PlayerController> ().MoveToNextActionPoint ();
			}

			public override void Execute () {
				if (Time.time > owner.gameStartTime + owner.timeLimit) {
					HighScoreManager.thisGameScore.SetEndTime ();
					owner.ChangeState (GameState.Over);
				}
			}

			public override void Exit () {
				owner.rainObject.GetComponent<RainScript> ().RainIntensity = 0;
			}
		}

		private class StateOver : State<GameManager> {

			public StateOver (GameManager owner) : base (owner) { }

			public override void Enter () {
				owner.player.GetComponent<PlayerController> ().StopGhosts ();
				owner.gunManager.GetComponent<GunMechanism> ().enabled = false;
				Sequence sequence = DOTween.Sequence ();
				sequence.Append (owner.panel.DOFade (1, 2f));
				sequence.Append (owner.panel.DOFade (1, 0.2f).OnComplete (() => {
					owner.ChangeState (GameState.Ending);
				}));
			}

			public override void Execute () { }

			public override void Exit () { }
		}

		private class StateClear : State<GameManager> {

			public StateClear (GameManager owner) : base (owner) { }

			public override void Enter () {
				owner.player.GetComponent<PlayerController> ().StopGhosts ();
				owner.gunManager.GetComponent<GunMechanism> ().enabled = false;
				HighScoreManager.thisGameScore.SetEndTime ();
				HighScoreManager.thisGameScore.Cleared ();
				HighScoreManager.PutThisGameScoreIntoHighScore ();
				owner.display2Manager.GetComponent<Display2Manager> ().RankingUpdate ();
				Sequence sequence = DOTween.Sequence ();
				sequence.Append (owner.panel.DOFade (1, 2f));
				sequence.Append (owner.panel.DOFade (1, 0.2f).OnComplete (() => {
					owner.ChangeState (GameState.Ending);
				}));
			}

			public override void Execute () { }

			public override void Exit () { }
		}

		private class StateEnding : State<GameManager> {

			public StateEnding (GameManager owner) : base (owner) { }

			public override void Enter () {
				float clearTime = (float) Math.Round (HighScoreManager.thisGameScore.GetEndTime () - HighScoreManager.thisGameScore.GetStartTime (), 3);
				float clearTimeSeconds = clearTime % 60;
				int clearTimeMinutes = Mathf.FloorToInt (clearTime / 60);
				string[] gameClearTexts = {
					"君たちの活躍によってectoplasmを生成する装置は破壊され、\nそれらは全て瑞陵高校から消滅した。",
					"しかしながら、\nこの事件の首謀者とされる博士は未だ逃走中だ。",
					"事件開始から1ヶ月がたった今でも、\n彼の所在に関する情報は一切得られていない。",
					"「・・・この事件はまだ完全に解決したとは言えない。」",
					"「また君たちが活躍してくれることを願っている。」",
					$"今回の君たちのクリアタイムは\n{clearTimeMinutes}分{clearTimeSeconds}秒だ。"
				};

				//これの前に機械音声で「バッテリー切れです」的なのを入れる
				string[] timeOverTexts = {
					"残念だが、君たちはバッテリー切れによって撤退を余儀なくされた。",
					"未だ別部隊は奮闘中で、ectoplasmの掃討は完了していない。",
					"「・・・だがそう気にする事はない。\nなによりも、君たちが無事に帰還することができたのだから。」",
					"「次の出動ではもっと君たちが活躍してくれることを期待している。」",
					"GAME OVER"
				};

				if (HighScoreManager.thisGameScore.GetCleared ()) {
					owner.ShowEndingTexts (gameClearTexts);
				} else {
					owner.ShowEndingTexts (timeOverTexts);
				}
			}

			public override void Execute () { }

			public override void Exit () {
				owner.display2Manager.GetComponent<Display2Manager> ().GameEnd ();
			}
		}

		private void ShowEndingTexts (string[] endingTexts) {
			Sequence sequence = DOTween.Sequence ();
			foreach (string subtext in endingTexts) {
				sequence.Append (endingLabel.DOFade (1, 0.3f).OnStart (() => {
					endingLabel.text = subtext;
				}));
				sequence.Append (endingLabel.DOFade (1, 3f));
				sequence.Append (endingLabel.DOFade (0, 0.3f));
				if (subtext == endingTexts[endingTexts.Length - 1]) {
					sequence.Append (endingLabel.DOFade (0, 0.2f).OnComplete (() => {
						SceneManager.LoadSceneAsync ("main");
					}));
				}
			}
		}

		public void TutorialGhostKilled () {
			if (tutorialState == 1) {
				tutorialState = 2;
			}
		}
	}
}