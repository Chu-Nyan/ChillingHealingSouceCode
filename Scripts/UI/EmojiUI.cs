using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class EmojiUI : MonoBehaviour
{
    public enum EmojiType { Bang, Note, Heart, Idea, Sleep, Shy, Otl, Kyun, Disappointment, AD, FourClover, ThreeClover, Trade }

    public static Dictionary<EmojiType, List<Sprite>> _sprites;
    [SerializeField] private Image _image;
    [SerializeField] private Button _btn;

    private Coroutine _animation;
    private Coroutine _timer;
    private float _showTime;

    private event Action Clicked;

    private void Awake()
    {
        _btn.onClick.AddListener(() => Clicked?.Invoke());
    }

    public static void Init()
    {
        _sprites = new();
        int count = Enum.GetValues(typeof(EmojiType)).Length;
        var sprites = AssetManager.Instance.LoadAssetSync<SpriteAtlas>($"{Const.Sprite_Emoji}");
        int startNum = 0;
        for (int i = 0; i < count; i++)
        {
            var type = (EmojiType)i;
            if (type < EmojiType.FourClover)
            {
                _sprites.Add(type, new(8));
                for (int j = 0; j < 8; j++)
                {
                    _sprites[type].Add(sprites.GetSprite($"Emoji_{startNum + j}"));
                }
                startNum += 8;
            }
            else
            {
                _sprites.Add(type, new() { sprites.GetSprite($"Emoji_{startNum}") });
                startNum++;
            }
        }
    }

    public void Refresh(EmojiType type, float time, Action action = null)
    {
        if (gameObject.activeSelf == false)
        {
            gameObject.SetActive(true);
        }

        _showTime = time;
        Clicked = action;
        _btn.interactable = action != null;
        _image.raycastTarget = action != null;
        if (time > 0)
        {
            _timer = StartCoroutine(Timer());
        }

        if (type < EmojiType.FourClover)
        {
            StopAnimation();
            _animation = StartCoroutine(ShowAnimation(_sprites[type]));
        }
        else // 애니메이션 없음
        {
            _image.sprite = _sprites[type][0];
        }
    }

    private IEnumerator ShowAnimation(List<Sprite> sprites)
    {
        float frame = 0.05f;
        foreach (var item in sprites)
        {
            float time = 0;
            _image.sprite = item;
            while (frame > time)
            {
                time += Time.deltaTime;
                yield return null;
            }
        }
    }

    private IEnumerator Timer()
    {
        float time = 0;
        while (time < _showTime)
        {
            time += Time.deltaTime;
            yield return null;
        }
        Hide(true);
    }

    private void StopAnimation()
    {
        if (_animation != null)
        {
            StopCoroutine(_animation);
            _animation = null;
        }
    }

    public void Hide(bool isHard)
    {
        if (_showTime == -1 || isHard == true)
        {
            StopAnimation();
            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
        }
        gameObject.SetActive(false);
    }
}