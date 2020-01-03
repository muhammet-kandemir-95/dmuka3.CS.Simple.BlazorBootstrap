using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dmuka3.CS.Simple.BlazorBootstrap.TestServerApp.Models
{
	public class TestDataModel
	{
		public int id { get; set; }
		public string name { get; set; }
		public string surname { get; set; }

		public static List<TestDataModel> Rows = new List<TestDataModel>();

		static TestDataModel()
		{
			for (int i = 0; i < 41; i++)
			{
				Rows.Add(new TestDataModel()
				{
					id = i / 4,
					name = "User Name " + (i / 4) + "_" + i,
					surname = "User Surname " + (i / 4) + "_" + i
				});
			}
		}
	}
}
