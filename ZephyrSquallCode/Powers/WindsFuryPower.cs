using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Hooks;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using ZephyrSquall.ZephyrSquallCode.Hooks;

namespace ZephyrSquall.ZephyrSquallCode.Powers;

public sealed class WindsFuryPower : ZephyrSquallPower, IModifyAttackHitCountMultiplicative
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override object InitInternalData() => new Data();

    public int ModifyAttackHitCountMultiplicative(AttackCommand attack, int hitCount)
    {
        if (hitCount > 1 && attack.ModelSource is CardModel modelSource && modelSource.Type == CardType.Attack &&
            modelSource.Owner.Creature == Owner)
        {
            if (GetInternalData<Data>().checkedCards.Contains(modelSource)) return hitCount;

            GetInternalData<Data>().checkedCards.Add(modelSource);
            int multiHitCardsPlayedThisTurn = CombatManager.Instance.History.CardPlaysFinished.Count(e =>
            {
                if (!e.HappenedThisTurn(CombatState) || e.CardPlay.Player.Creature != Owner ||
                    e.CardPlay.Card.Type != CardType.Attack)
                    return false;

                // This means of getting the card's hit count comes from a combination of how Thrash gets the card's
                // damage and how HitCountVar previews the hit count.
                int hitCount = 1;
                if (e.CardPlay.Card.DynamicVars.ContainsKey("CalculatedHits"))
                    hitCount = (int)((CalculatedVar)e.CardPlay.Card.DynamicVars["CalculatedHits"]).Calculate(null);
                else if (e.CardPlay.Card.DynamicVars.ContainsKey("HitCount"))
                    hitCount = e.CardPlay.Card.DynamicVars.HitCount.IntValue;
                // Note this call is wrapped between adding and removing the current modelSource to checkedCards to
                // prevent infinite recursion.
                return Hook.ModifyAttackHitCount(CombatState,
                    DamageCmd.Attack(0M).FromCard(e.CardPlay.Card, e.CardPlay), hitCount) > 1;
            });
            GetInternalData<Data>().checkedCards.Remove(modelSource);

            int modifiedHitCount = multiHitCardsPlayedThisTurn < Amount ? hitCount * 2 : hitCount;

            return modifiedHitCount;
        }

        return hitCount;
    }

    // As we call the ModifyAttackHitCount hook within ModifyAttackHitCountMultiplicative, we must keep track of cards
    // we've already seen and stop upon seeing them again to avoid infinite recursion. (The second recursive call only
    // cares about whether the card hit exactly 1 time or not for the sake of counting how many multi-hit Attacks have
    // been played this turn. Wind's Fury's own effect cannot change whether a card hit exactly 1 time, so it is safe to
    // just ignore the card when it is found again recursively.)
    private class Data
    {
        public HashSet<CardModel> checkedCards = new();
    }
}