using HTH.InputHandlers;
using HTH.WorldElements;
using UnityEngine;
using UnityEngine.UI;

namespace HTH.UI
{
    [RequireComponent(typeof(Button))]
    public class ResetRoomRotationButton : MonoBehaviour
    {
        public void OnClick()
        {
            if (Player.instance.currentRoom != null)
                //Player.instance.currentRoom.transform.rotation = Quaternion.identity;
                CameraController.instance.SetDefault();
        }

        private void Awake()
        {
            GetComponent<Button>().onClick.AddListener(OnClick);
        }
    }
}