using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecordDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _placeText;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _scoreText;

    public void Initialize(int place, string name, int score)
    {
        _placeText.text = place.ToString();
        _nameText.text = name.ToString();
        _scoreText.text = score.ToString();
    }
}
