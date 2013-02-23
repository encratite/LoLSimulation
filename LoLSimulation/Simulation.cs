using System;
using System.Collections.Generic;
using System.IO;

namespace LoLSimulation
{
	class Simulation
	{
		List<Item> Items;
		Item Machete;
		Item Boots;
		Item NinjaTabi;
		Item MercurysTreads;

		public Simulation(List<Item> items)
		{
			Items = items;
			Machete = new Item("Machete", 300);
			Boots = new Item("Boots", 300);
			NinjaTabi = new Item("Ninja Tabi", 1000, armour: 25);
			MercurysTreads = new Item("Mercury's Treads", 1200, magicResistance: 25);
		}

		public void Run(string description, string logFile, double physicalDamageRatio)
		{
			using (FileStream stream = new FileStream(logFile, FileMode.Create))
			{
				StreamWriter writer = new StreamWriter(stream);
				writer.Write("Profile: {0}, physical damage ratio {1}\n\n", description, physicalDamageRatio);
				int[] levels = new int[] { 9, 12, 15, 18 };
				int[] goldLimits = new int[] { 5000, 6500, 9500, 12500 };
				for (int i = 0; i < levels.Length && i < goldLimits.Length; i++)
				{
					int level = levels[i];
					int goldLimit = goldLimits[i];
					EvaluateSetting(writer, level, goldLimit, physicalDamageRatio);
				}
			}
		}

		void EvaluateSetting(StreamWriter writer, int level, int goldLimit, double physicalDamageRatio)
		{
			List<Item> initialItems = new List<Item>();
			if(level <= 12)
				initialItems.Add(Machete);
			if (level < 12)
				initialItems.Add(Boots);
			else
			{
				if (physicalDamageRatio >= 0.8)
					initialItems.Add(NinjaTabi);
				else
					initialItems.Add(MercurysTreads);
			}
			List<ItemConfiguration> configurations = new List<ItemConfiguration>();
			DetermineItemConfigurations(level, goldLimit, physicalDamageRatio, initialItems, configurations);
			configurations.Sort((ItemConfiguration x, ItemConfiguration y) => y.GetScore(physicalDamageRatio).CompareTo(x.GetScore(physicalDamageRatio)));
			writer.Write("Level {0}, gold limit {1}, {2} item configurations\n\n", level, goldLimit, configurations.Count);
			foreach (var configuration in configurations)
				configuration.Serialise(writer, physicalDamageRatio);
			writer.Write("\n");
		}

		int GetGoldAvailable(int level, int goldLimit, List<Item> currentConfiguration)
		{
			int goldAvailable = goldLimit;
			// Health pots
			goldAvailable -= 8 * 35;
			// Sold Machete
			if (level >= 15)
				goldAvailable -= Machete.Gold;
			// Wards
			int wardCount;
			if (level == 18)
				wardCount = 4;
			else if (level >= 15)
				wardCount = 3;
			else if (level >= 12)
				wardCount = 2;
			else if (level >= 9)
				wardCount = 1;
			else
				wardCount = 0;
			goldAvailable -= wardCount * 75;
			foreach (var item in currentConfiguration)
				goldAvailable -= item.Gold;
			return goldAvailable;
		}

		void DetermineItemConfigurations(int level, int goldLimit, double physicalDamageRatio, List<Item> currentConfiguration, List<ItemConfiguration> configurations)
		{
			const int inventoryLimit = 6;
			int slotsUsedByPotsAndWards;
			if (level == 18)
				slotsUsedByPotsAndWards = 0;
			else if (level >= 12)
				slotsUsedByPotsAndWards = 1;
			else
				slotsUsedByPotsAndWards = 2;
			bool foundValidConfiguration = false;
			if (currentConfiguration.Count + slotsUsedByPotsAndWards < inventoryLimit)
			{
				int goldAvailable = GetGoldAvailable(level, goldLimit, currentConfiguration);
				foreach (var item in Items)
				{
					if (
						(!item.Stackable && currentConfiguration.Contains(item)) ||
						item.Gold > goldAvailable
						)
						continue;
					List<Item> newConfiguration = new List<Item>(currentConfiguration);
					newConfiguration.Add(item);
					foundValidConfiguration = true;
					DetermineItemConfigurations(level, goldLimit, physicalDamageRatio, newConfiguration, configurations);
				}
			}
			if (!foundValidConfiguration)
			{
				bool isNew = true;
				ItemConfiguration newConfiguration = GetItemConfiguration(level, currentConfiguration);
				foreach (var configuration in configurations)
				{
					if (configuration.HasSameItems(newConfiguration))
					{
						isNew = false;
						break;
					}
				}
				if(isNew)
					configurations.Add(newConfiguration);
			}
		}

		ItemConfiguration GetItemConfiguration(int level, List<Item> currentConfiguration)
		{
			ItemConfiguration configuration = new ItemConfiguration(currentConfiguration);

			int baseHealth = 445 + 87 * level;
			double baseArmour = 16.2 + 3.7 * level;
			double baseMagicResistance = 30 + 1.25 * level;

			int runeHealth = 0;
			double runeArmour = 9 * 1.41;
			double runeMagicResistance = 9 * 0.15 * level;

			int masteryHealth = 6 * level + 30;
			int masteryArmour = 2 + 2 + 1;
			int masteryMagicResistance = 2 + 2 + 1;

			int health = baseHealth + runeHealth + masteryHealth + configuration.Health;
			double armour = baseArmour + runeArmour + masteryArmour + configuration.Armour;
			double magicResistance = baseMagicResistance + runeMagicResistance + masteryMagicResistance + configuration.MagicResistance;

			double runeFlatArmourPenetration = 9 * 1.28;
			double runeFlatMagicPenetration = 9 * 0.87;

			int masteryFlatArmourPenetration = 2 + 2 + 1;
			double masteryPercentageArmourPenetration = 0.08;
			int masteryFlatMagicPenetration = 0;
			double masteryPercentageMagicPenetration = 0.08;

			double lastWhisperPercentageArmourPenetration = 0.35;
			double voidStaffPercentageMagicPenetration = 0.35;

			double flatArmourPenetration = runeFlatArmourPenetration + masteryFlatArmourPenetration;
			List<double> armourPercentagePenetrations = new List<double>();
			armourPercentagePenetrations.Add(masteryPercentageArmourPenetration);
			if (level >= 15)
				armourPercentagePenetrations.Add(lastWhisperPercentageArmourPenetration);

			double flatMagicPenetration = runeFlatMagicPenetration + masteryFlatMagicPenetration;
			List<double> magicPercentagePenetration = new List<double>();
			magicPercentagePenetration.Add(masteryPercentageMagicPenetration);
			if (level >= 15)
				magicPercentagePenetration.Add(voidStaffPercentageMagicPenetration);

			configuration.EffectiveHealthAgainstPhysicalDamage = GetEffectiveHealth(health, armour, flatArmourPenetration, armourPercentagePenetrations);
			configuration.EffectiveHealthAgainstMagicDamage = GetEffectiveHealth(health, magicResistance, flatMagicPenetration, magicPercentagePenetration);

			return configuration;
		}

		double GetEffectiveResistance(double resistance, double flatPenetration, List<double> percentagePenetrations)
		{
			double effectiveResistance = resistance;
			foreach (double percentagePenetration in percentagePenetrations)
				effectiveResistance *= 1.0 - percentagePenetration;
			effectiveResistance = Math.Max(effectiveResistance - flatPenetration, 0.0);
			return effectiveResistance;
		}

		double GetDamageRatio(double resistance)
		{
			return 100.0 / (100.0 + resistance);
		}

		int GetEffectiveHealth(int health, double resistance, double flatPenetration, List<double> percentagePenetrations)
		{
			double effectiveResistance = GetEffectiveResistance(resistance, flatPenetration, percentagePenetrations);
			double effectiveHealth = health / GetDamageRatio(effectiveResistance);
			return (int)effectiveHealth;
		}
	}
}
