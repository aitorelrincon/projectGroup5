using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

using SPM = MultiSpawner.SpawnPosMethod;
using UnityEditorInternal;

[CustomEditor(typeof(MultiSpawner))]
public class MultiSpawnerEditor : Editor
{
    #region Prefabs config
    const string s_spawnedParent  = "spawnedParent";
    const string s_pooledParent   = "pooledParent";
    const string s_prefabsConfig  = "prefabsConfig";
    #endregion

    #region Spawn config
    const string s_method           = "method";
    const string s_exactPos         = "exactPos";
    const string s_bounds           = "bounds";
    const string s_sphereRadius     = "sphereRadius";
    const string s_points           = "points";
    const string s_childrenAsPoints = "childrenAsPoints";
    const string s_sphereNorth      = "northHemisphere";
    #endregion

    #region Spawn time & chance
    const string s_waitInitSecs   = "waitInitSecs";
    const string s_waitLoopSecs   = "waitLoopSecs";
    const string s_spawnChance    = "spawnChance";
    #endregion

    #region Events
    const string s_beforeSpawn    = "beforeSpawn";
    const string s_afterSpawn     = "afterSpawn";
    #endregion

    ReorderableList prefabs;
    void OnEnable()
    {
        prefabs = new(serializedObject, serializedObject.FindProperty( s_prefabsConfig ), true, true, true, true);
#if false
        prefabs.onAddCallback = (l) => {
            var index = l.serializedProperty.arraySize;
            l.serializedProperty.arraySize++;
            l.index = index;
            var p = prefabs.serializedProperty.GetArrayElementAtIndex( index );
            var s = p.FindPropertyRelative( "_serialized" );
            var args = p.FindPropertyRelative( "_poolArgs" );
            args.FindPropertyRelative( "_prefab" ).objectReferenceValue = null;
            args.FindPropertyRelative( "_count" ).intValue = 1;
            p.FindPropertyRelative( "proportion" ).floatValue = 1f;
            s.boolValue = false;
        };

        prefabs.drawElementCallback = ( r, i, a, f ) => {
            var element = prefabs.serializedProperty.GetArrayElementAtIndex( i );
            EditorGUI.PropertyField(
                new Rect(r.x, r.y, 100, EditorGUIUtility.singleLineHeight ),
                element.FindPropertyRelative( "_poolArgs" ),
                GUIContent.none
            );

            EditorGUI.PropertyField(
                new Rect(r.x, r.y, 100, EditorGUIUtility.singleLineHeight ),
                element.FindPropertyRelative( "proportion" ),
                GUIContent.none
            );
        };
#endif
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Prefabs config
        EditorGUILayout.PropertyField( prefabs.serializedProperty ); 
        // prefabs.DoLayoutList();

        // Spawn config
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_spawnedParent ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_pooledParent ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_method ) );
        switch ( (SPM) serializedObject.FindProperty(s_method).enumValueIndex )
        {
            case SPM.ExactPos:
                EditorGUILayout.PropertyField( serializedObject.FindProperty( s_exactPos ) );
                break;

            case SPM.InsideBounds:
                EditorGUILayout.PropertyField( serializedObject.FindProperty( s_bounds ) );
                break;

            case SPM.InsideSpherePoints:
                EditorGUILayout.PropertyField( serializedObject.FindProperty( s_sphereRadius ) );
                EditorGUILayout.PropertyField( serializedObject.FindProperty( s_sphereNorth ) );
                var children = serializedObject.FindProperty( s_childrenAsPoints );
                EditorGUILayout.PropertyField( children );
                    
                if ( !children.boolValue )
                    EditorGUILayout.PropertyField( serializedObject.FindProperty( s_points ) );
                
                break;
        }

        // Spawn time & chance 
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_waitInitSecs ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_waitLoopSecs ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_spawnChance ) );

        // Events
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_beforeSpawn ) );
        EditorGUILayout.PropertyField( serializedObject.FindProperty( s_afterSpawn ) );

        serializedObject.ApplyModifiedProperties();
    }
}
