using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using Unity.Cinemachine;

public static class ConfigurarCamara
{
    [MenuItem("Herramientas/Configurar cámara tercera persona")]
    static void Configurar()
    {
        CinemachineCamera cam = Object.FindAnyObjectByType<CinemachineCamera>();
        ObjetivoCamara objetivo = Object.FindAnyObjectByType<ObjetivoCamara>();
        if (cam == null || objetivo == null)
        {
            Debug.LogError("Falta la CinemachineCamera o el objeto con ObjetivoCamara en la escena");
            return;
        }

        GameObject go = cam.gameObject;
        Undo.RegisterFullObjectHierarchyUndo(go, "Configurar cámara");

        Quitar<CinemachineFreeLookModifier>(go);
        Quitar<CinemachineDeoccluder>(go);
        Quitar<CinemachineInputAxisController>(go);
        Quitar<CinemachineRotationComposer>(go);
        Quitar<CinemachineOrbitalFollow>(go);
        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);

        CinemachineThirdPersonFollow seguimiento = go.GetComponent<CinemachineThirdPersonFollow>();
        if (seguimiento == null)
            seguimiento = Undo.AddComponent<CinemachineThirdPersonFollow>(go);

        seguimiento.ShoulderOffset = new Vector3(0.4f, 0f, 0f);
        seguimiento.VerticalArmLength = 0.2f;
        seguimiento.CameraSide = 1f;
        seguimiento.CameraDistance = 3f;
        seguimiento.Damping = new Vector3(0.1f, 0.25f, 0.3f);
        seguimiento.AvoidObstacles.Enabled = true;
        seguimiento.AvoidObstacles.CollisionFilter = LayerMask.GetMask("Default");
        seguimiento.AvoidObstacles.IgnoreTag = "Player";
        seguimiento.AvoidObstacles.CameraRadius = 0.2f;
        seguimiento.AvoidObstacles.DampingIntoCollision = 0f;
        seguimiento.AvoidObstacles.DampingFromCollision = 0.4f;

        cam.Follow = objetivo.transform;
        cam.LookAt = null;
        cam.Lens.NearClipPlane = 0.05f;

        if (objetivo.personaje != null)
        {
            Undo.RecordObject(objetivo.personaje.gameObject, "Configurar cámara");
            objetivo.personaje.gameObject.tag = "Player";
        }

        EditorUtility.SetDirty(cam);
        EditorUtility.SetDirty(seguimiento);
        EditorSceneManager.MarkSceneDirty(go.scene);
        Selection.activeObject = go;
        Debug.Log("Cámara en tercera persona configurada");
    }

    static void Quitar<T>(GameObject go) where T : Component
    {
        T c = go.GetComponent<T>();
        if (c != null) Undo.DestroyObjectImmediate(c);
    }
}
