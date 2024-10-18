public class HUDChanger
{
    public enum Mode { Outside, Inside, Housing, Custom }

    private ButtonsUI _btnsUI;
    private PlayerInfoUI _playerUI;
    private HousingUI _housingUI;
    private CharacterCustomUI _customUI;
    private Mode _mode;

    public void Init(ButtonsUI btn, PlayerInfoUI playerUI, HousingUI housing, CharacterCustomUI customUI)
    {
        _btnsUI = btn;
        _playerUI = playerUI;
        _housingUI = housing;
        _customUI = customUI;
        _customUI.Deactivate();
    }

    public void ChangeMode(Mode mode)
    {
        HideCurrentMode();
        _mode = mode;
        if (mode == Mode.Outside)
            ShowOutside();
        else if (mode == Mode.Inside)
            ShowInside();
        else if (mode == Mode.Housing)
            ShowHousing();
        else
            ShowCustom();
    }

    private void ShowOutside()
    {
        _btnsUI.Activate();
        _btnsUI.SwitchBtn(false);
        _playerUI.Activate();
    }

    private void ShowInside()
    {
        _btnsUI.Activate();
        _btnsUI.SwitchBtn(true);
        _playerUI.Activate();
    }

    private void ShowHousing()
    {
        _housingUI.Activate();
        _housingUI.Refresh();
    }

    private void ShowCustom()
    {
        _customUI.Activate();
        _customUI.Init();
    }

    private void HideCurrentMode()
    {
        switch (_mode)
        {
            case Mode.Outside:
            case Mode.Inside:
                _btnsUI.Deactivate();
                _playerUI.Deactivate();
                break;
            case Mode.Housing:
                _housingUI.Deactivate();
                break;
            case Mode.Custom:
                _customUI.Deactivate();
                break;
        }
    }
}
