using System.Collections.Generic;
using System.IO;

namespace LoLSimulation
{
	class ItemConfiguration
	{
		public readonly List<Item> Items;
		public readonly int Gold;
		public readonly int Armour;
		public readonly int MagicResistance;
		public readonly int Health;

		public int EffectiveHealthAgainstPhysicalDamage;
		public int EffectiveHealthAgainstMagicDamage;

		public ItemConfiguration(List<Item> items)
		{
			Items = new List<Item>(items);
			Items.Sort((Item x, Item y) => x.Name.CompareTo(y.Name));
			int gold = 0;
			int armour = 0;
			int magicResistance = 0;
			int health = 0;
			foreach (var item in items)
			{
				gold += item.Gold;
				armour += item.Armour;
				magicResistance += item.MagicResistance;
				health += item.Health;
			}
			Gold = gold;
			Armour = armour;
			MagicResistance = magicResistance;
			Health = health;
		}

		public bool HasSameItems(ItemConfiguration configuration)
		{
			if (Items.Count != configuration.Items.Count)
				return false;
			for (int i = 0; i < Items.Count; i++)
			{
				if (!object.ReferenceEquals(Items[i], configuration.Items[i]))
					return false;
			}
			return true;
		}

		public double GetScore(double physicalDamageRatio)
		{
			return EffectiveHealthAgainstPhysicalDamage * physicalDamageRatio + EffectiveHealthAgainstMagicDamage * (1.0 - physicalDamageRatio);
		}

		public void Serialise(StreamWriter writer, double physicalDamageRatio)
		{
			bool first = true;
			foreach (var item in Items)
			{
				if (first)
					first = false;
				else
					writer.Write(", ");
				writer.Write(item.Name);
			}
			writer.Write("\n");
			writer.Write("{0} g, {1} armour, {2} MR, {3} health, {4} EH vs. physical, {5} EH vs. magic, {6} score\n", Gold, Armour, MagicResistance, Health, EffectiveHealthAgainstPhysicalDamage, EffectiveHealthAgainstMagicDamage, GetScore(physicalDamageRatio));
		}
	}
}
