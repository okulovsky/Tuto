using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing
{
	public static class Matchers
	{
		public static Matcher<TItem,TData> ByName<TItem,TData>(IEnumerable<TData> allExternal, Func<TData, string> externalName, Func<TData, TData, bool> equals)
			where TItem : IItem
		{
            return new Matcher<TItem, TData>(
                allExternal,
                (item, datas) => NameMatchAlgorithm.MatchNames(item.Caption,externalName(datas)),
                equals);
        }

		public static Matcher<FolderOrLectureItem,YoutubePlaylist> Playlists(
			IEnumerable<YoutubePlaylist> playlists 
			)
		{
			return ByName<FolderOrLectureItem, YoutubePlaylist>(
				playlists,
				z => z.PlaylistTitle,
				(a, b) => a.PlaylistId == b.PlaylistId
				);
		}

		public static Matcher<VideoItem, YoutubeClip> Clips(
				IEnumerable<YoutubeClip> playlists
				)
		{
			return ByName<VideoItem, YoutubeClip>(
				playlists,
				z => z.Name,
				(a, b) => a.Id== b.Id
				);
		}
	}
}
