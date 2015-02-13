using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ObjectReader
{
    public enum Team : int
    {
        Team1 = 100,
        Team2 = 200,
        Neutral = 300
    }
    public class Unit
    {
        internal readonly uint baseAddr;
        private readonly int id;
        public readonly bool isDead;
        public readonly string name;
        public readonly string className;

        public readonly float x;
        public readonly float y;
        public readonly float z ;

        public readonly float hp;
        public readonly float maxhp;
        public readonly float shield;
        public readonly float mana;
        public readonly float maxmana;
        public readonly float baseAd;
        public readonly float bonusAd;
        public readonly float ap;
        public readonly float lifeSteal;
        public readonly float spellVemp;
        public readonly float AttSpeedMultiplier;
        public readonly float critchance;
        public readonly float armor;
        public readonly float MR;
        public readonly float tenacity;
        public readonly float hpRegenPerSec;
        public readonly float manaRegenPerSec;
        public readonly float MovementSpeed;
        public readonly float attackRange;
        public readonly float armorPen;
        public readonly float magicPen;
        public readonly float armorPenPercent;
        public readonly float magicPenPercent;
        public readonly float cdr;
        public readonly Team team;
        public readonly string championName;

        internal Unit(int id, int baseAddr)
        {
            this.id = id;
            this.baseAddr = (uint)baseAddr;
            byte[] buffer = new byte[4];
            IntPtr process = Engine.processHandle;

            this.name = GetName(process, baseAddr, buffer);
            this.className = GetClassName(process, baseAddr, buffer);
            this.x = Memory.ReadFloat(process, baseAddr + Offsets.Unit.x, buffer);
            this.y = Memory.ReadFloat(process, baseAddr + Offsets.Unit.y, buffer);
            this.z = Memory.ReadFloat(process, baseAddr + Offsets.Unit.z, buffer);

            this.championName = Memory.ReadString(process, baseAddr + Offsets.Unit.championName, buffer);
            this.isDead = Memory.ReadByte(process, baseAddr + Offsets.Unit.isDead, buffer) == 1;
            this.hp = Memory.ReadFloat(process, baseAddr + Offsets.Unit.hp, buffer);
            this.maxhp = Memory.ReadFloat(process, baseAddr + Offsets.Unit.maxHp, buffer);
            this.mana = Memory.ReadFloat(process, baseAddr + Offsets.Unit.mana, buffer);
            this.maxmana = Memory.ReadFloat(process, baseAddr + Offsets.Unit.maxMana, buffer);
            this.shield = Memory.ReadFloat(process, baseAddr + Offsets.Unit.shield, buffer);
            this.bonusAd = Memory.ReadFloat(process, baseAddr + Offsets.Unit.bonusAd, buffer);
            this.ap = Memory.ReadFloat(process, baseAddr + Offsets.Unit.ap, buffer);
            this.lifeSteal = Memory.ReadFloat(process, baseAddr + Offsets.Unit.lifeSteal, buffer);
            this.spellVemp = Memory.ReadFloat(process, baseAddr + Offsets.Unit.spellVemp, buffer);
            this.AttSpeedMultiplier = Memory.ReadFloat(process, baseAddr + Offsets.Unit.AttSpeedMultiplier, buffer);
            this.baseAd = Memory.ReadFloat(process, baseAddr + Offsets.Unit.baseAd, buffer);
            this.critchance = Memory.ReadFloat(process, baseAddr + Offsets.Unit.critChance, buffer);
            this.armor = Memory.ReadFloat(process, baseAddr + Offsets.Unit.armor, buffer);
            this.MR = Memory.ReadFloat(process, baseAddr + Offsets.Unit.MR, buffer);
            this.hpRegenPerSec = Memory.ReadFloat(process, baseAddr + Offsets.Unit.hpRegenPerSec, buffer);
            this.manaRegenPerSec = Memory.ReadFloat(process, baseAddr + Offsets.Unit.ManaRegenPerSec, buffer);
            this.MovementSpeed = Memory.ReadFloat(process, baseAddr + Offsets.Unit.MovementSpeed, buffer);
            this.attackRange = Memory.ReadFloat(process, baseAddr + Offsets.Unit.attackRange, buffer);
            this.team = (Team)Memory.ReadInt(process, baseAddr + Offsets.Unit.team, buffer);
            this.tenacity = Memory.ReadFloat(process, baseAddr + Offsets.Unit.tenacity, buffer);
            this.cdr = Memory.ReadFloat(process, baseAddr + Offsets.Unit.cdr, buffer);
            this.armorPen = Memory.ReadFloat(process, baseAddr + Offsets.Unit.armorPen, buffer);
            this.magicPen = Memory.ReadFloat(process, baseAddr + Offsets.Unit.magicPen, buffer);
            this.armorPenPercent = Memory.ReadFloat(process, baseAddr + Offsets.Unit.armorPenPercent, buffer);
            this.magicPenPercent = Memory.ReadFloat(process, baseAddr + Offsets.Unit.magicPenPercent, buffer);

        }
        public int GetId()
        {
            return id;
        }

        public override bool Equals(object obj)
        {
            Unit unit = obj as Unit;
            if (unit == null)
                return false;
            if (unit.id == this.id && unit.name == this.name) //if same id in the list, and same name, this is the same object (probably)
                return true;
            return false;
        }

        public override int GetHashCode()
        {
            return id;
        }

        private static string GetName(IntPtr process, int unitBaseAddr, byte[] buffer)
        {
            string Object = Memory.ReadString(process, unitBaseAddr + Offsets.Unit.objectOffset, buffer, 6);
            if (Object == "Object")
            {
                int nameBaseAddr = Memory.ReadInt(process, unitBaseAddr + Offsets.Unit.name, buffer);
                return Memory.ReadString(process, nameBaseAddr, buffer);
            }
            else
                return Memory.ReadString(process, unitBaseAddr + Offsets.Unit.name, buffer);
        }

        private static string GetClassName(IntPtr process, int unitBaseAddr, byte[] buffer)
        {
            int objClass = Memory.ReadInt(process, unitBaseAddr + 4, buffer);
            return Memory.ReadString(process, objClass + 4, buffer);
        }

        public static Unit GetUnit(IntPtr process, int listBaseAddress, int idInList)
        {
            byte[] buffer = new byte[4]; //temporary buffer for reading values
            int unitBaseAddr = Memory.ReadInt(process, listBaseAddress + 4 * idInList, buffer);
            if (unitBaseAddr == 0) //no unit was found there
                return null;
            string objClassName = GetClassName(process, unitBaseAddr, buffer);

            string name = GetName(process, unitBaseAddr, buffer);

            Unit unit;
            switch (objClassName)
            {
                case "obj_AI_Minion":
                    if (name == "SightWard" || name == "VisionWard")
                        unit = new Ward(idInList, unitBaseAddr, (name == "SightWard" ? WardType.Regular : WardType.Pink));
                    else
                        unit = new Minion(idInList, unitBaseAddr);
                    break;
                case "obj_AI_Turret":
                    unit = new Turret(idInList, unitBaseAddr);
                    break;
                case "AIHeroClient":
                    unit = new Champion(idInList, unitBaseAddr);
                    break;
                default:
                    return null;
            }
            return unit;
        }
    }
}
