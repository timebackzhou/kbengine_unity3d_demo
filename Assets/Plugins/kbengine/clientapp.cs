using UnityEngine;
using System;
using System.IO;  
using System.Collections;
using KBEngine;

public class clientapp : MonoBehaviour {
	public static KBEngineApp gameapp = null;
	
	void Awake() 
	 {
		DontDestroyOnLoad(transform.gameObject);
	 }
 
	// Use this for initialization
	void Start () 
	{
		MonoBehaviour.print("clientapp::start()");
			
		KBEngine.Event.registerOut("onImportClientMessages", this, "onImportClientMessages");
		KBEngine.Event.registerOut("onImportServerErrorsDescr", this, "onImportServerErrorsDescr");
		KBEngine.Event.registerOut("onImportClientEntityDef", this, "onImportClientEntityDef");
		KBEngine.Event.registerOut("onVersionNotMatch", this, "onVersionNotMatch");
		KBEngine.Event.registerOut("onScriptVersionNotMatch", this, "onScriptVersionNotMatch");
		
		gameapp = new KBEngineApp();
		KBEngineApp.url = "http://127.0.0.1";
		KBEngineApp.app.clientType = 5;
		KBEngineApp.app.ip = "127.0.0.1";
		KBEngineApp.app.port = 20013;
		
		//gameapp.autoImportMessagesFromServer(true);
		
		byte[] loginapp_onImportClientMessages = loadFile (Application.persistentDataPath, "loginapp_onImportClientMessages." + 
		                                                   KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);

		byte[] baseapp_onImportClientMessages = loadFile (Application.persistentDataPath, "baseapp_onImportClientMessages." + 
		                                                  KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);

		byte[] onImportServerErrorsDescr = loadFile (Application.persistentDataPath, "onImportServerErrorsDescr." + 
		                                             KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);

		byte[] onImportClientEntityDef = loadFile (Application.persistentDataPath, "onImportClientEntityDef." + 
		                                           KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);

		if(loginapp_onImportClientMessages.Length > 0 && baseapp_onImportClientMessages.Length > 0)
			KBEngineApp.app.importMessagesFromMemoryStream (loginapp_onImportClientMessages, baseapp_onImportClientMessages, onImportClientEntityDef, onImportServerErrorsDescr);
	}
	
	void OnDestroy()
	{
		MonoBehaviour.print("clientapp::OnDestroy(): begin");
		KBEngineApp.app.destroy();
		MonoBehaviour.print("clientapp::OnDestroy(): over, isbreak=" + gameapp.isbreak + ", over=" + gameapp.kbethread.over);
	}
	
	void FixedUpdate () {
		KBEUpdate();
	}
		
	void KBEUpdate()
	{
		KBEngine.Event.processOutEvents();
	}

	public void onImportClientMessages(string currserver, byte[] stream)
	{
		if(currserver == "loginapp")
			createFile (Application.persistentDataPath, "loginapp_onImportClientMessages." + 
			            KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion, stream);
		else
			createFile (Application.persistentDataPath, "baseapp_onImportClientMessages." + 
			            KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion, stream);
	}

	public void onImportServerErrorsDescr(byte[] stream)
	{
		createFile (Application.persistentDataPath, "onImportServerErrorsDescr." + 
		            KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion, stream);
	}
	
	public void onImportClientEntityDef(byte[] stream)
	{
		createFile (Application.persistentDataPath, "onImportClientEntityDef." + 
		            KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion, stream);
	}
	
	public void clearMessageFiles()
	{
		deleteFile(Application.persistentDataPath, "loginapp_onImportClientMessages." + KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);
		deleteFile(Application.persistentDataPath, "baseapp_onImportClientMessages." + KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);
		deleteFile(Application.persistentDataPath, "onImportServerErrorsDescr." + KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);
		deleteFile(Application.persistentDataPath, "onImportClientEntityDef." + KBEngineApp.app.clientVersion + "." + KBEngineApp.app.clientScriptVersion);
		KBEngineApp.app.resetMessages();
	}
	
	public void onVersionNotMatch(string verInfo, string serVerInfo)
	{
		clearMessageFiles();
	}

	public void onScriptVersionNotMatch(string verInfo, string serVerInfo)
	{
		clearMessageFiles();
	}
	
	void createFile(string path, string name, byte[] datas)  
   {  
		deleteFile(path, name);
		Debug.Log("createFile: " + path + "//" + name);
		FileStream fs = new FileStream (path + "//" + name, FileMode.OpenOrCreate, FileAccess.Write);
		fs.Write (datas, 0, datas.Length);
		fs.Close ();
		fs.Dispose ();
   }  
   
   byte[] loadFile(string path, string name)  
   {  
		FileStream fs;

		try{
			fs = new FileStream (path + "//" + name, FileMode.Open, FileAccess.Read);
		}
		catch (Exception e)
		{
			Debug.Log("loadFile: " + path + "//" + name);
			Debug.Log(e.ToString());
			return new byte[0];
		}

		byte[] datas = new byte[fs.Length];
		fs.Read (datas, 0, datas.Length);
		fs.Close ();
		fs.Dispose ();

		Debug.Log("loadFile: " + path + "//" + name + ", datasize=" + datas.Length);
		return datas;
   }  
   
   void deleteFile(string path, string name)  
   {  
		Debug.Log("deleteFile: " + path + "//" + name);
		try{
        	File.Delete(path + "//"+ name);  
		}
		catch (Exception e)
		{
		}
   }  
}
