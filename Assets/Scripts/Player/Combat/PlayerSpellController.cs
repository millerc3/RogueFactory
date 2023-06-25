using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpellController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Camera playerCamera;
    public Camera PlayerCamera => playerCamera;
    public Transform HUDTransform;
    [SerializeField] private InputManager inputManager;

    [Tooltip("Just a point a good distance away from the camera as a fall back if the player isn't targetting anything")]
    [SerializeField] private Transform defaultTarget;
    public Vector3 TargetPoint;

    [Header("Spell info")]
    [SerializeField] private SpellData spellDatabse;
    [SerializeField] private SpellData primarySpellData;
    private Spell primarySpell;
    [SerializeField] private SpellData secondarySpellData;
    private Spell secondarySpell;
    [SerializeField] private Transform footSpellCastPoint;
    [SerializeField] private Transform bodySpellCastPoint;

    public void SetPrimarySpell(SpellData spellData)
    {
        if (spellData == null) return;

        UnsetPrimarySpell();
        primarySpellData = spellData;
    }

    public void SetSecondarySpell(SpellData spellData)
    {
        if (spellData == null) return;

        UnsetSecondarySpell();
        secondarySpellData = spellData;
    }

    public void UnsetPrimarySpell()
    {
        if (primarySpellData == null) return;

        primarySpell = null;
    }

    public void UnsetSecondarySpell()
    {
        if (secondarySpellData == null) return;

        secondarySpell = null;
    }
}
