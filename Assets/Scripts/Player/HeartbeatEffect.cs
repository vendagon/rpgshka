using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HeartbeatEffect : MonoBehaviour
{
    [SerializeField] private float minScale = 0.9f;
    [SerializeField] private float maxScale = 1.1f;
    [SerializeField] private float minAlpha = 0.8f;
    [SerializeField] private float maxAlpha = 1f;

    private Image _image;
    private RectTransform _rect;
    private float _currentSpeed = 1f;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _rect = GetComponent<RectTransform>();
    }

    public void SetSpeed(float speed)
    {
        _currentSpeed = Mathf.Clamp(speed, 0.5f, 3f);
    }

    private void Update()
    {
        float pulse = Mathf.Sin(Time.time * _currentSpeed * 2f);
        float scale = Mathf.Lerp(minScale, maxScale, (pulse + 1f) / 2f);
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (pulse + 1f) / 2f);

        _rect.localScale = Vector3.one * scale;

        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}