using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    internal abstract class Offsets
    {
        //all of the offsets are outdated.
        //Level Structure
        public abstract class Level
        {
            internal const int baseOffset = 0x15012E0;
            internal const int offset0 = 0x100;

            internal const int level = 0x58;
            internal const int summonerSpell1OnCd = 0x114;
            internal const int summonerSpell2OnCd = 0x130;
            internal const int spellQOnCd = 0xA4;
            internal const int spellWOnCd = 0xC0;
            internal const int spellEOnCd = 0xDC;
            internal const int spellROnCd = 0xF8;
        }
        //List
        public abstract class ObjectList
        {
            internal const int ListBegin = 0x3000BC8;

            //Our Hero
            internal const int OurHero = 0x1500D88;
        }
        //Camera
        public abstract class Camera
        {
            internal const int baseAdress = 0x01501294;
            internal const int Offset0 = 0x00;
            internal const int X = 0x104;
            internal const int Y = 0x10c;
            internal const int AngleLook = 0x120;
            internal const int AngleRotation = 0x124;
            internal const int FovY = 0x130;
            internal const int Z = 0x1B8;
        }
        //Unit structure
        public abstract class Unit
        {
            internal const int unitVisibleStruct = 0x17C;
            internal const int unitVisibleStruct_unitVisible = 0x24;

            internal const int team = 0x18; //100 blue 200 red 300 jungle
            internal const int x = 0x60;
            internal const int z = 0x64;
            internal const int y = 0x68;
            internal const int isDead = 0x118;
            internal const int hp = 0x140;
            internal const int maxHp = 0x150;
            internal const int mana = 0x1AC;
            internal const int maxMana = 0x1BC;
            internal const int shield = 0x210;
            internal const int cdr = 0x624;
            internal const int tenacity = 0x674;
            internal const int armorPen = 0x6A0;
            internal const int magicPen = 0x69c;
            internal const int armorPenPercent = 0x6A8;
            internal const int magicPenPercent = 0x6AC;
            internal const int bonusAd = 0x6D4;
            internal const int ap = 0x6DC;
            internal const int lifeSteal = 0x728;
            internal const int spellVemp = 0x72C;
            internal const int AttSpeedMultiplier = 0x738;
            internal const int baseAd = 0x73C;
            internal const int critChance = 0x754;
            internal const int armor = 0x758;
            internal const int MR = 0x75C;
            internal const int hpRegenPerSec = 0x760;
            internal const int ManaRegenPerSec = 0x770;
            internal const int MovementSpeed = 0x774;
            internal const int attackRange = 0x778;
        }

    }
}
