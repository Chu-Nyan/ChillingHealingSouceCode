using UnityEngine;
using UnityEngine.UI;

public class CreditUI : UIBase
{
    [SerializeField] private Button _image;
    private DimmedUI _dimmed;

    public void Awake()
    {
        _image.onClick.AddListener(Deactivate);
    }

    public override void Activate()
    {
        base.Activate();
        _dimmed = UIManager.Instance.GetUI<DimmedUI>(UIName.DimmedUI);
        _dimmed.SetDimmed(this);
    }

    public override void Deactivate()
    {
        _dimmed.ReturnDimmed(this);
        base.Deactivate();
    }
}
