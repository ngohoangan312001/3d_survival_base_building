using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AN
{
    public class AICharacterCombatManager : CharacterCombatManager
    {
        [Header("Detection")] 
        [SerializeField] private float detectionRadius = 15;
        [SerializeField] private float minimumDetectionAngle = -35;
        [SerializeField] private float maximumDetectionAngle = 35;

        public void FindTargetViaLineOfSign(AICharacterManager aiCharacter)
        {
            if (currentTarget != null)
                return;

            Collider[] colider = Physics.OverlapSphere(aiCharacter.transform.position, detectionRadius,
                WorldUtilityManager.Instance.GetCharacterLayers());

            for (int i = 0; i < colider.Length; i++)
            {
                CharacterManager targetCharacter = colider[i].GetComponent<CharacterManager>();

                if (targetCharacter == null)
                    continue;
                
                if (targetCharacter == aiCharacter)
                    continue;
                
                if (targetCharacter.isDead.Value)
                    continue;

                //Check if can attack this targetCharacter, if true, make this target
                if(WorldUtilityManager.Instance.CheckCharacterGroup(aiCharacter.characterGroup,targetCharacter.characterGroup))
                {
                    //Target must be in front of character 
                    Vector3 targetDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                    float viewableAngle = Vector3.Angle(targetDirection, aiCharacter.transform.forward);

                    if (viewableAngle > minimumDetectionAngle && viewableAngle < maximumDetectionAngle)
                    {
                        //check enviroment block
                        if (Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position,
                                targetCharacter.characterCombatManager.lockOnTransform.position,
                                WorldUtilityManager.Instance.GetEnviromentLayers()))
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position,
                                targetCharacter.characterCombatManager.lockOnTransform.position);
                            Debug.Log("Blocked");
                        }
                        else
                        {
                            Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position,
                                targetCharacter.characterCombatManager.lockOnTransform.position, Color.green);
                            aiCharacter.characterCombatManager.SetTarget(targetCharacter);
                        }
                    }

                }
            }
        }
    }
}
