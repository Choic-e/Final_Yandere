using UnityEngine;
using UnityEngine.UI;

// 컷씬의 단계별 정보를 담는 구조체
[System.Serializable]
public struct CutsceneStep
{
    // 표시할 서술 텍스트
    public string narrationText;
    
    // 이 단계에서 컷씬 이미지를 변경해야 하는지 여부
    public bool changeImage;
    
    // changeImage가 true일 경우 새 이미지
    public Sprite newCutsceneImage;
    
    // 이 단계 이후에 컷씬이 종료되는지 여부 (Game Over 텍스트 표시)
    public bool isEndingStep;
}

// 컷씬 전체 데이터
[CreateAssetMenu(fileName = "NewCutsceneData", menuName = "Cutscene/Cutscene Data")]
public class CutsceneData : ScriptableObject
{
    // 컷씬 단계들의 배열
    public CutsceneStep[] steps;
}