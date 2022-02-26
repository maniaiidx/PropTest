using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditorInternal;
using System.IO;

public class DefauldFolderWindow : EditorWindow {
	

	[MenuItem ("Window/Defauld Folder Window")]
	public static void Init () {
		EditorWindow.GetWindow<DefauldFolderWindow> (false, "Defauld Folder Window");
	}



	GUIContent btnCntent = new GUIContent();
	private Vector2 scrollPosition;


	private bool m_isEditorApplication = true;
	private bool m_isApplication = true;
	private bool m_isInternalEditorUtility = true;
	private bool m_isOther = true;



	void OnGUI () {

		string path = "";
		m_itemNo = 0;

		#region Horizontal checkbox
		GUILayout.BeginHorizontal();

		m_isEditorApplication = GUIToggle(m_isEditorApplication, "EditorApplication");
		m_isApplication = GUIToggle(m_isApplication, "Application");
		m_isInternalEditorUtility = GUIToggle(m_isInternalEditorUtility, "InternalEditorUtility");
		m_isOther = GUIToggle(m_isOther, "Other");

		GUILayout.EndHorizontal();
		#endregion Horizontal



		scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, false);


		#region EditorApplication

		btnCntent.text = AddItemNo() + "EditorApplication.applicationContentsPath";
		btnCntent.tooltip = "";
		if(m_isEditorApplication)
			OnGUIFolderPath(btnCntent, EditorApplication.applicationContentsPath);


		btnCntent.text = AddItemNo() + "EditorApplication.applicationPath";
		btnCntent.tooltip = "";

		if (m_isEditorApplication)
			OnGUIFolderPath(btnCntent, EditorApplication.applicationPath, false);

		#endregion EditorApplication



		#region Application

		btnCntent.text = AddItemNo() + "Application.dataPath";
		btnCntent.tooltip = "";

		if (m_isApplication) OnGUIFolderPath(btnCntent, Application.dataPath);


		btnCntent.text = AddItemNo() + "Application.persistentDataPath";
		btnCntent.tooltip = "";
		if (m_isApplication) OnGUIFolderPath(btnCntent, Application.persistentDataPath);


		btnCntent.text = AddItemNo() + "Application.temporaryCachePath";
		btnCntent.tooltip = "";
		if (m_isApplication) OnGUIFolderPath(btnCntent, Application.temporaryCachePath);

		#endregion Application



		#region InternalEditorUtility

		btnCntent.text = AddItemNo() + "InternalEditorUtility.unityPreferencesFolder";
		btnCntent.tooltip = "";
		if (m_isInternalEditorUtility) OnGUIFolderPath(btnCntent, InternalEditorUtility.unityPreferencesFolder);


		btnCntent.text = AddItemNo() + "InternalEditorUtility.GetEditorAssemblyPath()";
		btnCntent.tooltip = "";
		if (m_isInternalEditorUtility) OnGUIFolderPath(btnCntent, InternalEditorUtility.GetEditorAssemblyPath(), false);


		btnCntent.text = AddItemNo() + "InternalEditorUtility.GetEngineAssemblyPath()";
		btnCntent.tooltip = "";
		if (m_isInternalEditorUtility) OnGUIFolderPath(btnCntent, InternalEditorUtility.GetEngineAssemblyPath(), false);



		//btnCntent.text = AddItemNo() + "InternalEditorUtility.GetExternalScriptEditor()";
		//btnCntent.tooltip = "";
		//if (m_isInternalEditorUtility) OnGUIFolderPath(btnCntent, InternalEditorUtility.GetExternalScriptEditor(), false);





		#endregion InternalEditorUtility


		#region Other

		path = InternalEditorUtility.unityPreferencesFolder + Path.DirectorySeparatorChar + "Layouts";
		btnCntent.text = AddItemNo() + "Layouts Folder";
		btnCntent.tooltip = "";
		if (m_isOther) OnGUIFolderPath(btnCntent, path);



		if (mOS == OsType.MAC)
			path = InternalEditorUtility.unityPreferencesFolder + Path.DirectorySeparatorChar + "../../../Unity/Asset Store";
		else
			path = InternalEditorUtility.unityPreferencesFolder + Path.DirectorySeparatorChar + "../../Asset Store";
		
		path = Path.GetFullPath(path);
		btnCntent.text = AddItemNo() + "Asset Store  Download Foloder";
		btnCntent.tooltip = "";
		if (m_isOther) OnGUIFolderPath(btnCntent, path);



		if (mOS == OsType.MAC)
			path = Environment.GetFolderPath(Environment.SpecialFolder.MyMusic) + "/../Library/Logs/Unity/";
		else
			path = Environment.GetEnvironmentVariable("LocalAppData") + "/Unity/Editor/";

		path = Path.GetFullPath(path);
		btnCntent.text = AddItemNo() + "Editor Log";
		btnCntent.tooltip = "";
		if (m_isOther) OnGUIFolderPath(btnCntent, path);


		#endregion Other


		GUILayout.Space(50);

		GUILayout.EndScrollView();
	}



	private bool GUIToggle(bool isEnable, string label)
	{
		isEnable = EditorGUILayout.Toggle(isEnable, GUILayout.Width(20));
		EditorGUILayout.LabelField(label);
		return isEnable;
	}



	private void OnGUIFolderPath(GUIContent guiCntent, string path, bool isBtnOpen = true)
	{

		#region Vertical
		EditorGUILayout.BeginVertical();

		GUILayout.Label(guiCntent, EditorStyles.boldLabel);


		#region Horizontal
		EditorGUILayout.BeginHorizontal(GUILayout.Width(Screen.width-50));

		string AppLogOutFolder = Application.persistentDataPath;
		EditorGUILayout.TextField(path);

		if (isBtnOpen)
			if (GUILayout.Button("Open", GUI.skin.button, GUILayout.Width(50)))
				OpenDirectory(path);


		EditorGUILayout.EndHorizontal();
		#endregion Horizontal


		EditorGUILayout.EndVertical();
		#endregion Vertical
	}



	private static void OpenDirectory(string path)
	{
		System.Diagnostics.Process.Start(path);
	}



	private OsType m_osType = OsType.UNKOWN;
	private OsType mOS
	{
		get
		{
			if (m_osType != OsType.UNKOWN) return m_osType;

			if (SystemInfo.operatingSystem.Contains("Windows"))
				m_osType = OsType.WIN;
			else if (SystemInfo.operatingSystem.Contains("Mac"))
				m_osType = OsType.MAC;
			return m_osType;
		}
	}

	private enum OsType
	{
		UNKOWN,
		WIN,
		MAC,
	}


	private int m_itemNo = 0;
	private string AddItemNo()
	{
		++m_itemNo;
		return m_itemNo.ToString() + " ";
	}


}