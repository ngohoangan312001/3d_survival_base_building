using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AN
{
    public class AICharacterManager : CharacterManager
    {
        [HideInInspector] public AICharacterCombatManager aiCharacterCombatManager;

        [Header("Navmesh Agent")] public NavMeshAgent navMeshAgent;
        
        [Header("Current State")] 
        [SerializeField] private AIState currentState;

        protected override void Awake()
        {
            base.Awake();

            aiCharacterCombatManager = GetComponent<AICharacterCombatManager>();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            ProcessStateMachine();
        }

        public void ProcessStateMachine()
        {
            AIState nextState = currentState?.Tick(this);

            if (nextState != null)
            {
                currentState = nextState;
            }
        }
    }
}
