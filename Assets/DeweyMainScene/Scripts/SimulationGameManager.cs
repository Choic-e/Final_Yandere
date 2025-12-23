using UnityEngine;
using Fungus;

public class SimulationGameManager : MonoBehaviour
{
    [Header("Components")]
    public Flowchart flowchart;
    public UIManager uiManager;
    public PostProcessingController ppController;

    [Header("Game Stats")]
    public int maxTurn = 20;
    public int currentTurn = 1;
    public int maxMP = 100;
    public int currentMP = 100;
    public int love = 0;
    public int obsession = 0;
    public int anger = 0;

    [Header("Timing Logic")]
    private float timerStartTimestamp;
    private bool isTimerRunning = false;
    private float currentFastLimit;      
    private float currentSlowStartLimit; 
    private float currentTimeoutLimit;   

    [Header("Fungus Block Names")]
    public string dialogueBlockPrefix = "Turn_";
    public string complaintFastBlock = "Complaint_Fast"; 
    public string complaintSlowBlock = "Complaint_Slow"; 
    public string timeoutBlockName = "Reaction_Timeout"; 
    public string gameOverBlock = "GameOver";

    [Header("Audio")]
    public AudioClip decisionSound;

 
    [Header("Ending Blocks")]
    public string happyEndingBlock = "Ending_Happy";
    public string obsessionEndingBlock = "Ending_Bad";
    public string angerEndingBlock = "Ending_Normal";
    public string mixedEndingBlock = "Ending_Hidden";


    private string pendingComplaintBlock = ""; 
    private string targetReactionBlock = "";  

    void Start()
    {
        if (uiManager != null) uiManager.InitUI(maxMP, maxTurn);
        if (ppController != null) ppController.UpdateVisualsByMP(currentMP);
        StartTurn();
    }

    void Update()
    {
        if (isTimerRunning)
        {
            float elapsed = Time.time - timerStartTimestamp;
            if (uiManager != null) uiManager.UpdateTimingSlider(elapsed, currentFastLimit, currentSlowStartLimit, currentTimeoutLimit);
            
            if (elapsed >= currentTimeoutLimit)
            {
                OnTimeOut();
            }
        }
    }

    public void StartTurn()
    {
        RefreshUI();
        ActivateMenuDialogs();

        currentFastLimit = Random.Range(0.5f, 2.5f);        
        currentSlowStartLimit = Random.Range(5.0f, 6.0f);   
        currentTimeoutLimit = 8.0f;                        

        Debug.Log($"[Turn {currentTurn}] 시작");

        flowchart.SetIntegerVariable("Turn", currentTurn);
        flowchart.SetIntegerVariable("MP", currentMP);
        
        string blockName = dialogueBlockPrefix + currentTurn;
        if (flowchart.HasBlock(blockName)) flowchart.ExecuteBlock(blockName);
    }

    public void OnDialogueFinished()
    {
        timerStartTimestamp = Time.time;
        isTimerRunning = true;
        if (uiManager != null) uiManager.SetTimingSliderActive(true);
    }

    void ClearMenuDialogs() { MenuDialog[] menuDialogs = FindObjectsOfType<MenuDialog>(); foreach (var menu in menuDialogs) { menu.Clear(); menu.gameObject.SetActive(false); } }
    void ActivateMenuDialogs() { MenuDialog[] menuDialogs = Resources.FindObjectsOfTypeAll<MenuDialog>(); foreach (var menu in menuDialogs) { if (!menu.gameObject.activeInHierarchy) menu.gameObject.SetActive(true); } }

    void OnTimeOut()
    {
        if (!isTimerRunning) return; 
        Debug.Log("타임아웃! -> 즉시 타임아웃 블록 실행 및 턴 종료");
        
        isTimerRunning = false; 
        if (uiManager != null) uiManager.SetTimingSliderActive(false);

        // 모든 진행 중인 대화 중단 및 메뉴 삭제
        flowchart.StopAllBlocks();
        ClearMenuDialogs();

        obsession += 10;
        RefreshUI();

        // 타임아웃은 예약 없이 즉시 실행
        if (flowchart.HasBlock(timeoutBlockName)) 
        {
            flowchart.ExecuteBlock(timeoutBlockName);
            // 주의: Reaction_Timeout 블록 끝에는 EndTurn()이 연결되어야 함.
        }
        else
        {
            EndTurn();
        }
    }

    // ▼▼▼ 플레이어 선택 처리 (저장만 함!) ▼▼▼
    // 이 함수는 버튼 클릭 시 호출되어 계산만 수행합니다.
    public void OnPlayerSelect(int loveBonus, int obsessionBonus, int angerBonus, int mpCost, string reactionBlockName)
    {
        if (!isTimerRunning) return; 

        if (uiManager != null && decisionSound != null) uiManager.PlaySound(decisionSound);
        
        isTimerRunning = false;
        if (uiManager != null) uiManager.SetTimingSliderActive(false);
        ClearMenuDialogs(); // 메뉴창 닫기

        ApplyStats(loveBonus, obsessionBonus, angerBonus, mpCost);
        
        // [핵심] 나중에 갈 반응 블록 이름 저장
        targetReactionBlock = reactionBlockName; 

        // 타이밍 판정 및 불평 블록 예약 (실행 X)
        float responseTime = Time.time - timerStartTimestamp;
        if (responseTime < currentFastLimit)
        {
            anger += 10;
            pendingComplaintBlock = complaintFastBlock; // 예약
        }
        else if (responseTime > currentSlowStartLimit)
        {
            obsession += 5;
            pendingComplaintBlock = complaintSlowBlock; // 예약
        }
        else
        {
            pendingComplaintBlock = ""; // 불평 없음
        }

        RefreshUI();
        // 여기서 아무 블록도 실행하지 않습니다! 주인공 대사가 끝날 때까지 기다립니다.
    }

    public void TryPlayComplaint()
    {
        // 예약된 불평이 있으면 실행
        if (!string.IsNullOrEmpty(pendingComplaintBlock) && flowchart.HasBlock(pendingComplaintBlock))
        {
            flowchart.ExecuteBlock(pendingComplaintBlock);
        }
        else
        {
            ProceedToReaction();
        }
        
        pendingComplaintBlock = "";
    }


    public void ProceedToReaction()
    {
        if (!string.IsNullOrEmpty(targetReactionBlock) && flowchart.HasBlock(targetReactionBlock))
        {
            flowchart.ExecuteBlock(targetReactionBlock);
        }
        else
        {
            EndTurn();
        }
        targetReactionBlock = ""; // 초기화
    }
    
    void ApplyStats(int l, int o, int a, int mp)
    {
        love += l;
        obsession += o;
        anger += a;
        currentMP -= mp;
        if (currentMP < 0) currentMP = 0;
        if (ppController != null) ppController.UpdateVisualsByMP(currentMP);
        RefreshUI();
    }

    void RefreshUI()
    {
        if (uiManager != null) uiManager.UpdateAllStats(currentTurn, currentMP, maxMP, love, obsession, anger);
    }

    public void EndTurn()
    {
        currentTurn++;
        StartTurn();
    }

    public void CheckAndPlayEnding()
    {
        Debug.Log($"엔딩 판정 시작 - Love: {love}, Obsession: {obsession}, Anger: {anger}");

        int maxStat = Mathf.Max(love, obsession, anger);
        string targetEndingBlock = "";


        if (love == maxStat)
        {
            Debug.Log("판정: Love가 최고점(우선순위 1위) -> 해피 엔딩");
            targetEndingBlock = happyEndingBlock;
        }
        else 
        {
            if (obsession == anger)
            {
                Debug.Log("판정: Obsession과 Anger가 동점 -> 믹스드(히든) 엔딩");
                targetEndingBlock = mixedEndingBlock;
            }

            else if (obsession > anger)
            {
                Debug.Log("판정: Obsession 단독 1등 -> 배드(집착) 엔딩");
                targetEndingBlock = obsessionEndingBlock;
            }
            else 
            {
                Debug.Log("판정: Anger 단독 1등 -> 노말(분노) 엔딩");
                targetEndingBlock = angerEndingBlock;
            }
        }

        if (!string.IsNullOrEmpty(targetEndingBlock) && flowchart.HasBlock(targetEndingBlock))
        {
            flowchart.ExecuteBlock(targetEndingBlock);
        }
        else
        {
            Debug.LogError($"엔딩 블록을 찾을 수 없거나 설정되지 않음: {targetEndingBlock}");
        }
    }
}