/* 
 * UnityAdsController.cs
 * http://bit.ly/unityadscontroller
 * 
 * if the errors are caused in Editor, please check the following things
 *  - Missing "UnityEngine.Advertisements"
 *    -> Turning ON the UnityAds Service from Services Window.
 * 
 * もしUnityエディタでエラーがでる場合は、下記のことをご確認ください
 *  - "UnityEngine.Advertisements"がみつからないといわれたとき
 *    -> ServiceウィンドウからAdsサービスをONにしてください
 */
using UnityEngine;
using System.Collections;
using UnityEngine.Advertisements;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class UnityAdsController : MonoBehaviour
{
	[SerializeField]
	string zoneID = "rewardedVideo";
	[SerializeField]
	string gameID_iOS = "1050908";
	[SerializeField]
	string gameID_Android = "1050907";

	[Header("OnReady Enable Button")]
	public Button adsButton;
	[Header("OnReady Callback")]
	public UnityEvent OnReadyAds;

	[Header("OnFinished Callback")]
	public UnityEvent OnFinishedAds;
	[Header("OnSkipped Callback")]
	public UnityEvent OnSkippedAds;
	[Header("OnFailed Callback")]
	public UnityEvent OnFailedAds;

	void Awake ()
	{
		if (Advertisement.isSupported && !Advertisement.isInitialized) {
			#if UNITY_ANDROID
			Assert.IsNotNull (gameID_Android);
			Advertisement.Initialize(gameID_Android);
			#elif UNITY_IOS
			Assert.IsNotNull (gameID_iOS);
			Advertisement.Initialize(gameID_iOS);
			#endif
		}
	}

	void OnEnable ()
	{
		StartCoroutine (waitForAdsReady());
	}

	IEnumerator waitForAdsReady ()
	{
		if (adsButton != null) adsButton.gameObject.SetActive (false);
		yield return new WaitUntil (() => Advertisement.IsReady () == true);
		if (adsButton != null) adsButton.gameObject.SetActive (true);
		OnReady ();
	}

	public void ShowUnityAds ()
	{
		#if UNITY_ANDROID || UNITY_IOS
		if (Advertisement.IsReady(zoneID)) {
			var options = new ShowOptions { resultCallback = HandleShowResult };
			Advertisement.Show(zoneID, options);
		}
		#endif
	}

	private void HandleShowResult(ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log ("The ad was successfully shown.");
			OnFinished ();
			break;
		case ShowResult.Skipped:
			Debug.Log ("The ad was skipped before reaching the end.");
			OnSkipped ();
			break;
		case ShowResult.Failed:
			Debug.LogError ("The ad failed to be shown.");
			OnFailed ();
			break;
		}
	}

	void OnReady ()
	{
		OnReadyAds.Invoke();
	}

	void OnFinished ()
	{
		OnFinishedAds.Invoke();
	}

	void OnSkipped ()
	{
		OnSkippedAds.Invoke();
	}

	void OnFailed ()
	{
		OnFailedAds.Invoke();
	}
}