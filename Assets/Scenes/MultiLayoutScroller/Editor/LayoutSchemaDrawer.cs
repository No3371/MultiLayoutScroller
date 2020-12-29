// Developed by Tom Kail at Inkle
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT
// Modified by Ren Chen (Aesthetic)

// Must be placed within a folder named "Editor"
using UnityEditor;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CustomPropertyDrawer(typeof(LayoutSchema), true)]
    public class LayoutSchemaDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded? EditorGUI.GetPropertyHeight(property, true) : base.GetPropertyHeight(property, label);
        }
        
        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty (position, label, property);
            int type = property.FindPropertyRelative("typeID").intValue;
            int itemCount = property.FindPropertyRelative("items").arraySize;
            Rect r = position;
            r.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, string.Format("Type{0} ({1} items)", type, itemCount));
            if (property.isExpanded)
            {
                r = EditorGUI.IndentedRect(position);
                int indentCache = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                
                using (new EditorGUI.IndentLevelScope())
                {
                    Handles.color = Color.gray;
                    Handles.DrawLine(
                        new Vector2(r.x - 6, r.min.y + EditorGUIUtility.singleLineHeight),
                        new Vector2(r.x - 6, r.max.y - 4)
                    );

                    r.height = EditorGUIUtility.singleLineHeight;
                    r.y += EditorGUIUtility.singleLineHeight;
                    Rect tmpRect = r;
                    tmpRect.width = 48 + EditorGUIUtility.labelWidth;
                    EditorGUI.PropertyField(tmpRect, property.FindPropertyRelative("typeID"));
                    tmpRect.width = r.width - 48 - 4 - EditorGUIUtility.labelWidth;
                    tmpRect.x = r.x + 48 + 4 + EditorGUIUtility.labelWidth;
                    if (TypeIndex.LayoutTypes.ContainsKey(type)) EditorGUI.HelpBox(tmpRect, TypeIndex.LayoutTypes[type], MessageType.Info);
                    else EditorGUI.HelpBox(tmpRect, "Unnamed type", MessageType.Info);
                    r.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
                    EditorGUI.PropertyField(r, property.FindPropertyRelative("items"), true);
                }
                EditorGUI.indentLevel = indentCache;
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty ();
        }
    }
}