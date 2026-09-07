using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Canvas canvas;
    [SerializeField] private RectTransform uiRoot;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError($"Duplicate {nameof(UIManager)} detected on {gameObject.name}.", gameObject);

            Destroy(gameObject);
            return;
        }

        Instance = this;
    }
}
