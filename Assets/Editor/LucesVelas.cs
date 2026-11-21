using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

public static class LucesVelas
{
    const string NombreLuz = "LuzVela";

    static readonly Vector3[] Velas =
    {
        new Vector3(-0.920f, -0.443f, 0.000f),
        new Vector3(-0.467f, -0.443f, 0.914f),
        new Vector3(0.467f, -0.443f, 0.914f),
        new Vector3(0.920f, -0.443f, 0.000f),
        new Vector3(0.467f, -0.443f, -0.914f),
        new Vector3(-0.467f, -0.443f, -0.914f),
    };

    [MenuItem("Herramientas/Añadir luces a las velas")]
    static void Anadir()
    {
        int anadidas = 0;

        foreach (MeshFilter mf in Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include))
        {
            Transform t = mf.transform;
            if (!t.name.ToLower().StartsWith("lampara")) continue;
            if (mf.sharedMesh == null || t.Find(NombreLuz) != null) continue;

            Bounds b = mf.sharedMesh.bounds;
            float altoLlama = b.size.y * t.lossyScale.y * 0.02f;

            foreach (Vector3 vela in Velas)
            {
                Vector3 local = b.center + Vector3.Scale(b.extents, vela);

                GameObject go = new GameObject(NombreLuz);
                Undo.RegisterCreatedObjectUndo(go, "Luces velas");
                go.transform.position = t.TransformPoint(local) + Vector3.up * altoLlama;
                go.transform.SetParent(t, true);

                Light luz = go.AddComponent<Light>();
                luz.type = LightType.Point;
                luz.color = new Color(1f, 0.7f, 0.4f);
                luz.intensity = 0.8f;
                luz.range = 2.5f;
                luz.shadows = LightShadows.None;

                go.AddComponent<ParpadeoLuz>();
                anadidas++;
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log($"Luces de velas añadidas: {anadidas}");
    }

    [MenuItem("Herramientas/Quitar luces de las velas")]
    static void Quitar()
    {
        int quitadas = 0;
        foreach (Light luz in Object.FindObjectsByType<Light>(FindObjectsInactive.Include))
        {
            if (luz.name != NombreLuz) continue;
            Undo.DestroyObjectImmediate(luz.gameObject);
            quitadas++;
        }
        Debug.Log($"Luces de velas quitadas: {quitadas}");
    }

    [MenuItem("Herramientas/Ajustar luces de las velas")]
    static void Ajustar()
    {
        Light[] luces = System.Array.FindAll(
            Object.FindObjectsByType<Light>(FindObjectsInactive.Include), l => l.name == NombreLuz);
        Selection.objects = System.Array.ConvertAll(luces, l => (Object)l.gameObject);
        Debug.Log($"Seleccionadas {luces.Length} luces de velas");
    }
}
