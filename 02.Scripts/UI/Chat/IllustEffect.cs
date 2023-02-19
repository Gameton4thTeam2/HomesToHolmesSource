using System;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 일러스트 효과 타입
    /// </summary>
    public enum IllustEffectType
    {
        None,
        FadeIn,
        FadeOut,
        Oscillation
    }

    [Serializable]
    public struct IllustEffect
    {
        public IllustEffectType effectType;
        public float duration;
    }
}