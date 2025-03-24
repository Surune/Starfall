using UnityEngine;
using BackEnd;
using System;

namespace Starfall.Manager
{
    public class BackendManager
    {
        public BackendManager()
        {
            var bro = Backend.Initialize(true); // 뒤끝 초기화

            // 뒤끝 초기화에 대한 응답값
            if (bro.IsSuccess())
            {
                Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
            }
            else
            {
                Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생
            }

            Backend.BMember.DeleteGuestInfo();
            BackendReturnObject bro_login = Backend.BMember.GuestLogin("게스트 로그인으로 로그인함");
            if (bro_login.IsSuccess())
            {
                Debug.Log("로그인 성공 : " + bro_login); // 성공일 경우 statusCode 204 Success
            }
            else
            {
                Debug.LogError("로그인 실패 : " + bro_login); // 실패일 경우 statusCode 400대 에러 발생
            }
        }

        public void UploadGameData(bool cleared)
        {
            try
            {
                // 능력들과 클리어 여부, 게임 버전을 서버에 업로드
                Param param = new()
                {
                    { "Version", Application.version },
                    { "Cleared", cleared },
                    { "Abilities", GameManager.Instance.AbilityNumbers }
                };
                Debug.Log(param);

                var bro = Backend.GameData.Insert("gameData", param);

                if (bro.IsSuccess())
                {
                    Debug.Log("내 playerInfo의 indate : " + bro.GetInDate());
                }
                else
                {
                    Debug.LogError("게임 정보 삽입 실패 : " + bro.ToString());
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Processing failed : " + e);
            }
        }
    }
}
