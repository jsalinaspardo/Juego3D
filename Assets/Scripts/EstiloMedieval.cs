using UnityEngine;

public static class EstiloMedieval
{
    static readonly Color Tinta = new Color(0.23f, 0.13f, 0.06f);
    static readonly Color Oro = new Color(0.85f, 0.66f, 0.3f);
    static readonly Color MarronOscuro = new Color(0.2f, 0.11f, 0.05f);

    static Font fuente;
    static Texture2D texPergamino, texSello;
    static GUIStyle estiloPanel, estiloTexto, estiloTecla;

    public static void DibujarAviso(string tecla, string texto, float alpha)
    {
        Preparar();

        float escala = Screen.height / 1080f;
        estiloTexto.fontSize = Mathf.RoundToInt(34 * escala);
        estiloTecla.fontSize = Mathf.RoundToInt(36 * escala);

        float lado = 58 * escala;
        float margen = 28 * escala;
        float hueco = string.IsNullOrEmpty(tecla) ? 0 : lado + 18 * escala;
        Vector2 medida = estiloTexto.CalcSize(new GUIContent(texto));

        float ancho = margen * 2 + hueco + medida.x;
        float alto = Mathf.Max(lado, medida.y) + 30 * escala;
        float x = (Screen.width - ancho) / 2f;
        float y = Screen.height - alto - 70 * escala + (1 - alpha) * 20 * escala;

        Color anterior = GUI.color;
        GUI.color = new Color(1, 1, 1, alpha);

        GUI.Box(new Rect(x, y, ancho, alto), GUIContent.none, estiloPanel);

        if (hueco > 0)
        {
            Rect sello = new Rect(x + margen, y + (alto - lado) / 2f, lado, lado);
            GUI.DrawTexture(sello, texSello);
            GUI.Label(sello, tecla, estiloTecla);
        }

        GUI.Label(new Rect(x + margen + hueco, y, medida.x, alto), texto, estiloTexto);

        GUI.color = anterior;
    }

    static void Preparar()
    {
        if (estiloPanel != null) return;

        fuente = Resources.Load<Font>("Fuentes/MedievalSharp");
        texPergamino = CrearPergamino(256, 96);
        texSello = CrearSello(64);

        estiloPanel = new GUIStyle();
        estiloPanel.normal.background = texPergamino;
        estiloPanel.border = new RectOffset(14, 14, 14, 14);

        estiloTexto = new GUIStyle();
        estiloTexto.font = fuente;
        estiloTexto.alignment = TextAnchor.MiddleLeft;
        estiloTexto.normal.textColor = Tinta;

        estiloTecla = new GUIStyle();
        estiloTecla.font = fuente;
        estiloTecla.alignment = TextAnchor.MiddleCenter;
        estiloTecla.fontStyle = FontStyle.Bold;
        estiloTecla.normal.textColor = Oro;
    }

    static Texture2D CrearPergamino(int ancho, int alto)
    {
        Texture2D tex = NuevaTextura(ancho, alto);
        Color papel = new Color(0.87f, 0.77f, 0.58f);
        Color quemado = new Color(0.55f, 0.36f, 0.18f);

        for (int py = 0; py < alto; py++)
        {
            for (int px = 0; px < ancho; px++)
            {
                int borde = Mathf.Min(px, py, ancho - 1 - px, alto - 1 - py);
                Color c;

                if (borde < 3) c = MarronOscuro;
                else if (borde == 5) c = Oro;
                else
                {
                    float manchas = Mathf.PerlinNoise(px * 0.05f, py * 0.05f) * 0.6f
                                  + Mathf.PerlinNoise(px * 0.2f, py * 0.2f) * 0.4f;
                    float bordeQuemado = Mathf.Clamp01(1f - (borde - 6) / 10f);
                    c = Color.Lerp(papel, quemado, manchas * 0.25f + bordeQuemado * 0.55f);
                }

                c.a = 0.96f;
                tex.SetPixel(px, py, c);
            }
        }

        tex.Apply();
        return tex;
    }

    static Texture2D CrearSello(int lado)
    {
        Texture2D tex = NuevaTextura(lado, lado);
        float radio = lado / 2f - 1;
        Vector2 centro = new Vector2(lado / 2f - 0.5f, lado / 2f - 0.5f);
        Color madera = new Color(0.27f, 0.15f, 0.07f);

        for (int py = 0; py < lado; py++)
        {
            for (int px = 0; px < lado; px++)
            {
                float d = Vector2.Distance(new Vector2(px, py), centro);
                Color c;

                if (d > radio) c = Color.clear;
                else if (d > radio - 4) c = Oro;
                else if (d > radio - 6) c = MarronOscuro;
                else
                {
                    float veta = Mathf.PerlinNoise(px * 0.08f, py * 0.6f);
                    c = Color.Lerp(madera, MarronOscuro, veta * 0.6f);
                }

                tex.SetPixel(px, py, c);
            }
        }

        tex.Apply();
        return tex;
    }

    static Texture2D NuevaTextura(int ancho, int alto)
    {
        Texture2D tex = new Texture2D(ancho, alto, TextureFormat.RGBA32, false);
        tex.wrapMode = TextureWrapMode.Clamp;
        tex.filterMode = FilterMode.Bilinear;
        tex.hideFlags = HideFlags.HideAndDontSave;
        return tex;
    }
}
