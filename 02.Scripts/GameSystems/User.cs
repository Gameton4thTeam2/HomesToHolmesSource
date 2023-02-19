using UnityEngine;
using HTH.DataDependencySources;

namespace HTH.GameSystems
{
    /// <summary>
    /// 작성자  : 조영민
    /// 작성일  : 2023_01_09
    /// 설명    : 접속중인 유저 정보
    /// 수정자  : 권병석
    /// 수정일  : 2023_01_26
    /// 설명    : 파이어베이스에 유저 닉네임을 저장시키는 부분 추가
    /// </summary>
    public static class User
    {
        public static bool isloggedIn;
        public static string nickName = "Guest";

        public static string dataRepoDirectory
        {
            get
            {
                return $"{Application.persistentDataPath}/{nickName}";
            }
        }

        public static void Login(string name)
        {
            if (System.IO.Directory.Exists($"{Application.persistentDataPath}/{name}") == false)
            {
                System.IO.Directory.CreateDirectory($"{Application.persistentDataPath}/{name}");
            }
            if(name != "Guest")
                FirebaseManager.instance.MakeNickNameDirectory(name);

            nickName = name;
            isloggedIn = true;
        }
    }
}
    