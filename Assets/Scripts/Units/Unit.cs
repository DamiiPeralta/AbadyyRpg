using UnityEngine;
using System.Collections.Generic;

// Unidad en runtime: instancia de UnitSO con estado mutable
public class Unit
{
    // Datos básicos - Identificación y estadísticas
    public string unitName;           // Nombre de la unidad (para identificarla)
    public int maxHP;                 // HP máximo
    public int currentHP;             // HP actual
    public int speed;                 // Velocidad (determina orden de turnos)
    public int baseDamage;            // Daño base del ataque (legacy, calculado como fis + mag si se usa)
    
    // Daño físico y mágico (editable desde inspector vía UnitData)
    public int physicalDamage;        // Daño físico del ataque
    public int magicalDamage;         // Daño mágico del ataque

    // Armaduras
    public int maxPhysicalArmor;      // Armadura física máxima
    public int currentPhysicalArmor;  // Armadura física actual
    public int maxMagicalArmor;       // Armadura mágica máxima
    public int currentMagicalArmor;   // Armadura mágica actual

    // Estado mutable
    public bool isAlive;              // Estado de vida
    public UnitData unitData;         // Referencia a UnitSO (opcional)
    public UnitView unitView;         // Referencia a la visualización (opcional)

    // Sistema de habilidades
    public List<AbilitySO> abilities = new List<AbilitySO>();
    private int nextAbilityIndex = 0; // cual usar a continuación

    // Efectos de estado simples
    public bool skipNextTurn = false;

    // Accede a la siguiente habilidad en la lista (y avanza el índice)
    public AbilitySO GetNextAbility()
    {
        if (abilities == null || abilities.Count == 0)
            return null;
        AbilitySO ability = abilities[nextAbilityIndex];
        nextAbilityIndex = (nextAbilityIndex + 1) % abilities.Count;
        return ability;
    }

    public Unit(string name, int hp, int spd, int dmg)
    {
        // constructor simple, sigue existiendo por compatibilidad
        unitName = name;
        maxHP = hp;
        currentHP = hp;
        speed = spd;
        baseDamage = dmg;
        physicalDamage = dmg;  // Por defecto, todo el daño es físico
        magicalDamage = 0;
        maxPhysicalArmor = 0;
        currentPhysicalArmor = 0;
        maxMagicalArmor = 0;
        currentMagicalArmor = 0;
        isAlive = true;
    }
    
    // Constructor con daños y armaduras iniciales
    public Unit(string name, int hp, int spd, int physDmg, int magDmg, int physArmor, int magArmor)
    {
        unitName = name;
        maxHP = hp;
        currentHP = hp;
        speed = spd;
        baseDamage = physDmg + magDmg;
        physicalDamage = physDmg;
        magicalDamage = magDmg;
        maxPhysicalArmor = physArmor;
        currentPhysicalArmor = physArmor;
        maxMagicalArmor = magArmor;
        currentMagicalArmor = magArmor;
        isAlive = true;
    }

    // Si en el futuro se necesitan valores de inicio diferentes al máximo se pueden ajustar después de crear la unidad
    // Obtiene el HP actual como porcentaje
    public float GetHPPercentage()
    {
        return (float)currentHP / maxHP;
    }

    // Aplica daño físico y mágico, reduciendo armadura primero
    // Retorna: (daño absorbido por armadura física, daño absorbido por armadura mágica, daño aplicado a HP)
    public (int physArmorAbsorbed, int magArmorAbsorbed, int hpDamage) TakeDamage(int physicalDmg, int magicalDmg)
    {
        // Calcular daño absorbido por armadura física
        int physArmorAbsorbed = Mathf.Min(currentPhysicalArmor, physicalDmg);
        currentPhysicalArmor -= physArmorAbsorbed;
        int physRemainder = physicalDmg - physArmorAbsorbed;

        // Calcular daño absorbido por armadura mágica
        int magArmorAbsorbed = Mathf.Min(currentMagicalArmor, magicalDmg);
        currentMagicalArmor -= magArmorAbsorbed;
        int magRemainder = magicalDmg - magArmorAbsorbed;

        // Aplicar daño restante a HP
        int totalHPDamage = physRemainder + magRemainder;
        currentHP -= totalHPDamage;
        if (currentHP < 0)
            currentHP = 0;

        if (currentHP == 0)
            isAlive = false;

        return (physArmorAbsorbed, magArmorAbsorbed, totalHPDamage);
    }

    // Método legacy: si solo se pasa un daño, se trata como físico
    public void TakeDamage(int damage)
    {
        TakeDamage(damage, 0);
    }

    // Sana a la unidad (respeta el HP máximo)
    public void Heal(int amount)
    {
        currentHP += amount;
        if (currentHP > maxHP)
            currentHP = maxHP;
    }

    // Restaura HP completo
    public void FullHeal()
    {
        currentHP = maxHP;
        isAlive = true;
    }

    // Obtiene información de la unidad como string
    public string GetInfo()
    {
        string info = $"{unitName} | HP: {currentHP}/{maxHP} | SPD: {speed} | Phys DMG: {physicalDamage} | Mag DMG: {magicalDamage} | Phys Armor: {currentPhysicalArmor}/{maxPhysicalArmor} | Mag Armor: {currentMagicalArmor}/{maxMagicalArmor}";
        if (abilities != null && abilities.Count > 0)
        {
            info += " | Abilities:";
            foreach (var a in abilities)
                info += " " + a.abilityName;
        }
        return info;
    }

    // Métodos y responsabilidades para el futuro
    // -----------------------------------------------

    // Resetea el índice de habilidades (útil al iniciar una batalla)
    public void ResetAbilities()
    {
        nextAbilityIndex = 0;
    }

    // Inicializa la unidad en combate a partir de UnitSO
    // - Crea instancias de Ability a partir de AbilitySO
    // public void Initialize(UnitSO data);

    // Aplica daño considerando resistencias y vulnerabilidades
    // public void TakeDamage(int damage, DamageType type);

    // Calcula el daño que esta unidad puede infligir
    // public int DealDamage(Ability ability, Unit target);

    // Ejecuta el turno de la unidad
    // - Revisa la lista de habilidades en orden
    // - Si la primera habilidad tiene AP suficiente, la ejecuta
    // - Si no tiene AP suficiente, realiza un ataque básico
    // - Genera AP según el ataque básico o efectos pasivos
    // public void TakeTurn(List<Unit> allies, List<Unit> enemies);

    // Verifica si la unidad sigue viva
    // public bool CheckAlive();

    // Recupera AP, aplica efectos de pasivas, buffs/debuffs
    // public void EndTurnUpdate();

    // Métodos opcionales para mercenarios:
    // - Equipar o desequipar items
    // - Aplicar efectos de equipo
}
