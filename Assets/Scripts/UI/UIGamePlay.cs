using System;
using UnityEngine;
using UnityEngine.UI;

public class UIGamePlay : MonoBehaviour
{
    [SerializeField] private Button BtnKick;
    [SerializeField] private Button BtnAutoKick;
    [SerializeField] private Button BtnReset;

    public event Action KickButtonClicked;
    public event Action AutoKickButtonClicked;
    public event Action ResetButtonClicked;

    #region Public API

    public void ShowKickButton()
    {
        BtnKick.gameObject.SetActive(true);
    }

    public void HideKickButton()
    {
        BtnKick.gameObject.SetActive(false);
    }

    #endregion

    #region Unity Lifecycle

    private void OnEnable()
    {
        BtnKick.onClick.AddListener(HandleKickButtonClicked);
        BtnAutoKick.onClick.AddListener(HandleAutoKickButtonClicked);
        BtnReset.onClick.AddListener(HandleResetButtonClicked);
    }

    private void OnDisable()
    {
        BtnKick.onClick.RemoveListener(HandleKickButtonClicked);
        BtnAutoKick.onClick.RemoveListener(HandleAutoKickButtonClicked);
        BtnReset.onClick.RemoveListener(HandleResetButtonClicked);
    }

    #endregion

    #region Private Methods

    private void HandleKickButtonClicked()
    {
        KickButtonClicked?.Invoke();
    }

    private void HandleAutoKickButtonClicked()
    {
        AutoKickButtonClicked?.Invoke();
    }

    private void HandleResetButtonClicked()
    {
        ResetButtonClicked?.Invoke();
    }

    #endregion
}
