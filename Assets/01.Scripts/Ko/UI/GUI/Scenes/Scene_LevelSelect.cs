using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Scene_LevelSelect : UI_Scene
{
    [SerializeField] private float _snapForce;
    [SerializeField] private Vector2 _slotSize;

    private bool _isMouseDown = false;
    private bool _isSnapped = true;
    private float _snapSpeed = 0;
    private ScrollRect _scrollRect;
    private RectTransform _contentPanel;
    private HorizontalLayoutGroup _horGroup;

    private int _curIndex = -1;

    protected override void Init()
    {
        base.Init();

        if(TryGetComponent(out Image _image))
        {
            _image.color = new Color(0, 0, 0, 0);
            _image.DOColor(new Color(0, 0, 0, 0.7f), 0.4f);
        }

        Get<Image>("Image_LevelContainer").rectTransform.DOAnchorPosY(-500,0);
        Get<Image>("Image_LevelContainer").rectTransform.DOAnchorPosY(0, 0.5f);

        Bind<HorizontalLayoutGroup>();
        _scrollRect = Get<ScrollRect>("Scroll View");
        _horGroup = Get<HorizontalLayoutGroup>("Content");
        _contentPanel = _horGroup.gameObject.GetComponent<RectTransform>();

        _scrollRect.onValueChanged.AddListener(OnScrollMove);

        BindEvent(Get<ScrollRect>("Scroll View").gameObject, (PointerEventData _data, Transform _transform) => { _isMouseDown = true; }, Define.ClickType.Down);
        BindEvent(Get<ScrollRect>("Scroll View").gameObject, (PointerEventData _data, Transform _transform) => { _isMouseDown = false; }, Define.ClickType.Up);

        //BindSlot();
    }

    private void OnScrollMove(Vector2 _vec)
    {
        _curIndex = Mathf.RoundToInt(0 - _contentPanel.localPosition.x / (_slotSize.x + _horGroup.spacing));


        if (_scrollRect.velocity.magnitude < 200 && !_isSnapped && !_isMouseDown)
        {
            _scrollRect.velocity = Vector2.zero;
            _snapSpeed += _snapForce * Time.deltaTime;

            _contentPanel.localPosition = new Vector3(
                Mathf.MoveTowards(_contentPanel.localPosition.x, 0 - (_curIndex * (_slotSize.x + _horGroup.spacing)), _snapSpeed),
                _contentPanel.localPosition.y,
                _contentPanel.localPosition.z);


            if (_contentPanel.localPosition.x == 0 - (_curIndex * (_slotSize.x + _horGroup.spacing)))
            {
                _isSnapped = true;
            }
        }

        if (_scrollRect.velocity.magnitude > 200)
        {
            _isSnapped = false;
            _snapSpeed = 0;
        }
    }
}