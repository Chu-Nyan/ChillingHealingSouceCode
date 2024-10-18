using ChillingHealing.AI;
using UnityEngine;

public class NPCAI
{
    private readonly BehaviorAI<NPC> _ai;
    private readonly float _waitSoloTime = 5.0f;
    private readonly float _waitAfterMovement = 3.0f;
    private float _stopwatch;

    public BehaviorAI<NPC> AI
    {
        get => _ai;
    }

    public NPCAI()
    {
        _ai = new();
    }

    public NPCAI SetHealingNPC()
    {
        _ai.Init(new Selector<NPC>());

        var sequnce1 = new Sequence<NPC>();
        sequnce1.AddNode(new Leaf<NPC>(IsFixed));
        _ai.AddRootChild(sequnce1);

        var selector1_2 = new Selector<NPC>();
        selector1_2.AddNode(new Leaf<NPC>(IsCampingTogether));
        sequnce1.AddNode(selector1_2);

        var sequnce2 = new Sequence<NPC>();
        sequnce2.AddNode(new Leaf<NPC>(Wait));
        sequnce2.AddNode(new Leaf<NPC>(MoveRandomPosition));
        _ai.AddRootChild(sequnce2);
        return this;
    }

    private MethodResult IsFixed(NPC npc)
    {
        if (npc.IsSetCamping)
        {
            return MethodResult.Success;
        }
        else
        {
            return MethodResult.Failure;
        }
    }

    private MethodResult IsCampingTogether(NPC npc)
    {
        if (npc.CurrentEvent == null)
            return MethodResult.Failure;
        if (npc.CurrentEvent.Data.Universal.InteractionType != InteractionType.Running)
            return MethodResult.Failure;

        if (npc.CurrentEvent.IsPlaying == false)
        {
            _stopwatch += Time.deltaTime;
            if (_stopwatch >= _waitSoloTime)
            {
                _stopwatch = 0;
                npc.CancelInteraction();
                return MethodResult.Failure;
            }
            return MethodResult.Running;
        }
        else
        {
            //npc.RunInteractionAction();
            return MethodResult.Success;
        }
    }


    private MethodResult MoveRandomPosition(NPC npc)
    {
        if (npc.IsMoveing == false)
        {
            var pos = npc.PatrolPosition;
            for (int i = 0; i < 10; i++)
            {
                int randomX = Random.Range(-3, 3);
                int randomY = Random.Range(-4, 4);
                var newPos = new Vector3(pos.x + randomX, pos.y + randomY);
                if (npc.Map.IsMoveableNode(newPos))
                {
                    npc.Move(newPos);
                    break;
                }
            }

            return MethodResult.Success;
        }
        else
        {
            return MethodResult.Failure;
        }
    }

    private MethodResult Wait(NPC npc)
    {
        if (npc.IsMoveing == true)
            return MethodResult.Failure;
        if (_waitAfterMovement <= _stopwatch)
        {
            _stopwatch = 0;
            return MethodResult.Success;
        }
        else
        {
            _stopwatch += Time.fixedDeltaTime;
            return MethodResult.Running;
        }
    }
}
