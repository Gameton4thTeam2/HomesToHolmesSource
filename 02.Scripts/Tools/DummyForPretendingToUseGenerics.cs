using HTH.DataDependencySources;
using HTH.InputHandlers;

namespace HTH
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_02_09
    /// 설명    : IL2CPP 빌드에서 Code Stripping 시 사용하지 않는다고 판단되는 코드를 제거하는 것을 막기위한 더미클래스.
    /// 예로, Generic 은 CPP 빌드시 동적으로 타입생성이 되는게 아니라 빌드시에 사용될만한 모든 타입에 대해 클래스를 
    /// 개별적으로 정의해서 사용하기때문에 코드가 제거되면 생성자 호출이 불가능하게됨. 
    /// 이런 현상이 발생할만한 클래스들의 생성자들을 사용하는척 하기만 해주도록 함.
    /// </summary>
    public class DummyForPretendingToUseGenerics
    {
        private static AssetViewModel _assetViewModel = new AssetViewModel();
        private static ControllerManager _controllerManager = new ControllerManager();
    }
}
