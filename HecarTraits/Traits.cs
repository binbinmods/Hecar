using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using Obeliskial_Content;
using UnityEngine;
using static Hecar.CustomFunctions;
using static Hecar.Plugin;

namespace Hecar
{
    [HarmonyPatch]
    internal class Traits
    {
        // list of your trait IDs
        public static string heroName = "ulfvitr";

        public static string subclassname = "stormshaman";

        public static string[] simpleTraitList = ["trait0", "trait1a", "trait1b", "trait2a", "trait2b", "trait3a", "trait3b", "trait4a", "trait4b"];

        public static string[] myTraitList = (string[])simpleTraitList.Select(trait => heroName + trait); // Needs testing

        static string trait0 = myTraitList[0];
        static string trait2a = myTraitList[3];
        static string trait2b = myTraitList[4];
        static string trait4a = myTraitList[7];
        static string trait4b = myTraitList[8];

        public static bool isDamagePreviewActive = false;

        public static bool isCalculateDamageActive = false;
        public static int infiniteProctection = 0;
        public static int infiniteProctectionPowerful = 0;

        public static string debugBase = "Binbin - Testing " + heroName + " ";

        public static void DoCustomTrait(string _trait, ref Trait __instance)
        {
            // get info you may need
            Enums.EventActivation _theEvent = Traverse.Create(__instance).Field("theEvent").GetValue<Enums.EventActivation>();
            Character _character = Traverse.Create(__instance).Field("character").GetValue<Character>();
            Character _target = Traverse.Create(__instance).Field("target").GetValue<Character>();
            int _auxInt = Traverse.Create(__instance).Field("auxInt").GetValue<int>();
            string _auxString = Traverse.Create(__instance).Field("auxString").GetValue<string>();
            CardData _castedCard = Traverse.Create(__instance).Field("castedCard").GetValue<CardData>();
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            TraitData traitData = Globals.Instance.GetTraitData(_trait);
            List<CardData> cardDataList = [];
            List<string> heroHand = MatchManager.Instance.GetHeroHand(_character.HeroIndex);
            Hero[] teamHero = MatchManager.Instance.GetTeamHero();
            NPC[] teamNpc = MatchManager.Instance.GetTeamNPC();

            if (_trait == trait0)
            { // TODO trait 0
                string traitName = traitData.TraitName;
                string traitId = _trait;
                LogDebug($"Handling Trait {traitId}: {traitName}");
                // When this hero would overheal a character, apply Shield equal to 50% of the amount overhealed (Benefits 50% from Shield bonuses). Suffer 20% of the amount as Shielded as Insane.
                if (CanIncrementTraitActivations(_trait))
                {
                    IncrementTraitActivations(_trait);
                    DisplayRemainingChargesForTrait(ref _character, traitData);

                }
            }


            else if (_trait == trait2a)
            {
                // Scourge on this hero increases Damage and Healing by 10% per charge.
                // When this character blocks, Suffer and Apply 2 Scourge. 

                string traitName = traitData.TraitName;
                string traitId = _trait;
                LogDebug($"Handling Trait {traitId}: {traitName}");
                DisplayTraitScroll(ref _character, traitData);

                if(_character!=null&&_target!=null&&_character.Alive&&_target.Alive)
                {
                    _character.SetAuraTrait(_character,"scourge",2);
                    _target.SetAuraTrait(_character,"scourge",2);
                }

            }



            else if (_trait == trait2b)
            { // TODO trait 2b
                string traitName = traitData.TraitName;
                string traitId = _trait;
                LogDebug($"Handling Trait {traitId}: {traitName}");
                DisplayTraitScroll(ref _character, traitData);
                // Insane on this hero increases Shield charges by 1 per 20 insane.
                // If this character has more than 40 insane, when they cast a spell, reduce insane by 25%. Decrease the cost of their highest cost card by 1 for every 10 Insane removed (2x per turn).
                if(!IsLivingHero(_character)||_character.GetAuraCharges("insane")<40)
                {
                    return;
                }

                int bonusActivations = _character.HaveTrait(trait4a) ? 1 : 0;
                if(CanIncrementTraitActivations(traitId))
                {                    
                    AuraCurseData insane = GetAuraCurseData("insane");
                    int nInsane = _character.GetAuraCharges("insane");
                    int insaneToApply = Mathf.RoundToInt(nInsane*0.75f);
                    int amountToReduce = Mathf.FloorToInt(nInsane*0.025f);
                    _character.HealAuraCurse(insane);
                    _character.SetAura(_character,insane,insaneToApply);
                    IncrementTraitActivations(traitId);
                
                }
                

            }

            else if (_trait == trait4a)
            { // Scourge on all characters can stack. 
                // Scourge on enemies reduces damage done by 5% per charge (caps at 50%). 
                // Increases activations of 2b by 1. 

                string traitName = traitData.TraitName;
                string traitId = _trait;
                LogDebug($"Handling Trait {traitId}: {traitName}");

            }

            else if (_trait == trait4b)
            {
                // 4b: Insane on this hero increases mind damage and healing by 2% per charge.
                // 4b: Crack on enemies decreases mind resistance by 1% per charge.
                // Done in GACM

            }

        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Trait), "DoTrait")]
        public static bool DoTrait(Enums.EventActivation _theEvent, string _trait, Character _character, Character _target, int _auxInt, string _auxString, CardData _castedCard, ref Trait __instance)
        {
            if ((UnityEngine.Object)MatchManager.Instance == (UnityEngine.Object)null)
                return false;
            Traverse.Create(__instance).Field("character").SetValue(_character);
            Traverse.Create(__instance).Field("target").SetValue(_target);
            Traverse.Create(__instance).Field("theEvent").SetValue(_theEvent);
            Traverse.Create(__instance).Field("auxInt").SetValue(_auxInt);
            Traverse.Create(__instance).Field("auxString").SetValue(_auxString);
            Traverse.Create(__instance).Field("castedCard").SetValue(_castedCard);
            if (Content.medsCustomTraitsSource.Contains(_trait) && myTraitList.Contains(_trait))
            {
                DoCustomTrait(_trait, ref __instance);
                return false;
            }
            return true;
        }


        [HarmonyPostfix]
        [HarmonyPatch(typeof(AtOManager), "GlobalAuraCurseModificationByTraitsAndItems")]
        public static void GlobalAuraCurseModificationByTraitsAndItemsPostfix(ref AtOManager __instance, ref AuraCurseData __result, string _type, string _acId, Character _characterCaster, Character _characterTarget)
        {
            LogInfo($"GACM {subclassName}");

            Character characterOfInterest = _type == "set" ? _characterTarget : _characterCaster;
            string traitOfInterest;

            switch (_acId)
            {
                // 2a: Scourge on this hero increases Damage and Healing by 10% per charge.
                // 4a: Scourge on enemies reduces damage done by 5% per charge (caps at 50%). 
                // 4b: Insane on this hero increases mind damage and healing by 2% per charge.
                // 4b: Crack on enemies decreases mind resistance by 1% per charge.
                case "scourge":
                    traitOfInterest = trait2a;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                    {
                        __result.AuraDamageType2 = Enums.DamageType.All;
                        __result.AuraDamageIncreasedPercentPerStack2 = 10.0f;
                        __result.HealDonePercentPerStack = 10;

                    }

                    traitOfInterest = trait4a;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                    {
                        __result.AuraDamageType3 = Enums.DamageType.All;
                        __result.AuraDamageIncreasedPercentPerStack3 = -5.0f;
                    }
                    break;

                case "crack":
                    traitOfInterest = trait4b;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.Monsters))
                    {
                        __result = __instance.GlobalAuraCurseModifyResist(__result, Enums.DamageType.Mind, 0, 1.0f);
                    }
                    break;

                case "mind":
                    traitOfInterest = trait4b;
                    if (IfCharacterHas(characterOfInterest, CharacterHas.Trait, traitOfInterest, AppliesTo.ThisHero))
                    {
                        __result.AuraDamageType3 = Enums.DamageType.Mind;
                        __result.AuraDamageIncreasedPercentPerStack3 = 2.0f;
                        __result.HealDonePercentPerStack = 2;

                    }
                    break;
            }
        }


        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), nameof(Character.HealReceivedFinal))]
        public static void HealReceivedFinalPrefix(Character __instance, int __result, int heal, bool isIndirect = false)
        {
            if (infiniteProctection > 100)
                return;
            if (isDamagePreviewActive || isCalculateDamageActive)
                return;
            if (MatchManager.Instance == null)
                return;
            if (!IsLivingHero(__instance) || MatchManager.Instance.GetHeroHeroActive() == null)
                return;

            infiniteProctection++;

            // MatchManager.Instance.cast
            Hero activeHero = MatchManager.Instance.GetHeroHeroActive();
            PLog("Inf " + infiniteProctection);
            PLog("Active Hero: " + activeHero.SubclassName);
            PLog("Targeted/Instanced Hero: " + __instance.SubclassName);
            if (__result >= 0 && activeHero.HaveTrait(trait0) && IsLivingHero(__instance) && IsLivingHero(activeHero) && heal > 0 && !isIndirect)
            {
                int amountOverhealed = __result - __instance.GetHpLeftForMax();
                if (amountOverhealed<=0) {return;}            
                int amountToShield = Mathf.RoundToInt((amountOverhealed + activeHero.AuraCurseModification["shield"])*0.5f);
                __instance.SetAura(__instance, GetAuraCurseData("shield"), amountToShield, useCharacterMods: false);
            }
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetDamagePreview))]
        public static void SetDamagePreviewPrefix()
        {
            isDamagePreviewActive = true;
        }
        [HarmonyPostfix]
        [HarmonyPatch(typeof(MatchManager), nameof(MatchManager.SetDamagePreview))]
        public static void SetDamagePreviewPostfix()
        {
            isDamagePreviewActive = false;
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Character), nameof(Character.BeginTurn))]
        public static void BeginTurnPrefix(ref Character __instance)
        {
 
            infiniteProctection = 0;
        }

    }
}

