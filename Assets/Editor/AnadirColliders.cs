using UnityEngine;
using UnityEditor;

public static class AnadirColliders
{
    [MenuItem("Herramientas/Añadir colliders a la selección")]
    static void Anadir()
    {
        int anadidos = 0;
        foreach (GameObject raiz in Selection.gameObjects)
        {
            foreach (MeshFilter mf in raiz.GetComponentsInChildren<MeshFilter>(true))
            {
                GameObject go = mf.gameObject;
                if (mf.sharedMesh == null) continue;
                if (go.GetComponent<MeshRenderer>() == null) continue;
                if (go.GetComponent<Collider>() != null) continue;
                if (go.GetComponentInParent<CharacterController>() != null) continue;

                MeshCollider mc = Undo.AddComponent<MeshCollider>(go);
                mc.sharedMesh = mf.sharedMesh;
                anadidos++;
            }
        }
        Debug.Log($"Colliders añadidos: {anadidos}");
    }

    [MenuItem("Herramientas/Añadir colliders a la selección", true)]
    static bool Validar() => Selection.gameObjects.Length > 0;
}
