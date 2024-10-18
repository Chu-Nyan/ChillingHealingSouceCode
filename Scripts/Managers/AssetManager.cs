using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.U2D;

public class AssetManager : MonoBehaviourSingleton<AssetManager>
{
    // 비동기 로드
    public void LoadGameObject(string path)
    {
        Addressables.LoadAssetAsync<GameObject>(path).Completed += OnLoadCompleted;
    }

    private void OnLoadCompleted(AsyncOperationHandle<GameObject> handler)
    {
        if (handler.Status != AsyncOperationStatus.Succeeded)
            return;

        Instantiate(handler.Result);
    }

    // 동기 로드
    public T LoadAssetSync<T>(string path)
    {
        var handle = Addressables.LoadAssetAsync<T>(path);
        handle.WaitForCompletion();
        if (handle.Status != AsyncOperationStatus.Succeeded)
            Debug.LogError("handle.Status is not AsyncOperationStatus.Succeeded");

        return handle.Result;
    }

    // 동기 로드 후 오브젝트 생성
    public T GenerateLoadAssetSync<T>(string path, string name = default) where T : Behaviour
    {
        var item = LoadAssetSync<GameObject>(path);
        var gameObj = Instantiate(item);
        gameObj.name = name == default ? item.name : name;
        return gameObj.GetComponent<T>();
    }

    // Json 역직렬화 , 함수이름 Path로 변경
    public T DeserializeJsonSync<T>(string path)
    {
        var handle = LoadAssetSync<TextAsset>(path);
        T datas = JsonConvert.DeserializeObject<T>(handle.text);

        return datas;
    }

    // Json 역직렬화, 컨버터 커스텀
    public T DeserializeJsonSync<T>(string json, JsonConverter converter)
    {
        T datas = JsonConvert.DeserializeObject<T>(json, converter);

        return datas;
    }

    // Json만 불러오기
    public string LoadJsonText(string path)
    {
        var handle = LoadAssetSync<TextAsset>(path);
        return handle.text;
    }

    // Json 합치기
    public string MergeJsonsSync(string path1, string path2)
    {
        JObject data = JObject.Parse(LoadJsonText(path1));
        JObject data2 = JObject.Parse(LoadJsonText(path2));
        data.Merge(data2);
        return data.ToString();
    }

    public static Sprite GetSpriteWithAtlas(string path, string name)
    {
        var spriteAtlas = Instance.LoadAssetSync<SpriteAtlas>(path);
        return spriteAtlas.GetSprite(name);
    }

    public static Sprite GetItemIconSprite(ItemType type)
    {
        return GetSpriteWithAtlas(Const.SpriteAtlas_ItemIcons, $"Icon{(int)type}");
    }
}
