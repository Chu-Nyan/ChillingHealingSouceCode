using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionSlotUI : UIBase
{
    [SerializeField] public TextMeshProUGUI MissionName;
    [SerializeField] public Image MissionProgressBar;
    [SerializeField] public TextMeshProUGUI MissionProgressValue;
    
    [SerializeField] public Button RewardsBtn;
    [SerializeField] public Image RewardsReadyBtnImage;
    [SerializeField] public GameObject RewardsReady;
    [SerializeField] public TextMeshProUGUI RewardsCoinText;
    
    [SerializeField] public Image RewardsDoneImage;

    public Mission Mission;
    private int _requiredScore;
    
    public void Init(Mission mission)
    {
        Mission = mission;
        _requiredScore = Mission.RequireScore;
        MissionName.text = Mission.Title;
        RewardsCoinText.text = $"{Const.TextEmoji_MissionCoin} {mission.RewardCoin.ToString()}";
        Mission.OnScoreChanged = UpdateProgressBar;
        Mission.OnStateChanged = UpdateRewardsBtn;
        UpdateProgressBar(Mission.Score);
        UpdateRewardsBtn(Mission.State);
        OnDeactivation += Deactivation;

        if (mission is EventMission eventMission)
        {

            RewardsBtn.onClick.AddListener(() =>
            {
                var ui = UIManager.Instance.GetUI<EventMissionCompleteUI>(UIName.EventMissionCompleteUI);
                ui.Init(eventMission.Orderer, eventMission.SucceedDialog);
            });
        }
    }

    private void UpdateProgressBar(int score)
    {
        if (Mission.State > MissionState.CanComplete)
        {
            MissionProgressBar.fillAmount = 1;
            return;
        }
        MissionProgressValue.text = $"{score} / {_requiredScore}";
        MissionProgressBar.fillAmount = score / (float)_requiredScore;
    }

    private void UpdateRewardsBtn(MissionState state)
    {
        RewardsReady.SetActive(state != MissionState.Completed);
        RewardsReadyBtnImage.enabled = state != MissionState.Completed;
        RewardsReadyBtnImage.color = state == MissionState.InProgress ? Color.gray : Color.cyan;
        RewardsBtn.enabled = state == MissionState.CanComplete;
        RewardsCoinText.enabled = state != MissionState.Completed;
        RewardsDoneImage.enabled = state == MissionState.Completed;
        MissionProgressValue.enabled = state != MissionState.Completed;
    }

    public void Deactivation(UIBase uiBase)
    {
        RewardsBtn.onClick.RemoveAllListeners();
        Mission.OnScoreChanged = null;
        Mission.OnStateChanged = null;
    }
}
