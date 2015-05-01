using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    internal abstract class Offsets
    {
        //Level Structure
        public abstract class Level
        {
            internal const int baseOffset = 0x10AADD0;
            internal const int offset0 = 0x100;

            internal const int level = 0x58;

            internal const int summonerSpell1OnCd = 0x114;
            internal const int summonerSpell1Cd = 0x11C;

            internal const int summonerSpell2OnCd = 0x130;
            internal const int summonerSpell2Cd = 0x138;

            internal const int spellQLearnt = 0x9C;
            internal const int spellQOnCd = 0xA4;
            internal const int spellQCd = 0xAC;

            internal const int spellWLearnt = 0xB8;
            internal const int spellWOnCd = 0xC0;
            internal const int spellWCd = 0xC8;

            internal const int spellELearnt = 0xD4;
            internal const int spellEOnCd = 0xDC;
            internal const int spellECd = 0xE4;

            internal const int spellRLearnt = 0xF0;
            internal const int spellROnCd = 0xF8;
            internal const int spellRCd = 0x100;
        }
        //List
        public abstract class ObjectList
        {
            internal const int ListBegin = 0x2E17620;

            internal const int OurHero = 0x011918B0;
        }
        //Camera
        public abstract class Camera
        {
            internal const int baseAdress = 0x10a8674;
            internal const int Offset0 = 0x00;
            //x and y are actually the x and y the camera looks at, and not the actual x and y of the camera
            internal const int X = 0x120;
            internal const int Y = 0x128;
            internal const int AngleLook = 0x13C;
            internal const int AngleRotation = 0x140;
            internal const int FovY = 0x14C;
            internal const int Z = 0x244; //zoom
        }
        //Unit structure
        public abstract class Unit
        {
            internal const int unitVisibleStruct = 0x1BC;
            internal const int unitVisibleStruct_unitVisible = 0x84;

            internal const int team = 0x14; //100 blue 200 red 300 jungle
            internal const int name = 0x20;
            internal const int objectOffset = 0x24; //on some units it is written "Object" in some offset, and they have name in a different offset
            internal const int x = 0x5C;
            internal const int z = 0x60;
            internal const int y = 0x64;
            internal const int isDead = 0x114;
            internal const int hp = 0x170;
            internal const int maxHp = 0x180;
            internal const int mana = 0x1EC;
            internal const int maxMana = 0x1FC;
            internal const int shield = 0x250;
            internal const int championName = 0x774;
            internal const int cdr = 0x85c;
            internal const int tenacity = 0x8c0;
            internal const int armorPen = 0x8f0;
            internal const int magicPen = 0x8f4;
            internal const int armorPenPercent = 0x8f8;
            internal const int magicPenPercent = 0x8fc;
            internal const int bonusAd = 0x928;
            internal const int ap = 0x930;
            internal const int lifeSteal = 0x97c;
            internal const int spellVemp = 0x980;
            internal const int AttSpeedMultiplier = 0x98c;
            internal const int baseAd = 0x990;
            internal const int critChance = 0x9a8;
            internal const int armor = 0x9ac;
            internal const int MR = 0x9b0;
            internal const int hpRegenPerSec = 0x9b4;
            internal const int ManaRegenPerSec = 0x9c4;
            internal const int MovementSpeed = 0x9c8;
            internal const int attackRange = 0x9cc;
        }

        public abstract class LineMissile //not updated for the latest patch yet
        {
            internal const int lineWidth = 0x79c; //this is not right
            internal const int originX = 0x16c;
            internal const int originZ = 0x170;
            internal const int originY= 0x174;

            internal const int endX = 0x184;
            internal const int endZ = 0x188;
            internal const int endY = 0x18c;

            internal const int currentX = 0x91; //this is more accurate than x,z,y
            internal const int currentZ = 0x95;
            internal const int currentY = 0x99;
        }

    }
}
