using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tuto.Publishing;

namespace Tuto.Tests
{
	[TestClass]
	public class YoutubeClipTests
	{
		public void TestPrefix(string prefix, string result)
		{
			var clip = new YoutubeClip { Name = prefix + result };
			Assert.AreEqual(result, clip.GetProperName());
		}

		[TestMethod]
		public void RightPrefix()
		{
			TestPrefix("BP-2-3-1 ", "Основы программирования");
		}

		[TestMethod]
		public void WrongPrefix()
		{
			TestPrefix("", "340 Test");
			TestPrefix("", "Levenstein Distance");
		}

		public void TestGuidAddition(string desc, Guid guid)
		{
			var clip = new YoutubeClip { Description = desc };
			clip.UpdateGuid(guid);
			string expected = desc + "\n" + "[GUID: " + guid + "]";
			Assert.AreEqual(expected, clip.Description);
		}

		Guid guid1 = Guid.Parse("93D208B4-3A07-49E4-92CE-6BD1CEEAA972");
		Guid guid2 = Guid.Parse("2CC61B8F-C31E-49AC-9AEE-7F8B098D7852");


		[TestMethod]
		public void UpdateWithoutGuid()
		{
			TestGuidAddition("Lalalalal\nlalala\nLala", guid1);
		}

		public void TestGuidReplacement(string before, string after, Guid guid1, Guid guid2)
		{
			var clip = new YoutubeClip { Description = before + "\n" + YoutubeClip.GuidMarker(guid1) + "\n" + after };
			clip.UpdateGuid(guid2);
			var expected = before + "\n" + YoutubeClip.GuidMarker(guid2) + "\n" + after;
			Assert.AreEqual(expected,clip.Description);
		}

		[TestMethod]
		public void UpdateWithGuid()
		{
			TestGuidReplacement("lalala", "tututu", guid1, guid2);
		}

		public void TestGuidDeletion(string before, string after, Guid guid)
		{
			var clip = new YoutubeClip { Description = before + "\n" + YoutubeClip.GuidMarker(guid) + "\n" + after };
			clip.UpdateGuid(null);
			Assert.AreEqual(before + "\n\n" + after, clip.Description);
		}

		[TestMethod]
		public void DeleteGuid()
		{
			TestGuidDeletion("lalala", "tututu", guid1);
		}

		

	}
}
