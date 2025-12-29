using cfg.HuntingConfig;
using cfg.HuntingConfig.Enum;
using GameFramework.Core.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIComponentQuest : MonoBehaviour, IUIComponent
{
    [SerializeField] private TextMeshProUGUI questInfoText;
    [SerializeField] private Image questProgress;
    [SerializeField] private TextMeshProUGUI questProgressText;

    public void Init()
    {
        GameServiceLocator.EventManager.AddListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        GameServiceLocator.EventManager.AddListener(QuestEvents.QuestProgressUpdated, OnQuestProgressUpdated);
        GameServiceLocator.EventManager.AddListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        GameServiceLocator.EventManager.AddListener(QuestEvents.QuestTimeout, OnQuestTimeout);
        gameObject.SetActive(false);
    }

    private void OnQuestDispatched(QuestDispatchedEventArgs eventArgs)
    {
        gameObject.SetActive(true);
        Quest quest = eventArgs.QuestData;
        string questInfo = GetQuestInfoFromQuestType(quest.QuestType);
        questInfoText.text = questInfo;
        questProgress.fillAmount = 0f;
        questProgressText.text = $"0/{eventArgs.TargetValue}";
    }
    
    private void OnQuestProgressUpdated(QuestProgressUpdatedEventArgs eventArgs)
    {
        questProgress.fillAmount = (float)eventArgs.CurrentProgress / eventArgs.TargetValue;
        questProgressText.text = $"{eventArgs.CurrentProgress}/{eventArgs.TargetValue}";
    }
    
    private void OnQuestCompleted()
    {
        gameObject.SetActive(false);
    }
    
    private void OnQuestTimeout()
    {
        gameObject.SetActive(false);
    }

    private string GetQuestInfoFromQuestType(EQuestType questType)
    {
        switch (questType)
        {
            case EQuestType.CollectMeat: return "获取数量肉";
            case EQuestType.KillLargeAnimal: return "击杀大型动物";
            case EQuestType.Settle: return "进行结算次数";
            case EQuestType.UsePaidItem: return "使用道具";
            default: return "";
        }
    }

    public void CleanUp()
    {
        GameServiceLocator.EventManager.RemoveListener(QuestEvents.QuestDispatched, OnQuestDispatched);
        GameServiceLocator.EventManager.RemoveListener(QuestEvents.QuestProgressUpdated, OnQuestProgressUpdated);
        GameServiceLocator.EventManager.RemoveListener(QuestEvents.QuestCompleted, OnQuestCompleted);
        GameServiceLocator.EventManager.RemoveListener(QuestEvents.QuestTimeout, OnQuestTimeout);
    }
}