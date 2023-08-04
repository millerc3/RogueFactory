using MagicArsenal;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ResourceChunkInteractable : MonoBehaviour, IInteractable
{
    [SerializeField] private EntityHealthController healthController;
    [SerializeField] private GameObject resourceWorldItemPrefab;
    [SerializeField] private GameObject mineEffect;
    [SerializeField] private GameObject miningBuddySpotParent;
    private List<Transform> miningBuddySpots = new List<Transform>();
    private Transform miningBuddyOriginalSpot;

    public float InteractionRadius = 25f;

    [SerializeField] private float timeBetweenDamageTicks = .25f;
    [SerializeField] private int damagePerTick = 1;

    private bool isMining = false;
    //private bool isInteracting = false;
    private bool canDealDamage = true;
    private bool shouldMove = true;

    [SerializeField] private MiningBuddyMovement miningBuddy;
    [SerializeField] private GameObject miningBuddyLaserPrefab;
    private GameObject miningBuddyLaserGameObject;
    private MagicBeamStatic miningBuddyLaserEffect;

    private GameObject linkedInteractor;

    private void Awake()
    {
        foreach (Transform t in miningBuddySpotParent.transform)
        {
            miningBuddySpots.Add(t);
        }
    }

    private void Start()
    {
        miningBuddy = FindObjectOfType<MiningBuddyMovement>();
        miningBuddyOriginalSpot = miningBuddy.GetTargetTransform();
    }


    private void Update()
    {
        if (isMining && canDealDamage)
        {
            StartCoroutine(DamageAndPause(timeBetweenDamageTicks));
        }

        if (isMining)
        {
            ConnectLaserToMiningBuddy();

            if (shouldMove)
            {
                StartCoroutine(MoveAndPause(2.25f));
            }

            if (!IsInteractorInRange(linkedInteractor))
            {
                isMining = false;
                linkedInteractor = null;
            }
        }
        else if (miningBuddyLaserGameObject != null && miningBuddyLaserGameObject.activeSelf)
        {
            miningBuddyLaserGameObject.SetActive(false);
            miningBuddy.SetTargetTransform(miningBuddyOriginalSpot);
        }

        
    }

    private bool IsInteractorInRange(GameObject interactor)
    {
        return Vector3.Distance(transform.position, interactor.transform.position) < InteractionRadius;
    }

    private IEnumerator DamageAndPause(float timeToWait)
    {
        EjectItem();
        healthController.Damage(damagePerTick);
        canDealDamage = false;
        yield return new WaitForSeconds(timeToWait);
        canDealDamage = true;
    }

    private IEnumerator MoveAndPause(float timeToWait)
    {
        shouldMove = false;
        yield return new WaitForSeconds(timeToWait);
        if (isMining)
        {
            MoveMiningBuddyToRandomSpot();
        }
        shouldMove = true;
    }

    private void MoveMiningBuddyToRandomSpot()
    {
        miningBuddy.SetTargetTransform(miningBuddySpots[Random.Range(0, miningBuddySpots.Count)]);
    }

    private void ConnectLaserToMiningBuddy()
    {
        miningBuddy.transform.LookAt(transform.position);

        if (miningBuddyLaserGameObject == null)
        {
            miningBuddyLaserGameObject = Instantiate(miningBuddyLaserPrefab, miningBuddy.transform);
            miningBuddyLaserEffect = miningBuddyLaserGameObject.GetComponent<MagicBeamStatic>();
        }

        miningBuddyLaserGameObject.SetActive(true);
    }

    private void EjectItem()
    {
        Instantiate(mineEffect, Random.onUnitSphere * 2f + transform.position, Quaternion.identity);
        //print($"Spawning item: {resourceWorldItemPrefab.name}");
    }

    public UnityAction<IInteractable> OnInteractionComplete { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void StartHover(GameObject interactor)
    {
        print($"Press F to begin mining {gameObject.name}");
    }

    public void EndHover(GameObject interactor)
    {

    }

    public void HoldInteract(GameObject interactor, out bool interactSuccessful)
    {
        //isInteracting = true;
        interactSuccessful = true;
    }

    public void StopHoldInteract(GameObject interactor)
    {
        //isInteracting = false;
    }

    public void TapInteract(GameObject interactor, out bool interactSuccessful)
    {
        if (!IsInteractorInRange(interactor))
        {
            interactSuccessful = false;
            return;
        }

        linkedInteractor = interactor;
        isMining = !isMining;
        interactSuccessful = false;

        if (isMining)
        {
            MoveMiningBuddyToRandomSpot();
        }
    }
}
