using UnityEngine;
using UnityEngine.UI;

public class ButtonsUI : UIBase
{
    [SerializeField] private Button DEBUG_SaveFileRemoveBtn;
    
    [SerializeField] private Button _gameSettingBtn;
    [SerializeField] private Button _NPCCollectionBtn;
    [SerializeField] private Button _campingItemCollectionBtn;
    
    [SerializeField] private Button _inventoryBtn;
    [SerializeField] private Button _shopBtn;
    [SerializeField] private Button _housingBtn;
    [SerializeField] private Button _customizeBtn;
    
    [SerializeField] private Button _missionBtn;
    [SerializeField] private Button _worldBtn;

    public HousingController _housingController;
    
    private UIManager _UIM = UIManager.Instance;

    public void Init(HousingController housingController, GameSetting gameSetting)
    {
        _housingController = housingController;
        DEBUG_SaveFileRemoveBtn.onClick.AddListener(() => SaveManager.Instance.DEBUG_IsSaveFileRemoved = true);
        
        _gameSettingBtn.onClick.AddListener(gameSetting.OpenUI);
        _NPCCollectionBtn.onClick.AddListener(() => _UIM.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.NPCCollection));
        _campingItemCollectionBtn.onClick.AddListener(() =>
            _UIM.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.CampingItemCollection));
        
        _inventoryBtn.onClick.AddListener(() => _UIM.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.Inventory));
        _shopBtn.onClick.AddListener(() => _UIM.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.Shop));
        _housingBtn.onClick.AddListener(() => { _housingController.StartHousingMode(); });
        _customizeBtn.onClick.AddListener(() => { _UIM.ChangeHUDUIMode(HUDChanger.Mode.Custom); });
        
        _missionBtn.onClick.AddListener(() => _UIM.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.Mission));
        _worldBtn.onClick.AddListener(()=>_UIM.GetUI<MenuUI>(UIName.MenuUI).Init(MenuType.WorldMap));
    }

    public void SwitchBtn(bool isActive)
    {
        _housingBtn.gameObject.SetActive(isActive);
        _customizeBtn.gameObject.SetActive(isActive);
    }
}
