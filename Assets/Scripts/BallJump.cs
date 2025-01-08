using UnityEngine;

public class BallJump : MonoBehaviour
{
    public float jumpDuration = 0.5f;
    public float moveDuration = 2f; 
    public Vector2 startPosition = new Vector2(-800, -470); 
    public Vector2 endPosition = new Vector2(800, -470); 
    public float jumpHeight = 170f; 

    void Start()
    {
        transform.localPosition = startPosition;

        StartJumpCycle();
    }

    void StartJumpCycle()
    {
        LeanTween.moveLocalX(gameObject, endPosition.x, moveDuration)
            .setEase(LeanTweenType.linear)
            .setOnComplete(() => {
                LeanTween.moveLocalX(gameObject, startPosition.x, moveDuration)
                    .setEase(LeanTweenType.linear)
                    .setOnComplete(StartJumpCycle);
            });

        PerformJump();
    }

    void PerformJump()
    {
        LeanTween.moveLocalY(gameObject, startPosition.y + jumpHeight, jumpDuration / 2)
            .setEase(LeanTweenType.easeOutQuad)
            .setOnComplete(() => {
                LeanTween.moveLocalY(gameObject, startPosition.y, jumpDuration / 2)
                    .setEase(LeanTweenType.easeInQuad)
                    .setOnComplete(PerformJump); 
            });
    }
}
