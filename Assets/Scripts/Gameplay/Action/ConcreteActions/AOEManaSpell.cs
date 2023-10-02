using System;
using System.Collections;
using Unity.BossRoom.Gameplay.GameplayObjects;
using Unity.BossRoom.Gameplay.GameplayObjects.Character;
using UnityEngine;

namespace Unity.BossRoom.Gameplay.Actions
{
    /// <summary>
    /// Area-of-effect attack Action. The attack is centered on a point provided by the client.
    /// </summary>
    [CreateAssetMenu(menuName = "BossRoom/Actions/AOE Action Mana Spell")]
    public class AOEManaSpell : AOEAction
    {
        private int repeatsAmount = 0;
        private float nextRepeatTime = 0f;
        private bool hasPerformedAtLeastOnce = false;
        public override bool OnStart(ServerCharacter serverCharacter)
        {
            repeatsAmount = Config.RepeatAmount;
            nextRepeatTime = TimeRunning + Config.RepeatTime;

            return base.OnStart(serverCharacter);
        }

        public override bool OnUpdate(ServerCharacter clientCharacter)
        {
            if (TimeRunning >= Config.ExecTimeSeconds)
            {
                if (!hasPerformedAtLeastOnce)
                {
                    PerformAoE(clientCharacter);
                    hasPerformedAtLeastOnce = true;
                }
                else if (repeatsAmount > 0 && TimeRunning >= nextRepeatTime)
                {
                    PerformAoE(clientCharacter);

                    nextRepeatTime = TimeRunning + Config.RepeatTime;
                    repeatsAmount--;

                    if (repeatsAmount == 0)
                        return ActionConclusion.Continue;

                }
            }

            return ActionConclusion.Continue;
        }

        public override void PerformAoE(ServerCharacter parent)
        {
            var colliders = Physics.OverlapSphere(m_Data.Position, Config.Radius, LayerMask.GetMask("PCs"));
            for (var i = 0; i < colliders.Length; i++)
            {
                var ally = colliders[i].GetComponent<IDamageable>();
                if (ally != null)
                {
                    ally.ReceiveMana(parent, Config.Amount);
                }
            }
        }
    }
}
