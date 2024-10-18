using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NPCInfoUI : UIBase
{
    [SerializeField] private TMP_Text _title;
    [SerializeField] private Image _image;
    [SerializeField] private TMP_Text _desc;
    [SerializeField] private Button _btn;

    [SerializeField] private RectTransform _contentRectTransform;

    private DimmedUI _dimmedUI;

    private void Awake()
    {
        _btn.onClick.AddListener(Deactivate);
    }

    public void Init(string title, Sprite image, string desc)
    {
        _title.text = title;
        _image.sprite = image;
        _desc.text = desc;
        _dimmedUI = UIManager.Instance.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmedUI.SetDimmed(this);
        _contentRectTransform.anchoredPosition = new Vector3(0, 0, 0);
    }

    public override void Deactivate()
    {
        _dimmedUI.ReturnDimmed(this);
        base.Deactivate();
    }
}
