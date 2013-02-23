namespace LoLSimulation
{
	class Item
	{
		public readonly string Name;
		public readonly int Gold;
		public readonly int Armour;
		public readonly int MagicResistance;
		public readonly int Health;
		public readonly bool Stackable;

		public Item(string name, int gold, int armour = 0, int magicResistance = 0, int health = 0, bool stackable = false)
		{
			Name = name;
			Gold = gold;
			Armour = armour;
			MagicResistance = magicResistance;
			Health = health;
			Stackable = stackable;
		}
	}
}
