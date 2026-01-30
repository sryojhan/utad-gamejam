#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;

// --- 1. EXTENSIONES (Para replicar el estilo VInspector que me pasaste) ---
public static class VInspectorLiteExtensions
{
    public static Rect SetHeight(this Rect r, float h) { r.height = h; return r; }
    public static Rect SetWidth(this Rect r, float w) { r.width = w; return r; }
    public static Rect MoveX(this Rect r, float x) { r.x += x; return r; }
    public static Rect MoveY(this Rect r, float y) { r.y += y; return r; }
    public static Rect AddWidthFromRight(this Rect r, float w) { r.width += w; return r; }
    public static Rect AddHeightFromBottom(this Rect r, float h) { r.height += h; return r; }

    // Detectar si una propiedad es "inline" (int, float, vector) o "bloque" (clase)
    public static bool IsSingleLine(this SerializedProperty prop)
    {
        return prop.propertyType != SerializedPropertyType.Generic || !prop.hasVisibleChildren;
    }
}

// --- 2. EL DRAWER ---
[CustomPropertyDrawer(typeof(EnumArray<,>))]
public class EnumArrayDrawer : PropertyDrawer
{
    // Caches para no usar Reflection en cada frame
    private Dictionary<string, SerializedProperty> arrayProps = new Dictionary<string, SerializedProperty>();
    private Dictionary<string, string[]> enumNamesCache = new Dictionary<string, string[]>();

    public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
    {
        // 1. Setup inicial (igual que tu ejemplo)
        SetupProps(prop);

        // Sincronizar tamaño si hace falta
        EnsureArraySize(prop);

        // Rectángulo indentado base
        var indentedRect = EditorGUI.IndentedRect(rect);

        // --- FUNCIONES LOCALES (Estilo VInspector) ---

        void header()
        {
            var headerRect = indentedRect.SetHeight(EditorGUIUtility.singleLineHeight);

            // Dibujar Foldout y Label
            // Usamos el sistema nativo para el foldout, pero sobre el rect calculado
            prop.isExpanded = EditorGUI.Foldout(headerRect, prop.isExpanded, prop.displayName, true);
        }

        void list_()
        {
            if (!prop.isExpanded) return;

            var arrayProp = arrayProps[prop.propertyPath];
            var enumNames = enumNamesCache[prop.propertyPath];

            // Empezamos a dibujar debajo del header
            float currentY = rect.y + EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            // Iteramos manualmente (mejor que ReorderableList para tamaño fijo)
            EditorGUI.indentLevel++;

            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var element = arrayProp.GetArrayElementAtIndex(i);
                string enumLabel = (i < enumNames.Length) ? enumNames[i] : $"Element {i}";

                // Calculamos altura de este elemento
                float h = EditorGUI.GetPropertyHeight(element, new GUIContent(enumLabel), true);
                Rect elementRect = new Rect(rect.x, currentY, rect.width, h);

                // DIBUJAR ELEMENTO
                DrawElement(elementRect, element, enumLabel);

                currentY += h + EditorGUIUtility.standardVerticalSpacing;
            }

            EditorGUI.indentLevel--;
        }

        // Ejecución
        header();
        list_();
    }

    // Lógica de dibujado de cada fila
    private void DrawElement(Rect rect, SerializedProperty element, string labelStr)
    {
        var content = new GUIContent(labelStr);

        // Si es una línea simple (int, float), Unity lo dibuja centrado verticalmente bien.
        // Si es complejo (Struct), Unity necesita el rect completo.

        // BeginProperty es CLAVE para que el prefab override y el menú contextual funcionen
        EditorGUI.BeginProperty(rect, content, element);

        // "true" en includeChildren hace que Unity gestione el desplegable de clases complejas automáticamente
        EditorGUI.PropertyField(rect, element, content, true);

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
    {
        SetupProps(prop);

        float height = EditorGUIUtility.singleLineHeight; // Cabecera

        if (prop.isExpanded)
        {
            var arrayProp = arrayProps[prop.propertyPath];
            height += EditorGUIUtility.standardVerticalSpacing;

            for (int i = 0; i < arrayProp.arraySize; i++)
            {
                var element = arrayProp.GetArrayElementAtIndex(i);
                // Sumamos la altura real (sea simple o expandida)
                height += EditorGUI.GetPropertyHeight(element, GUIContent.none, true) + EditorGUIUtility.standardVerticalSpacing;
            }
        }
        return height;
    }

    // --- SETUP & REFLECTION (Solo se ejecuta una vez por propiedad) ---

    private void SetupProps(SerializedProperty prop)
    {
        if (arrayProps.ContainsKey(prop.propertyPath)) return;

        // Buscar internalArray
        var p = prop.FindPropertyRelative("internalArray");
        arrayProps[prop.propertyPath] = p;

        // Buscar Nombres Enum
        enumNamesCache[prop.propertyPath] = GetEnumNames(fieldInfo);
    }

    private void EnsureArraySize(SerializedProperty prop)
    {
        var p = arrayProps[prop.propertyPath];
        var names = enumNamesCache[prop.propertyPath];

        if (names != null && p.arraySize != names.Length)
        {
            p.arraySize = names.Length;
        }
    }

    private string[] GetEnumNames(FieldInfo field)
    {
        if (field == null) return new string[0];
        Type t = field.FieldType;

        // Desempaquetar jerarquía hasta encontrar EnumArray<TEnum, TValue>
        if (t.IsArray) t = t.GetElementType();
        else if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>)) t = t.GetGenericArguments()[0];

        while (t != null)
        {
            if (t.IsGenericType && t.GetGenericTypeDefinition() == typeof(EnumArray<,>))
            {
                Type[] args = t.GetGenericArguments();
                // Buscamos cuál es el Enum
                foreach (var arg in args) if (arg.IsEnum) return Enum.GetNames(arg);
            }
            t = t.BaseType;
        }
        return new string[] { "Error" };
    }
}
#endif