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
        internal uint baseAddr { get; private set; }
        private int id;
        public bool isDead { get; private set; }
        public string name { get; private set; }
        public string className { get; private set; }

        public float x { get; private set; }
        public float y { get; private set; }
        public float z { get; private set; }

        public float hp { get; private set; }
        public float maxhp { get; private set; }
        public float shield { get; private set; }
        public float mana { get; private set; }
        public float maxmana { get; private set; }
        public float baseAd { get; private set; }
        public float bonusAd { get; private set; }
        public float ap { get; private set; }
        public float lifeSteal { get; private set; }
        public float spellVemp { get; private set; }
        public float AttSpeedMultiplier { get; private set; }
        public float critchance { get; private set; }
        public float armor { get; private set; }
        public float MR { get; private set; }
        public float tenacity { get; private set; }
        public float hpRegenPerSec { get; private set; }
        public float manaRegenPerSec { get; private set; }
        public float MovementSpeed { get; private set; }
        public float attackRange { get; private set; }
        public float armorPen { get; private set; }
        public float magicPen { get; private set; }
        public float armorPenPercent { get; private set; }
        public float magicPenPercent { get; private set; }
        public float cdr { get; private set; }
        public Team team { get; private set; }

        public Unit(int id)
        {
            this.id = id;
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

        protected static string GetName(IntPtr process, int unitBaseAddr, byte[] buffer)
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

        public static Unit GetUnit(IntPtr process, int listBaseAddress, int idInList)
        {
            byte[] buffer = new byte[4]; //temporary buffer for reading values
            int unitBaseAddr = Memory.ReadInt(process, listBaseAddress + 4 * idInList, buffer);
            if (unitBaseAddr == 0) //no unit was found there
                return null;
            int objClass = Memory.ReadInt(process, unitBaseAddr + 4, buffer);
            string objClassName = Memory.ReadString(process, objClass + 4, buffer);

            string name = GetName(process, unitBaseAddr, buffer);

            Unit unit;
            switch (objClassName)
            {
                case "obj_AI_Minion":
                    if (name == "SightWard" || name == "VisionWard")
                        unit = new Ward(idInList, (name == "SightWard" ? WardType.Regular : WardType.Pink));
                    else
                        unit = new Minion(idInList);
                    break;
                case "obj_AI_Turret":
                    unit = new Turret(idInList);
                    break;
                case "AIHeroClient":
                    unit = new Champion(idInList);
                    break;
                default:
                    return null;
            }

            unit.baseAddr = (uint)unitBaseAddr;
            unit.name = name;
            unit.className = objClassName;
            unit.x = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.x, buffer);
            unit.y = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.y, buffer);
            unit.z = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.z, buffer);
            unit.isDead = Memory.ReadByte(process, unitBaseAddr + Offsets.Unit.isDead, buffer) == 1;
            unit.hp = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.hp, buffer);
            unit.maxhp = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.maxHp, buffer);
            unit.mana = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.mana, buffer);
            unit.maxmana = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.maxMana, buffer);
            unit.shield = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.shield, buffer);
            unit.bonusAd = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.bonusAd, buffer);
            unit.ap = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.ap, buffer);
            unit.lifeSteal = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.lifeSteal, buffer);
            unit.spellVemp = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.spellVemp, buffer);
            unit.AttSpeedMultiplier = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.AttSpeedMultiplier, buffer);
            unit.baseAd = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.baseAd, buffer);
            unit.critchance = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.critChance, buffer);
            unit.armor = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.armor, buffer);
            unit.MR = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.MR, buffer);
            unit.hpRegenPerSec = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.hpRegenPerSec, buffer);
            unit.manaRegenPerSec = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.ManaRegenPerSec, buffer);
            unit.MovementSpeed = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.MovementSpeed, buffer);
            unit.attackRange = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.attackRange, buffer);
            unit.team = (Team)Memory.ReadInt(process, unitBaseAddr + Offsets.Unit.team, buffer);
            unit.tenacity = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.tenacity, buffer);
            unit.cdr = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.cdr, buffer);
            unit.armorPen = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.armorPen, buffer);
            unit.magicPen = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.magicPen, buffer);
            unit.armorPenPercent = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.armorPenPercent, buffer);
            unit.magicPenPercent = Memory.ReadFloat(process, unitBaseAddr + Offsets.Unit.magicPenPercent, buffer);

            return unit;
        }
    }
}
