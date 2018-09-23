using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Bunkasai2018 {
	public class BGMManager : MonoBehaviour {

		[SerializeField] private AudioClip A0Clip;
		[SerializeField] private AudioClip A1Clip;
		[SerializeField] private AudioClip B0Clip;
		[SerializeField] private AudioClip B1Clip;
		private AudioSource source0;
		private AudioSource source1;

		// Use this for initialization
		void Start () {
			if (Display2Manager.teamNumber % 2 == 0) {
				source0 = GetComponents<AudioSource> () [0];
				source1 = GetComponents<AudioSource> () [1];
				source0.clip = A0Clip;
				source1.clip = A1Clip;
				source0.PlayScheduled (AudioSettings.dspTime);
				source1.PlayScheduled (AudioSettings.dspTime + (((double) source0.clip.samples + 5) / (double) source0.clip.frequency));
			} else {
				source0 = GetComponents<AudioSource> () [0];
				source1 = GetComponents<AudioSource> () [1];
				source0.clip = B0Clip;
				source1.clip = B1Clip;
				source0.PlayScheduled (AudioSettings.dspTime);
				source1.PlayScheduled (AudioSettings.dspTime + (((double) source0.clip.samples + 5) / (double) source0.clip.frequency));
			}
		}

		// Update is called once per frame
		void Update () {

		}

		public void FadeOutBGM () {
			if (source0.isPlaying) {
				source0.DOFade (0, 2).OnComplete (() => {
					source0.Stop ();
				});
			}
			if (source1.isPlaying) {
				source1.DOFade (0, 2).OnComplete (() => {
					source1.Stop ();
				});
			}
		}
	}
	/* 
			public void CrossFade (float maxVolume, float fadingTime) {
				var fadeInSource =
					source0.isPlaying ?
					source1 :
					source0;

				var fadeOutSource =
					source0.isPlaying ?
					source0 :
					source1;

				fadeInSource.Play ();
				fadeInSource.DOKill ();
				fadeInSource.DOFade (maxVolume, fadingTime);

				fadeOutSource.DOKill ();
				fadeOutSource.DOFade (0, fadingTime);
			}*/

}