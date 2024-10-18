using UnityEngine;

public class CloverRespawner : ITimeListener
{
    private static readonly float RespawnTime = 6f * TimeManager.HourScale; // 리젠 타임 : (기준) * 60초
    private Map _targetMap;
    private CampingUniversalData _cloverData;
    private float _remainTime;

    public CloverRespawner()
    {
        TimeManager.Instance.RegisterListener(this); // TODO : 시간 밸런스 결정 나면 분 체크로 할지 초체크로할지 결정
        _cloverData = CampingGenerator.Instance.GetData(CampingType.Clover);
    }

    public void SetTargetMap(Map map)
    {
        _targetMap = map;
    }

    public void GetTime(float time)
    {
        _remainTime += time;
        if (_remainTime > RespawnTime)
        {
            GenerateClover();
            _remainTime = 0;
        }
    }

    public void GenerateClover()
    {
        int posX = 0;
        int posY = 0;
        int x = (int)(_targetMap.Size.x * 0.2f);
        int y = (int)(_targetMap.Size.y * 0.2f);
        int count = 0;
        while (count < 100)
        {
            count++;
            posX = Random.Range(x, _targetMap.Size.x - x);
            posY = Random.Range(y, _targetMap.Size.y - y);
            var newPos = new Vector3(posX, posY);

            if (_targetMap.CheckValidation(_cloverData.Size, newPos) == false)
                continue;

            var start = _targetMap.Teleport.transform.position;
            if (Pathfinding.Instance.IsPossiblePath(_targetMap, Vector2Int.FloorToInt(start), Vector2Int.FloorToInt(newPos)) == false)
                continue;

            break;
        }

        var pos = new Vector3(posX, posY);
        var rect = new Rect(pos, new Vector3(2, 2));
        var camping = CampingGenerator.Instance
            .Generate(CampingType.Clover, _targetMap, 1)
            .SetSprite(false, false)
            .SetPosition(pos, rect)
            .GetNewCamping();

        _targetMap.AddCamping(camping);
    }
}
