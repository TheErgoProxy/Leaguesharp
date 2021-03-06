﻿#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using LeagueSharp;
using LeagueSharp.Common;
using SharpDX;

#endregion

namespace Darker_Than_Black_Vayne
{
    class Vayne
    {
        public static Spell E;
        public static Spell Q;
        public static Orbwalking.Orbwalker orbwalker;
        public static Menu menu;
        public static Dictionary<string, SpellSlot> spellData;
        public static string[] interrupt;
        public static string[] notarget;
        public static string[] gapcloser;
        public static Obj_AI_Hero tar;
        public const string ChampName = "Vayne";
        public static Obj_AI_Hero Player;
        private static Items.Item _youmuu, _blade, _bilge;

        static void Main(string[] args)
        {
            try
            {
                CustomEvents.Game.OnGameLoad += Game_OnGameLoad;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return;
            }

        }

        public static void Game_OnGameLoad(EventArgs args)
        {
            Player = ObjectManager.Player;
            if (Player.ChampionName != ChampName) return;
            spellData = new Dictionary<string, SpellSlot>();
            menu = new Menu("Darker Than Bl#ck", "Gosu", true);
            //Orbwalker
            menu.AddSubMenu(new Menu("[D#rk] Orbwalker", "Orbwalker"));
            orbwalker = new Orbwalking.Orbwalker(menu.SubMenu("Orbwalker"));
            //TS
            var TargetSelectorMenu = new Menu("[D#rk] Target Selector", "Target Selector");
            TargetSelector.AddToMenu(TargetSelectorMenu);
            menu.AddSubMenu(TargetSelectorMenu);
            menu.AddItem(
                new MenuItem("UseET", "Use E (Toggle)").SetValue(
                    new KeyBind("T".ToCharArray()[0], KeyBindType.Toggle)));
            menu.AddItem(new MenuItem("UseEInterrupt", "Use E To Interrupt").SetValue(true));
            menu.AddItem(
                new MenuItem("PushDistance", "E Push Distance").SetValue(new Slider(425, 475, 300)));
            menu.AddItem(new MenuItem("UseQC", "Use Q").SetValue(true));
            menu.AddItem(new MenuItem("UseEC", "Use E").SetValue(true));
            menu.AddItem(
                new MenuItem("UseEaa", "Use E after auto").SetValue(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle)));
            menu.AddSubMenu(new Menu("[D#rk] Gapcloser List", "gap"));
            menu.AddSubMenu(new Menu("[D#rk] Gapcloser List 2", "gap2"));
            menu.AddSubMenu(new Menu("[D#rk] Interrupt List", "int"));
            Q = new Spell(SpellSlot.Q, 0f);
            E = new Spell(SpellSlot.E, float.MaxValue);

            menu.AddSubMenu(new Menu("[D#rk] Items", "items"));
            menu.SubMenu("items").AddSubMenu(new Menu("Offensive", "Offensive"));
            menu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Youmuu", "Use Youmuu's")).SetValue(true);
            menu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Bilge", "Use Bilge")).SetValue(true);
            menu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BilgeEnemyhp", "If Enemy Hp <").SetValue(new Slider(85, 1, 100)));
            menu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Bilgemyhp", "Or your Hp < ").SetValue(new Slider(85, 1, 100)));
            menu.SubMenu("items").SubMenu("Offensive").AddItem(new MenuItem("Blade", "Use Blade")).SetValue(true);
            menu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("BladeEnemyhp", "If Enemy Hp <").SetValue(new Slider(85, 1, 100)));
            menu.SubMenu("items")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("Blademyhp", "Or Your  Hp <").SetValue(new Slider(85, 1, 100)));
            menu.SubMenu("items").AddSubMenu(new Menu("Potions", "Potions"));
            menu.SubMenu("items")
                    .SubMenu("Potions")
                .AddItem(new MenuItem("usehppotions", "Use Health Potion/Flask/Biscuit"))
                        .SetValue(true);
            menu.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usepotionhp", "If Health % <").SetValue(new Slider(35, 1, 100)));
            menu.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usemppotions", "Use Mana Potion/Flask/Biscuit"))
                .SetValue(true);
            menu.SubMenu("items")
                .SubMenu("Potions")
                .AddItem(new MenuItem("usepotionmp", "If Mana % <").SetValue(new Slider(35, 1, 100)));


            menu.AddSubMenu(new Menu("[D#rk] Misc", "MiscT"));
                        menu.SubMenu("MiscT").AddSubMenu(new Menu("Offensive", "Offensive"));
            menu.SubMenu("MiscT").SubMenu("Offensive").AddItem(new MenuItem("NoAAStealth", "Don't AA while stealthed below % HP.")).SetValue(true);
            menu.SubMenu("MiscT")
                .SubMenu("Offensive")
                .AddItem(new MenuItem("OwnHPercStealth", "Min. own HP % to Stealth.").SetValue(new Slider(30, 1, 100)));
            menu.SubMenu("[D#rk] Combo")
            .AddItem(new MenuItem("ActiveCombo", "Dream Darkness").SetValue(new KeyBind(32, KeyBindType.Press)));

            Game.PrintChat("<font color=\"#000000\">Darker Than Bl#ck</font> <font color=\"#FFFFFF\"> - </font> <font color=\"#8A0707\">The slaughter has begun.</font>");

            gapcloser = new[]
            {
                "AkaliShadowDance", "Headbutt", "DianaTeleport", "IreliaGatotsu", "JaxLeapStrike", "JayceToTheSkies",
                "MaokaiUnstableGrowth", "MonkeyKingNimbus", "Pantheon_LeapBash", "PoppyHeroicCharge", "QuinnE",
                "XenZhaoSweep", "blindmonkqtwo", "FizzPiercingStrike", "RengarLeap"
            };
            notarget = new[]
            {
                "AatroxQ", "GragasE", "GravesMove", "HecarimUlt", "JarvanIVDragonStrike", "JarvanIVCataclysm", "KhazixE",
                "khazixelong", "LeblancSlide", "LeblancSlideM", "LeonaZenithBlade", "UFSlash", "RenektonSliceAndDice",
                "SejuaniArcticAssault", "ShenShadowDash", "RocketJump", "slashCast"
            };
            interrupt = new[]
            {
                "KatarinaR", "GalioIdolOfDurand", "Crowstorm", "Drain", "AbsoluteZero", "ShenStandUnited", "UrgotSwap2",
                "AlZaharNetherGrasp", "FallenOne", "Pantheon_GrandSkyfall_Jump", "VarusQ", "CaitlynAceintheHole",
                "MissFortuneBulletTime", "InfiniteDuress", "LucianR"
            };
            for (int i = 0; i < gapcloser.Length; i++)
            {
                menu.SubMenu("gap").AddItem(new MenuItem(gapcloser[i], gapcloser[i])).SetValue(true);
            }
            for (int i = 0; i < notarget.Length; i++)
            {
                menu.SubMenu("gap2").AddItem(new MenuItem(notarget[i], notarget[i])).SetValue(true);
            }
            for (int i = 0; i < interrupt.Length; i++)
            {
                menu.SubMenu("int").AddItem(new MenuItem(interrupt[i], interrupt[i])).SetValue(true);
            }
            menu.AddSubMenu(new Menu("[D#rk] Harass Options", "harass"));
            menu.SubMenu("harass").AddItem(new MenuItem("hq", "Use Q Harass").SetValue(true));
            menu.SubMenu("harass").AddItem(new MenuItem("he", "Use E Harass").SetValue(true));
            E.SetTargetted(0.25f, 2200f);
            Obj_AI_Base.OnProcessSpellCast += Game_ProcessSpell;
            Game.OnGameUpdate += Game_OnGameUpdate;
            Orbwalking.AfterAttack += Orbwalking_AfterAttack;
            menu.AddToMainMenu();
            
            _youmuu = new Items.Item(3142, 10);
            _bilge = new Items.Item(3144, 450f);
            _blade = new Items.Item(3153, 450f);
        }

        public static void Game_ProcessSpell(Obj_AI_Base hero, GameObjectProcessSpellCastEventArgs args)
        {
            if (menu.Item("UseEInterrupt").GetValue<bool>() && hero.IsValidTarget(550f) &&
                menu.Item(args.SData.Name).GetValue<bool>())
            {
                if (interrupt.Any(str => str.Contains(args.SData.Name)))
                {
                    E.Cast(hero);
                }
            }

            if (gapcloser.Any(str => str.Contains(args.SData.Name)) && args.Target == ObjectManager.Player &&
                hero.IsValidTarget(550f) && menu.Item(args.SData.Name).GetValue<bool>())
            {
                E.Cast(hero);
            }

            if (notarget.Any(str => str.Contains(args.SData.Name)) &&
                Vector3.Distance(args.End, ObjectManager.Player.Position) <= 300 && hero.IsValidTarget(550f) &&
                menu.Item(args.SData.Name).GetValue<bool>())
            {
                E.Cast(hero);
            }
        }

        public static void Orbwalking_AfterAttack(AttackableUnit unit, AttackableUnit target)
        {

            if (unit.IsMe)
            {
                tar = (Obj_AI_Hero)target;

                if (menu.Item("UseEaa").GetValue<KeyBind>().Active)
                {
                    E.Cast((Obj_AI_Base)target);
                    menu.Item("UseEaa").SetValue<KeyBind>(new KeyBind("G".ToCharArray()[0], KeyBindType.Toggle));
                }

                if (((orbwalker.ActiveMode.ToString() == "Combo" && menu.Item("UseQC").GetValue<bool>()) || (orbwalker.ActiveMode.ToString() == "Mixed" && menu.Item("hq").GetValue<bool>()) &&
                    Q.IsReady()))
                {
                    var after = ObjectManager.Player.Position +
                                Normalize(Game.CursorPos - ObjectManager.Player.Position) * 300;
                   
                    var disafter = Vector3.DistanceSquared(after, tar.Position);
                 
                    if ((disafter < 630 * 630) && disafter > 150 * 150)
                    {
                        Q.Cast(Game.CursorPos);
                    }
                    if (Vector3.DistanceSquared(tar.Position, ObjectManager.Player.Position) > 630 * 630 &&
                        disafter < 630 * 630)
                    {
                        Q.Cast(Game.CursorPos);
                    }
                }
            }
        }

        void NoAAStealth()
        {
            var iStealthmyhp = Player.Health <=
                             (Player.MaxHealth * (menu.Item("OwnHPercStealth").GetValue<Slider>().Value) / 100);
            if ((iStealthmyhp) && Player.HasBuff("vaynetumblefade"))
            {
                orbwalker.SetAttack(false);
            }
        }

        public static Vector3 Normalize(Vector3 A)
        {
            double distance = Math.Sqrt(A.X * A.X + A.Y * A.Y);
            return new Vector3(new Vector2((float)(A.X / distance)), (float)(A.Y / distance));
        }

        private static void Usepotion()
        {
            var mobs = MinionManager.GetMinions(Player.ServerPosition, Q.Range,
                MinionTypes.All,
                MinionTeam.Neutral, MinionOrderTypes.MaxHealth);
            var iusehppotion = menu.Item("usehppotions").GetValue<bool>();
            var iusepotionhp = Player.Health <=
                               (Player.MaxHealth * (menu.Item("usepotionhp").GetValue<Slider>().Value) / 100);
            var iusemppotion = menu.Item("usemppotions").GetValue<bool>();
            var iusepotionmp = Player.Mana <=
                               (Player.MaxMana * (menu.Item("usepotionmp").GetValue<Slider>().Value) / 100);

            {
                if (iusepotionhp && iusehppotion &&
                     !(ObjectManager.Player.HasBuff("RegenerationPotion", true) ||
                       ObjectManager.Player.HasBuff("ItemCrystalFlask", true) ||
                       ObjectManager.Player.HasBuff("ItemMiniRegenPotion", true)))
                {
                    if (Items.HasItem(2041) && Items.CanUseItem(2041))
                    {
                        Items.UseItem(2041);
                    }
                    else if (Items.HasItem(2010) && Items.CanUseItem(2010))
                    {
                        Items.UseItem(2010);
                    }
                    else if (Items.HasItem(2003) && Items.CanUseItem(2003))
                    {
                        Items.UseItem(2003);
                    }
                }
            }
        }   
        #region Items
        private static void UseItemes(Obj_AI_Hero tar)
        {
            var iBilge = menu.Item("Bilge").GetValue<bool>();
            var iBilgeEnemyhp = tar.Health <=
                                (tar.MaxHealth * (menu.Item("BilgeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBilgemyhp = Player.Health <=
                             (Player.MaxHealth * (menu.Item("Bilgemyhp").GetValue<Slider>().Value) / 100);
            var iBlade = menu.Item("Blade").GetValue<bool>();
            var iBladeEnemyhp = tar.Health <=
                                (tar.MaxHealth * (menu.Item("BladeEnemyhp").GetValue<Slider>().Value) / 100);
            var iBlademyhp = Player.Health <=
                             (Player.MaxHealth * (menu.Item("Blademyhp").GetValue<Slider>().Value) / 100);
            var iYoumuu = menu.Item("Youmuu").GetValue<bool>();

            if ((iBilgeEnemyhp || iBilgemyhp) && _bilge.IsReady())
            {
                _bilge.Cast(tar);

            }
            if ((iBladeEnemyhp || iBlademyhp) && _blade.IsReady())
            {
                _blade.Cast(tar);

            }
            if  (_youmuu.IsReady())
            {
                _youmuu.Cast();
            }
        }


        float getPerValue(bool mana)
        {
            if (mana) return (Player.Mana / Player.MaxMana) * 100;
            return (Player.Health / Player.MaxHealth) * 100;
        }

        float getPerValueTarget(Obj_AI_Hero target, bool mana)
        {
            if (mana) return (target.Mana / target.MaxMana) * 100;
            return (target.Health / target.MaxHealth) * 100;
        }

        public static void UseItem(int id, Obj_AI_Hero target = null)
        {
            if (Items.HasItem(id) && Items.CanUseItem(id))
            {
                Items.UseItem(id, target);
            }
        }
        #endregion
        public static void Game_OnGameUpdate(EventArgs args)
        {
            if (menu.Item("ActiveCombo").GetValue<KeyBind>().Active)
            {
                Combo();
            }
            if (!E.IsReady()) return; //||
            if (((orbwalker.ActiveMode.ToString() == "Combo" && menu.Item("UseEC").GetValue<bool>()) || (orbwalker.ActiveMode.ToString() == "Mixed" && menu.Item("he").GetValue<bool>()) || menu.Item("UseET").GetValue<KeyBind>().Active))
                foreach (var hero in from hero in ObjectManager.Get<Obj_AI_Hero>().Where(hero => hero.IsValidTarget(550f))
                                     let prediction = E.GetPrediction(hero)
                                     where NavMesh.GetCollisionFlags(
                                         prediction.UnitPosition.To2D()
                                             .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                                 -menu.Item("PushDistance").GetValue<Slider>().Value)
                                             .To3D())
                                         .HasFlag(CollisionFlags.Wall) || NavMesh.GetCollisionFlags(
                                             prediction.UnitPosition.To2D()
                                                 .Extend(ObjectManager.Player.ServerPosition.To2D(),
                                                     -(menu.Item("PushDistance").GetValue<Slider>().Value / 2))
                                                 .To3D())
                                             .HasFlag(CollisionFlags.Wall)
                                     select hero)
                {
                    E.Cast(hero);
                }
        }
        public static bool isMenuEnabled(String val)
        {
            return menu.Item(val).GetValue<bool>();
        }
        private static void Combo()
        {
            UseItemes(tar);
        }
    }
}