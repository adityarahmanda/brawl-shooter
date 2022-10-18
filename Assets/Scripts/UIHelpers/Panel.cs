using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Panel : MonoBehaviour
{
    [SerializeField] private bool _isShowing;
    public bool isShowing => _isShowing;

    protected void Awake()
    {
        gameObject.SetActive(_isShowing);
    }

    public void SetVisible(bool v)
    {
        _isShowing = v;
        gameObject.SetActive(v);
    }
}
