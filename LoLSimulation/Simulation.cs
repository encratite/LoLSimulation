using System;
using System.Collections.Generic;

namespace LoLSimulation
{
	class Simulation
	{
		List<Item> Items;

		public Simulation(List<Item> items)
		{
			Items = items;
		}

		public void Run()
		{
			int[] levels = new int[] { 9, 12, 15, 18 };
			int[] goldLimits = new int[] { 5000, 6500, 9500, 12500 };
			for (int i = 0; i < levels.Length && i < goldLimits.Length; i++)
			{
				int level = levels[i];
				int goldLimit = goldLimits[i];
				EvaluateSetting(level, goldLimit);
			}
		}

		void EvaluateSetting(int level, int goldLimit)
		{
			List<Item> initialConfiguration = new List<Item>();
			List<ItemConfiguration> configurations = new List<ItemConfiguration>();
			DetermineItemConfigurations(level, goldLimit, initialConfiguration, configurations);
		}

		void DetermineItemConfigurations(int level, int goldLimit, List<Item> currentConfiguration, List<ItemConfiguration> configurations)
		{
			const int itemCountLimit = 6;
			bool foundValidConfiguration = false;
			if (currentConfiguration.Count < itemCountLimit)
			{
				int goldAvailable = goldLimit;
				foreach(var item in currentConfiguration)
					goldAvailable -= item.Gold;
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
					DetermineItemConfigurations(level, goldLimit, newConfiguration, configurations);
				}
			}
			if (!foundValidConfiguration)
			{
				ItemConfiguration configuration = GetItemConfiguration(level, currentConfiguration);
				configurations.Add(configuration);
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
