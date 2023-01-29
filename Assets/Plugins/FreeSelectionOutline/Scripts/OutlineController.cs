using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
public class OutlineController : MonoBehaviour
{
    public enum AlphaType : int
    {
        KeepHoles = 0,
        Intact = 1
    }

    public enum OutlineMode : int
    {
        Whole = 0,
        ColorizeOccluded = 1,
        OnlyVisible = 2
    }

    private Material OutlineMat;
    private Shader OutlineShader, TargetShader;
    private RenderTexture Mask, Outline;
    private Camera cam;
    private CommandBuffer cmd;
    private bool Init = false;

    [Tooltip("The last two type will require rendering an extra Camera Depth Texture.")]
    public OutlineMode OutlineType = OutlineMode.Whole;
    [Tooltip("Decide whether the alpha data of the main texture affect the outline.")]
    public AlphaType AlphaMode = AlphaType.Intact;

    [ColorUsageAttribute(true, true)]
    public Color OutlineColor = new Color(1f, 1f, 1f), OccludedColor = new Color(0.5f, 0.9f, 0.3f);
    [Range(0, 1)]
    public float OutlineWidth = 0.1f;
    [Range(0, 1)]
    public float OutlineHardness = 0.1f;

    private Dictionary<int, Renderer> _renderers = new Dictionary<int, Renderer>();

    private void OnEnable()
    {
        Initial();
        _renderers = new Dictionary<int, Renderer>();
    }

    private void OnDisable()
    {
        ClearAllRender();
    }

    void Initial()
    {
#if UNITY_WEBGL
        Shader.EnableKeyword("_WEBGL");
#endif
        OutlineShader = Shader.Find("Outline/PostprocessOutline");
        TargetShader = Shader.Find("Outline/Target");
        if(OutlineShader==null||TargetShader==null)
        {
            Debug.LogError("Can't find the outline shaders,please check the Always Included Shaders in Graphics settings.");
            return;
        }
        cam = GetComponent<Camera>();
        cam.depthTextureMode = OutlineType > 0 ? DepthTextureMode.None : DepthTextureMode.Depth;
        OutlineMat = new Material(OutlineShader);
        if (OutlineType > 0)
        {
            Shader.EnableKeyword("_COLORIZE");
            Mask = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.RFloat);
            Outline = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.RG16);
            if (OutlineType == OutlineMode.OnlyVisible)
                Shader.EnableKeyword("_OCCLUDED");
            else
                Shader.DisableKeyword("_OCCLUDED");

        }
        else
        {
            Shader.DisableKeyword("_OCCLUDED");
            Shader.DisableKeyword("_COLORIZE");
            Mask = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.R8);
            Outline = new RenderTexture(cam.pixelWidth, cam.pixelHeight, 0, RenderTextureFormat.R8);
        }
        cam.RemoveAllCommandBuffers();
        cmd = new CommandBuffer { name = "Outline Command Buffer" };
        cmd.SetRenderTarget(Mask);
        cam.AddCommandBuffer(CameraEvent.BeforeImageEffects, cmd);
        Init = true;
    }

    private void OnValidate()
    {
        if (!Init)
        {
            Initial();
        }
        cam.depthTextureMode = OutlineType > 0 ? DepthTextureMode.Depth : DepthTextureMode.None;
        if (OutlineType > 0)
        {
            Shader.EnableKeyword("_COLORIZE");

            if (OutlineType == OutlineMode.OnlyVisible)
                Shader.EnableKeyword("_OCCLUDED");
            else
                Shader.DisableKeyword("_OCCLUDED");

        }
        else
        {
            Shader.DisableKeyword("_OCCLUDED");
            Shader.DisableKeyword("_COLORIZE");
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (OutlineMat == null)
        {
            Initial();
            if(!Init)
            return;
        }

        OutlineMat.SetFloat("_OutlineWidth", OutlineWidth * 10f);
        OutlineMat.SetFloat("_OutlineHardness", 8.99f * (1f - OutlineHardness) + 0.01f);
        OutlineMat.SetColor("_OutlineColor", OutlineColor);
        OutlineMat.SetColor("_OccludedColor", OccludedColor);

        OutlineMat.SetTexture("_Mask", Mask);
        Graphics.Blit(source, Outline, OutlineMat, 0);
        OutlineMat.SetTexture("_Outline", Outline);
        Graphics.Blit(source, destination, OutlineMat, 1);
        //Graphics.Blit(Outline, destination);

    }

    void RenderTarget(Renderer target)
    {
        Material TargetMat = new Material(TargetShader);
        bool MainTexFlag = false;
        string[] attrs = target.sharedMaterial.GetTexturePropertyNames();
        foreach (var c in attrs)
        {
            if (c == "_MainTex")
            {
                MainTexFlag = true;
                break;
            }
        }
        if (MainTexFlag && target.sharedMaterial.mainTexture != null && AlphaMode == AlphaType.KeepHoles)
        {
            TargetMat.mainTexture = target.sharedMaterial.mainTexture;
        }

        cmd.DrawRenderer(target, TargetMat);
        Graphics.ExecuteCommandBuffer(cmd);
    }

    public void AddRenderers(Renderer[] renderers)
    {
        if (!Init) return;

        cmd.SetRenderTarget(Mask);
        cmd.ClearRenderTarget(true, true, Color.black);
        if (renderers.Length != 0)
        {
            foreach (var renderer in renderers)
            {
                RenderTarget(renderer);
                _renderers.Add(renderer.gameObject.GetInstanceID(), renderer);
            }
        }
        else
        {
            Debug.LogWarning("No renderer provided for outline.");
        }
    }

    public void RemoveRenderers(Renderer[] renderers)
    {
        if (!Init) return;

        foreach (var renderer in renderers)
        {
            _renderers.Remove(renderer.gameObject.GetInstanceID());
        }

        Rerender();
    }

    public void Rerender()
    {
        if (!Init) return;

        cmd.SetRenderTarget(Mask);
        cmd.ClearRenderTarget(true, true, Color.black);
        if (_renderers.Count != 0)
        {
            foreach (var renderer in _renderers.Values)
            {
                RenderTarget(renderer);
            }
        }
        else
        {
            Debug.LogWarning("No renderer provided for outline.");
        }
    }

    public void ClearAllRender()
    {
        if (!Init) return;

        cmd.ClearRenderTarget(true, true, Color.black);

        Graphics.ExecuteCommandBuffer(cmd);
        cmd.Clear();
    }
}
