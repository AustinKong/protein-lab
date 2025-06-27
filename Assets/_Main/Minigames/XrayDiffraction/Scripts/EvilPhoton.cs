using UnityEngine;

public class EvilPhoton : Ball
{
    protected override void HandlePaddleHit()
    {
        if (!isTopBall)
        {
            SoundManager.Instance.PlaySFX("DM-CGS-13");
        }
        // Evil behavior: break combo and lock paddle
        PaddleController.Instance.RegisterMiss();

        // Destroy the photon
        Destroy(gameObject);
    }
}
