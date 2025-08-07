using Runtime.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BalloonController : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private float _speed = 2.0f;
    [SerializeField] private float _floatStrength = 0.5f;
    [SerializeField] private float _floatFrequency = 2.0f;

    private Vector2 _targetPosition;

    public event Action OnCollided;
    public event Action<GameObject> OnCollected;

    private void OnEnable()
    {
        _targetPosition = Vector3.zero;
        _spriteRenderer.enabled = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        OnCollided?.Invoke();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCollected?.Invoke(collision.gameObject);
    }

    private void Update()
    {
        Move();

        if (AnyInput())
            UpdateTargetPosition();
    }

    public void SetSprite(Sprite sprite) => _spriteRenderer.sprite = sprite;

    private bool AnyInput() => Input.touchCount > 0 && !Tools.IsPointerOverUIElement();

    private void Move()
    {
        Vector3 currentPos = transform.position;
        transform.position = Vector2.Lerp(currentPos, _targetPosition, _speed * Time.deltaTime);
        transform.position += (Vector3.up * Mathf.Sin(Time.time * _floatFrequency) * _floatStrength * Time.deltaTime);
    }

    private void UpdateTargetPosition()
    {
        Touch touch = Input.GetTouch(0);
        Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);
        _targetPosition = touchPosition;
    }
}
