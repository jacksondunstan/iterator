/// <summary>
/// Iterator library inspired by the C++ Standard Library's <iterator> and <algorithm>
/// </summary>
/// <author>Jackson Dunstan, http://jacksondunstan.com/articles/3471</code>
/// <license>MIT</license>

using System;
using System.Collections.Generic;

public struct ArrayIterator<T>
{
	public T[] Array;
	public int Index;
}

public static class ArrayIteratorExtensions
{
	public static ArrayIterator<T> Begin<T>(this T[] array)
	{
		return new ArrayIterator<T> { Array = array };
	}

	public static ArrayIterator<T> End<T>(this T[] array)
	{
		return new ArrayIterator<T> { Array = array, Index = array.Length };
	}

	public static ArrayIterator<T> IteratorAt<T>(this T[] array, int index)
	{
		return new ArrayIterator<T> { Array = array, Index = index };
	}

	public static T GetCurrent<T>(this ArrayIterator<T> it)
	{
		return it.Array[it.Index];
	}

	public static ArrayIterator<T> GetNext<T>(this ArrayIterator<T> it)
	{
		it.Index++;
		return it;
	}

	public static ArrayIterator<T> GetPrev<T>(this ArrayIterator<T> it)
	{
		it.Index--;
		return it;
	}

	public static bool IsEqual<T>(this ArrayIterator<T> it, ArrayIterator<T> other)
	{
		return it.Array == other.Array && it.Index == other.Index;
	}

	public static bool NotEqual<T>(this ArrayIterator<T> it, ArrayIterator<T> other)
	{
		return it.Array != other.Array || it.Index != other.Index;
	}

	public static ArrayIterator<T> GetAdvanced<T>(this ArrayIterator<T> it, int distance)
	{
		return new ArrayIterator<T> { Array = it.Array, Index = it.Index + distance };
	}

	public static int Distance<T>(this ArrayIterator<T> first, ArrayIterator<T> last)
	{
		return last.Index - first.Index;
	}

	public static bool AllOf<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()) == false)
			{
				return false;
			}
			first = first.GetNext();
		}
		return true;
	}

	public static bool AnyOf<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				return true;
			}
			first = first.GetNext();
		}
		return false;
	}

	public static bool NoneOf<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				return false;
			}
			first = first.GetNext();
		}
		return true;
	}

	public static void ForEach<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Action<T> callback
	)
	{
		while (first.NotEqual(last))
		{
			callback(first.GetCurrent());
			first = first.GetNext();
		}
	}

	public static ArrayIterator<T> Find<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		T val,
		Func<T, T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent(), val))
			{
				return first;
			}
			first = first.GetNext();
		}
		return last;
	}

	public static ArrayIterator<T> FindIf<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				return first;
			}
			first = first.GetNext();
		}
		return last;
	}

	public static ArrayIterator<T> FindIfNot<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T,bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()) == false)
			{
				return first;
			}
			first = first.GetNext();
		}
		return last;
	}

	public static ArrayIterator<T> FindEnd<T>(
		this ArrayIterator<T> first1,
		ArrayIterator<T> last1,
		ArrayIterator<T> first2,
		ArrayIterator<T> last2,
		Func<T, T, bool> pred
	)
	{
		if (first2.IsEqual(last2))
		{
			return last1;
		}
		var ret = last1;
		while (first1.NotEqual(last1))
		{
			var it1 = first1;
			var it2 = first2;
			while (pred(it1.GetCurrent(), it2.GetCurrent()))
			{
				it1 = it1.GetNext();
				it2 = it2.GetNext();
				if (it2.IsEqual(last2))
				{
					ret = first1;
					break;
				}
				if (it1.IsEqual(last1))
				{
					return ret;
				}
			}
			first1 = first1.GetNext();
		}
		return ret;
	}

	public static ArrayIterator<T> FindFirstOf<T>(
		this ArrayIterator<T> first1,
		ArrayIterator<T> last1,
		ArrayIterator<T> first2,
		ArrayIterator<T> last2,
		Func<T, T, bool> pred
	)
	{
		while (first1.NotEqual(last1))
		{
			for (var it = first2; it.NotEqual(last2); it = it.GetNext())
			{
				if (pred(it.GetCurrent(), first1.GetCurrent()))
				{
					return first1;
				}
			}
			first1 = first1.GetNext();
		}
		return last1;
	}

	public static ArrayIterator<T> AdjacentFind<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T, T, bool> pred
	)
	{
		if (first.NotEqual(last))
		{
			var next=first;
			next = next.GetNext();
			while (next.NotEqual(last))
			{
				if (pred(first.GetCurrent(), next.GetCurrent()))
				{
					return first;
				}
				first = first.GetNext();
				next = next.GetNext();
			}
		}
		return last;
	}

	public static int Count<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		T val,
		Func<T, T, bool> pred
	)
	{
		var count = 0;
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent(), val))
			{
				count++;
			}
			first = first.GetNext();
		}
		return count;
	}

	public static int CountIf<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		Func<T, bool> pred
	)
	{
		var count = 0;
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				count++;
			}
			first = first.GetNext();
		}
		return count;
	}

	public static void Mismatch<T>(
		this ArrayIterator<T> first1,
		ArrayIterator<T> last1,
		ArrayIterator<T> first2,
		Func<T, T, bool> pred,
		out ArrayIterator<T> mismatch1,
		out ArrayIterator<T> mismatch2
	)
	{
		while (first1.NotEqual(last1))
		{
			if (pred(first1.GetCurrent(), first2.GetCurrent()) == false)
			{
				break;
			}
			first1 = first1.GetNext();
			first2 = first2.GetNext();
		}
		mismatch1 = first1;
		mismatch2 = first2;
	}

	public static bool Equal<T>(
		this ArrayIterator<T> first1,
		ArrayIterator<T> last1,
		ArrayIterator<T> first2,
		Func<T, T, bool> pred
	)
	{
		while (first1.NotEqual(last1))
		{
			if (pred(first1.GetCurrent(), first2.GetCurrent()) == false)
			{
				return false;
			}
			first1 = first1.GetNext();
			first2 = first2.GetNext();
		}
		return true;
	}

	public static bool IsPermutation<T>(
		this ArrayIterator<T> first1,
		ArrayIterator<T> last1,
		ArrayIterator<T> first2,
		Func<T, T, bool> pred
	)
	{
		first1.Mismatch(last1, first2, pred, out first1, out first2);
		if (first1.IsEqual(last1))
		{
			return true;
		}
		var last2 = first2;
		last2 = last2.GetAdvanced(first1.Distance(last1));
		for (var it1 = first1; it1.NotEqual(last1); it1 = it1.GetNext())
		{
			if (first1.Find(it1, it1.GetCurrent(), pred).IsEqual(it1))
			{
				var n = first2.Count(last2, it1.GetCurrent(), pred);
				if (n == 0 || it1.Count(last1, it1.GetCurrent(), pred) != n)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static ArrayIterator<T> Search<T>(
		this ArrayIterator<T> first1,
		ArrayIterator<T> last1,
		ArrayIterator<T> first2,
		ArrayIterator<T> last2,
		Func<T, T, bool> pred
	)
	{
		if (first2.IsEqual(last2))
		{
			return first1;
		}

		while (first1.NotEqual(last1))
		{
			var it1 = first1;
			var it2 = first2;
			while (pred(it1.GetCurrent(), it2.GetCurrent()))
			{
				it1 = it1.GetNext();
				it2 = it2.GetNext();
				if (it2.IsEqual(last2))
				{
					return first1;
				}
				if (it1.IsEqual(last1))
				{
					return last1;
				}
			}
			first1 = first1.GetNext();
		}
		return last1;
	}

	public static ArrayIterator<T> SearchN<T>(
		this ArrayIterator<T> first,
		ArrayIterator<T> last,
		int count,
		T val,
		Func<T, T, bool> pred
	)
	{
		var limit = first.GetAdvanced(first.Distance(last)-count);
		while (first.NotEqual(limit))
		{
			var it = first;
			var i = 0;
			while (pred(val, it.GetCurrent()))
			{
				it = it.GetNext();
				if (++i == count)
				{
					return first;
				}
			}
			first = first.GetNext();
		}
		return last;
	}
}

public struct ListIterator<T>
{
	public IList<T> List;
	public int Index;
}

public static class ListIteratorExtensions
{
	public static ListIterator<T> Begin<T>(this IList<T> List)
	{
		return new ListIterator<T> { List = List };
	}

	public static ListIterator<T> End<T>(this IList<T> List)
	{
		return new ListIterator<T> { List = List, Index = List.Count };
	}

	public static ListIterator<T> IteratorAt<T>(this IList<T> List, int index)
	{
		return new ListIterator<T> { List = List, Index = index };
	}

	public static T GetCurrent<T>(this ListIterator<T> it)
	{
		return it.List[it.Index];
	}

	public static ListIterator<T> GetNext<T>(this ListIterator<T> it)
	{
		it.Index++;
		return it;
	}

	public static ListIterator<T> GetPrev<T>(this ListIterator<T> it)
	{
		it.Index--;
		return it;
	}

	public static bool IsEqual<T>(this ListIterator<T> it, ListIterator<T> other)
	{
		return it.List == other.List && it.Index == other.Index;
	}

	public static bool NotEqual<T>(this ListIterator<T> it, ListIterator<T> other)
	{
		return it.List != other.List || it.Index != other.Index;
	}

	public static ListIterator<T> GetAdvanced<T>(this ListIterator<T> it, int distance)
	{
		return new ListIterator<T> { List = it.List, Index = it.Index + distance };
	}

	public static int Distance<T>(this ListIterator<T> first, ListIterator<T> last)
	{
		return last.Index - first.Index;
	}

	public static bool AllOf<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()) == false)
			{
				return false;
			}
			first = first.GetNext();
		}
		return true;
	}

	public static bool AnyOf<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				return true;
			}
			first = first.GetNext();
		}
		return false;
	}

	public static bool NoneOf<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				return false;
			}
			first = first.GetNext();
		}
		return true;
	}

	public static void ForEach<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Action<T> callback
	)
	{
		while (first.NotEqual(last))
		{
			callback(first.GetCurrent());
			first = first.GetNext();
		}
	}

	public static ListIterator<T> Find<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		T val,
		Func<T, T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent(), val))
			{
				return first;
			}
			first = first.GetNext();
		}
		return last;
	}

	public static ListIterator<T> FindIf<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T, bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				return first;
			}
			first = first.GetNext();
		}
		return last;
	}

	public static ListIterator<T> FindIfNot<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T,bool> pred
	)
	{
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()) == false)
			{
				return first;
			}
			first = first.GetNext();
		}
		return last;
	}

	public static ListIterator<T> FindEnd<T>(
		this ListIterator<T> first1,
		ListIterator<T> last1,
		ListIterator<T> first2,
		ListIterator<T> last2,
		Func<T, T, bool> pred
	)
	{
		if (first2.IsEqual(last2))
		{
			return last1;
		}
		var ret = last1;
		while (first1.NotEqual(last1))
		{
			var it1 = first1;
			var it2 = first2;
			while (pred(it1.GetCurrent(), it2.GetCurrent()))
			{
				it1 = it1.GetNext();
				it2 = it2.GetNext();
				if (it2.IsEqual(last2))
				{
					ret = first1;
					break;
				}
				if (it1.IsEqual(last1))
				{
					return ret;
				}
			}
			first1 = first1.GetNext();
		}
		return ret;
	}

	public static ListIterator<T> FindFirstOf<T>(
		this ListIterator<T> first1,
		ListIterator<T> last1,
		ListIterator<T> first2,
		ListIterator<T> last2,
		Func<T, T, bool> pred
	)
	{
		while (first1.NotEqual(last1))
		{
			for (var it = first2; it.NotEqual(last2); it = it.GetNext())
			{
				if (pred(it.GetCurrent(), first1.GetCurrent()))
				{
					return first1;
				}
			}
			first1 = first1.GetNext();
		}
		return last1;
	}

	public static ListIterator<T> AdjacentFind<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T, T, bool> pred
	)
	{
		if (first.NotEqual(last))
		{
			var next=first;
			next = next.GetNext();
			while (next.NotEqual(last))
			{
				if (pred(first.GetCurrent(), next.GetCurrent()))
				{
					return first;
				}
				first = first.GetNext();
				next = next.GetNext();
			}
		}
		return last;
	}

	public static int Count<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		T val,
		Func<T, T, bool> pred
	)
	{
		var count = 0;
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent(), val))
			{
				count++;
			}
			first = first.GetNext();
		}
		return count;
	}

	public static int CountIf<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		Func<T, bool> pred
	)
	{
		var count = 0;
		while (first.NotEqual(last))
		{
			if (pred(first.GetCurrent()))
			{
				count++;
			}
			first = first.GetNext();
		}
		return count;
	}

	public static void Mismatch<T>(
		this ListIterator<T> first1,
		ListIterator<T> last1,
		ListIterator<T> first2,
		Func<T, T, bool> pred,
		out ListIterator<T> mismatch1,
		out ListIterator<T> mismatch2
	)
	{
		while (first1.NotEqual(last1))
		{
			if (pred(first1.GetCurrent(), first2.GetCurrent()) == false)
			{
				break;
			}
			first1 = first1.GetNext();
			first2 = first2.GetNext();
		}
		mismatch1 = first1;
		mismatch2 = first2;
	}

	public static bool Equal<T>(
		this ListIterator<T> first1,
		ListIterator<T> last1,
		ListIterator<T> first2,
		Func<T, T, bool> pred
	)
	{
		while (first1.NotEqual(last1))
		{
			if (pred(first1.GetCurrent(), first2.GetCurrent()) == false)
			{
				return false;
			}
			first1 = first1.GetNext();
			first2 = first2.GetNext();
		}
		return true;
	}

	public static bool IsPermutation<T>(
		this ListIterator<T> first1,
		ListIterator<T> last1,
		ListIterator<T> first2,
		Func<T, T, bool> pred
	)
	{
		first1.Mismatch(last1, first2, pred, out first1, out first2);
		if (first1.IsEqual(last1))
		{
			return true;
		}
		var last2 = first2;
		last2 = last2.GetAdvanced(first1.Distance(last1));
		for (var it1 = first1; it1.NotEqual(last1); it1 = it1.GetNext())
		{
			if (first1.Find(it1, it1.GetCurrent(), pred).IsEqual(it1))
			{
				var n = first2.Count(last2, it1.GetCurrent(), pred);
				if (n == 0 || it1.Count(last1, it1.GetCurrent(), pred) != n)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static ListIterator<T> Search<T>(
		this ListIterator<T> first1,
		ListIterator<T> last1,
		ListIterator<T> first2,
		ListIterator<T> last2,
		Func<T, T, bool> pred
	)
	{
		if (first2.IsEqual(last2))
		{
			return first1;
		}

		while (first1.NotEqual(last1))
		{
			var it1 = first1;
			var it2 = first2;
			while (pred(it1.GetCurrent(), it2.GetCurrent()))
			{
				it1 = it1.GetNext();
				it2 = it2.GetNext();
				if (it2.IsEqual(last2))
				{
					return first1;
				}
				if (it1.IsEqual(last1))
				{
					return last1;
				}
			}
			first1 = first1.GetNext();
		}
		return last1;
	}

	public static ListIterator<T> SearchN<T>(
		this ListIterator<T> first,
		ListIterator<T> last,
		int count,
		T val,
		Func<T, T, bool> pred
	)
	{
		var limit = first.GetAdvanced(first.Distance(last)-count);
		while (first.NotEqual(limit))
		{
			var it = first;
			var i = 0;
			while (pred(val, it.GetCurrent()))
			{
				it = it.GetNext();
				if (++i == count)
				{
					return first;
				}
			}
			first = first.GetNext();
		}
		return last;
	}
}
