using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        WeaponData weaponData = (WeaponData)target;

        DrawDefaultInspector();

        if (weaponData.weaponImage != null)
        {
            GUILayout.Label("미리보기");
            Rect rect = GUILayoutUtility.GetRect(64, 64);
            GUILayout.Box(weaponData.weaponImage.texture, 
                GUILayout.Width(weaponData.weaponImage.texture.width), 
                GUILayout.Height(weaponData.weaponImage.texture.height));
        }
    }
}
