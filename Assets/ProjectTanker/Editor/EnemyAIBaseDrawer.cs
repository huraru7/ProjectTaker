using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(EnemyAIBase))]
public class EnemyAIBaseDrawer : PropertyDrawer
{
    // 初回のみスキャン、以降はキャッシュを使用
    private static Type[] _cachedTypes;
    private static string[] _cachedDisplayNames;

    private static void EnsureCache()
    {
        if (_cachedTypes != null) return;

        _cachedTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => { try { return a.GetTypes(); } catch { return Array.Empty<Type>(); } })
            .Where(t => !t.IsAbstract && !t.IsGenericTypeDefinition && typeof(EnemyAIBase).IsAssignableFrom(t))
            .OrderBy(t => t.Name)
            .ToArray();

        _cachedDisplayNames = new string[] { "None" }
            .Concat(_cachedTypes.Select(t => t.Name))
            .ToArray();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float height = EditorGUIUtility.singleLineHeight + 2f;
        if (property.managedReferenceValue != null)
        {
            var it = property.Copy();
            var end = property.GetEndProperty();
            bool enter = true;
            while (it.NextVisible(enter) && !SerializedProperty.EqualContents(it, end))
            {
                height += EditorGUI.GetPropertyHeight(it, true) + 2f;
                enter = false;
            }
        }
        return height;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EnsureCache();
        EditorGUI.BeginProperty(position, label, property);

        int currentIdx = 0;
        if (property.managedReferenceValue != null)
        {
            int found = Array.IndexOf(_cachedTypes, property.managedReferenceValue.GetType());
            if (found >= 0) currentIdx = found + 1;
        }

        Rect popupRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
        int newIdx = EditorGUI.Popup(popupRect, label.text, currentIdx, _cachedDisplayNames);

        if (newIdx != currentIdx)
            property.managedReferenceValue = newIdx == 0 ? null : Activator.CreateInstance(_cachedTypes[newIdx - 1]);

        if (property.managedReferenceValue != null)
        {
            float y = position.y + EditorGUIUtility.singleLineHeight + 2f;
            EditorGUI.indentLevel++;
            var it = property.Copy();
            var end = property.GetEndProperty();
            bool enter = true;
            while (it.NextVisible(enter) && !SerializedProperty.EqualContents(it, end))
            {
                float h = EditorGUI.GetPropertyHeight(it, true);
                EditorGUI.PropertyField(new Rect(position.x, y, position.width, h), it, true);
                y += h + 2f;
                enter = false;
            }
            EditorGUI.indentLevel--;
        }

        EditorGUI.EndProperty();
    }
}
