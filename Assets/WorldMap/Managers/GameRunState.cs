using System.Collections.Generic;
using UnityEngine;

public class GameRunState : MonoBehaviour
{
    public static GameRunState Instance { get; private set; }

    [Header("Scene Flow")]
    public string returnSceneName = "WorldMapScene";

    [Header("Battle Request")]
    public string currentBattleGroupId;
    public string returnNodeId;

    [Header("Battle Result")]
    public bool hasPendingCombatResult;
    public bool lastCombatWasVictory;
    public string lastCompletedCombatNodeId;

    [Header("Pending Battle Reward")]
    public bool hasPendingBattleReward;
    public RewardData pendingBattleReward;

    [Header("Map State")]
    public string currentNodeId;

    public List<string> completedNodeIds = new List<string>();
    public List<string> unlockedNodeIds = new List<string>();
    public List<string> visitedNodeIds = new List<string>();
    public List<string> eventCompletedNodeIds = new List<string>();
    public List<string> flags = new List<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RequestBattle(string battleGroupId, string nodeId, string sceneToReturn)
    {
        RequestBattle(battleGroupId, nodeId, sceneToReturn, null);
    }

    public void RequestBattle(string battleGroupId, string nodeId, string sceneToReturn, RewardData rewardOnVictory)
    {
        currentBattleGroupId = battleGroupId;
        returnNodeId = nodeId;
        returnSceneName = sceneToReturn;

        RegisterCurrentNode(nodeId);
        RegisterVisitedNode(nodeId);

        hasPendingCombatResult = false;
        lastCombatWasVictory = false;
        lastCompletedCombatNodeId = "";

        pendingBattleReward = rewardOnVictory;
        hasPendingBattleReward = rewardOnVictory != null && !rewardOnVictory.IsEmpty();
    }

    public void RegisterCombatResult(bool victory)
    {
        hasPendingCombatResult = true;
        lastCombatWasVictory = victory;
        lastCompletedCombatNodeId = returnNodeId;

        RegisterCurrentNode(returnNodeId);
        RegisterVisitedNode(returnNodeId);

        if (victory && !completedNodeIds.Contains(returnNodeId))
        {
            completedNodeIds.Add(returnNodeId);
        }
    }

    public RewardData ConsumePendingBattleReward()
    {
        if (!hasPendingBattleReward || pendingBattleReward == null)
            return null;

        RewardData reward = pendingBattleReward;

        pendingBattleReward = null;
        hasPendingBattleReward = false;

        return reward;
    }

    public void ClearPendingBattleReward()
    {
        pendingBattleReward = null;
        hasPendingBattleReward = false;
    }

    public void RegisterCurrentNode(string nodeId)
    {
        if (!string.IsNullOrWhiteSpace(nodeId))
        {
            currentNodeId = nodeId;
        }
    }

    public void RegisterVisitedNode(string nodeId)
    {
        if (!string.IsNullOrWhiteSpace(nodeId) && !visitedNodeIds.Contains(nodeId))
        {
            visitedNodeIds.Add(nodeId);
        }
    }

    public void RegisterUnlockedNode(string nodeId)
    {
        if (!string.IsNullOrWhiteSpace(nodeId) && !unlockedNodeIds.Contains(nodeId))
        {
            unlockedNodeIds.Add(nodeId);
        }
    }

    public void RegisterEventCompleted(string nodeId)
    {
        if (!string.IsNullOrWhiteSpace(nodeId) && !eventCompletedNodeIds.Contains(nodeId))
        {
            eventCompletedNodeIds.Add(nodeId);
        }
    }

    public void RegisterFlag(string flag)
    {
        if (!string.IsNullOrWhiteSpace(flag) && !flags.Contains(flag))
        {
            flags.Add(flag);
        }
    }

    public bool HasCompletedNode(string nodeId)
    {
        return completedNodeIds.Contains(nodeId);
    }

    public bool HasUnlockedNode(string nodeId)
    {
        return unlockedNodeIds.Contains(nodeId);
    }

    public bool HasVisitedNode(string nodeId)
    {
        return visitedNodeIds.Contains(nodeId);
    }

    public bool HasCompletedEvent(string nodeId)
    {
        return eventCompletedNodeIds.Contains(nodeId);
    }

    public bool HasFlag(string flag)
    {
        return flags.Contains(flag);
    }

    public void ClearPendingCombatResult()
    {
        hasPendingCombatResult = false;
        lastCombatWasVictory = false;
        lastCompletedCombatNodeId = "";
    }
}