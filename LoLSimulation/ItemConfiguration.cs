using System.Collections.Generic;

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
			Items.Sort((Item x, Item y) => x.Gold.CompareTo(y.Gold));
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
			Items = items;
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
				if (object.ReferenceEquals(Items[i], configuration.Items[i]))
					return false;
			}
			return true;
		}
	}
}
