using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    // 아까 만든 로딩 UI 캔버스를 여기에 연결하세요
    public GameObject loadingScreenObject; 

    // 싱글톤 (어디서든 부를 수 있게)
    public static SceneLoader Instance;

    void Awake()
    {
        // 이 오브젝트는 씬이 넘어가도 파괴되지 않음
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            DontDestroyOnLoad(loadingScreenObject); // UI도 같이 살려감
        }
        else
        {
            Destroy(gameObject); // 중복 생성 방지
        }
    }

    // ★ 펑거스에서 이 함수를 호출할 겁니다!
    public void LoadSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneProcess(sceneName));
    }

    IEnumerator LoadSceneProcess(string sceneName)
    {
        // 1. 로딩 화면 켜기
        loadingScreenObject.SetActive(true);

        // 2. 비동기 로딩 시작 (화면 멈춤 방지)
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        
        // 로딩이 끝날 때까지 대기 (여기서 뱅글뱅글 아이콘 넣어도 됨)
        while (!op.isDone)
        {
            yield return null;
        }

        // 3. 로딩 끝나면 화면 끄기
        // (너무 순식간에 지나가면 깜빡이는 거 같으니까 살짝 딜레이 줌)
        yield return new WaitForSeconds(0.5f); 
        loadingScreenObject.SetActive(false);
    }
}