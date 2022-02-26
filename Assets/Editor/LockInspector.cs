using System.Reflection;
using UnityEditor;
using UnityEngine;

public class LockInspector
{
	[MenuItem ("Window/LockInspector/Tab %q")]
	public static void ShowInspectorWindow ()
	{
		Object target = Selection.activeObject;
		if (target == null)
			return;

		var inspectorType = typeof(Editor).Assembly.GetType ("UnityEditor.InspectorWindow");
		var inspectorInstance = ScriptableObject.CreateInstance (inspectorType) as EditorWindow;
		inspectorInstance.Show (true);
		var isLocked = inspectorType.GetProperty ("isLocked", BindingFlags.Instance | BindingFlags.Public);
		isLocked.GetSetMethod ().Invoke (inspectorInstance, new object[] { true });
	}
}