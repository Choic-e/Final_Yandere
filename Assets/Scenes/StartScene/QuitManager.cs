using UnityEngine;

public class QuitManager : MonoBehaviour
{
    public void QuitGame()
    {
        Application.Quit();


        #if UNITY_EDITOR
            Debug.Log("게임 종료 요청 (에디터에서는 작동하지 않음)");

        #endif
    }
}