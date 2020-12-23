// Developed by Tom Kail at Inkle
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT
// Modified by Ren Chen (Aesthetic)

// Must be placed within a folder named "Editor"
using UnityEditor;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CustomPropertyDrawer(typeof(MutliScrollerViewSchema), true)]
    public class ViewSchemaDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded?
                      EditorGUI.GetPropertyHeight(property, true)
                    //   (EditorGUIUtility.singleLineHeight * 5
                    // + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("autoLayoutPadding"))
                    // + EditorGUI.GetPropertyHeight(property.FindPropertyRelative("layouts")))
                    : base.GetPropertyHeight(property, label);
        }
        
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty (position, label, property);
            int type = property.FindPropertyRelative("viewID").intValue;
            int layoutCount = property.FindPropertyRelative("layouts").arraySize;
            Rect r = position;
            r.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, string.Format("View#{0} ({1} layouts)", type, layoutCount));
            if (property.isExpanded)
            {
                r = EditorGUI.IndentedRect(position);
                
				Handles.color = Color.gray;
				Handles.DrawLine(
                    new Vector2(r.x - 6, r.min.y + EditorGUIUtility.singleLineHeight),
                    new Vector2(r.x - 6, r.max.y - 4)
				);

                int indentCache = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                using (new EditorGUI.IndentLevelScope())
                {
                    r.height = EditorGUIUtility.singleLineHeight;
                    r.y += EditorGUIUtility.singleLineHeight;                    
                    Rect tmpRect = r;
                    tmpRect.width = 64 + EditorGUIUtility.labelWidth;
                    EditorGUI.PropertyField(tmpRect, property.FindPropertyRelative("viewID"));
                    tmpRect.width = r.width - 64 - 4 - EditorGUIUtility.labelWidth;
                    tmpRect.x = r.x + 64 + 4 + EditorGUIUtility.labelWidth;
                    if (TypeIndex.Views.ContainsKey(type)) EditorGUI.HelpBox(tmpRect, TypeIndex.Views[type], MessageType.Info);
                    else EditorGUI.HelpBox(tmpRect, "Unnamed type", MessageType.Info);
                    r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(r, property.FindPropertyRelative("viewLayoutType"));
                    r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(r, property.FindPropertyRelative("autoLayoutSpacing"));
                    r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(r, property.FindPropertyRelative("autoLayoutPadding"), true);
                    r.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("autoLayoutPadding"));
                    EditorGUI.PropertyField(r, property.FindPropertyRelative("layouts"), true);
                }
                EditorGUI.indentLevel = indentCache;
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty ();
        }
    }
}