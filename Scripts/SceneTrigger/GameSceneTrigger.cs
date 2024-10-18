using System.Collections;
using System.Diagnostics;
using UnityEngine;

public class GameSceneTrigger : MonoBehaviour
{
    [SerializeField] private GameObject _camera;
    [SerializeField] private float _cameraDistance = 10;

    private CameraController _cameraController;
    private ClickModeChanger _clickModeChanger;
    private MapController _mapController;
    private MissionBoard _missionBoard;
    private HousingController _housingController;
    private Shop _shop;
    private GameSetting _gameSetting;
    private CoinTrading _coinTrading;

    private Player _player;

    private void Awake()
    {
        Time.timeScale = 0f;
        StartCoroutine(AwakeAsync());
        // GenerateStaticClass();
        // GenerateDynamicClass();
        //
        // InitStaticClass();
        // InitDynamicClass();
        //
        // GameStart();
        // UnitTest();
        Time.timeScale = 1f;
    }

    IEnumerator AwakeAsync()
    {
        GenerateStaticClass();
        var ui = UIManager.Instance.GetUI<FadeInUI>(UIName.FadeInUI);
        ui.ChangeText("퇴근 중...");
        var sw = new Stopwatch();
        sw.Start();

        yield return null;
        GenerateSaveManager();
        ui.ChangeText("힐링이 필요해...");
        yield return null;
        GenerateDynamicClass();
        ui.ChangeText("나에게 필요한 건 휴식...");
        yield return null;
        InitStaticClass();
        ui.ChangeText("휴가 결재 받는 중...");
        yield return null;
        InitDynamicClass();
        ui.ChangeText("캠핑 도구 준비 중...");
        yield return null;
        GameStart();
        ui.ChangeText("외모 체크 중...");
        yield return null;
        UnitTest();
        ui.ChangeText("출발이다!!!");
        yield return null;
        StartAfterAwake();

        while (true)
        {
            yield return null;
            sw.Stop();
            if (sw.ElapsedMilliseconds < 3500)
                sw.Start();
            else
            {
                ui.DoFadeIn();
                yield break;
            }
        }
    }

    private void StartAfterAwake()
    {
        InputManager.Instance.Enable();
        IAPManager.Instance.Init();
    }

    private void GenerateStaticClass()
    {
        gameObject.AddComponent<AssetManager>();
        gameObject.AddComponent<TimeManager>();
        GenerateSoundManager();
        gameObject.AddComponent<NPCGenerator>();
        gameObject.AddComponent<IAPManager>();
        gameObject.AddComponent<AdMobManager>();
        new InputManager();
        new ItemGenerator();
        new CampingGenerator();
        new UIManager();
        new Pathfinding();
        new PartSpritesList();
        new PointEventRayCastHelper();
    }

    private void GenerateDynamicClass()
    {
        _mapController = AssetManager.Instance.GenerateLoadAssetSync<MapController>(Const.Map_MapController);
        _player = AssetManager.Instance.GenerateLoadAssetSync<Player>(Const.PlayerPrefab);
        _cameraController = _camera.AddComponent<CameraController>();
        _clickModeChanger = gameObject.AddComponent<ClickModeChanger>();
        _missionBoard = new();
        _shop = new();
        _housingController = new();
        _gameSetting = new();
        _coinTrading = new();
    }

    private void InitStaticClass()
    {
        InputManager.Instance.Init();
        NPCGenerator.Instance.Init(_mapController);
        UIManager.Instance.Init(_player, _missionBoard, _mapController, _shop);
        PartSpritesList.Instance.Init();
        IAPManager.Instance.SetPlayer(_player);
    }

    private void InitDynamicClass()
    {
        UIManager.Instance.InitUI(_housingController, _gameSetting);
        _player.Init(_mapController);
        _mapController.InitBeginningMap(_player);
        _clickModeChanger.Init(_player, _housingController.Blueprint, _cameraController);
        _shop.InitItemList();
        _housingController.Init(_clickModeChanger, _player);
        InitCamera();
        InitMissionBoard();
        _gameSetting.InitSettingUI();
        _coinTrading.Init(_player);
    }

    private void InitCamera()
    {
        _cameraController.Init(_mapController);
        _cameraController.SetFollowTarget(_player.CameraArm);
        _cameraController.SetDistance(_cameraDistance);
    }

    private void InitMissionBoard()
    {
        _missionBoard.SetMissionInventory(_player);
        _player.BehaviorEvent += _missionBoard.AddMissionScore;
        _clickModeChanger.RegisterBehaviorEvent(_missionBoard.AddMissionScore);
    }

    private void GenerateSoundManager()
    {
        var obj = new GameObject("SoundManager").AddComponent<SoundManager>();
        obj.transform.SetParent(transform);
        obj.Init();
    }

    private void GenerateSaveManager()
    {
        var obj = new GameObject("SaveManager").AddComponent<SaveManager>();
        obj.transform.SetParent(transform);
        obj.Init();
    }

    private void GameStart()
    {
        _player.MoveInsideAndOutside(MapCategory.Outside);
        _clickModeChanger.ChangeMode(ClickMode.Player);
        _missionBoard.AddMissionScore(MissionType.CheckIn, -1, 1);
        SoundManager.Instance.IsOn = true;
        _mapController.CurrentMap.TryPlayMapThemeMusic();
    }

    private void UnitTest()
    {
#if UNITY_EDITOR
        GameSceneTester _tester = new();
#endif
    }
}
