using System.Collections.Generic;
using UnityEngine;

public class OutlineEffect : MonoBehaviour
{
    private OutlineController _controller;
    private Renderer[] _renderers;

    private void Awake()
    {
        _controller = FindObjectOfType<OutlineController>();
        _renderers = GetComponentsInChildren<Renderer>();
    }

    private void OnEnable()
    {
        _controller.AddRenderers(_renderers);
    }

    private void OnDisable()
    {
        _controller.RemoveRenderers(_renderers);
    }
}
