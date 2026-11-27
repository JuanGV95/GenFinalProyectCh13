// Constructor de escenas y paneles de UI (solo Editor).
// Menú de Unity:
//   H-EROE > Escenas > Crear Selección de misión   → Assets/Scenes/UI/SeleccionMision.unity
//   H-EROE > Escenas > Crear Victoria              → Assets/Scenes/UI/Victoria.unity
//   H-EROE > Escenas > Crear Derrota               → Assets/Scenes/UI/Derrota.unity
//   H-EROE > Pausa > Crear panel y prefab          → en la escena abierta + Assets/Prefabs/UI/PausePanel.prefab
//   H-EROE > Conectar JUGAR con Selección de misión (con la escena MainMenu abierta)
//   H-EROE > Enlazar ayudas como botones (todas las pantallas)
//
// Todo lo que se puede clicar es un Button con su On Click () enlazado en el Inspector:
// opciones del menú, tarjetas y las teclas de la barra inferior ([ENTER] CONTINUAR, [ESC] VOLVER…).
// Las teclas del teclado y del mando (ESC, ENTER, R, Q/E) las escucha el script de cada pantalla,
// porque un Button no puede detectar una tecla por sí solo.
//
// Las escenas nuevas se agregan solas a Build Settings, igual que MainMenu y EscenaSprint1.
// Usa los sprites de Assets/UI y las fuentes Orbitron de Assets/Fonts.
// Este archivo se puede borrar después de construir: todo queda guardado en escenas y prefabs.
#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using TMPro;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.UI;
#endif

namespace Interfaz.Constructor
{
    public static class ConstructorEscenas
    {
        const string RUTA_UI = "Assets/UI/";
        const string RUTA_FUENTES = "Assets/Fonts/";
        const string CARPETA_ESCENAS = "Assets/Scenes/UI/";
        const string CARPETA_PREFABS = "Assets/Prefabs/UI/";
        const string RUTA_MAINMENU = "Assets/Scenes/SrSwagsitoScenes/MainMenu.unity";
        const string RUTA_SPRINT = "Assets/Scenes/EscenaSprint1/EscenaSprint1.unity";

        static readonly Color HUESO = Hex("D5D9DC");
        static readonly Color AZUL = Hex("8799AB");
        static readonly Color ROJO = Hex("C33A37");
        static readonly Color FRANJA = Hex("0B0F14");
        static readonly Color NEGRO = Hex("11161D");
        static readonly Color ACERO = Hex("273341");

        enum Tipo { TL, C, Lleno }

        // =====================================================================
        // SELECCIÓN DE MISIÓN
        // =====================================================================
        [MenuItem("H-EROE/Escenas/Crear Selección de misión", false, 40)]
        static void CrearSeleccion()
        {
            if (!NuevaEscena(ResultadoMision.ESCENA_SELECCION, out Scene escena, out Transform canvas)) return;
            RectTransform raiz = Nodo(canvas, "MissionSelectPanel", Tipo.Lleno);

            Fondo(raiz);
            MarcaDeAgua(raiz, "MISIONES");
            BarraSuperior(raiz, "LICENCIA: PROVISIONAL", out TextMeshProUGUI fondos, out TextMeshProUGUI rep, out TextMeshProUGUI viales);
            Titulo(raiz, "MISIONES");

            // Pestañas de actos (solo el Acto I está abierto en el MVP)
            Tecla(raiz, "TeclaQ", 120f, 244f, "Q", Color.white, NEGRO, 40f, 34f, "Comun/tecla-cuadrada", 14f);
            string[] actos = { "ACTO I", "ACTO II", "ACTO III", "REJUGABLES" };
            for (int i = 0; i < actos.Length; i++)
            {
                float x = 176f + i * 244f;
                bool activa = i == 0;
                RectTransform p = Nodo(raiz, "Pestana" + (i + 1), Tipo.TL, activa ? x : x - 5f, activa ? 236f : 231f);
                Img(p, activa ? "Configuracion/pestana-activa" : "Configuracion/pestana-normal");
                TextMeshProUGUI t = Txt(Nodo(raiz, "PestanaTexto" + (i + 1), Tipo.TL, x, 236f, 230f, 50f), actos[i], "Bold", 20f,
                    activa ? HUESO : AZUL, TextAlignmentOptions.Center);
                t.characterSpacing = 8f;
            }
            Tecla(raiz, "TeclaE", 1158f, 244f, "E", Color.white, NEGRO, 40f, 34f, "Comun/tecla-cuadrada", 14f);

            DatosMision[] datos =
            {
                new DatosMision { id = "laboratorios-alfa", nombre = "LABORATORIOS ALFA", detalle = "Infiltración y rescate de científicos.",
                    escena = File.Exists(RUTA_SPRINT) ? Path.GetFileNameWithoutExtension(RUTA_SPRINT) : "",
                    pago = 400, reputacion = 40, costo = 0, dificultad = 1 },
                new DatosMision { id = "almacen-del-puerto", nombre = "ALMACÉN DEL PUERTO", detalle = "Detén el envío de prótesis en el muelle.",
                    escena = "", pago = 550, reputacion = 60, costo = 100, dificultad = 2 },
                new DatosMision { id = "rescate-en-el-metro", nombre = "RESCATE EN EL METRO", detalle = "Saca a los civiles antes del apagón.",
                    escena = "", pago = 800, reputacion = 90, costo = 100, dificultad = 3 },
                new DatosMision { id = "planta-de-ensamblaje", nombre = "PLANTA DE ENSAMBLAJE", detalle = "Mini-jefe: Especialista Plasmático.",
                    escena = "", pago = 1500, reputacion = 0, costoViales = 2, dificultad = 3, misionDeActo = true, reputacionRequerida = 300 },
            };

            SeleccionMision control = raiz.gameObject.AddComponent<SeleccionMision>();
            TarjetaMision[] tarjetas = new TarjetaMision[4];
            for (int i = 0; i < 3; i++) tarjetas[i] = Contrato(raiz, i, 120f + i * 370f, control);
            tarjetas[3] = MisionDeActo(raiz, 3, 1250f, control);

            Selectable[] sel = new Selectable[tarjetas.Length];
            for (int i = 0; i < tarjetas.Length; i++)
            {
                sel[i] = tarjetas[i].GetComponent<Button>();
                UnityEventTools.AddIntPersistentListener(((Button)sel[i]).onClick, control.Desplegar, i);
            }
            NavegacionHorizontal(sel);

            TextMeshProUGUI aviso = Txt(Nodo(raiz, "Aviso", Tipo.TL, 120f, 940f, 1680f, 30f), "", "Bold", 18f, ROJO, TextAlignmentOptions.Center);

            RectTransform barra = BarraInferior(raiz, "PROTOTIPO v0.1  /  MISIONES", out float bx);
            bx = Ayuda(barra, "AyudaDesplegar", bx, "ENTER", "DESPLEGAR");
            bx = Ayuda(barra, "AyudaNavegar", bx, "A/D", "NAVEGAR");
            bx = Ayuda(barra, "AyudaPestana", bx, "Q/E", "PESTAÑA");
            Ayuda(barra, "AyudaVolver", bx, "ESC", "VOLVER");

            control.Configurar(datos, tarjetas, fondos, rep, viales, aviso);
            EditorUtility.SetDirty(control);
            ConvertirAyudas(raiz);
            PrimeraSeleccion(tarjetas[0].gameObject);
            Guardar(escena, "Selección de misión creada. Revisa en el Inspector de MissionSelectPanel la escena de cada misión (vacía = PRÓXIMAMENTE).");
        }

        static TarjetaMision Contrato(RectTransform raiz, int indice, float x, SeleccionMision control)
        {
            RectTransform raizT = Nodo(raiz, "Tarjeta" + (indice + 1), Tipo.TL, x, 330f, 340f, 560f);
            Image zona = Img(raizT, null, nativo: false, color: new Color(0, 0, 0, 0), raycast: true);
            RectTransform cuerpo = Nodo(raizT, "Cuerpo", Tipo.TL, 0f, 0f, 340f, 560f);

            Image fondo = Img(Nodo(cuerpo, "Fondo", Tipo.TL, 0f, 0f), "Misiones/tarjeta-normal");
            Button b = raizT.gameObject.AddComponent<Button>();
            b.targetGraphic = fondo;
            b.transition = Selectable.Transition.SpriteSwap;
            Sprite sel = CargarSprite("Misiones/tarjeta-seleccionada");
            b.spriteState = new SpriteState { highlightedSprite = sel, selectedSprite = sel, pressedSprite = sel };

            Image imagen = Img(Nodo(cuerpo, "Imagen", Tipo.TL, 12f, 12f, 316f, 170f), null, nativo: false, color: NEGRO);
            Color marca = ACERO;
            Txt(Nodo(cuerpo, "ImagenTexto", Tipo.TL, 12f, 70f, 316f, 60f), "ACTO I", "Black", 44f, marca, TextAlignmentOptions.Center);

            TarjetaMision t = raizT.gameObject.AddComponent<TarjetaMision>();
            t.seleccion = control;
            t.indice = indice;
            t.cuerpo = cuerpo;
            t.imagen = imagen;
            t.nombre = Txt(Nodo(cuerpo, "Nombre", Tipo.TL, 24f, 198f, 296f, 52f), "MISIÓN", "Black", 20f, HUESO, ajustar: true);
            t.detalle = Txt(Nodo(cuerpo, "Detalle", Tipo.TL, 24f, 252f, 296f, 46f), "", "Medium", 14f, AZUL, ajustar: true);
            Color sep = AZUL; sep.a = 0.4f;
            Img(Nodo(cuerpo, "Separador", Tipo.TL, 24f, 308f, 292f, 1f), null, nativo: false, color: sep);
            Etiqueta(cuerpo, "PagoEtiqueta", 24f, 324f, 292f, "PAGO POR CORRIDA");
            t.pago = Txt(Nodo(cuerpo, "Pago", Tipo.TL, 24f, 342f, 296f, 30f), "", "Black", 22f, HUESO);
            Etiqueta(cuerpo, "DificultadEtiqueta", 24f, 390f, 140f, "DIFICULTAD");
            t.dificultad = new Image[3];
            for (int i = 0; i < 3; i++)
                t.dificultad[i] = Img(Nodo(cuerpo, "Dificultad" + (i + 1), Tipo.TL, 24f + i * 22f, 410f, 16f, 16f), null, nativo: false, color: HUESO);
            Etiqueta(cuerpo, "MejorEtiqueta", 170f, 390f, 150f, "MEJOR / CORRIDAS");
            t.mejor = Txt(Nodo(cuerpo, "Mejor", Tipo.TL, 170f, 407f, 150f, 24f), "-  /  x0", "Black", 18f, HUESO);
            t.costo = Txt(Nodo(cuerpo, "Costo", Tipo.TL, 24f, 450f, 296f, 18f), "", "Bold", 12f, AZUL);
            t.costo.characterSpacing = 4f;
            t.estado = Txt(Nodo(cuerpo, "Estado", Tipo.TL, 24f, 494f, 292f, 26f), "", "Black", 18f, AZUL, TextAlignmentOptions.Center);
            t.botonDesplegar = BotonDesplegar(cuerpo, 24f, 482f);
            EditorUtility.SetDirty(t);
            return t;
        }

        static TarjetaMision MisionDeActo(RectTransform raiz, int indice, float x, SeleccionMision control)
        {
            RectTransform raizT = Nodo(raiz, "MisionDeActo", Tipo.TL, x, 330f, 550f, 560f);
            Img(raizT, null, nativo: false, color: new Color(0, 0, 0, 0), raycast: true);
            RectTransform cuerpo = Nodo(raizT, "Cuerpo", Tipo.TL, 0f, 0f, 550f, 560f);

            Image fondo = Img(Nodo(cuerpo, "Fondo", Tipo.TL, 0f, 0f), "Misiones/mision-acto");
            Button b = raizT.gameObject.AddComponent<Button>();
            b.targetGraphic = fondo;
            b.transition = Selectable.Transition.SpriteSwap;
            Sprite sel = CargarSprite("Misiones/mision-acto-seleccionada");
            b.spriteState = new SpriteState { highlightedSprite = sel, selectedSprite = sel, pressedSprite = sel };

            TextMeshProUGUI e = Txt(Nodo(cuerpo, "Etiqueta", Tipo.TL, 30f, 30f, 490f, 18f), "MISIÓN DE ACTO  /  ABRE EL ACTO II", "Bold", 13f, ROJO);
            e.characterSpacing = 8f;

            TarjetaMision t = raizT.gameObject.AddComponent<TarjetaMision>();
            t.seleccion = control;
            t.indice = indice;
            t.cuerpo = cuerpo;
            t.subida = 10f;
            t.candado = Nodo(cuerpo, "Candado", Tipo.TL, 235f, 72f).gameObject;
            Img((RectTransform)t.candado.transform, "Misiones/candado");
            t.nombre = Txt(Nodo(cuerpo, "Nombre", Tipo.TL, 30f, 196f, 490f, 40f), "MISIÓN DE ACTO", "Black", 28f, HUESO, TextAlignmentOptions.Top);
            t.detalle = Txt(Nodo(cuerpo, "Detalle", Tipo.TL, 30f, 240f, 490f, 22f), "", "Medium", 15f, AZUL, TextAlignmentOptions.Top);
            Etiqueta(cuerpo, "BarraEtiqueta", 30f, 356f, 300f, "REPUTACIÓN");
            t.barraTexto = Txt(Nodo(cuerpo, "BarraTexto", Tipo.TL, 340f, 352f, 180f, 20f), "000 / 300", "Black", 15f, HUESO, TextAlignmentOptions.TopRight);
            Img(Nodo(cuerpo, "BarraPista", Tipo.TL, 30f, 378f, 490f, 12f), null, nativo: false, color: ACERO);
            t.barraValor = Nodo(cuerpo, "BarraValor", Tipo.TL, 30f, 378f, 0f, 12f);
            Img(t.barraValor, null, nativo: false, color: ROJO);
            t.anchoBarra = 490f;
            t.faltan = Txt(Nodo(cuerpo, "Faltan", Tipo.TL, 30f, 404f, 490f, 28f), "", "Black", 20f, HUESO);
            t.pista = Txt(Nodo(cuerpo, "Pista", Tipo.TL, 30f, 436f, 490f, 40f), "Juega las misiones de este acto para subir tu reputación.", "Medium", 14f, AZUL, ajustar: true);
            Color sep = AZUL; sep.a = 0.4f;
            Img(Nodo(cuerpo, "Separador", Tipo.TL, 30f, 486f, 490f, 1f), null, nativo: false, color: sep);
            t.costo = Txt(Nodo(cuerpo, "Costo", Tipo.TL, 30f, 502f, 490f, 18f), "", "Bold", 12f, AZUL);
            t.costo.characterSpacing = 4f;
            t.botonDesplegar = BotonDesplegar(cuerpo, 129f, 412f);
            EditorUtility.SetDirty(t);
            return t;
        }

        static GameObject BotonDesplegar(RectTransform padre, float x, float y)
        {
            RectTransform b = Nodo(padre, "BotonDesplegar", Tipo.TL, x, y, 292f, 52f);
            Img(b, "Misiones/boton-desplegar");
            Tecla(b, "TeclaEnter", 16f, 11f, "ENTER", FRANJA, HUESO, 66f, 30f);
            Txt(Nodo(b, "Texto", Tipo.TL, 94f, 13f, 190f, 28f), "DESPLEGAR", "Black", 20f, HUESO);
            b.gameObject.SetActive(false);
            return b.gameObject;
        }

        static void Etiqueta(RectTransform padre, string nombre, float x, float y, float w, string texto)
        {
            TextMeshProUGUI t = Txt(Nodo(padre, nombre, Tipo.TL, x, y, w, 16f), texto, "Bold", 11f, AZUL);
            t.characterSpacing = 6f;
        }

        // =====================================================================
        // VICTORIA Y DERROTA
        // =====================================================================
        [MenuItem("H-EROE/Escenas/Crear Victoria", false, 41)]
        static void CrearVictoria() => CrearResultado(true);

        [MenuItem("H-EROE/Escenas/Crear Derrota", false, 42)]
        static void CrearDerrota() => CrearResultado(false);

        static void CrearResultado(bool victoria)
        {
            string nombreEscena = victoria ? ResultadoMision.ESCENA_VICTORIA : ResultadoMision.ESCENA_DERROTA;
            if (!NuevaEscena(nombreEscena, out Scene escena, out Transform canvas)) return;
            RectTransform raiz = Nodo(canvas, victoria ? "VictoryPanel" : "DefeatPanel", Tipo.Lleno);

            Fondo(raiz);
            MarcaDeAgua(raiz, "LIQUIDACIÓN");
            BarraSuperior(raiz, victoria ? "LICENCIA: APROBADA" : "LICENCIA: EN RIESGO",
                out TextMeshProUGUI fondos, out TextMeshProUGUI rep, out TextMeshProUGUI viales);
            Titulo(raiz, victoria ? "¡MISIÓN COMPLETADA!" : "MISIÓN FALLIDA");

            // Hoja de Liquidación
            RectTransform hoja = Nodo(raiz, "Hoja", Tipo.C, 664f, 595f, 1100f, 650f, 1f);
            Img(Nodo(hoja, "Fondo", Tipo.TL, -7f, -7f), "Resultados/hoja-fondo");
            Img(Nodo(hoja, "Banda", Tipo.TL, -14f, 23f), "Resultados/hoja-banda");
            Txt(Nodo(hoja, "BandaTexto", Tipo.TL, 22f, 42f, 600f, 28f), "HOJA DE LIQUIDACIÓN  /  GDN", "Black", 22f, HUESO);

            RectTransform vineta = Nodo(hoja, "VinetaAgente", Tipo.C, 169f, 279f, 240f, 330f, -3f);
            Img(Nodo(vineta, "Marco", Tipo.TL, -4f, -4f), "Resultados/vineta-agente");
            TextMeshProUGUI agente = Txt(Nodo(vineta, "Nombre", Tipo.TL, 20f, 270f, 200f, 28f), "MAYA", "Black", 22f, HUESO);
            TextMeshProUGUI alias = Txt(Nodo(vineta, "Alias", Tipo.TL, 20f, 300f, 200f, 15f), "SOMBRA", "Bold", 12f, ROJO);

            TextMeshProUGUI[] etiquetas = new TextMeshProUGUI[5];
            TextMeshProUGUI[] valores = new TextMeshProUGUI[5];
            for (int i = 0; i < 5; i++)
            {
                float y = 124f + i * 52f;
                etiquetas[i] = Txt(Nodo(hoja, "Fila" + (i + 1), Tipo.TL, 330f, y, 520f, 24f), "", "Bold", 18f, HUESO);
                valores[i] = Txt(Nodo(hoja, "Valor" + (i + 1), Tipo.TL, 860f, y, 200f, 26f), "", "Black", 20f, HUESO, TextAlignmentOptions.TopRight);
            }
            Color sep = AZUL; sep.a = 0.5f;
            Img(Nodo(hoja, "Separador", Tipo.TL, 330f, 392f, 730f, 2f), null, nativo: false, color: sep);
            Txt(Nodo(hoja, "TotalEtiqueta", Tipo.TL, 330f, 418f, 400f, 30f), "PUNTUACIÓN FINAL", "Black", 24f, HUESO);
            TextMeshProUGUI total = Txt(Nodo(hoja, "TotalValor", Tipo.TL, 860f, 406f, 200f, 50f), "0", "Black", 40f, HUESO, TextAlignmentOptions.TopRight);
            TextMeshProUGUI re = Txt(Nodo(hoja, "ReputacionEtiqueta", Tipo.TL, 330f, 500f, 300f, 18f), "REPUTACIÓN GDN", "Bold", 14f, AZUL);
            re.characterSpacing = 6f;
            Img(Nodo(hoja, "BarraPista", Tipo.TL, 330f, 530f, 730f, 24f), null, nativo: false, color: FRANJA);
            RectTransform barraValor = Nodo(hoja, "BarraValor", Tipo.TL, 330f, 530f, 0f, 24f);
            Img(barraValor, null, nativo: false, color: ROJO);
            TextMeshProUGUI barraTexto = Txt(Nodo(hoja, "BarraTexto", Tipo.TL, 330f, 566f, 730f, 18f), "", "Medium", 14f, AZUL, TextAlignmentOptions.TopRight);

            // Coordinadora
            Globo(raiz, 1028f, 133f, 800f, 125f, 1060f, 112f, 62f, 31f, 700f,
                victoria ? "¡Buen trabajo! Aunque los daños en la recepción se descuentan de la recompensa."
                         : "Esto va a tu expediente. Y a tu sueldo. Recursos Humanos te espera el lunes.", 17f);

            // Calificación
            RectTransform cal = Nodo(raiz, "Calificacion", Tipo.C, 1535f, 418f, 340f, 340f, -8f);
            Img(Nodo(cal, "Estallido", Tipo.TL, -5f, -5f), victoria ? "Resultados/estallido-s" : "Resultados/estallido-d");
            TextMeshProUGUI letraSombra = Txt(Nodo(cal, "LetraSombra", Tipo.TL, 6f, 56f, 340f, 220f), victoria ? "S" : "D", "Black", 170f, FRANJA, TextAlignmentOptions.Center);
            TextMeshProUGUI letra = Txt(Nodo(cal, "Letra", Tipo.TL, 0f, 50f, 340f, 220f), victoria ? "S" : "D", "Black", 170f, HUESO, TextAlignmentOptions.Center);
            TextMeshProUGUI ce = Txt(Nodo(raiz, "CalificacionEtiqueta", Tipo.TL, 1410f, 630f, 250f, 20f), "CALIFICACIÓN", "Bold", 15f, AZUL, TextAlignmentOptions.Top);
            ce.characterSpacing = 10f;

            // Sello
            RectTransform sello = Nodo(raiz, "Sello", Tipo.C, 1518f, 727f, 460f, 70f, 3f);
            Img(Nodo(sello, "Marco", Tipo.TL, 0f, 0f), victoria ? "Resultados/sello-aprobado" : "Resultados/sello-advertencia");
            Txt(Nodo(sello, "Texto", Tipo.Lleno), victoria ? "LICENCIA PROVISIONAL APROBADA" : "ADVERTENCIA DE RR.HH.", "Black", 19f, ROJO, TextAlignmentOptions.Center);

            // Bonificación / sanción
            Img(Nodo(raiz, "RecompensaFondo", Tipo.TL, 1265f, 785f), victoria ? "Resultados/bonificacion-fondo" : "Resultados/sancion-fondo");
            TextMeshProUGUI bt = Txt(Nodo(raiz, "RecompensaTitulo", Tipo.TL, 1300f, 812f, 400f, 18f), victoria ? "RECOMPENSA" : "SANCIÓN", "Bold", 14f, ROJO);
            bt.characterSpacing = 8f;
            Img(Nodo(raiz, "RecompensaVial", Tipo.TL, 1302f, 852f), "Comun/vial-gen-h");
            TextMeshProUGUI recompensa = Txt(Nodo(raiz, "RecompensaTexto", Tipo.TL, 1330f, 852f, 450f, 30f), "", "Black", 22f, HUESO);

            RectTransform barra = BarraInferior(raiz, "MISIÓN  /  RESULTADOS", out float bx);
            if (victoria)
            {
                bx = Ayuda(barra, "AyudaContinuar", bx, "ENTER", "CONTINUAR");
                Ayuda(barra, "AyudaRepetir", bx, "R", "REPETIR MISIÓN");
            }
            else
            {
                bx = Ayuda(barra, "AyudaContinuar", bx, "ENTER", "REINTENTAR");
                Ayuda(barra, "AyudaVolver", bx, "ESC", "VOLVER A MISIONES");
            }

            PantallaResultado control = raiz.gameObject.AddComponent<PantallaResultado>();
            control.Configurar(victoria, etiquetas, valores, total, barraValor, barraTexto, agente, alias, letra, recompensa, fondos, rep, viales);
            EditorUtility.SetDirty(control);
            // La sombra de la letra sigue a la letra: se actualiza en el Editor y PantallaResultado escribe solo "Letra".
            letraSombra.gameObject.AddComponent<CopiaTexto>().origen = letra;
            ConvertirAyudas(raiz);

            Guardar(escena, (victoria ? "Victoria" : "Derrota") + " creada.");
        }

        // =====================================================================
        // PAUSA (panel en la escena abierta + prefab)
        // =====================================================================
        [MenuItem("H-EROE/Pausa/Crear panel y prefab", false, 50)]
        static void CrearPausa()
        {
            Scene escena = SceneManager.GetActiveScene();
            if (string.IsNullOrEmpty(escena.path))
            {
                EditorUtility.DisplayDialog("H-EROE", "Guarda primero la escena abierta (por ejemplo EscenaSprint1) y vuelve a intentarlo.", "OK");
                return;
            }
            string rutaPrefab = CARPETA_PREFABS + "PausePanel.prefab";
            if (AssetDatabase.LoadAssetAtPath<GameObject>(rutaPrefab) != null &&
                !EditorUtility.DisplayDialog("H-EROE", "Ya existe el prefab PausePanel. ¿Reemplazarlo?", "Reemplazar", "Cancelar"))
                return;
            GameObject existente = GameObject.Find("PausePanel");
            if (existente != null) Undo.DestroyObjectImmediate(existente);

            AsegurarEventSystem();
            Canvas canvas = CrearCanvas("PausePanel", 100);
            RectTransform raiz = (RectTransform)canvas.transform;
            Undo.RegisterCreatedObjectUndo(canvas.gameObject, "Crear PausePanel");

            RectTransform cont = Nodo(raiz, "Contenido", Tipo.Lleno);
            Color osc = FRANJA; osc.a = 0.82f;
            Img(Nodo(cont, "Oscurecer", Tipo.Lleno), null, nativo: false, color: osc, raycast: true);

            RectTransform titulo = Nodo(cont, "Titulo", Tipo.C, 405f, 206f, 600f, 150f, 4f);
            Txt(Nodo(titulo, "Sombra", Tipo.TL, 6f, 9f, 600f, 150f), "PAUSA", "Black", 110f, ROJO);
            Txt(Nodo(titulo, "Texto", Tipo.TL, 0f, 0f, 600f, 150f), "PAUSA", "Black", 110f, HUESO);

            // Menú (mismo dibujo y comportamiento que el menú principal)
            RectTransform menu = Nodo(cont, "Menu", Tipo.C, 440f, 640f, 660f, 540f, 3f);
            Img(Nodo(menu, "Panel", Tipo.TL, -8f, -8f), "Menu/menu-panel");
            string[] nombres = { "OpContinuar", "OpReiniciar", "OpSalir" };
            string[] textos = { "CONTINUAR", "REINICIAR MISIÓN", "SALIR A MISIONES" };
            string[] descripciones =
            {
                "Vuelve a la misión. Los ciborgs no se desactivan solos.",
                "Empieza la misión de nuevo. El progreso de esta corrida se pierde.",
                "Abandona la misión y vuelve a la Selección de misión.",
            };
            float[] ys = { 56f, 148f, 224f };
            Sprite sNormal = CargarSprite("Menu/opcion-normal");
            Sprite sSeleccion = CargarSprite("Menu/menu-seleccion");
            Sprite sPresionada = CargarSprite("Menu/opcion-presionada");
            Button[] botones = new Button[3];
            OpcionMenu[] opciones = new OpcionMenu[3];
            for (int i = 0; i < 3; i++)
            {
                RectTransform op = Nodo(menu, nombres[i], Tipo.TL, 66f, ys[i], 541f, 71f);
                Image img = op.gameObject.AddComponent<Image>();
                img.sprite = sNormal;
                img.raycastTarget = true;
                Button b = op.gameObject.AddComponent<Button>();
                b.targetGraphic = img;
                b.transition = Selectable.Transition.SpriteSwap;
                b.spriteState = new SpriteState { highlightedSprite = sSeleccion, selectedSprite = sSeleccion, pressedSprite = sPresionada, disabledSprite = sNormal };
                botones[i] = b;
                RectTransform tecla = Tecla(op, "TeclaEnter", 26f, 18f, "ENTER", FRANJA, HUESO, 70f, 30f);
                tecla.gameObject.SetActive(i == 0);
                RectTransform texto = Nodo(op, "Texto", Tipo.TL, i == 0 ? 114f : 44f, 16f, 440f, 40f);
                TextMeshProUGUI tmp = Txt(texto, textos[i], "Black", i == 0 ? 32f : 30f, HUESO);
                OpcionMenu opcion = op.gameObject.AddComponent<OpcionMenu>();
                opcion.Configurar(texto, tmp, tecla.gameObject, descripciones[i]);
                opciones[i] = opcion;
            }
            NavegacionVertical(botones);
            TextMeshProUGUI descTxt = Txt(Nodo(menu, "Descripcion", Tipo.TL, 62f, 462f, 520f, 22f), descripciones[0], "Medium", 16f, HUESO);
            MenuPrincipal controlMenu = menu.gameObject.AddComponent<MenuPrincipal>();
            controlMenu.Configurar(opciones, descTxt);
            foreach (OpcionMenu o in opciones) { o.AsignarMenu(controlMenu); EditorUtility.SetDirty(o); }
            EditorUtility.SetDirty(controlMenu);

            // Estado de la misión
            RectTransform estado = Nodo(cont, "EstadoMision", Tipo.C, 1440f, 507f, 740f, 580f, -2f);
            Img(Nodo(estado, "Fondo", Tipo.TL, -7f, -7f), "Pausa/estado-mision-panel");
            Img(Nodo(estado, "Banda", Tipo.TL, -17f, 23f), "Pausa/banda-mision");
            TextMeshProUGUI tituloMision = Txt(Nodo(estado, "MisionTitulo", Tipo.TL, 20f, 42f, 640f, 28f), "MISIÓN: LABORATORIOS ALFA", "Black", 22f, HUESO);
            Txt(Nodo(estado, "Objetivo", Tipo.TL, 34f, 108f, 660f, 20f), "OBJETIVO: rescata a los científicos secuestrados", "Medium", 16f, AZUL);
            string[] datos = { "REHENES", "CIBORGS", "VIALES DE GEN H", "DAÑOS", "TIEMPO", "AGENTE" };
            TextMeshProUGUI[] valores = new TextMeshProUGUI[6];
            for (int i = 0; i < 6; i++)
            {
                RectTransform caja = Nodo(estado, "Dato" + (i + 1), Tipo.TL, 34f + (i % 3) * 230f, i < 3 ? 150f : 280f, 210f, 110f);
                Img(caja, null, nativo: false, color: FRANJA);
                TextMeshProUGUI et = Txt(Nodo(caja, "Etiqueta", Tipo.TL, 18f, 16f, 180f, 16f), datos[i], "Bold", 13f, AZUL);
                et.characterSpacing = 4f;
                valores[i] = Txt(Nodo(caja, "Valor", Tipo.TL, 18f, 40f, 180f, 44f), "0", "Black", 34f, HUESO);
            }
            Txt(Nodo(estado, "NotaCoordinadora", Tipo.TL, 34f, 430f, 650f, 52f),
                "COORDINADORA: \"Pausa de cinco minutos. El reloj de multas sigue corriendo.\"", "Bold", 16f, HUESO, ajustar: true);
            Img(Nodo(estado, "Rayo", Tipo.TL, 718f, 329f), "Pausa/rayo-estado");

            RectTransform barra = BarraInferior(cont, "MISIÓN  /  PAUSA", out float bx);
            bx = Ayuda(barra, "AyudaSeleccionar", bx, "ENTER", "SELECCIONAR");
            bx = Ayuda(barra, "AyudaNavegar", bx, "W/S", "NAVEGAR");
            Ayuda(barra, "AyudaContinuar", bx, "ESC", "CONTINUAR");

            PanelPausa pausa = canvas.gameObject.AddComponent<PanelPausa>();
            pausa.Configurar(cont.gameObject, tituloMision, valores[0], valores[1], valores[2], valores[3], valores[4], valores[5]);
            UnityEventTools.AddVoidPersistentListener(botones[0].onClick, pausa.Reanudar);
            UnityEventTools.AddVoidPersistentListener(botones[1].onClick, pausa.ReiniciarMision);
            UnityEventTools.AddVoidPersistentListener(botones[2].onClick, pausa.SalirDeLaMision);
            EditorUtility.SetDirty(pausa);
            ConvertirAyudas(raiz);

            cont.gameObject.SetActive(false); // arranca oculto; PanelPausa lo muestra con ESC

            Directory.CreateDirectory(CARPETA_PREFABS);
            PrefabUtility.SaveAsPrefabAssetAndConnect(canvas.gameObject, rutaPrefab, InteractionMode.UserAction);
            AgregarABuild(escena.path);
            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
            Selection.activeGameObject = canvas.gameObject;
            Debug.Log("[H-EROE] Pausa creada en la escena y guardada como prefab en " + rutaPrefab +
                      ". Para los demás niveles, arrastra el prefab a la escena. Para editarla, activa su hijo Contenido.");
        }

        // =====================================================================
        // MENÚ PRINCIPAL: JUGAR → SELECCIÓN DE MISIÓN
        // =====================================================================
        [MenuItem("H-EROE/Conectar JUGAR con Selección de misión", false, 60)]
        static void ConectarJugar()
        {
            GameObject op = GameObject.Find("OpJugar");
            Button b = op != null ? op.GetComponent<Button>() : null;
            if (b == null)
            {
                EditorUtility.DisplayDialog("H-EROE", "No encontré la opción JUGAR (OpJugar). Abre la escena MainMenu y vuelve a intentarlo.", "OK");
                return;
            }
            Undo.RecordObject(b, "Conectar JUGAR");
            CambioEscena cambio = op.GetComponent<CambioEscena>();
            if (cambio == null) cambio = Undo.AddComponent<CambioEscena>(op);
            for (int i = b.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                if (b.onClick.GetPersistentTarget(i) == cambio) UnityEventTools.RemovePersistentListener(b.onClick, i);
            UnityEventTools.AddStringPersistentListener(b.onClick, cambio.Cargar, ResultadoMision.ESCENA_SELECCION);
            EditorUtility.SetDirty(b);
            AgregarABuild(SceneManager.GetActiveScene().path);
            EditorSceneManager.MarkSceneDirty(op.scene);
            Debug.Log("[H-EROE] JUGAR ahora abre la Selección de misión. Guarda la escena con Ctrl+S.");
        }

        // =====================================================================
        // AYUDAS DE LA BARRA INFERIOR → BUTTONS
        // =====================================================================
        [MenuItem("H-EROE/Enlazar ayudas como botones (todas las pantallas)", false, 61)]
        static void EnlazarAyudasEnTodo()
        {
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;
            string original = SceneManager.GetActiveScene().path;
            System.Text.StringBuilder log = new System.Text.StringBuilder();

            foreach (string nombre in new[] { ResultadoMision.ESCENA_SELECCION, ResultadoMision.ESCENA_VICTORIA, ResultadoMision.ESCENA_DERROTA })
            {
                string ruta = CARPETA_ESCENAS + nombre + ".unity";
                if (!File.Exists(ruta)) continue;
                Scene s = EditorSceneManager.OpenScene(ruta, OpenSceneMode.Single);
                int n = 0;
                foreach (GameObject r in s.GetRootGameObjects()) n += ConvertirAyudas(r.transform);
                EditorSceneManager.MarkSceneDirty(s);
                EditorSceneManager.SaveScene(s);
                log.Append($"{nombre}: {n} botones. ");
            }

            string rutaPrefab = CARPETA_PREFABS + "PausePanel.prefab";
            if (File.Exists(rutaPrefab))
            {
                GameObject contenido = PrefabUtility.LoadPrefabContents(rutaPrefab);
                int n = ConvertirAyudas(contenido.transform);
                PrefabUtility.SaveAsPrefabAsset(contenido, rutaPrefab);
                PrefabUtility.UnloadPrefabContents(contenido);
                log.Append($"PausePanel: {n} botones.");
            }

            if (!string.IsNullOrEmpty(original)) EditorSceneManager.OpenScene(original, OpenSceneMode.Single);
            Debug.Log("[H-EROE] Ayudas enlazadas como botones. " + log);
        }

        // Convierte en Button las teclas de la barra inferior y les enlaza su método en el On Click ().
        static int ConvertirAyudas(Transform raiz)
        {
            PantallaResultado resultado = raiz.GetComponentInChildren<PantallaResultado>(true);
            SeleccionMision seleccion = raiz.GetComponentInChildren<SeleccionMision>(true);
            PanelPausa pausa = raiz.GetComponentInChildren<PanelPausa>(true);
            int n = 0;
            if (resultado != null)
            {
                n += Enlazar(raiz, "AyudaContinuar", resultado.Continuar);
                n += Enlazar(raiz, "AyudaRepetir", resultado.Repetir);
                n += Enlazar(raiz, "AyudaVolver", resultado.Volver);
            }
            if (seleccion != null)
            {
                n += Enlazar(raiz, "AyudaDesplegar", seleccion.DesplegarSeleccionada);
                n += Enlazar(raiz, "AyudaVolver", seleccion.Volver);
                n += Enlazar(raiz, "AyudaPestana", seleccion.AvisarActos);
                n += Enlazar(raiz, "TeclaQ", seleccion.AvisarActos);
                n += Enlazar(raiz, "TeclaE", seleccion.AvisarActos);
            }
            if (pausa != null)
                n += Enlazar(raiz, "AyudaContinuar", pausa.Reanudar);
            return n;
        }

        static int Enlazar(Transform raiz, string nombre, UnityAction accion)
        {
            Transform t = Buscar(raiz, nombre);
            if (t == null) return 0;

            Image grafico = t.GetComponent<Image>();
            if (grafico == null)
            {
                Transform tecla = t.Find("Tecla");
                grafico = tecla != null ? tecla.GetComponent<Image>() : t.GetComponentInChildren<Image>(true);
            }
            foreach (Graphic g in t.GetComponentsInChildren<Graphic>(true)) g.raycastTarget = true;

            Button b = t.GetComponent<Button>();
            if (b == null) b = t.gameObject.AddComponent<Button>();
            b.targetGraphic = grafico;
            b.transition = Selectable.Transition.ColorTint;
            ColorBlock c = ColorBlock.defaultColorBlock;
            c.highlightedColor = new Color(1f, 0.72f, 0.7f);
            c.pressedColor = ROJO;
            c.selectedColor = Color.white;
            b.colors = c;
            // Sin navegación: el teclado y el mando se quedan en el menú o en las tarjetas.
            b.navigation = new Navigation { mode = Navigation.Mode.None };

            for (int i = b.onClick.GetPersistentEventCount() - 1; i >= 0; i--)
                UnityEventTools.RemovePersistentListener(b.onClick, i);
            UnityEventTools.AddVoidPersistentListener(b.onClick, accion);
            EditorUtility.SetDirty(b);
            return 1;
        }

        static Transform Buscar(Transform t, string nombre)
        {
            if (t.name == nombre) return t;
            foreach (Transform h in t)
            {
                Transform r = Buscar(h, nombre);
                if (r != null) return r;
            }
            return null;
        }

        // =====================================================================
        // ESCENAS
        // =====================================================================
        static bool NuevaEscena(string nombre, out Scene escena, out Transform canvas)
        {
            escena = default;
            canvas = null;
            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return false;
            string ruta = CARPETA_ESCENAS + nombre + ".unity";
            if (File.Exists(ruta) &&
                !EditorUtility.DisplayDialog("H-EROE", $"Ya existe la escena {nombre}. ¿Reemplazarla? Se pierden los cambios hechos a mano.", "Reemplazar", "Cancelar"))
                return false;

            Directory.CreateDirectory(CARPETA_ESCENAS);
            escena = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            GameObject cam = new GameObject("Main Camera", typeof(Camera), typeof(AudioListener));
            cam.tag = "MainCamera";
            Camera c = cam.GetComponent<Camera>();
            c.clearFlags = CameraClearFlags.SolidColor;
            c.backgroundColor = NEGRO;
            c.orthographic = true;

            AsegurarEventSystem();
            canvas = CrearCanvas("UICanvas", 0).transform;

            EditorSceneManager.SaveScene(escena, ruta);
            AgregarABuild(ruta);
            if (File.Exists(RUTA_MAINMENU)) AgregarABuild(RUTA_MAINMENU);
            if (File.Exists(RUTA_SPRINT)) AgregarABuild(RUTA_SPRINT);
            return true;
        }

        static void Guardar(Scene escena, string mensaje)
        {
            EditorSceneManager.MarkSceneDirty(escena);
            EditorSceneManager.SaveScene(escena);
            Debug.Log("[H-EROE] " + mensaje + " Escena guardada en " + escena.path + " y agregada a Build Settings.");
        }

        static Canvas CrearCanvas(string nombre, int orden)
        {
            GameObject go = new GameObject(nombre, typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            go.layer = LayerMask.NameToLayer("UI");
            Canvas c = go.GetComponent<Canvas>();
            c.renderMode = RenderMode.ScreenSpaceOverlay;
            c.sortingOrder = orden;
            CanvasScaler s = go.GetComponent<CanvasScaler>();
            s.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            s.referenceResolution = new Vector2(1920f, 1080f);
            s.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            s.matchWidthOrHeight = 0.5f;
            return c;
        }

        static void AsegurarEventSystem()
        {
            if (Object.FindFirstObjectByType<EventSystem>() != null) return;
            GameObject es = new GameObject("EventSystem", typeof(EventSystem));
#if ENABLE_INPUT_SYSTEM
            es.AddComponent<InputSystemUIInputModule>();
#else
            es.AddComponent<StandaloneInputModule>();
#endif
            Undo.RegisterCreatedObjectUndo(es, "Crear EventSystem");
        }

        static void PrimeraSeleccion(GameObject go)
        {
            EventSystem es = Object.FindFirstObjectByType<EventSystem>();
            if (es == null) return;
            es.firstSelectedGameObject = go;
            EditorUtility.SetDirty(es);
        }

        static void AgregarABuild(string ruta)
        {
            if (string.IsNullOrEmpty(ruta)) return;
            List<EditorBuildSettingsScene> lista = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
            if (lista.Exists(e => e.path == ruta)) return;
            lista.Add(new EditorBuildSettingsScene(ruta, true));
            EditorBuildSettings.scenes = lista.ToArray();
        }

        // =====================================================================
        // PIEZAS COMUNES (iguales a las del menú principal)
        // =====================================================================
        static void Fondo(RectTransform raiz)
        {
            RectTransform fondo = Nodo(raiz, "Background", Tipo.Lleno);
            Img(fondo, "Comun/fondo-base", nativo: false);
            AspectRatioFitter arf = fondo.gameObject.AddComponent<AspectRatioFitter>();
            arf.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
            arf.aspectRatio = 16f / 9f;
        }

        static void MarcaDeAgua(RectTransform raiz, string texto)
        {
            Color c = HUESO; c.a = 0.05f;
            Txt(Nodo(raiz, "MarcaDeAgua", Tipo.TL, 90f, 70f, 1700f, 213f), texto, "Black", 170f, c);
        }

        static void Titulo(RectTransform raiz, string texto)
        {
            RectTransform t = Nodo(raiz, "Titulo", Tipo.C, 570f, 155f, 900f, 90f, 2f);
            Txt(Nodo(t, "Sombra", Tipo.TL, 4f, 7f, 900f, 90f), texto, "Black", 60f, ROJO);
            Txt(Nodo(t, "Texto", Tipo.TL, 0f, 0f, 900f, 90f), texto, "Black", 60f, HUESO);
        }

        static void BarraSuperior(RectTransform padre, string licencia, out TextMeshProUGUI fondos, out TextMeshProUGUI reputacion, out TextMeshProUGUI viales)
        {
            RectTransform barra = Nodo(padre, "BarraSuperior", Tipo.TL, 0f, 0f);
            Img(barra, "Comun/barra-superior");
            Txt(Nodo(barra, "Terminal", Tipo.TL, 120f, 24f, 340f, 18f), "GDN // TERMINAL DE DESPACHO", "SemiBold", 14f, AZUL);
            Img(Nodo(barra, "LicenciaFondo", Tipo.TL, 960f, 8f), "Comun/etiqueta-licencia");
            Txt(Nodo(barra, "LicenciaTexto", Tipo.TL, 988f, 25f, 230f, 16f), licencia, "Bold", 13f, ROJO);
            Txt(Nodo(barra, "FondosEtiqueta", Tipo.TL, 1278f, 25f, 75f, 16f), "FONDOS", "SemiBold", 13f, AZUL);
            fondos = Txt(Nodo(barra, "FondosValor", Tipo.TL, 1356f, 23f, 110f, 20f), "$1.240", "Black", 16f, HUESO);
            Txt(Nodo(barra, "RepEtiqueta", Tipo.TL, 1488f, 25f, 120f, 16f), "REPUTACIÓN", "SemiBold", 13f, AZUL);
            reputacion = Txt(Nodo(barra, "RepValor", Tipo.TL, 1618f, 23f, 70f, 20f), "000", "Black", 16f, HUESO);
            Txt(Nodo(barra, "GenHEtiqueta", Tipo.TL, 1708f, 25f, 60f, 16f), "GEN H", "SemiBold", 13f, AZUL);
            float[] xs = { 1782f, 1800f, 1818f };
            for (int i = 0; i < xs.Length; i++)
                Img(Nodo(barra, "Vial" + (i + 1), Tipo.TL, xs[i], 19f), "Comun/vial-gen-h");
            viales = Txt(Nodo(barra, "GenHValor", Tipo.TL, 1844f, 23f, 60f, 20f), "3/3", "Black", 16f, HUESO);
        }

        static RectTransform BarraInferior(RectTransform padre, string version, out float x)
        {
            RectTransform barra = Nodo(padre, "BarraInferior", Tipo.TL, 0f, 1001f);
            Img(barra, "Comun/barra-inferior");
            Txt(Nodo(barra, "Version", Tipo.TL, 1475f, 35f, 400f, 15f), version, "SemiBold", 12f, AZUL, TextAlignmentOptions.TopRight);
            x = 120f;
            return barra;
        }

        static float Ayuda(RectTransform barra, string nombre, float x, string tecla, string accion)
        {
            RectTransform g = Nodo(barra, nombre, Tipo.TL, x, 27f, 10f, 28f);
            RectTransform t = Tecla(g, "Tecla", 0f, 0f, tecla, Color.white, NEGRO, 0f, 26f, "Comun/tecla-ancha", 12f, null, 28f);
            float anchoTecla = t.sizeDelta.x;
            TextMeshProUGUI a = Txt(Nodo(g, "Accion", Tipo.TL, anchoTecla + 11f, 6f, 10f, 18f), accion, "Bold", 15f, HUESO);
            float anchoTexto = Mathf.Ceil(a.GetPreferredValues(accion).x) + 2f;
            ((RectTransform)a.transform).sizeDelta = new Vector2(anchoTexto, 18f);
            g.sizeDelta = new Vector2(anchoTecla + 11f + anchoTexto, 28f);
            return x + g.sizeDelta.x + 34f;
        }

        static RectTransform Tecla(RectTransform padre, string nombre, float x, float y, string texto, Color colorTecla, Color colorTexto,
            float ancho, float alto, string sprite = "Comun/tecla-ancha", float tamTexto = 12f, Vector4? borde = null, float anchoMin = 0f)
        {
            RectTransform t = Nodo(padre, nombre, Tipo.TL, x, y, Mathf.Max(ancho, anchoMin), alto);
            Img(t, sprite, nativo: false, sliced: true, color: colorTecla, borde: borde ?? new Vector4(6, 6, 6, 6));
            TextMeshProUGUI tmp = Txt(Nodo(t, "Texto", Tipo.Lleno), texto, "Bold", tamTexto, colorTexto, TextAlignmentOptions.Center);
            if (ancho <= 0f)
            {
                float w = Mathf.Max(anchoMin, Mathf.Ceil(tmp.GetPreferredValues(texto).x) + 22f);
                t.sizeDelta = new Vector2(w, alto);
            }
            return t;
        }

        static void Globo(RectTransform raiz, float x, float y, float w, float h, float etqX, float etqY,
            float textoX, float textoY, float textoAncho, string texto, float tam)
        {
            RectTransform globo = Nodo(raiz, "Globo", Tipo.TL, x, y, w, h);
            Img(globo, "Comun/globo", nativo: w <= 0f);
            float alto = (w <= 0f ? globo.sizeDelta.y : h) - textoY - 20f;
            Txt(Nodo(globo, "Texto", Tipo.TL, textoX, textoY, textoAncho, alto), texto, "Bold", tam, HUESO, ajustar: true);
            RectTransform etq = Nodo(raiz, "GloboEtiqueta", Tipo.TL, etqX, etqY);
            Img(etq, "Comun/globo-etiqueta");
            TextMeshProUGUI n = Txt(Nodo(etq, "Nombre", Tipo.TL, 16f, 0f, 210f, 36f), "COORDINADORA", "Bold", 14f, HUESO, TextAlignmentOptions.MidlineLeft);
            n.characterSpacing = 15f;
        }

        static void NavegacionVertical(Selectable[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnUp = items[(i - 1 + items.Length) % items.Length],
                    selectOnDown = items[(i + 1) % items.Length],
                };
                EditorUtility.SetDirty(items[i]);
            }
        }

        static void NavegacionHorizontal(Selectable[] items)
        {
            for (int i = 0; i < items.Length; i++)
            {
                items[i].navigation = new Navigation
                {
                    mode = Navigation.Mode.Explicit,
                    selectOnLeft = items[(i - 1 + items.Length) % items.Length],
                    selectOnRight = items[(i + 1) % items.Length],
                };
                EditorUtility.SetDirty(items[i]);
            }
        }

        static RectTransform Nodo(Transform padre, string nombre, Tipo tipo, float x = 0f, float y = 0f, float w = 0f, float h = 0f, float rotZ = 0f)
        {
            GameObject go = new GameObject(nombre, typeof(RectTransform));
            go.layer = LayerMask.NameToLayer("UI");
            RectTransform rt = (RectTransform)go.transform;
            rt.SetParent(padre, false);
            if (tipo == Tipo.Lleno)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.pivot = new Vector2(0.5f, 0.5f);
                rt.offsetMin = Vector2.zero;
                rt.offsetMax = Vector2.zero;
            }
            else
            {
                rt.anchorMin = new Vector2(0f, 1f);
                rt.anchorMax = new Vector2(0f, 1f);
                rt.pivot = tipo == Tipo.TL ? new Vector2(0f, 1f) : new Vector2(0.5f, 0.5f);
                rt.anchoredPosition = new Vector2(x, -y);
                rt.sizeDelta = new Vector2(w, h);
                if (tipo == Tipo.C) rt.localEulerAngles = new Vector3(0f, 0f, rotZ);
            }
            return rt;
        }

        static Sprite CargarSprite(string nombre, Vector4? borde = null)
        {
            string ruta = RUTA_UI + nombre + ".png";
            TextureImporter imp = AssetImporter.GetAtPath(ruta) as TextureImporter;
            if (imp != null)
            {
                bool cambiar = imp.textureType != TextureImporterType.Sprite;
                if (borde.HasValue && imp.spriteBorder != borde.Value) cambiar = true;
                if (cambiar)
                {
                    imp.textureType = TextureImporterType.Sprite;
                    imp.spriteImportMode = SpriteImportMode.Single;
                    imp.alphaIsTransparency = true;
                    imp.mipmapEnabled = false;
                    imp.textureCompression = TextureImporterCompression.Uncompressed;
                    if (borde.HasValue) imp.spriteBorder = borde.Value;
                    imp.SaveAndReimport();
                }
            }
            Sprite s = AssetDatabase.LoadAssetAtPath<Sprite>(ruta);
            if (s == null) Debug.LogWarning($"[H-EROE] No encontré el sprite {ruta}");
            return s;
        }

        static Image Img(RectTransform rt, string sprite, bool nativo = true, bool sliced = false, Color? color = null,
            Vector4? borde = null, bool raycast = false)
        {
            Image img = rt.gameObject.AddComponent<Image>();
            img.sprite = sprite != null ? CargarSprite(sprite, borde) : null;
            img.color = color ?? Color.white;
            img.raycastTarget = raycast;
            if (sliced) img.type = Image.Type.Sliced;
            if (nativo && img.sprite != null) img.SetNativeSize();
            return img;
        }

        static TextMeshProUGUI Txt(RectTransform rt, string texto, string peso, float tam, Color color,
            TextAlignmentOptions alineacion = TextAlignmentOptions.TopLeft, bool ajustar = false)
        {
            TextMeshProUGUI t = rt.gameObject.AddComponent<TextMeshProUGUI>();
            TMP_FontAsset f = AssetDatabase.LoadAssetAtPath<TMP_FontAsset>($"{RUTA_FUENTES}Orbitron-{peso} SDF.asset");
            if (f != null) t.font = f;
            else Debug.LogWarning($"[H-EROE] No encontré {RUTA_FUENTES}Orbitron-{peso} SDF.asset");
            t.text = texto;
            t.fontSize = tam;
            t.color = color;
            t.alignment = alineacion;
            t.textWrappingMode = ajustar ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            t.overflowMode = TextOverflowModes.Overflow;
            t.raycastTarget = false;
            return t;
        }

        static Color Hex(string h)
        {
            ColorUtility.TryParseHtmlString("#" + h, out Color c);
            return c;
        }
    }
}
#endif
