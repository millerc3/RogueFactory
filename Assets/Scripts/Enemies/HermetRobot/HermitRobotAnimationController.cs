using EasyCharacterMovement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HermitRobotAnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private EntityHealthController healthController;
    [SerializeField] private HermitRobotAIController aiController;
    [SerializeField] private Character character;
    private CharacterMovement characterMovement;

    private bool isMoving = false;
    private bool isDefending = false;

    // Animator strings
    private readonly string OnDamage_trigger = "OnDamage";
    private readonly string OnShoot_trigger = "OnShoot";
    private readonly string OnAOE_trigger = "OnAOE";
    private readonly string isMoving_bool = "isMoving";
    private readonly string isDefending_bool = "isDefending";
    private readonly string forwardSpeed_float = "forwardSpeed";

    private void Awake()
    {
        characterMovement = character.GetCharacterMovement();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        bool tmpMoving = characterMovement.velocity.magnitude > 0;
        if (isMoving != tmpMoving)
        {
            isMoving = tmpMoving;
            animator.SetBool(isMoving_bool, isMoving);
        }

        if (tmpMoving)
        {
            animator.SetFloat(forwardSpeed_float, characterMovement.forwardSpeed);
        }

        bool tmpDefending = aiController.IsDefending;
        if (isDefending != tmpDefending)
        {
            isDefending = tmpDefending;
            animator.SetBool(isDefending_bool, isDefending);
        }
    }

    private void OnEnable()
    {
        healthController.OnDamageAtPoint += SetDamageTrigger;
        aiController.OnShoot += SetShootTrigger;
        aiController.OnAOE += SetAOETrigger;
    }

    private void OnDisable()
    {
        healthController.OnDamageAtPoint -= SetDamageTrigger;
        aiController.OnShoot -= SetShootTrigger;
        aiController.OnAOE -= SetAOETrigger;
    }

    private void SetDamageTrigger(int _, Vector3? __)
    {
        StartCoroutine(SetAnimatorTrigger(OnDamage_trigger, .1f));
    }

    private void SetShootTrigger()
    {
        StartCoroutine(SetAnimatorTrigger(OnShoot_trigger, .1f));
    }

    private void SetAOETrigger()
    {
        StartCoroutine(SetAnimatorTrigger(OnAOE_trigger, .1f));
    }

    private IEnumerator SetAnimatorTrigger(string triggerString, float resetDelay)
    {
        animator.SetTrigger(triggerString);
        yield return new WaitForSeconds(resetDelay);
        animator.ResetTrigger(triggerString);
    }
}
