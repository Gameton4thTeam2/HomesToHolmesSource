using HTH.GameSystems;
using TMPro;

namespace HTH.UI
{
    /// <summary>
    /// 작성자      : 권병석
    /// 작성일      : 2023_01_19
    /// 설명        : Localization을 위해 TMP_Text에 부여하는 클래스
    /// 최종수정자  : 권병석
    /// 최종수정일  : 2023_02_12
    /// 설명        : Localization 데이터 인덱서에 맞게 수정 [2023_02_09, 권병석]
    ///               Localization이 제대로 적용되지 않았던 부분 수정
    /// </summary>
    public class TMP_LocalizationText : TextMeshProUGUI
    {
        private string _original;

        public override string text
        {
            get => base.text;
            set
            {
                if (string.IsNullOrEmpty(value))
                    return;
                if (value == "#USERNAME")
                {
                    base.text = User.nickName;
                    return;
                }
                if (value.Split('_').Length < 2)
                    return;
                if (string.IsNullOrEmpty(_original))
                    _original = value;

                string tableName = $"{value.Split('_')[0]}_{value.Split('_')[1]}";
                base.text = Localization.instance[tableName, value];
            }
        }


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public void Refresh()
        {
            if (string.IsNullOrEmpty(_original))
                return;

            text = _original;
        }


        //===========================================================================
        //                             Private Methods
        //===========================================================================

        override protected void Awake()
        {
            base.Awake();
            _original = text;
            Refresh();
        }

        override protected void OnEnable()
        {
            base.OnEnable();
            Localization.instance.OnLanguageChange += Refresh;
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            Localization.instance.OnLanguageChange -= Refresh;
        }

    }
}