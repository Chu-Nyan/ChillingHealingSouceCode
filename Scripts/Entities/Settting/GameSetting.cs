using UnityEngine;

public class GameSetting
{
    private SoundManager _sm;
    private GameSettingUI _settingUI;

    public void InitSettingUI()
    {
        _sm = SoundManager.Instance;
        _settingUI = UIManager.Instance.GetUI<GameSettingUI>(UIName.GameSettingUI);
        _settingUI.Deactivate();
        _settingUI.InitButtonEvent(ShowTutorials, ShowInstagram, ShowCredits, ShowLicense, QuitGame);
        _settingUI.InitToggleEvent(ChangeBGMVolume, ChangeSEVolume);
    }

    public void OpenUI()
    {
        _settingUI.Activate();
        var isBgmON = _sm.BGMVolume > 0;
        var isSEON = _sm.SEVolume > 0;
        _settingUI.Refresh(isBgmON, isSEON);
    }

    private void ChangeBGMVolume(bool isOn)
    {
        var volume = isOn == true ? 1 : 0;
        _sm.ChangeBGMVolume(volume);
    }

    private void ChangeSEVolume(bool isOn)
    {
        var volume = isOn == true ? 1 : 0;
        _sm.ChangeSEVolume(volume);
    }

    private void ShowTutorials()
    {
        UIManager.Instance.GetUI<TutorialListUI>(UIName.TutorialListUI);
    }

    private void ShowInstagram()
    {
        Application.OpenURL("https://www.instagram.com/chilling__healing/");
    }

    private void ShowCredits()
    {
        UIManager.Instance.GetUI<CreditUI>(UIName.CreditUI);
    }

    private void ShowLicense()
    {
        UIManager.Instance.GetUI<GameNotificationUI>(UIName.GameNotificationUI);
    }

    private void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        // 실제 앱 종료
        Application.Quit();
#endif
    }
}
