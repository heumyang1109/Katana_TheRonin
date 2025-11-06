using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSceneManager : MonoBehaviour
{
    // 씬 로드 함수
    public void LoadScene(string sceneName)
    {
        // 다음 씬으로 넘어갈 씬의 이름 
        SceneManager.LoadScene(sceneName);
    }
}
