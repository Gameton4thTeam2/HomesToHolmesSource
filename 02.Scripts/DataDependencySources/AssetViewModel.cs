using System.ComponentModel;
using HTH.DataModels;
using HTH.DataStructures;

namespace HTH.DataDependencySources
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 유저 재화(자산) 뷰 모델
    /// </summary>
    public class AssetViewModel : SingletonBase<AssetViewModel>
    {
        public GoldSource goldSource { get; private set; }


        //===========================================================================
        //                             Public Methods
        //===========================================================================

        public class GoldSource : INotifyPropertyChanged
        {
            public Gold value { get; private set; }
            public GoldSource(AssetData data)
            {
                value = data.gold;

                data.GoldChanged += (gold) =>
                {
                    value = gold;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(value)));
                };
            }

            public event PropertyChangedEventHandler PropertyChanged;
        }


        //===========================================================================
        //                             Protected Methods
        //===========================================================================

        protected override void Init()
        {
            base.Init();
            goldSource = new GoldSource(AssetData.instance);
        }
    }
}
