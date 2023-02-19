using System;

namespace HTH
{
    public enum Rarity
    {
        None,
        Low,
        Proper,
        High,
        Super,
        Legend
    }

    public enum Rank
    {
        None,
        F,
        E,
        D,
        C,
        B,
        A,
        S
    }

    [Flags]
    public enum Axis
    {
        None,
        X,
        Y,
        Z
    }   

    public enum Pos
    {
        Right,
        Left,
        Center,
        Bottom,
        Top,
    }

    /// <summary>
    /// 수정자 : 권병석
    /// 수정일 : 2023_02_01
    /// 설명   : 일러스트 타입 7가지로 변경
    /// </summary>
    public enum IllustType
    {
        Idle,
        CloseEyes,
        Thinking,
        Smile,
        Cry,
        Embarrassed,
        Silhouette
    }

    /// <summary>
    /// 작성자 : 권병석
    /// 작성일 : 2023_02_01
    /// 설명   : 채팅 타입 추가 => 대화타입, 생각타입
    /// </summary>
    public enum ChattingType
    {
        Chatting,
        Thinking
    }
}
