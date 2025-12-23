using UnityEngine;

public class CameraAspectRatio : MonoBehaviour
{
    void Start()
    {
        // 고정하고 싶은 비율 (4:3 = 1.333...)
        float targetaspect = 4.0f / 3.0f;

        // 현재 모니터(창)의 비율
        float windowaspect = (float)Screen.width / (float)Screen.height;

        // 비율에 따른 높이 계산
        float scaleheight = windowaspect / targetaspect;

        Camera camera = GetComponent<Camera>();

        // 1. 현재 창이 4:3보다 홀쭉할 때 (위아래로 길 때) -> 레터박스(위아래 검은띠) 필요 없음, 좌우가 짤림 방지
        if (scaleheight < 1.0f)
        {
            Rect rect = camera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            camera.rect = rect;
        }
        else // 2. 현재 창이 4:3보다 넓을 때 (16:9 등) -> 좌우에 검은띠(필러박스) 생성
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = camera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            camera.rect = rect;
        }
    }
}
