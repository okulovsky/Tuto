using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public enum YoutubeMatchType
	{
		AlreadyEstablished,
		AlreadyEstablishedFromYoutubeSide,
		AlreadyEstablishedFromTutoSide,
		AlreadyEstablishedButBroken,
		MatchedConfidently,
		NotFound
	}

	public class YoutubeMatchData
	{
		YoutubeMatchType MatchType { get; set; }
		YoutubeClip Clip { get; set; }
		List<YoutubeClip> Suggestions { get; set; }
	}
}
