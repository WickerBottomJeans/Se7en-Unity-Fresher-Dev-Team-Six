using System;
using UnityEngine;

public partial class UIManager
{
    [SerializeField] private UIGamePlay uiGamePlayPrefab;

    private UIGamePlay uiGamePlayInstance;

    public event Action KickButtonClicked;
    public event Action AutoKickButtonClicked;
    public event Action ResetButtonClicked;

    #region Public API

    public void ShowGamePlayUI()
    {
        if (uiGamePlayInstance == null)
        {
            uiGamePlayInstance = Instantiate(uiGamePlayPrefab, uiRoot);
            uiGamePlayInstance.KickButtonClicked += HandleKickButtonClicked;
            uiGamePlayInstance.AutoKickButtonClicked += HandleAutoKickButtonClicked;
            uiGamePlayInstance.ResetButtonClicked += HandleResetButtonClicked;
        }
        this.uiGamePlayInstance.gameObject.SetActive(true);        
    }

    public void HideGamePlayUI()
    {
        if (uiGamePlayInstance == null)
        {
            return;
        }

        uiGamePlayInstance.gameObject.SetActive(false);
    }

    public void ShowKickButton()
    {
        if (uiGamePlayInstance == null)
        {
            throw new InvalidOperationException("Gameplay UI has not been created.");
        }

        uiGamePlayInstance.ShowKickButton();
    }

    public void HideKickButton()
    {
        if (uiGamePlayInstance == null)
        {
            throw new InvalidOperationException("Gameplay UI has not been created.");
        }

        uiGamePlayInstance.HideKickButton();
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
