
using UnityEngine;
using UnityEditor ;
using UnityEditor.Callbacks;

using System.IO;
using System.Collections;


public class MyAssetModificationProcessor : UnityEditor.AssetModificationProcessor
{
    
	[MenuItem("GameObject/MemoryDetectObjCreate")]
	public static void CreateReporter()
	{
		GameObject reporterObj = new GameObject();
		reporterObj.name = "Reporter";
		Reporter reporter = reporterObj.AddComponent<Reporter>();
		reporterObj.AddComponent<ReporterMessageReceiver>();
		//reporterObj.AddComponent<TestReporter>();

		reporter.images = new Images();
		reporter.images.textureImage        = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/texture.png", typeof(Texture2D));
        reporter.images.allImage            = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/All.png", typeof(Texture2D));
        reporter.images.materialImage 		= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/material.png", typeof(Texture2D));
		reporter.images.meshImage           = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/mesh.png", typeof(Texture2D));
		reporter.images.audioImage          = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/audio.png", typeof(Texture2D));
		reporter.images.searchImage 		= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/search.png", typeof(Texture2D));
		reporter.images.closeImage 			= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/close.png", typeof(Texture2D));
   		reporter.images.newObjectImage      = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/newobject.png", typeof(Texture2D));
		reporter.images.remainObjectImage   = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/remainobject.png", typeof(Texture2D));
		reporter.images.removeObjectImage   = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/removeobject.png", typeof(Texture2D));
		reporter.images.barImage 			= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/bar.png", typeof(Texture2D));
		reporter.images.button_activeImage 	= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/button_active.png", typeof(Texture2D));
		reporter.images.even_logImage 		= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/even_log.png", typeof(Texture2D));
		reporter.images.odd_logImage 		= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/odd_log.png", typeof(Texture2D));
		reporter.images.selectedImage 		= (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/selected.png", typeof(Texture2D));
        reporter.images.normalImage         = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/normal_icon.png", typeof(Texture2D));
        reporter.images.dublicateImage      = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/dublicate_icon.png", typeof(Texture2D));
        reporter.images.leakImage           = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/leak_icon.png", typeof(Texture2D));

        reporter.images.reporterScrollerSkin = (GUISkin)AssetDatabase.LoadAssetAtPath("Assets/Reporter/Images/reporterScrollerSkin.guiskin", typeof(GUISkin));

	}
	[InitializeOnLoad]
	public class BuildInfo
	{
		static BuildInfo ()
	    {
	        EditorApplication.update += Update;
	    }
	 
		static bool isCompiling = true ; 
	    static void Update ()
	    {
			if( !EditorApplication.isCompiling && isCompiling )
			{
	        	//Debug.Log("Finish Compile");
				if( !Directory.Exists( Application.dataPath + "/StreamingAssets"))
				{
					Directory.CreateDirectory( Application.dataPath + "/StreamingAssets");
				}
				string info_path = Application.dataPath + "/StreamingAssets/build_info.txt" ;
				StreamWriter build_info = new StreamWriter( info_path );
				build_info.Write(  "Build from " + SystemInfo.deviceName + " at " + System.DateTime.Now.ToString() );
				build_info.Close();
			}
			
			isCompiling = EditorApplication.isCompiling ;
	    }
	}
}
