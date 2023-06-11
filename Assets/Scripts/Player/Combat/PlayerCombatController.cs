using UnityEngine;
using QFSW.QC;

public class PlayerCombatController : MonoBehaviour
{
    [Header("Refernces")]
    [SerializeField] private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    [SerializeField] public Transform HUDTransform;
    [SerializeField] private InputManager inputManager;

    [Tooltip("Just a point a good distance away from the camera as a fall back if the player isn't targetting anything")]
    [SerializeField] private Transform defaultTarget;
    public Vector3 TargetPoint;

    [Header("Weapon Info")]
    [SerializeField] private WeaponData currentWeaponData;
    private Weapon currentWeapon;
    [SerializeField] private Transform weaponParent;
    [SerializeField] private Transform projectileStartPoint;
    [SerializeField] private WeaponDatabase weaponDatabase;

    

    private void Start()
    {
        if (currentWeapon == null)
        {
            SetWeapon(currentWeaponData);
        }
    }

    public void Update()
    {
        GetTargetFromCameraLook();
    }

    public void SetWeapon(WeaponData weaponData)
    {
        if (weaponData == null) return;

        UnsetWeapon();

        currentWeaponData = weaponData;

        currentWeapon = Instantiate(currentWeaponData.Prefab, weaponParent).GetComponent<Weapon>();

        currentWeapon.Init(this, projectileStartPoint);

        inputManager.combatControls.PrimaryAttack.performed += currentWeapon.OnPrimaryAction;
        inputManager.combatControls.PrimaryAttack.canceled += currentWeapon.OnPrimaryAction;
        inputManager.combatControls.SecondaryAttack.performed += currentWeapon.OnSecondaryAction;
        inputManager.combatControls.SecondaryAttack.canceled += currentWeapon.OnSecondaryAction;
    }

    [Command]
    public void SetWeapon(int weaponDataId)
    {
        if (weaponDataId == -1)
        {
            UnsetWeapon();
            return;
        }

        SetWeapon(weaponDatabase.GetWeapon(weaponDataId));
    }

    public void UnsetWeapon()
    {
        if (currentWeapon == null) return;

        inputManager.combatControls.PrimaryAttack.performed -= currentWeapon.OnPrimaryAction;
        inputManager.combatControls.PrimaryAttack.canceled -= currentWeapon.OnPrimaryAction;
        inputManager.combatControls.SecondaryAttack.performed -= currentWeapon.OnSecondaryAction;
        inputManager.combatControls.SecondaryAttack.canceled -= currentWeapon.OnSecondaryAction;

        Destroy(currentWeapon.gameObject);
        currentWeapon = null;
        currentWeaponData = null;
    }

    private void GetTargetFromCameraLook()
    {
        RaycastHit hit;
        if (Physics.Raycast(PlayerCamera.transform.position, PlayerCamera.transform.forward, out hit, 500f))
        {
            TargetPoint = hit.point;
        }
        else
        {
            TargetPoint = defaultTarget.position;
        }
    }
}
