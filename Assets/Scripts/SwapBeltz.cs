using DG.Tweening;
using UnityEngine;

public class SwapBeltz : MonoBehaviour
{
    [SerializeField] private Sprite beltX;
    [SerializeField] private Sprite beltO;
    [SerializeField] public bool isCorrect;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (!Input.GetMouseButtonDown(0)) return;
        var mousePos = Camera.main!.ScreenToWorldPoint(Input.mousePosition);
        var mousePos2D = new Vector2(mousePos.x, mousePos.y);
        var hits = Physics2D.RaycastAll(mousePos2D, Vector2.zero);
        foreach (var item in hits)
        {
            if (item.collider == null || item.transform.gameObject != gameObject) continue;
            isCorrect ^= true;
            PlaySwitchSound();
            ChangeSprite(_spriteRenderer.sprite == beltX ? beltO : beltX);
            gameObject.transform.DOScale(14, 0);
            gameObject.transform.DOScale(18, 0.2f).SetLoops(2, LoopType.Yoyo);
        }
    }

    private void ChangeSprite(Sprite sprite)
    {
        _spriteRenderer.sprite = sprite;
    }

    private void PlaySwitchSound()
    {
        if (PlayerPrefs.GetString("Sound") != "OFF") GetComponent<AudioSource>().Play();
    }
}