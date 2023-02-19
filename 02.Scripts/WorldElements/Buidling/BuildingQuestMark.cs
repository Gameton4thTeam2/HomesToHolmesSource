using UnityEngine;
using HTH.UI;
using HTH.DataDependencySources;
using Cysharp.Threading.Tasks;
using HTH.DataModels;
using UnityEngine.UI;
using HTH.IDs;
using System.Threading.Tasks;

namespace HTH.WorldElements
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_14
    /// 설명    : 퀘스트가 있는 빌딩에 띄울 마크
    /// </summary>
    public class BuildingQuestMark : MonoBehaviour
    {
        public Building building;
        private Transform _tr;
        private Transform _cam;
        private QuestsAcceptedPresenter _questAcceptedPresenter;
        [SerializeField] private Transform _npcIconsContent;
        [SerializeField] private GameObject _npcIconSlotPrefab;

        //===========================================================================
        //                             Private Methods
        //===========================================================================

        private void OnMouseDown()
        {
            QuestListInBuildingUI.instance.Show(building);
        }

        private void Awake()
        {
            _tr = GetComponent<Transform>();
            _cam = Camera.main.transform;

            _questAcceptedPresenter = new QuestsAcceptedPresenter();
            UniTask.Create(async () =>
            {
                await UniTask.WaitUntil(() => NPCAssets.instance != null);

                foreach(var npcInfo in NPCAssets.instance.npcInfos)
                {
                    if (npcInfo.buildilngID != null &&
                        npcInfo.buildilngID.value == building.id.value)
                    {
                        CreateNPCIconSlot(NPCIconAssets.instance[npcInfo.id]);                      
                    }
                    await UniTask.Delay(100);
                }

                await UniTask.WaitUntil(() => _questAcceptedPresenter.source != null);
                _questAcceptedPresenter.source.CollectionChanged += () =>
                {
                    gameObject.SetActive(_questAcceptedPresenter
                         .source
                         .Find(q => NPCAssets.instance[QuestAssets.instance[q].npcId.value].buildilngID.value == building.id.value) >= 0);
                };
                gameObject.SetActive(_questAcceptedPresenter
                         .source
                         .Find(q => NPCAssets.instance[QuestAssets.instance[q].npcId.value].buildilngID.value == building.id.value) >= 0);
            });
        }

        private void Update()
        {
            _tr.LookAt(_cam);
        }

        private void CreateNPCIconSlot(Sprite sprite)
        {
            GameObject tmp = Instantiate(_npcIconSlotPrefab, _npcIconsContent);
            tmp
                .transform
                .GetChild(0)
                .GetComponent<Image>()
                .sprite = sprite;
        }
    }
}