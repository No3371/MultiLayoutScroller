// Developed by Tom Kail at Inkle
// Released under the MIT Licence as held at https://opensource.org/licenses/MIT
// Modified by Ren Chen (Aesthetic)

// Must be placed within a folder named "Editor"
using UnityEditor;
using UnityEngine;

namespace BAStudio.MultiLayoutScroller
{
    [CustomPropertyDrawer(typeof(MutliLayoutScrollerLayoutSchema), true)]
    public class LayoutSchemaDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded? EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 4 : base.GetPropertyHeight(property, label);
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

				Handles.color = Color.gray;
				Handles.DrawLine(
                    new Vector2(r.x - 6, r.min.y + EditorGUIUtility.singleLineHeight),
                    new Vector2(r.x - 6, r.max.y - 4)
				);

                r.x -= 32;
                r.width += 32;
                r.height = EditorGUIUtility.singleLineHeight;
                r.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("typeID"));
                r.y += EditorGUIUtility.singleLineHeight + 1;
                Rect indentR = EditorGUI.IndentedRect(r);
                indentR.height += 4;
                if (TypeIndex.LayoutTypes.ContainsKey(type)) EditorGUI.HelpBox(indentR, TypeIndex.LayoutTypes[type], MessageType.Info);
                else EditorGUI.HelpBox(indentR, "Unnamed type", MessageType.Info);
                r.y += EditorGUIUtility.singleLineHeight + 5;
                r.height = EditorGUIUtility.singleLineHeight * (1 + itemCount);
                EditorGUI.PropertyField(r, property.FindPropertyRelative("items"), true);
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty ();
        }
    }
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
                    + 4 + 10
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

                r.x -= 16;
                r.width += 16;
                r.height = EditorGUIUtility.singleLineHeight;
                r.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("viewID"));
                r.y += EditorGUIUtility.singleLineHeight + 1;
                Rect indentR = EditorGUI.IndentedRect(r);
                indentR.height += 4;
                if (TypeIndex.Views.ContainsKey(type)) EditorGUI.HelpBox(indentR, TypeIndex.Views[type], MessageType.Info);
                else EditorGUI.HelpBox(indentR, "Unnamed type", MessageType.Info);
                r.y += EditorGUIUtility.singleLineHeight + 5;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("viewLayoutType"));
                r.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("autoLayoutSpacing"));
                r.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("autoLayoutPadding"), true);
                r.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative("autoLayoutPadding"));
                EditorGUI.PropertyField(r, property.FindPropertyRelative("layouts"), true);
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty ();
        }
    }

    [CustomPropertyDrawer(typeof(ItemTypeIDPair), true)]
    public class ItemSchemaDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return property.isExpanded? EditorGUI.GetPropertyHeight(property, true) + EditorGUIUtility.singleLineHeight + 3  : base.GetPropertyHeight(property, label);
        }

        public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
            EditorGUI.BeginProperty (position, label, property);
            int type = property.FindPropertyRelative("type").intValue;
            int id = property.FindPropertyRelative("dataID").intValue;
            Rect r = position;
            r.height = EditorGUIUtility.singleLineHeight;
            property.isExpanded = EditorGUI.Foldout(r, property.isExpanded, string.Format("Type{0}->#{1}", type, id));
            if (property.isExpanded)
            {
                r = EditorGUI.IndentedRect(position);

				Handles.color = Color.gray;
				Handles.DrawLine(
                    new Vector2(r.x - 6, r.min.y + EditorGUIUtility.singleLineHeight),
                    new Vector2(r.x - 6, r.max.y - 4)
				);

                r.x -= 32;
                r.width += 32;
                r.height = EditorGUIUtility.singleLineHeight;
                r.y += EditorGUIUtility.singleLineHeight + 1;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("type"));
                r.y += EditorGUIUtility.singleLineHeight + 1;
                Rect indentR = EditorGUI.IndentedRect(r);
                indentR.height += 4;
                if (TypeIndex.ItemPrefabTypes.ContainsKey(type)) EditorGUI.HelpBox(indentR, TypeIndex.ItemPrefabTypes[type], MessageType.Info);
                else EditorGUI.HelpBox(indentR, "Unnamed type", MessageType.Info);
                r.y += EditorGUIUtility.singleLineHeight + 5;
                EditorGUI.PropertyField(r, property.FindPropertyRelative("dataID"));
            }
            property.serializedObject.ApplyModifiedProperties();
            EditorGUI.EndProperty ();
        }
    }
}