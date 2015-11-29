using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSonNetIgnore
{
	class Program
	{
		static void Main(string[] args)
		{
			var reader = new StreamReader("testfile.json");
			reader.ReadLine();
			JsonTextReader rd = new Newtonsoft.Json.JsonTextReader(reader);
			var stream = new StringWriter();
			JsonTextWriter writer = new JsonTextWriter(stream);
			while (rd.Read())
			{
				Console.WriteLine(rd.TokenType + "\t" + rd.Value);
				if (rd.TokenType == JsonToken.PropertyName)
				{
					var value = rd.Value.ToString();
					if (value == "Chunks" || value == "SoundIntervals")
					{
						rd.Skip();
						continue;
					}
				}
				writer.WriteToken(rd,false);
			}
			writer.Close();
			Console.WriteLine(stream.GetStringBuilder().ToString());


		}
	}
}
