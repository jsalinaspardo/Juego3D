using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class LucesAntorchas
{
    const string NombreLuz = "LuzAntorcha";

    [MenuItem("Herramientas/Añadir luces a las antorchas")]
    static void Anadir()
    {
        int anadidas = 0;

        foreach (Transform t in Object.FindObjectsByType<Transform>(FindObjectsInactive.Include))
        {
            if (!t.name.ToLower().StartsWith("antorcha")) continue;
            if (t.Find(NombreLuz) != null) continue;

            Renderer r = t.GetComponentInChildren<Renderer>();
            Vector3 posicion = r != null
                ? new Vector3(r.bounds.center.x, r.bounds.max.y + 0.1f, r.bounds.center.z)
                : t.position;

            GameObject go = new GameObject(NombreLuz);
            Undo.RegisterCreatedObjectUndo(go, "Luces antorchas");
            go.transform.position = posicion;
            go.transform.SetParent(t, true);

            Light luz = go.AddComponent<Light>();
            luz.type = LightType.Point;
            luz.color = new Color(1f, 0.58f, 0.25f);
            luz.intensity = 3f;
            luz.range = 5f;
            luz.shadows = LightShadows.None;

            go.AddComponent<ParpadeoLuz>();
            anadidas++;
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"Luces añadidas: {anadidas}");
    }

    [MenuItem("Herramientas/Quitar luces de las antorchas")]
    static void Quitar()
    {
        int quitadas = 0;
        foreach (Light luz in Object.FindObjectsByType<Light>(FindObjectsInactive.Include))
        {
            if (luz.name != NombreLuz) continue;
            Undo.DestroyObjectImmediate(luz.gameObject);
            quitadas++;
        }
        Debug.Log($"Luces quitadas: {quitadas}");
    }

    [MenuItem("Herramientas/Ajustar luces de las antorchas")]
    static void Ajustar()
    {
        Light[] luces = System.Array.FindAll(
            Object.FindObjectsByType<Light>(FindObjectsInactive.Include), l => l.name == NombreLuz);
        Selection.objects = System.Array.ConvertAll(luces, l => (Object)l.gameObject);
        Debug.Log($"Seleccionadas {luces.Length} luces. Cambia los valores en el Inspector y se aplican a todas.");
    }
}
