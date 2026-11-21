using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public static class ConfigurarPostprocesado
{
    [MenuItem("Herramientas/Configurar postprocesado")]
    static void Configurar()
    {
        Volume volumen = Object.FindAnyObjectByType<Volume>();
        if (volumen == null || volumen.sharedProfile == null)
        {
            Debug.LogError("No hay ningún Global Volume con perfil en la escena");
            return;
        }

        VolumeProfile perfil = volumen.sharedProfile;
        Undo.RecordObject(perfil, "Configurar postprocesado");

        Tonemapping tonemapping = Obtener<Tonemapping>(perfil);
        tonemapping.mode.Override(TonemappingMode.Neutral);

        Bloom bloom = Obtener<Bloom>(perfil);
        bloom.threshold.Override(0.9f);
        bloom.intensity.Override(0.6f);
        bloom.scatter.Override(0.65f);
        bloom.tint.Override(new Color(1f, 0.9f, 0.8f));
        bloom.highQualityFiltering.Override(true);

        ColorAdjustments color = Obtener<ColorAdjustments>(perfil);
        color.postExposure.Override(0.2f);
        color.contrast.Override(12f);
        color.saturation.Override(8f);
        color.colorFilter.Override(new Color(1f, 0.96f, 0.9f));

        Vignette vignette = Obtener<Vignette>(perfil);
        vignette.intensity.Override(0.3f);
        vignette.smoothness.Override(0.4f);

        if (perfil.TryGet(out MotionBlur motionBlur))
            motionBlur.active = false;

        EditorUtility.SetDirty(perfil);
        AssetDatabase.SaveAssets();
        Selection.activeObject = volumen.gameObject;
        Debug.Log("Postprocesado configurado en " + perfil.name);
    }

    static T Obtener<T>(VolumeProfile perfil) where T : VolumeComponent
    {
        if (!perfil.TryGet(out T componente))
        {
            componente = perfil.Add<T>();
            componente.name = typeof(T).Name;
            componente.hideFlags = HideFlags.HideInInspector | HideFlags.HideInHierarchy;
            AssetDatabase.AddObjectToAsset(componente, perfil);
        }
        Undo.RecordObject(componente, "Configurar postprocesado");
        componente.active = true;
        return componente;
    }
}
