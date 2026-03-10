using System.Collections.Generic;
using UnityEngine;

public enum AbilityTarget
{
    Self,
    Ally,
    Enemy
}

// ScriptableObject que representa una habilidad genérica
[CreateAssetMenu(menuName = "Abilities/GenericAbility")]
public class AbilitySO : ScriptableObject
{
    [Header("Información básica")]
    public string abilityName = "New Ability";
    [TextArea] public string description;
    public AbilityTarget targetType = AbilityTarget.Enemy;

    [Header("Efectos")]
    [Tooltip("Multiplica el daño físico/mágico del usuario al aplicarlo al objetivo")]    
    public float damageMultiplier = 1f;
    [Tooltip("Cura al objetivo un porcentaje de su HP máximo (0 = sin curación)")]
    [Range(0f, 1f)] public float healPercent = 0f;
    [Tooltip("Si es verdadero, el objetivo perderá su siguiente turno")]    
    public bool skipTargetTurn = false;

    // Ejecuta la habilidad. "allies" se refiere al equipo del usuario,
    // "enemies" al equipo contrario. BattleManager se pasa para poder
    // actualizar visuales o registrar mensajes.
    public void Execute(Unit user, List<Unit> allies, List<Unit> enemies, BattleManager battleManager)
    {
        Unit target = SelectTarget(user, allies, enemies);
        if (target == null)
            return;

        Debug.Log($"{user.unitName} usa {abilityName} sobre {target.unitName}");

        // Cura primero (si aplica)
        if (healPercent > 0f)
        {
            int healAmount = Mathf.CeilToInt(target.maxHP * healPercent);
            target.Heal(healAmount);
            Debug.Log($"   Cura {healAmount} HP ({target.GetInfo()})");
            battleManager.UpdateUnitVisualsIfExists(target);
            // un mismo ability no hace más de un efecto de curación
            return;
        }

        // Daño
        if (damageMultiplier != 0f)
        {
            int phys = Mathf.RoundToInt(user.physicalDamage * damageMultiplier);
            int mag = Mathf.RoundToInt(user.magicalDamage * damageMultiplier);
            var (physAbs, magAbs, hpDamage) = target.TakeDamage(phys, mag);
            Debug.Log($"   Daño infligido: {hpDamage} (Físico {phys}, Mágico {mag})");
            if (!target.isAlive)
                Debug.Log($"   💀 {target.unitName} ha caído.");
            battleManager.UpdateUnitVisualsIfExists(target);
        }

        // Efecto de incapacidad
        if (skipTargetTurn)
        {
            target.skipNextTurn = true;
            Debug.Log($"   {target.unitName} perderá el siguiente turno.");
        }
    }

    private Unit SelectTarget(Unit user, List<Unit> allies, List<Unit> enemies)
    {
        switch (targetType)
        {
            case AbilityTarget.Self:
                return user;
            case AbilityTarget.Ally:
                foreach (var u in allies)
                    if (u.isAlive)
                        return u;
                return null;
            case AbilityTarget.Enemy:
                foreach (var u in enemies)
                    if (u.isAlive)
                        return u;
                return null;
        }
        return null;
    }
}