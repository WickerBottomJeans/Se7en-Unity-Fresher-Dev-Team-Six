using System;

public class PlayerKickPresenter : IDisposable
{
    public PlayerKickPresenter()
    {
        HandleKickAvailabilityChanged(Player.Instance.CanKick);
        Player.Instance.KickAvailabilityChanged += HandleKickAvailabilityChanged;
        UIManager.Instance.KickButtonClicked += HandleKickButtonClicked;
        UIManager.Instance.AutoKickButtonClicked += HandleAutoKickButtonClicked;
    }

    public void Dispose()
    {
        if (Player.Instance != null)
        {
            Player.Instance.KickAvailabilityChanged -= HandleKickAvailabilityChanged;
        }
        UIManager.Instance.KickButtonClicked -= HandleKickButtonClicked;
        UIManager.Instance.AutoKickButtonClicked -= HandleAutoKickButtonClicked;
    }

    private void HandleKickAvailabilityChanged(bool canKick)
    {
        if (canKick)
        {
            UIManager.Instance.ShowKickButton();
        }
        else
        {
            UIManager.Instance.HideKickButton();
        }
    }

    private void HandleKickButtonClicked()
    {
        Player.Instance.RequestKick();
    }

    private void HandleAutoKickButtonClicked()
    {
        Player.Instance.RequestAutoKick();
    }
}
