using UnityEngine;
using System.Collections;

public class ShowInterstitialWallButton : GUITextureButton
{
	public GUIText globalStatusText = null;
	
	public string token = "token";
	public string bundleId = "com.dreamobi.simplelight";
	public string unitName = "金币";
	//---------------------------------------------------------------------------
	
	// Use this for initialization
	public override void Start() {
		base.Start();
		Dreamobi.Configure(token, bundleId, unitName);
	}
	
	// Update is called once per frame
	void Update() {
	}
	
	public void OnVideoStarted(int amount)
	{
		string text = this.gameObject.name + " triggered playing a video ad - reward [" + amount + "].";
		Debug.Log(text);
		globalStatusText.text = text;
	}
	
	/// <summary>
	/// This checks every update if the zone specified is ready to be played. If it is, it sets the GUITexture being used to display the status to the correct image.
	/// </summary>
	public void OnAdAttemptFinished(bool success) {
		string text = null;
		if (success)
		{
			text = "Success - Wall";
		}
		else
		{
			text = "Failed - Wall";
		}
		Debug.Log(text);
		globalStatusText.text = text;
	}
	
	
	/// <summary>
	/// This is the default logic to be performed on button pressed
	/// </summary>
	public override void PerformButtonPressLogic() {
		Dreamobi.OnAdAttemptFinished = OnAdAttemptFinished;
		Dreamobi.OnVideoStarted = OnVideoStarted;
		Dreamobi.ShowInterstitialWall();
	}
}
