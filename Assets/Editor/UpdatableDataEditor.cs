using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(UpdatableData), true)]
public class UpdatableDataEditor : Editor {
	public override void OnInspectorGUI() {
		base.OnInspectorGUI();
		
		if (GUI.changed) {
			((UpdatableData)target).NotifyOfUpdatedValues();
			EditorUtility.SetDirty(target);
		}
	}
}
