using Runtime.Services.Audio;
using Runtime.UI;
using Core.Services.Audio;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Animation), typeof(Button))]
public class LevelSelectionButton : MonoBehaviour
{
    [SerializeField] private int _levelID = 0;
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Image _lockImage;
    [SerializeField] private TextMeshProUGUI _levelText;

    private IAudioService _audioService;

    public Animation PressAnimation;
    public SimpleButton Button;

    public event Action<int> OnLevelSelected;

    [Inject]
    public void Construct(IAudioService audioService)
    {
        _audioService = audioService;
    }

    private void OnValidate()
    {
        SetFieldValuesAutomatically();
    }

    private void OnDestroy()
    {
        Button.Button.onClick.RemoveAllListeners();
    }

    public void Initialize(bool locked)
    {
        _lockImage.gameObject.SetActive(locked);
        _levelText.gameObject.SetActive(!locked);
        Button.Button.interactable = !locked;

        Button.Button.onClick.AddListener(() =>
        {
            ProcessClick();
            OnLevelSelected?.Invoke(_levelID);
        });
    }

    public void SetSprite(Sprite sprite) => _buttonImage.sprite = sprite;

    public void SetColor(Color color) => _buttonImage.color = color;

    private void SetFieldValuesAutomatically()
    {
        _levelID = transform.GetSiblingIndex();
        _levelText.text = (_levelID + 1).ToString();
    }

    private void ProcessClick()
    {
        PressAnimation.Play();
        _audioService.PlaySound(ConstAudio.PressButtonSound);
    }
}
