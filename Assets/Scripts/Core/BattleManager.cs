using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

// Controla la secuencia completa de combate
public class BattleManager : MonoBehaviour
{
    // Referencias a unidades en combate
    public List<Unit> playerUnits;   // Mercenarios u aliados
    public List<Unit> enemyUnits;    // Enemigos

    // Lista de turnos ordenados por velocidad
    private List<Unit> turnOrder;
    private int currentTurnIndex;
    public GameManager GameManager;

    // Estado de combate
    public bool battleActive;

    // --- INICIALIZACIÓN DE COMBATE ---

    // Inicia el combate - ordena las unidades por velocidad
    public void StartBattle(List<Unit> players, List<Unit> enemies)
    {
        playerUnits = new List<Unit>(players);
        enemyUnits = new List<Unit>(enemies);
        battleActive = true;
        currentTurnIndex = 0;
        GameManager.SetHealthBars(playerUnits, enemyUnits);

        // Crear el orden de turnos basado en velocidad (de mayor a menor)
        turnOrder = new List<Unit>();
        turnOrder.AddRange(playerUnits);
        turnOrder.AddRange(enemyUnits);
        turnOrder = turnOrder.OrderByDescending(unit => unit.speed).ToList();

        

        Debug.Log("=== BATALLA INICIADA ===");
        Debug.Log("Orden de turnos:");
        for (int i = 0; i < turnOrder.Count; i++)
        {
            Debug.Log($"{i + 1}. {turnOrder[i].GetInfo()}");
        }
        Debug.Log("=======================\n");
    }

    // --- EJECUCIÓN DE TURNOS ---

    // Ejecuta un turno completo - obtiene la unidad actual y la hace atacar
    public void ExecuteTurn()
    {
        if (!battleActive)
            return;

        // Obtener la unidad cuyo turno es
        Unit currentUnit = turnOrder[currentTurnIndex];

        // Visual: indicar que es el turno de esta unidad
        if (currentUnit.unitView != null)
        {
            currentUnit.unitView.FlashTurn();
        }

        // Si la unidad no está viva, saltar su turno
        if (!currentUnit.isAlive)
        {
            Debug.Log($"[SALTO] {currentUnit.unitName} está muerto.");
            AdvanceTurn();
            return;
        }

        // Determinar si es unidad aliada o enemiga
        bool isPlayer = playerUnits.Contains(currentUnit);
        List<Unit> targets = isPlayer ? enemyUnits : playerUnits;

        // Atacar al primer enemigo vivo
        Unit target = GetFirstAliveTarget(targets);

        

        if (target != null)
        {
            PerformAttack(currentUnit, target);
        }
        else
        {
            Debug.Log("No hay objetivos vivos para atacar.");
        }

        // Avanzar al siguiente turno
        AdvanceTurn();

        // Verificar si el combate terminó
        CheckBattleEnd();
    }

    // Obtiene el primer objetivo vivo de una lista
    private Unit GetFirstAliveTarget(List<Unit> targets)
    {
        foreach (Unit unit in targets)
        {
            if (unit.isAlive)
                return unit;
        }
        return null;
    }

    // Realiza un ataque: atacante daña al objetivo con daño físico y mágico
    private void PerformAttack(Unit attacker, Unit target)
    {
        // Visual: movimiento rápido del atacante al realizar el ataque
        if (attacker.unitView != null)
        {
            attacker.unitView.AttackMotion();
        }

        // Usar daño físico y mágico del atacante
        var (physArmorAbsorbed, magArmorAbsorbed, hpDamage) = target.TakeDamage(attacker.physicalDamage, attacker.magicalDamage);

        Debug.Log($"⚔️ {attacker.unitName} ataca a {target.unitName}");
        Debug.Log($"   Daño Físico: {attacker.physicalDamage} | Daño Mágico: {attacker.magicalDamage}");
        Debug.Log($"   Armadura Física absorbió: {physArmorAbsorbed} | Armadura Mágica absorbió: {magArmorAbsorbed} | Daño a HP: {hpDamage}");
        Debug.Log($"   {target.GetInfo()}");

        if (!target.isAlive)
        {
            Debug.Log($"💀 {target.unitName} ha sido derrotado.\n");
        }

        // Visual: destello y sacudida en el objetivo atacado
        if (target.unitView != null)
        {
            target.unitView.FlashHit();
            target.unitView.ShakeOnHit();
        }

        // Actualizar visualización de la unidad atacada
        UpdateUnitVisualsIfExists(target);
    }

    // Actualiza la visualización de una unidad si tiene una vista asignada
    private void UpdateUnitVisualsIfExists(Unit unit)
    {
        if (unit.unitView != null)
        {
            unit.unitView.UpdateVisuals();
        }
    }

    // Avanza al siguiente turno
    private void AdvanceTurn()
    {
        currentTurnIndex++;
        if (currentTurnIndex >= turnOrder.Count)
            currentTurnIndex = 0;
    }

    // --- VERIFICACIÓN DE FIN DE COMBATE ---

    // Verifica si el combate terminó
    public void CheckBattleEnd()
    {
        bool playerAlive = playerUnits.Any(u => u.isAlive);
        bool enemiesAlive = enemyUnits.Any(u => u.isAlive);

        if (!playerAlive)
        {
            EndBattle(false);
        }
        else if (!enemiesAlive)
        {
            EndBattle(true);
        }
    }

    // Termina el combate
    private void EndBattle(bool playerWon)
    {
        battleActive = false;

        if (playerWon)
        {
            Debug.Log("\n🎉 ¡EL JUGADOR HA GANADO!\n");
        }
        else
        {
            Debug.Log("\n💀 ¡LOS ENEMIGOS HAN GANADO!\n");
        }
    }

    // --- MÉTODOS AUXILIARES ---

    // Obtiene el estado actual de la batalla
    public string GetBattleStatus()
    {
        string status = "=== ESTADO DE LA BATALLA ===\n";
        status += "Aliados:\n";
        foreach (Unit unit in playerUnits)
        {
            status += $"  {unit.GetInfo()}\n";
        }
        status += "\nEnemigos:\n";
        foreach (Unit unit in enemyUnits)
        {
            status += $"  {unit.GetInfo()}\n";
        }
        return status;
    }

    // Ejecuta la batalla automática completa
    public void AutomateBattle()
    {
        StartCoroutine(AutomateBattleCoroutine());
    }

    // Corrutina que ejecuta la batalla con delays entre turnos
    private System.Collections.IEnumerator AutomateBattleCoroutine()
    {
        while (battleActive)
        {
            ExecuteTurn();
            yield return new WaitForSeconds(1f);  // Espera 0.5 segundos entre turnos
        }
    }

    // Métodos opcionales para futuro:
    // - Pausar o reanudar combate
    // - Eventos para UI (turno actual, daño recibido, habilidades usadas)
}
