using UnityEngine;
using DG.Tweening;

public class PlayerGrabAnimation : BaseMonoBehaviour
{
    [SerializeField]
    private MeshRenderer rend = null;

    [SerializeField]
    private float tweenDuration = 0.1f;

    [SerializeField, ColorUsage(false, true)]
    private Color activatedColor = Color.white;

    [SerializeField]
    private string colorProperty = "_EmissionColor";

    [SerializeField]
    private float activatedScaleFactor = 1.2f;

    [SerializeField]
    private float flashStartInterval = 0.5f;

    [SerializeField]
    private float flashPeakInterval = 0.1f;

    private Color deactivatedColor;
    private Vector3 deactivatedScale;
    private Tween colorFlashTween;
    private StateSubscriber<bool> playerGrabbed;

    private Material Material => rend.material;
    
    private void Awake()
    {
        deactivatedColor = Material.GetColor(colorProperty);
        deactivatedScale = transform.localScale;
    }

    private void OnEnable()
    {
        colorFlashTween = DOTween.Sequence()
            .AppendInterval(flashStartInterval)
            .Append(Material.DOColor(activatedColor, colorProperty, tweenDuration))
            .AppendInterval(flashPeakInterval)
            .Append(Material.DOColor(deactivatedColor, colorProperty, tweenDuration))
            .SetLoops(-1);
        colorFlashTween.Rewind();
        
        playerGrabbed.Subscribe(State.Instance.Player.Grabbed, HandlePlayerGrabbedChange);
    }

    private void OnDisable()
    {
        playerGrabbed.Unsubscribe();
    }

    private void HandlePlayerGrabbedChange(bool value)
    {
        Vector3 scale = value ? deactivatedScale * activatedScaleFactor : deactivatedScale;
        transform.DOScale(scale, tweenDuration);
        transform.DOLocalMoveY(scale.x / 2f, tweenDuration);

        float duration = tweenDuration * (1f - colorFlashTween.ElapsedDirectionalPercentage());
        if (value)
        {
            colorFlashTween.Pause();
            Material.DOColor(activatedColor, colorProperty, duration);
        }
        else
        {
            colorFlashTween.Restart();
            Material.DOColor(deactivatedColor, colorProperty, duration);
        }
    }
}
