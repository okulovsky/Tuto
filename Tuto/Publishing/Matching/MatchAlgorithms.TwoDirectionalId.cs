using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tuto.Publishing.Matching
{
	
	public partial class TwoDirectionalIdMatch<TInternal, TExternal, TInternalKey, TExternalKey>
		where TInternalKey : class
		where TExternalKey : class
		where TInternal : class
		where TExternal : class
		
	{

		//input data
		readonly IEnumerable<TInternal> Internal;
		readonly IEnumerable<TExternal> External;
		readonly Func<TInternal, TInternalKey> intToInt;
		readonly Func<TInternal, TExternalKey> intToExt;
		readonly Func<TExternal, TInternalKey> extToInt;
		readonly Func<TExternal, TExternalKey> extToExt;

	

		//result
		readonly DataContainer<TInternal, TExternal> result;

		public TwoDirectionalIdMatch(
			IEnumerable<TInternal> Internal,
			IEnumerable<TExternal> External,
			Func<TInternal, TInternalKey> intToInt,
			Func<TInternal, TExternalKey> intToExt,
			Func<TExternal, TInternalKey> extToInt,
			Func<TExternal, TExternalKey> extToExt)
		{
			this.Internal=Internal;
			this.External=External;
			this.intToInt=intToInt;
			this.intToExt=intToExt;
			this.extToInt=extToInt;
			this.extToExt=extToExt;
			result = new DataContainer<TInternal, TExternal>(Internal, External);
		}






		public DataContainer<TInternal, TExternal> Run()
		{
			return result;

		}

	}
}
