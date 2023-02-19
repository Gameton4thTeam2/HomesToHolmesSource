using Cysharp.Threading.Tasks;
using System.Collections.Generic;

namespace HTH.AISystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_01
    /// 설명    : 자식이 여러개인 행동 베이스
    public abstract class Composite : Behaviour, IChildren
    {
        public List<Behaviour> children { get; set; }

        public Composite()
        {
            children = new List<Behaviour>();
        }
    }
}
