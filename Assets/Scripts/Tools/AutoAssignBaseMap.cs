using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class AutoAssignBaseMap : EditorWindow
{
    private DefaultAsset textureFolderAsset;
    private DefaultAsset materialFolderAsset;

    [MenuItem("Tools/Auto Assign Multi-Texture")]
    public static void ShowWindow()
    {
        GetWindow<AutoAssignBaseMap>("Auto Assign Multi-Texture");
    }

    private void OnGUI()
    {
        GUILayout.Label("Asignador Completo (Color, Normal, Mask)", EditorStyles.boldLabel);
        EditorGUILayout.Space();

        textureFolderAsset = (DefaultAsset)EditorGUILayout.ObjectField("📂 Texturas", textureFolderAsset, typeof(DefaultAsset), false);
        materialFolderAsset = (DefaultAsset)EditorGUILayout.ObjectField("🎨 Materiales", materialFolderAsset, typeof(DefaultAsset), false);

        if (GUILayout.Button("Asignar TODO", GUILayout.Height(40)))
        {
            AssignAllMaps();
        }
    }

    private void AssignAllMaps()
    {
        if (textureFolderAsset == null || materialFolderAsset == null) return;

        string texPath = AssetDatabase.GetAssetPath(textureFolderAsset);
        string matPath = AssetDatabase.GetAssetPath(materialFolderAsset);

        // 1. Indexar Texturas
        string[] textureExtensions = new string[] { "*.png", "*.tga", "*.jpg", "*.jpeg", "*.tif", "*.psd" };
        Dictionary<string, string> textureIndex = new Dictionary<string, string>(System.StringComparer.OrdinalIgnoreCase);

        foreach (string ext in textureExtensions)
        {
            string[] files = Directory.GetFiles(texPath, ext, SearchOption.AllDirectories);
            foreach (string file in files)
            {
                string fileName = Path.GetFileNameWithoutExtension(file);
                if (!textureIndex.ContainsKey(fileName))
                    textureIndex[fileName] = file.Replace("\\", "/");
            }
        }

        Debug.Log($"📘 Texturas indexadas: {textureIndex.Count}");

        // 2. Procesar Materiales
        string[] materialPaths = Directory.GetFiles(matPath, "*.mat", SearchOption.AllDirectories);
        int updates = 0;

        foreach (string mPath in materialPaths)
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(mPath.Replace("\\", "/"));
            if (mat == null) continue;

            string cleanName = CleanMaterialName(mat.name);
            bool changed = false;

            Undo.RecordObject(mat, "Auto Assign Maps");

            // --- 1. COLOR (_c) ---
            string colorPath = FindBestMatch(cleanName, "_c", textureIndex);
            if (colorPath != null)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(colorPath);
                if (mat.HasProperty("_BaseMap")) mat.SetTexture("_BaseMap", tex); // URP
                else if (mat.HasProperty("_MainTex")) mat.SetTexture("_MainTex", tex); // Standard

                // Asegurar que el color base sea BLANCO para que no se vea oscuro
                if (mat.HasProperty("_BaseColor")) mat.SetColor("_BaseColor", Color.white);
                else if (mat.HasProperty("_Color")) mat.SetColor("_Color", Color.white);

                changed = true;
            }

            // --- 2. NORMAL MAP (_n) ---
            string normalPath = FindBestMatch(cleanName, "_n", textureIndex);
            if (normalPath != null)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(normalPath);
                if (mat.HasProperty("_BumpMap"))
                {
                    mat.SetTexture("_BumpMap", tex);
                    mat.EnableKeyword("_NORMALMAP"); // Activar keyword por si acaso
                    changed = true;
                }
            }

            // --- 3. METALLIC / MASK (_m) ---
            string maskPath = FindBestMatch(cleanName, "_m", textureIndex);
            if (maskPath != null)
            {
                Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(maskPath);
                // URP suele usar _MetallicGlossMap, Standard también
                if (mat.HasProperty("_MetallicGlossMap"))
                {
                    mat.SetTexture("_MetallicGlossMap", tex);
                    mat.EnableKeyword("_METALLICSPECGLOSSMAP");
                    changed = true;
                }
                // Si es un shader simple que usa Occlusion map
                if (mat.HasProperty("_OcclusionMap"))
                {
                    mat.SetTexture("_OcclusionMap", tex);
                }
            }

            if (changed)
            {
                EditorUtility.SetDirty(mat);
                updates++;
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✨ MÁGICA COMPLETA: {updates} materiales actualizados con Color, Normales y Máscaras.");
    }

    private string CleanMaterialName(string name)
    {
        string lowerName = name.ToLower();
        if (lowerName.StartsWith("mtl_")) return name.Substring(4);
        if (lowerName.StartsWith("mi_")) return name.Substring(3);
        if (lowerName.StartsWith("m_")) return name.Substring(2);
        return name;
    }

    private string FindBestMatch(string name, string suffix, Dictionary<string, string> index)
    {
        // Buscamos: NombreLimpio + sufijo (ej: ropa_c, ropa_n, ropa_m)
        string candidate = $"{name}{suffix}";

        if (index.ContainsKey(candidate)) return index[candidate];

        // Intento extra: a veces el sufijo es mayúscula
        string candidateUpper = $"{name}{suffix.ToUpper()}";
        if (index.ContainsKey(candidateUpper)) return index[candidateUpper];

        return null;
    }
}