using Mono.CSharp;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using static GauntletWeapon;

[RequireComponent(typeof(GauntletWeapon))]
public class GauntletWeapon : Weapon
{
    [SerializeField] private GameManager shotEffectPrefab;
    [SerializeField] private GameObject shotImpactEffectPrefab;
    [SerializeField] private int shotsPerMag = 3;
    [SerializeField] private float timeBetweenShots = .5f;
    [SerializeField] public float TimeToReload = 1.5f;
    [SerializeField] private int damagePerShot = 15;
    [SerializeField] private LayerMask hitLayers;
    [SerializeField] private GauntletCrosshair crosshair;

    public bool IsAttemptingToShoot { get; private set; }
    public bool IsAttemptingToReload { get; private set; }
    public bool IsReloading { get; private set; }
    private int shotCount = 0;

    private StateMachine stateMachine;

    public delegate void GauntletShot();
    public GauntletShot OnGauntletShot;

    public delegate void GauntletStartReload();
    public GauntletStartReload OnGauntletStartReload;

    public delegate void GauntletFinishReload();
    public GauntletFinishReload OnGauntletFinishReload;


    protected override void Awake()
    {
        base.Awake();
        IsReloading = false;

        stateMachine = new StateMachine();

        // Define state machine states
        var hasAmmoState = new HasAmmo(this);
        var reloadingState = new Reloading(this);

        // Define state machine transitions
        stateMachine.AddTransition(hasAmmoState,
                                   reloadingState,
                                   () => shotCount == 0);
        stateMachine.AddTransition(hasAmmoState,
                                   reloadingState,
                                   () => IsAttemptingToReload == true && shotCount < shotsPerMag);
        stateMachine.AddTransition(reloadingState,
                                   hasAmmoState,
                                   () => IsReloading == false);
        stateMachine.SetState(hasAmmoState);
    }

    protected override void Start()
    {
        base.Start();

        IsAttemptingToShoot = false;
        shotCount = shotsPerMag;

        crosshair.SetGauntlet(this);
        crosshair.SetShotCount(shotCount, shotsPerMag);
    }

    protected override void Update()
    {
        base.Update();

        stateMachine.Tick();
    }

    public override void Init(PlayerCombatController combatController, Transform projectileStartTransform)
    {
        base.Init(combatController, projectileStartTransform);

        crosshair.Init(combatController.PlayerCamera, combatController.HUDTransform);
    }

    private void ShootGauntlet()
    {
        shotCount--;

        FireRaycast();
        CreateShotEffects();
        crosshair.SetShotCount(shotCount, shotsPerMag);

        OnGauntletShot?.Invoke();
    }

    private void FireRaycast()
    {
        Vector3 dir = playerCombatController.TargetPoint - projectileStartPoint.position;

        RaycastHit hit;
        if (Physics.Raycast(projectileStartPoint.position,
                            dir,
                            out hit,
                            300f,
                            hitLayers))
        {
            Destroy(Instantiate(shotImpactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal)), 2f);

            EntityHealthController healthController = hit.transform.GetComponent<EntityHealthController>();
            if (healthController == null) return;

            healthController.DamageAt(damagePerShot, hit.point);

        }
    }

    private void CreateShotEffects()
    {
        // TODO
    }

    public override void OnPrimaryPressed()
    {
        IsAttemptingToShoot = true;
    }

    public override void OnPrimaryReleased()
    {
        IsAttemptingToShoot = false;
    }

    public override void OnSecondaryPressed()
    {
        IsAttemptingToReload = true;
    }

    public override void OnSecondaryReleased()
    {
        IsAttemptingToReload = false;
    }

    #region States
    public class HasAmmo : IState
    {
        private GauntletWeapon gauntletWeapon;

        private float shotTimer = 0f;

        public HasAmmo(GauntletWeapon _gauntletWeapon)
        {
            gauntletWeapon = _gauntletWeapon;
        }

        public void OnEnter()
        {
            shotTimer = gauntletWeapon.timeBetweenShots;
        }

        public void OnExit()
        {

        }

        public void Tick()
        {
            if (gauntletWeapon.IsAttemptingToShoot && shotTimer >= gauntletWeapon.timeBetweenShots)
            {
                gauntletWeapon.ShootGauntlet();
                shotTimer = 0f;
            }

            shotTimer += Time.deltaTime;
        }
    }

    public class Reloading : IState
    {
        private GauntletWeapon gauntletWeapon;

        private float timer = 0f;

        public Reloading(GauntletWeapon _gauntletWeapon)
        {
            gauntletWeapon = _gauntletWeapon;
        }

        public void OnEnter()
        {
            gauntletWeapon.IsReloading = true;
            gauntletWeapon.OnGauntletStartReload?.Invoke();
        }

        public void OnExit()
        {
            gauntletWeapon.shotCount = gauntletWeapon.shotsPerMag;
            gauntletWeapon.crosshair.SetShotCount(gauntletWeapon.shotCount, 
                                                                         gauntletWeapon.shotsPerMag);
            gauntletWeapon.OnGauntletFinishReload?.Invoke();
        }

        public void Tick()
        {
            if (timer >= gauntletWeapon.TimeToReload)
            {
                gauntletWeapon.IsReloading = false;
                timer = 0f;
            }

            timer += Time.deltaTime;
        }
    }

    #endregion
}