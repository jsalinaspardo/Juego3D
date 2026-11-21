using UnityEngine;
using UnityEditor;
using System.IO;

public static class AsignarNormalMaps
{
    [MenuItem("Herramientas/Asignar normal maps")]
    static void Asignar()
    {
        int asignados = 0;

        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { "Assets/Materials" }))
        {
            string rutaMaterial = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(rutaMaterial);
            if (mat == null || !mat.HasProperty("_BaseMap") || !mat.HasProperty("_BumpMap")) continue;

            Texture baseMap = mat.GetTexture("_BaseMap");
            if (baseMap == null) continue;

            string rutaColor = AssetDatabase.GetAssetPath(baseMap);
            if (!rutaColor.Contains("_Color")) continue;

            string rutaNormal = rutaColor.Replace("_Color", "_NormalGL");
            if (!File.Exists(rutaNormal)) continue;

            TextureImporter importador = (TextureImporter)AssetImporter.GetAtPath(rutaNormal);
            if (importador.textureType != TextureImporterType.NormalMap)
            {
                importador.textureType = TextureImporterType.NormalMap;
                importador.SaveAndReimport();
            }

            Undo.RecordObject(mat, "Asignar normal maps");
            mat.SetTexture("_BumpMap", AssetDatabase.LoadAssetAtPath<Texture>(rutaNormal));
            mat.SetFloat("_BumpScale", 1f);
            mat.SetTextureScale("_BumpMap", mat.GetTextureScale("_BaseMap"));
            mat.SetTextureOffset("_BumpMap", mat.GetTextureOffset("_BaseMap"));
            mat.EnableKeyword("_NORMALMAP");
            EditorUtility.SetDirty(mat);

            Debug.Log($"{mat.name}: normal map {Path.GetFileName(rutaNormal)}");
            asignados++;
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Normal maps asignados: {asignados}");
    }
}
