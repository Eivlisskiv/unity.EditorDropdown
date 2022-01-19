using IgnitedBox.EditorDropdown.Utilities;
using UnityEditor;
using UnityEngine;

namespace IgnitedBox.EditorDropdown.Attribute
{
    [CustomPropertyDrawer(typeof(DropdownAttribute))]
    public class DropdownEditor : PropertyDrawer
    {
        private DropdownAttribute _attribute;

        private Editor customEditor;

        public DropdownEditor() : base()
        {
            _attribute = (DropdownAttribute)attribute;
        }

        public override void OnGUI(Rect position, SerializedProperty prop, GUIContent label)
        {
            if(_attribute == null) _attribute = (DropdownAttribute)attribute;
            _attribute.Load(fieldInfo, prop);

            if (!_attribute.Valid) return;

            DrawDropdownOption(prop, position);
        }

        private void DrawDropdownOption(SerializedProperty prop, Rect position)
        {
            if (_attribute.Index == null) return;

            int index = 0;
            string name = prop.displayName;
            if (_attribute.IsList)
            {
                index = prop.GetIndex();
                name = _attribute.FieldName + $" {index}";
            }

            if (_attribute.Index.Length <= index)
            {
                _attribute.SetIndex();
                if (_attribute.Index.Length <= index) return;
            }

            int selected = _attribute.Index[index];

            if (selected < 0 || selected > _attribute.Count)
            {
                _attribute.SetValue(0, index);
                selected = _attribute.Index[index];
            }

            //Draw and get the new selected index as "nindex"
            int nindex = EditorGUI.Popup(position, name,
                selected, _attribute.Options);

            if (_attribute.SetValue(nindex, index))
            {
                EditorUtility.SetDirty(prop.serializedObject.targetObject);      
            }

            switch (prop.propertyType)
            {
                case SerializedPropertyType.ObjectReference:
                    UnityObjectEditor(prop.objectReferenceValue);
                    break;
            }

        }

        private void UnityObjectEditor(Object target)
        {
            if (!target) return;

            Editor.CreateCachedEditor(target, null, ref customEditor);
            if (!customEditor) return;

            customEditor.OnInspectorGUI();
        }
    }
}