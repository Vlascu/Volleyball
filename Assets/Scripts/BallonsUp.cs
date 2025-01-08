using UnityEngine;

public class BalloonRise : MonoBehaviour
{

    public Vector2 startPosition = new Vector2(0, -470); 

    void Start()
    {
        transform.localPosition = startPosition;

        StartRising();
    }

    void StartRising()
    {
        LeanTween.moveLocalY(gameObject, startPosition.y + 800, 3f)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
            });
    }
}
