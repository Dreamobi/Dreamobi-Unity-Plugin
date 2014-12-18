//=============================================================================
//  Dreamobi.cs
//
//  Dreamobi Plugin for Unity.
//
//  Copyright 2014 Dreamobi, Inc.  All rights reserved.
//
//  ---------------------------------------------------------------------------
//
//  * INSTRUCTIONS *
//
//  For more information on Dreamobi concepts, refer to the Quick Start Guide.
//
//  GENERAL SETUP
//  1.  Copy this file into your Unity project's "Assets" folder.
//
//  2.  In the Start() method of one of your existing game scripts, write, for
//      example:
//
//        Dreamobi.Configure( "fd5fds985s55s7", "com.company.name", "Gold" );
//
//      - "fd5..." is your Dreamobi publisher token.
//      - "com.xxx..." is your bundle id.
//		-  "Gold" is your unit name of virtual currency.
//
//  3.  You can then call any of the methods listed below, often done when you
//      start a new game or start a new level.
//
//        IsInstantVideoAvailable():bool
//        ShowVideoAd():bool
//        ShowInterstitialWall():bool
//
//      NOTES
//      - Dreamobi only works on iOS and Android devices.  Calls to Dreamobi
//        made from a program running in the Unity editor will have no effect.
//
//  4.  You can set delegates that are notified when trying to show a video or
// 		 a video is started.  By
//      example:
//
//        public class MyGame : MonoBehavior
//        ...
//          void ShowAd()
//          {
//             	Dreamobi.OnVideoStarted = OnVideoStarted;
//             	Dreamobi.OnAdAttemptFinished = OnAdAttemptFinished;
//             	Dreamobi.ShowVideoAd();
//          }
//
//          void ShowInterstitialWall()
//          {
//             	Dreamobi.OnVideoStarted = OnVideoStarted;
//             	Dreamobi.OnAdAttemptFinished = OnAdAttemptFinished;
//             	Dreamobi.ShowInterstitialWall();
//          }
//
//          void OnVideoStarted()
//          {
//            	Debug.Log( "Ad playing." );
//          }
//
//          void OnAdAttemptFinished( boolean success )
//          {
// 			  	if (success)
//            	{
//            		Debug.Log( "Got one video" );
//			  	} 
//            	else
//            	{
//            		Debug.Log( "No Fill" );
//            	}
//          }
//
//  IOS SETUP
//
//  1.  To support iOS, copy "UnityDre.mm", "DreamobiPublic.h", and
//      "libADreamobi.a" into your Unity project's Assets/Plugins/iOS
//      folder.
//
//
//  ANDROID SETUP
//
//  1.  To support Android, copy "dreamobi.jar", "unitydre.jar",
//      "google-play-services.jar" , and "AndroidManifest.xml" into your Unity project's
//      Assets/Plugins/Android folder.
//
//  2.  Edit "Assets/Plugins/Android/AndroidManifest.xml" and change the
//      package name to that of your app.
//
//  3.  (Optional) If you are an experienced Android Unity user and wish to use
//      your own main Activity similar to class UnityPlayerActivity, just be
//      sure there are no calls to mUnityPlayer.quit().
//
//=============================================================================
using UnityEngine;
using System;
using System.Collections;
using System.Runtime.InteropServices;

public class Dreamobi : MonoBehaviour
{

	// The single instance of the Dreamobi component
	private static Dreamobi instance;
	
	private static void ensureInstance()
	{
		if (instance == null) {
			instance = FindObjectOfType(typeof(Dreamobi)) as Dreamobi;
			if (instance == null) {
				instance = new GameObject("Dreamobi").AddComponent<Dreamobi>();
			}
		}
	}

	// DELEGATE TYPE SPECIFICATIONS
	// Your class can define methods matching these signatures and assign them
	// to the 'OnVideoStarted', 'OnAdAttemptFinished'
	// delegates as described in Step 4 of the "GENERAL SETUP" instructions
	// above.
	public delegate void AttemptFinished(bool success);
	public delegate void VideoStarted(int amount);

	// DELEGATE PROPERTIES
	public static AttemptFinished OnAdAttemptFinished;
	public static VideoStarted OnVideoStarted;

	//---------------------------------------------------------------------------
	//  PUBLIC INTERFACE - NON-IOS/NON-ANDROID (stub functionality)
	//---------------------------------------------------------------------------
	#if (!UNITY_ANDROID && !UNITY_IPHONE) || UNITY_EDITOR
	public static void Configure(string token, string bundleId, string unitName)
	{
		if (configured) 
		{
			return;
		}
		ensureInstance();
		
		Debug.LogWarning( "Note: Dreamobi doesn't play videos in the editor." );
		configured = true;
	}

	public static bool IsInstantVideoAvailable() {return false;}
	public static bool ShowVideoAd() {return false;}
	public static bool ShowInterstitialWall() {return false;}

	#endif

	//---------------------------------------------------------------------------
	//  PUBLIC INTERFACE - ANDROID
	//---------------------------------------------------------------------------
	#if UNITY_ANDROID && !UNITY_EDITOR
	public static void Configure(string token, string bundleId, string unitName)
	{
		if (configured)
		{
			return;
		}
		
		#if UNITY_EDITOR
		Debug.LogWarning( "Note: Dreamobi doesn't play videos in the editor." );
		configured = false;
		#else
		ensureInstance();
		AndroidConfigure(token, bundleId, unitName);
		#endif
	}
	
	public static bool IsInstantVideoAvailable()
	{
		if (!configured)
		{
			return false;
		}
		return AndroidIsInstantVideoAvailable();
	}

	static public bool ShowVideoAd()
	{
		if (!configured)
		{
			return false;
		}
		return AndroidShowVideoAd();
	}

	public static bool ShowInterstitialWall()
	{
		if (!configured)
		{
			return false;
		}
		return AndroidShowInterstitialWall();
	}

	#endif
	
	//---------------------------------------------------------------------------
	//  PUBLIC INTERFACE - IOS
	//---------------------------------------------------------------------------
	#if UNITY_IPHONE && !UNITY_EDITOR
	
	public static void Configure(string token, string bundleId, string unitName)
	{
		if (configured)
		{
			return;
		}
		
		#if UNITY_EDITOR
		Debug.LogWarning( "Note: Dreamobi doesn't play videos in the editor." );
		configured = false;
		#else
		ensureInstance();
		IOSConfigure(token, bundleId, unitName);
		configured = true;
		#endif
	}
	
	public static bool IsVideoAvailable()
	{
		if (!configured)
		{
			return false;
		}
		return IOSIsVideoAvailable();
	}

	
	public static bool ShowVideoAd()
	{
		if (!configured)
		{
			return false;
		}
		return IOSShowVideoAd();
	}

	public static bool ShowInterstitialWall()
	{
		if (!configured)
		{
			return false;
		}
		return IOSShowInterstitialWall();
	}

	#endif

	//---------------------------------------------------------------------------
	// INTERNAL USE
	//---------------------------------------------------------------------------
	private static bool configured;
	private static bool was_paused;
	
	void Awake ()
	{
		// Set the name to allow UnitySendMessage to find this object.
		name = "Dreamobi";
		// Make sure this GameObject persists across scenes
		DontDestroyOnLoad(transform.gameObject);
	}
	
	void OnApplicationPause ()
	{
		was_paused = true;
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidPause();
		#endif
	}
	
	void OnApplicationQuit ()
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		AndroidRelease();
		#endif
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (was_paused)
		{
			was_paused = false;
			#if UNITY_ANDROID && !UNITY_EDITOR
			AndroidResume();
			#endif
		}
	}

	public void OnDreamobiAdAttemptFinished(string args)
	{
		if (OnAdAttemptFinished != null)
		{
			OnAdAttemptFinished("true".Equals(args));
		}
	}

	public void OnDreamobiAdStarted(string args)
	{
		if (OnVideoStarted != null)
		{
			OnVideoStarted(int.Parse(args));
		}
	}

	//---------------------------------------------------------------------------
	//  ANDROID NATIVE INTERFACE
	//---------------------------------------------------------------------------
	#if UNITY_ANDROID && !UNITY_EDITOR
	private static bool dre_initialized = false;
	private static AndroidJavaClass class_UnityPlayer;
	private static IntPtr class_UnityDre = IntPtr.Zero;
	private static IntPtr method_configure = IntPtr.Zero;
	private static IntPtr method_pause = IntPtr.Zero;
	private static IntPtr method_resume = IntPtr.Zero;
	private static IntPtr method_release = IntPtr.Zero;
	private static IntPtr method_showVideo = IntPtr.Zero;
	private static IntPtr method_showInterstitialWall = IntPtr.Zero;
	private static IntPtr method_isInstantVideoAvailable = IntPtr.Zero;
	
	private static void AndroidInitializePlugin()
	{
		bool success = true;
		IntPtr local_class_UnityDre = AndroidJNI.FindClass("com/dreamobi/unitydre/UnityDre");
		if (local_class_UnityDre != IntPtr.Zero) {
			class_UnityDre = AndroidJNI.NewGlobalRef(local_class_UnityDre);
			AndroidJNI.DeleteLocalRef(local_class_UnityDre);
			var local_class_Dreamobi = AndroidJNI.FindClass("com/dreamobi/dac/Dreamobi");
			if (local_class_Dreamobi != IntPtr.Zero) {
				AndroidJNI.DeleteLocalRef(local_class_Dreamobi);
			} else {
				success = false;
			}
		} else {
			success = false;
		}
		if (success) {
			class_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
			// Get additional method IDs for later use
			method_configure = AndroidJNI.GetStaticMethodID(class_UnityDre, "configure", "(Landroid/app/Activity;Ljava/lang/String;Ljava/lang/String;Ljava/lang/String;)V");
			method_pause = AndroidJNI.GetStaticMethodID(class_UnityDre, "pause", "(Landroid/app/Activity;)V");
			method_resume = AndroidJNI.GetStaticMethodID(class_UnityDre, "resume", "(Landroid/app/Activity;)V");
			method_release = AndroidJNI.GetStaticMethodID(class_UnityDre, "release", "(Landroid/app/Activity;)V");
			method_showVideo = AndroidJNI.GetStaticMethodID(class_UnityDre, "showVideo", "()Z");
			method_showInterstitialWall = AndroidJNI.GetStaticMethodID(class_UnityDre, "showInterstitialWall", "()Z");
			method_isInstantVideoAvailable = AndroidJNI.GetStaticMethodID(class_UnityDre, "isInstantVideoAvailable", "()Z");
			dre_initialized = true;
		} else {
			Debug.LogError("Dreamobi configuration error - make sure dreamobi.jar and "
			               + "unitydre.jar libraries are in your Unity project's Assets/Plugins/Android folder.");
		}
	}
	
	private static void AndroidConfigure(string token, string bundleId, string unitName)
	{
		if (!dre_initialized) {
			AndroidInitializePlugin();
		}
		// Prepare call arguments.
		class_UnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		
		var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
		var j_token = AndroidJNI.NewStringUTF(token);
		var j_bundleId = AndroidJNI.NewStringUTF(bundleId);
		var j_unitName = AndroidJNI.NewStringUTF(unitName);
		
		// Call UnityDre.configure( token, bundleId, unitName )
		jvalue[] args = new jvalue[4];
		args[0].l = j_activity.GetRawObject();
		args[1].l = j_token;
		args[2].l = j_bundleId;
		args[3].l = j_unitName;
		
		AndroidJNI.CallStaticVoidMethod(class_UnityDre, method_configure, args);
		configured = true;
	}

	private static void AndroidPause()
	{
		var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		jvalue[] args = new jvalue[1];
		args [0].l = j_activity.GetRawObject ();
		AndroidJNI.CallStaticVoidMethod(class_UnityDre, method_pause, args);
	}

	private static void AndroidResume()
	{
		var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		jvalue[] args = new jvalue[1];
		args [0].l = j_activity.GetRawObject ();
		AndroidJNI.CallStaticVoidMethod(class_UnityDre, method_resume, args);
	}

	private static void AndroidRelease()
	{
		var j_activity = class_UnityPlayer.GetStatic<AndroidJavaObject> ("currentActivity");
		jvalue[] args = new jvalue[1];
		args [0].l = j_activity.GetRawObject ();
		AndroidJNI.CallStaticVoidMethod(class_UnityDre, method_release, args);
	}

	private static bool AndroidIsInstantVideoAvailable()
	{
		jvalue[] args = new jvalue[0];
		return AndroidJNI.CallStaticBooleanMethod(class_UnityDre, method_isInstantVideoAvailable, args);
		
	}

	private static bool AndroidShowVideoAd()
	{
		jvalue[] args = new jvalue[0];
		return AndroidJNI.CallStaticBooleanMethod(class_UnityDre, method_showVideo, args);
	}

	private static bool AndroidShowInterstitialWall()
	{
		jvalue[] args = new jvalue[0];
		return AndroidJNI.CallStaticBooleanMethod(class_UnityDre, method_showInterstitialWall, args);
	}

	#endif

	//---------------------------------------------------------------------------
	//  IOS NATIVE INTERFACE
	//---------------------------------------------------------------------------
	#if UNITY_IPHONE && !UNITY_EDITOR
	[DllImport ("__Internal")]
	extern static private void IOSConfigure(string token, string bundleId, string unitName);
	[DllImport ("__Internal")]
	extern static private bool IOSIsVideoAvailable();
	[DllImport ("__Internal")]
	extern static private bool IOSShowVideoAd();
	[DllImport ("__Internal")]
	extern static private bool IOSShowInterstitialWall();
	#endif

}
