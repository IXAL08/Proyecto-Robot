using DG.Tweening;
using UnityEngine;

public static class DotweenAnimExtensions
{
    /// <summary>
    /// Hace un shake horizontal al RectTransform como feedback.
    /// </summary>
    /// <param name="rectTransform">El RectTransform a animar.</param>
    /// <param name="duration">Duración total del shake en segundos.</param>
    /// <param name="strength">Intensidad del shake en el eje X.</param>
    /// <param name="vibrato">Número de vibraciones.</param>
    /// <param name="randomness">Aleatoriedad de las vibraciones.</param>
    public static void ShakeAnimation(this RectTransform rectTransform, float duration = 0.5f, float strength = 20f, int vibrato = 10, float randomness = 90f)
    {
        Vector3 originalPos = rectTransform.anchoredPosition;

        // Shake solo en X
        rectTransform.DOShakeAnchorPos(duration, new Vector3(strength, 0, 0), vibrato, randomness)
            .OnComplete(() => rectTransform.anchoredPosition = originalPos);
    }
}
