using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 加载界面
/// </summary>
public class UILoadingTest01 : TestUIBase
{
    /// <summary>
    /// 可选的加载图片集合
    /// </summary>
    [SerializeField] private Image[] _imageLoadings;

    public override void OnInit(object userData)
    {
        base.OnInit(userData);

        int index = Random.Range(0, _imageLoadings.Length);

        for (int i = 0; i < _imageLoadings.Length; i++)
            _imageLoadings[i].gameObject.SetActive(i == index);
    }

    public override void OnClose()
    {
        base.OnClose();
    }
}

