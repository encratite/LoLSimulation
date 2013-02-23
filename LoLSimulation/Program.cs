using System.Collections.Generic;

namespace LoLSimulation
{
	class Program
	{
		static void Main(string[] arguments)
		{
			List<Item> items = new List<Item>();
			Simulation simulation = new Simulation(items);
		}
	}
}
