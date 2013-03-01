using System.Collections.Generic;

namespace LoLSimulation
{
	class Program
	{
		static void Main(string[] arguments)
		{
			List<Item> items = new List<Item>();

			items.Add(new Item("Cloth Armor", 300, armour: 15, stackable: true));
			items.Add(new Item("Chain Vest", 720, armour: 40, stackable: true));

			items.Add(new Item("Null-Magic Mantle", 400, magicResistance: 20, stackable: true));
			items.Add(new Item("Negatron Cloak", 720, magicResistance: 40, stackable: true));

			items.Add(new Item("Ruby Crystal", 475, health: 180, stackable: true));
			items.Add(new Item("Giant's Belt", 1000, health: 380, stackable: true));

			items.Add(new Item("Warmog's Armor", 2830, health: 1000));
			items.Add(new Item("Sunfire Cape", 2650 , armour: 45, health: 450));
			items.Add(new Item("Randuin's Omen", 3100, armour: 70, health: 500));
			items.Add(new Item("Spirit Visage", 2200, magicResistance: 50, health: 200));
			items.Add(new Item("Runic Bulwark", 3200, armour: 30, magicResistance: 60, health: 400));
			items.Add(new Item("Locket of the Iron Solari", 2000, armour: 35, health: 400));

			Simulation simulation = new Simulation(items);
			simulation.Run("Balanced", "Balanced.txt", 0.65);
			simulation.Run("Physical only", "Physical.txt", 1.0);
			simulation.Run("Magical only", "Magical.txt", 0.0);
		}
	}
}
