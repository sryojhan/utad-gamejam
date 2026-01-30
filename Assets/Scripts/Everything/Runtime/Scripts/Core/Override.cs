using UnityEngine;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

[Serializable]
public struct Override<T>
{
    public bool shouldOverride;
    public T value;

    public Override(T initialValue)
    {
        shouldOverride = false;
        value = initialValue;
    }


    public readonly void Apply(Action<T> func)
    {
        if (shouldOverride)
        {
            func(value);
        }
    }

    public readonly void ApplyTo(ref T other)
    {
        if (shouldOverride)
            other = value;
    }

}

public class OverrideGUIAttribute : PropertyAttribute { }


#if UNITY_EDITOR

// Le decimos al drawer que busque el ATRIBUTO, no el tipo genérico
[CustomPropertyDrawer(typeof(OverrideGUIAttribute))]
public class OverrideValueDrawer : PropertyDrawer
{
    // Constantes para ajustar el diseño
    private const int TOGGLE_WIDTH = 20; // Ancho del checkbox
    private const int PADDING = 5;      // Espacio entre checkbox y valor

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        // 1. Buscar las propiedades internas
        // FindPropertyRelative busca por el nombre de la variable en el struct
        var shouldOverrideProp = property.FindPropertyRelative("shouldOverride");

        if (shouldOverrideProp == null)
        {
            EditorGUI.HelpBox(position, $"[OverrideGUI] property must only be used with Override<T>", MessageType.Error);
            return; // ¡Abortamos el dibujo!
        }

        var valueProp = property.FindPropertyRelative("value");

        // 2. Dibujar el Checkbox
        // Calculamos un rectángulo pequeño a la izquierda para el toggle
        Rect toggleRect = new Rect(position.x, position.y, TOGGLE_WIDTH, EditorGUIUtility.singleLineHeight);

        // Dibujamos el toggle y actualizamos su valor.
        // No le pasamos 'label' para que no dibuje el nombre de la variable dos veces.
        shouldOverrideProp.boolValue = EditorGUI.Toggle(toggleRect, GUIContent.none, shouldOverrideProp.boolValue);

        // 3. Dibujar el Valor (si el checkbox está marcado)
        if (shouldOverrideProp.boolValue)
        {
            // Calculamos el espacio restante a la derecha del checkbox
            float startX = position.x + TOGGLE_WIDTH + PADDING;
            Rect valueRect = new Rect(startX, position.y, position.width - (TOGGLE_WIDTH + PADDING), position.height);

            // --- LA MAGIA ---
            // Usamos EditorGUI.PropertyField.
            // Esto le dice a Unity: "Aquí tienes esta propiedad 'value', arréglatelas tú para dibujarla".
            // Si 'T' es un int, dibujará un campo numérico. Si es un Color, un selector de color.
            // El 'true' final es importante: le dice que si 'T' es una clase compleja, dibuje a sus hijos también.
            EditorGUI.PropertyField(valueRect, valueProp, label, true);
        }
        else
        {
            float startX = position.x + TOGGLE_WIDTH + PADDING;
            Rect labelRect = new Rect(startX, position.y, position.width, EditorGUIUtility.singleLineHeight);
            EditorGUI.LabelField(labelRect, $"{label.text}: override?", EditorStyles.label);
        }

        EditorGUI.EndProperty();
    }

    // Esta función es CRUCIAL. Calcula la altura total que necesita el campo en el inspector.
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var shouldOverrideProp = property.FindPropertyRelative("shouldOverride");

        // Si el override está activo, necesitamos preguntar cuánto ocupa el valor 'T'.
        if (shouldOverrideProp.boolValue)
        {
            var valueProp = property.FindPropertyRelative("value");
            // EditorGUI.GetPropertyHeight calcula la altura automáticamente según el tipo 'T'
            return EditorGUI.GetPropertyHeight(valueProp, label, true);
        }

        // Si no está activo, solo ocupamos una línea estándar.
        return EditorGUIUtility.singleLineHeight;
    }
}
#endif