using UnityEngine;
using TMPro;
using HTH.DataDependencySources;
using HTH.DataStructures;
using Cysharp.Threading.Tasks;

namespace HTH.UI
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : Asset View model 에 골드 소스를 바인딩하는 텍스트용 컴포넌트.
    /// 수정자  : 권병석
    /// 수정일  : 2023_02_13
    /// 설명    : 골드의 set이 private으로 설정되어 오류 발생 수정
    /// </summary>
    [RequireComponent(typeof(TMP_Text))]
    public class GoldText : MonoBehaviour
    {
        private TMP_Text _text;
        [BindPropertyTo("value", SourceTag.Gold)]
        public Gold gold
        {
            get
            {
                return _gold;
            }
            set
            {
                _gold = value;
                _text.text = value.GetSimplifiedString();
            }
        }
        [SerializeField] private Gold _gold;
        private PropertyBinder<GoldText> _binder;


        private void Start()
        {
            _text = GetComponent<TMP_Text>();

            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => AssetViewModel.instance != null);
                _binder = new PropertyBinder<GoldText>(this, AssetViewModel.instance.goldSource, SourceTag.Gold);
                gold = AssetViewModel.instance.goldSource.value;
            });
        }
    }
}