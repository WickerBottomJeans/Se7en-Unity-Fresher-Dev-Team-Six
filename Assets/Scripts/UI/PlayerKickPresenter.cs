using System;

public class PlayerKickPresenter : IDisposable
{
    private readonly Player player;

    public PlayerKickPresenter(Player player)
    {
        this.player = player;
        HandleKickAvailabilityChanged(player.CanKick);
        player.KickAvailabilityChanged += HandleKickAvailabilityChanged;
        UIManager.Instance.KickButtonClicked += HandleKickButtonClicked;
        UIManager.Instance.AutoKickButtonClicked += HandleAutoKickButtonClicked;
    }

    public void Dispose()
    {
        player.KickAvailabilityChanged -= HandleKickAvailabilityChanged;
        if (UIManager.Instance != null)
        {
            UIManager.Instance.KickButtonClicked -= HandleKickButtonClicked;
            UIManager.Instance.AutoKickButtonClicked -= HandleAutoKickButtonClicked;
        }
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
        player.RequestKick();
    }

    private void HandleAutoKickButtonClicked()
    {
        player.RequestAutoKick();
    }
}
