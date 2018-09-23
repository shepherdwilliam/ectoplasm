using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Bunkasai2018 {
	public static class PlayerPrefsUtility {

		/// <summary>
		/// 指定されたオブジェクトの情報を保存します
		/// </summary>
		public static void SetObject<T> (string key, T obj) {
			var json = JsonUtility.ToJson (obj);
			PlayerPrefs.SetString (key, json);
		}

		/// <summary>
		/// 指定されたオブジェクトの情報を読み込みます
		/// </summary>
		public static T GetObject<T> (string key) {
			var json = PlayerPrefs.GetString (key);
			var obj = JsonUtility.FromJson<T> (json);
			return obj;
		}
	}
}