using UnityEngine;

public class woodenboard : MonoBehaviour
{
    public int hitsToBreak = 2;
    private int currentHits = 0;

    void Hit()
    {
        currentHits++;
        
        if (currentHits >= hitsToBreak)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        Bullet bullet = collision.gameObject.GetComponent<Bullet>();
        if (bullet != null)
        {
            Hit();
        }
    }
}
