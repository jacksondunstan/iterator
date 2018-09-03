//-----------------------------------------------------------------------
// <copyright file="NativeArrayIterator.cs" company="Jackson Dunstan">
//     Copyright (c) Jackson Dunstan. See LICENSE.txt.
// </copyright>
//-----------------------------------------------------------------------

// NativeArray<T> was introduced in Unity 2018.1
#if UNITY_2018_1_OR_NEWER

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace JacksonDunstanIterator
{
	public struct NativeArrayIterator<T>
		where T : struct
	{
		public NativeArray<T> Array;
		public int Index;
	}

	public static class NativeArrayIteratorExtensions
	{
		public static NativeArrayIterator<T> Begin<T>(this NativeArray<T> array)
			where T : struct
		{
			return new NativeArrayIterator<T> { Array = array };
		}

		public static NativeArrayIterator<T> End<T>(this NativeArray<T> array)
			where T : struct
		{
			return new NativeArrayIterator<T> { Array = array, Index = array.Length };
		}

		public static NativeArrayIterator<T> IteratorAt<T>(this NativeArray<T> array, int index)
			where T : struct
		{
			return new NativeArrayIterator<T> { Array = array, Index = index };
		}

		public static T GetCurrent<T>(this NativeArrayIterator<T> it)
			where T : struct
		{
			return it.Array[it.Index];
		}

		public static void SetCurrent<T>(this NativeArrayIterator<T> it, T val)
			where T : struct
		{
			it.Array[it.Index] = val;
		}

		public static NativeArrayIterator<T> GetNext<T>(this NativeArrayIterator<T> it)
			where T : struct
		{
			it.Index++;
			return it;
		}

		public static NativeArrayIterator<T> GetPrev<T>(this NativeArrayIterator<T> it)
			where T : struct
		{
			it.Index--;
			return it;
		}

		public static unsafe bool IsEqual<T>(this NativeArrayIterator<T> it, NativeArrayIterator<T> other)
			where T : struct
		{
			return it.Array.GetUnsafeReadOnlyPtr() == other.Array.GetUnsafeReadOnlyPtr()
				     && it.Index == other.Index;
		}

		public static unsafe bool NotEqual<T>(this NativeArrayIterator<T> it, NativeArrayIterator<T> other)
			where T : struct
		{
			return it.Array.GetUnsafeReadOnlyPtr() != other.Array.GetUnsafeReadOnlyPtr()
				     || it.Index != other.Index;
		}

		public static NativeArrayIterator<T> GetAdvanced<T>(this NativeArrayIterator<T> it, int distance)
			where T : struct
		{
			return new NativeArrayIterator<T> { Array = it.Array, Index = it.Index + distance };
		}

		public static int Distance<T>(this NativeArrayIterator<T> first, NativeArrayIterator<T> last)
			where T : struct
		{
			return last.Index - first.Index;
		}

		public static bool AllOf<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
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
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
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
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
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
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Action<T> callback
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				callback(first.GetCurrent());
				first = first.GetNext();
			}
		}

		public static NativeArrayIterator<T> Find<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			T val,
			Func<T, T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> FindIf<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> FindIfNot<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> FindEnd<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			Func<T, T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> FindFirstOf<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			Func<T, T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> AdjacentFind<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> pred
		)
			where T : struct
		{
			if (first.NotEqual(last))
			{
				var next = first;
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
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			T val,
			Func<T, T, bool> pred
		)
			where T : struct
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
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
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
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			Func<T, T, bool> pred,
			out NativeArrayIterator<T> mismatch1,
			out NativeArrayIterator<T> mismatch2
		)
			where T : struct
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
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			Func<T, T, bool> pred
		)
			where T : struct
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
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			Func<T, T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> Search<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			Func<T, T, bool> pred
		)
			where T : struct
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

		public static NativeArrayIterator<T> SearchN<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			int count,
			T val,
			Func<T, T, bool> pred
		)
			where T : struct
		{
			var limit = first.GetAdvanced(first.Distance(last) - count);
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

		public static NativeArrayIterator<T> Copy<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				result.SetCurrent(first.GetCurrent());
				result = result.GetNext();
				first = first.GetNext();
			}
			return result;
		}

		public static NativeArrayIterator<T> CopyN<T>(
			this NativeArrayIterator<T> first,
			int n,
			NativeArrayIterator<T> result
		)
			where T : struct
		{
			while (n > 0)
			{
				result.SetCurrent(first.GetCurrent());
				result = result.GetNext();
				first = first.GetNext();
				n--;
			}
			return result;
		}

		public static NativeArrayIterator<T> CopyIf<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result,
			Func<T, bool> pred
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				if (pred(first.GetCurrent()))
				{
					result.SetCurrent(first.GetCurrent());
					result = result.GetNext();
				}
				first = first.GetNext();
			}
			return result;
		}

		public static NativeArrayIterator<T> CopyBackward<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result
		)
			where T : struct
		{
			while (last.NotEqual(first))
			{
				result = result.GetPrev();
				last = last.GetPrev();
				result.SetCurrent(last.GetCurrent());
			}
			return result;
		}

		public static NativeArrayIterator<T> SwapRanges<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2
		)
			where T : struct
		{
			while (first1.NotEqual(last1))
			{
				Swap(first1, first2);
				first1 = first1.GetNext();
				first2 = first2.GetNext();
			}
			return first2;
		}

		public static void Swap<T>(this NativeArrayIterator<T> a, NativeArrayIterator<T> b)
			where T : struct
		{
			var temp = a.GetCurrent();
			a.SetCurrent(b.GetCurrent());
			b.SetCurrent(temp);
		}

		public static NativeArrayIterator<T> Transform<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> result,
			Func<T, T> op
		)
			where T : struct
		{
			while (first1.NotEqual(last1))
			{
				result.SetCurrent(op(first1.GetCurrent()));
				result = result.GetNext();
				first1 = first1.GetNext();
			}
			return result;
		}

		public static NativeArrayIterator<T> Transform<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> result,
			Func<T, T, T> binaryOp
		)
			where T : struct
		{
			while (first1.NotEqual(last1))
			{
				result.SetCurrent(binaryOp(first1.GetCurrent(), first2.GetCurrent()));
				first2 = first2.GetNext();
				result = result.GetNext();
				first1 = first1.GetNext();
			}
			return result;
		}

		public static void ReplaceIf<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred,
			T newValue
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				if (pred(first.GetCurrent()))
				{
					first.SetCurrent(newValue);

				}
				first = first.GetNext();
			}
		}

		public static NativeArrayIterator<T> ReplaceCopyIf<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result,
			Func<T, bool> pred,
			T newValue
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				result.SetCurrent(pred(first.GetCurrent()) ? newValue : first.GetCurrent());
				first = first.GetNext();
				result = result.GetNext();
			}
			return result;
		}

		public static NativeArrayIterator<T> Unique<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> pred
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				return last;
			}

			var result = first;
			while ((first = first.GetNext()).NotEqual(last))
			{
				if (pred(result.GetCurrent(), first.GetCurrent()) == false)
				{
					result = result.GetNext();
					result.SetCurrent(first.GetCurrent());
				}
			}
			result = result.GetNext();
			return result;
		}

		public static NativeArrayIterator<T> UniqueCopy<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result,
			Func<T, T, bool> pred
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				return last;
			}

			result.SetCurrent(first.GetCurrent());
			while ((first = first.GetNext()).NotEqual(last))
			{
				var val = first.GetCurrent();
				if (pred(result.GetCurrent(), val) == false)
				{
					result = result.GetNext();
					result.SetCurrent(val);
				}
			}
			result = result.GetNext();
			return result;
		}

		public static void Reverse<T>(this NativeArrayIterator<T> first, NativeArrayIterator<T> last)
			where T : struct
		{
			while ((first.NotEqual(last)) && (first.NotEqual((last = last.GetPrev()))))
			{
				first.Swap(last);
				first = first.GetNext();
			}
		}

		public static NativeArrayIterator<T> ReverseCopy<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				last = last.GetPrev();
				result.SetCurrent(last.GetCurrent());
				result = result.GetNext();
			}
			return result;
		}

		public static void Rotate<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> middle,
			NativeArrayIterator<T> last
		)
			where T : struct
		{
			var next = middle;
			while (first.NotEqual(last))
			{
				first.Swap(next);
				first = first.GetNext();
				next = next.GetNext();
				if (next.IsEqual(last))
				{
					next = middle;
				}
				else if (first.IsEqual(middle))
				{
					middle = next;
				}
			}
		}

		public static NativeArrayIterator<T> RotateCopy<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> middle,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> result
		)
			where T : struct
		{
			result = Copy(middle, last, result);
			return Copy(first, middle, result);
		}

		public static void RandomShuffle<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<int, int> gen
		)
			where T : struct
		{
			var n = Distance(first, last);
			for (var i = n - 1; i > 0; --i)
			{
				first.GetAdvanced(i).Swap(first.GetAdvanced(gen(i + 1)));
			}
		}

		public static bool IsPartitioned<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
		{
			while (first.NotEqual(last) && pred(first.GetCurrent()))
			{
				first = first.GetNext();
			}
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

		public static NativeArrayIterator<T> Partition<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				while (pred(first.GetCurrent()))
				{
					first = first.GetNext();
					if (first.IsEqual(last))
					{
						return first;
					}
				}
				do
				{
					last = last.GetPrev();
					if (first.IsEqual(last))
					{
						return first;
					}
				} while (pred(last.GetCurrent()) == false);
				first.Swap(last);
				first = first.GetNext();
			}
			return first;
		}

		public static void PartitionCopy<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			NativeArrayIterator<T> resultTrue,
			NativeArrayIterator<T> resultFalse,
			Func<T, bool> pred,
			out NativeArrayIterator<T> outResultTrue,
			out NativeArrayIterator<T> outResultFalse
		)
			where T : struct
		{
			while (first.NotEqual(last))
			{
				if (pred(first.GetCurrent()))
				{
					resultTrue.SetCurrent(first.GetCurrent());
					resultTrue = resultTrue.GetNext();
				}
				else
				{
					resultFalse.SetCurrent(first.GetCurrent());
					resultFalse = resultFalse.GetNext();
				}
				first = first.GetNext();
			}
			outResultTrue = resultTrue;
			outResultFalse = resultFalse;
		}

		public static NativeArrayIterator<T> PartitionPoint<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, bool> pred
		)
			where T : struct
		{
			var n = first.Distance(last);
			while (n > 0)
			{
				var it = first;
				var step = n / 2;
				it.GetAdvanced(step);
				if (pred(it.GetCurrent()))
				{
					first = it.GetNext();
					n -= step + 1;
				}
				else
				{
					n = step;
				}
			}
			return first;
		}

		public static void Sort<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			// Quicksort
			if (first.IsEqual(last))
			{
				return;
			}
			var sep = first;
			for (var i = first.GetNext(); i.NotEqual(last); i = i.GetNext())
			{
				if (comp(i.GetCurrent(), first.GetCurrent()))
				{
					sep = sep.GetNext();
					sep.Swap(i);
				}
			}
			first.Swap(sep);
			first.Sort(sep, comp);
			sep.GetNext().Sort(last, comp);
		}

		public static void StableSort<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			// TODO find a faster algorithm that doesn't create any garbage than insertion sort
			var arr = first.Array;
			for (var i = first.Index + 1; i < last.Index; i++)
			{
				var x = arr[i];
				var left = first.Index;
				var right = i - 1;
				while (left <= right)
				{
					var middle = (left + right) / 2;
					if (comp(x, arr[middle]))
					{
						right = middle - 1;
					}
					else
					{
						left = middle + 1;
					}
				}
				for (var j = i - 1; j >= left; j--)
				{
					arr[j + 1] = arr[j];
				}
				arr[left] = x;
			}
		}

		public static void PartialSort<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> middle,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			// TODO find a faster algorithm that doesn't create any garbage
			first.Sort(last, comp);
		}

		public static bool IsSorted<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				return true;
			}
			var next = first;
			while ((next = next.GetNext()).NotEqual(last))
			{
				if (comp(next.GetCurrent(), first.GetCurrent()))
				{
					return false;
				}
				first = first.GetNext();
			}
			return true;
		}

		public static NativeArrayIterator<T> IsSortedUntil<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				return first;
			}
			var next = first;
			while ((next = next.GetNext()).NotEqual(last))
			{
				if (comp(next.GetCurrent(), first.GetCurrent()))
				{
					return next;
				}
				first = first.GetNext();
			}
			return last;
		}

		public static void NthElement<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> nth,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			// TODO find a faster algorithm that doesn't create any garbage
			first.Sort(last, comp);
		}

		public static NativeArrayIterator<T> LowerBound<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			T val,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var count = first.Distance(last);
			while (count > 0)
			{
				var it = first;
				var step = count / 2;
				it = it.GetAdvanced(step);
				if (comp(it.GetCurrent(), val))
				{
					it = it.GetNext();
					first = it;
					count -= step + 1;
				}
				else
				{
					count = step;
				}
			}
			return first;
		}

		public static NativeArrayIterator<T> UpperBound<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			T val,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var count = Distance(first, last);
			while (count > 0)
			{
				var it = first;
				var step = count / 2;
				it = it.GetAdvanced(step);
				if (comp(val, it.GetCurrent()) == false)
				{
					it = it.GetNext();
					first = it;
					count -= step + 1;
				}
				else
				{
					count = step;
				}
			}
			return first;
		}

		public static void EqualRange<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			T val,
			Func<T, T, bool> comp,
			out NativeArrayIterator<T> lower,
			out NativeArrayIterator<T> upper
		)
			where T : struct
		{
			lower = first.LowerBound(last, val, comp);
			upper = lower.UpperBound(last, val, comp);
		}

		public static bool BinarySearch<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			T val,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			first = first.LowerBound(last, val, comp);
			return first.NotEqual(last) && comp(val, first.GetCurrent()) == false;
		}

		public static NativeArrayIterator<T> Merge<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			NativeArrayIterator<T> result,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (true)
			{
				if (first1.IsEqual(last1))
				{
					return first2.Copy(last2, result);
				}
				if (first2.IsEqual(last2))
				{
					return first1.Copy(last1, result);
				}
				if (comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					result.SetCurrent(first2.GetCurrent());
					first2 = first2.GetNext();
				}
				else
				{
					result.SetCurrent(first1.GetCurrent());
					first1 = first1.GetNext();
				}
				result = result.GetNext();
			}
		}

		public static void InplaceMerge<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> middle,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.Index >= middle.Index || middle.Index >= last.Index)
			{
				return;
			}
			if (last.Index - first.Index == 2)
			{
				if (comp(middle.GetCurrent(), first.GetCurrent()))
				{
					Swap(first, middle);
				}
				return;
			}
			NativeArrayIterator<T> firstCut;
			NativeArrayIterator<T> secondCut;
			if (middle.Index - first.Index > last.Index - middle.Index)
			{
				firstCut = first.GetAdvanced(first.Distance(middle) / 2);
				secondCut = middle.LowerBound(last, firstCut.GetCurrent(), comp);
			}
			else
			{
				secondCut = middle.GetAdvanced(middle.Distance(last) / 2);
				firstCut = first.UpperBound(middle, secondCut.GetCurrent(), comp);
			}
			Rotate(firstCut, middle, secondCut);
			middle = firstCut.GetAdvanced(middle.Distance(secondCut));
			InplaceMerge(first, firstCut, middle, comp);
			InplaceMerge(middle, secondCut, last, comp);
		}

		public static bool Includes<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (first2.NotEqual(last2))
			{
				if ((first1.IsEqual(last1)) || comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					return false;
				}
				if (comp(first1.GetCurrent(), first2.GetCurrent()) == false)
				{
					first2 = first2.GetNext();
				}
				first1 = first1.GetNext();
			}
			return true;
		}

		public static NativeArrayIterator<T> SetUnion<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			NativeArrayIterator<T> result,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (true)
			{
				if (first1.IsEqual(last1))
				{
					return first2.Copy(last2, result);
				}
				if (first2.IsEqual(last2))
				{
					return first1.Copy(last1, result);
				}
				if (comp(first1.GetCurrent(), first2.GetCurrent()))
				{
					result.SetCurrent(first1.GetCurrent());
					first1 = first1.GetNext();
				}
				else if (comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					result.SetCurrent(first2.GetCurrent());
					first2 = first2.GetNext();
				}
				else
				{
					result.SetCurrent(first1.GetCurrent());
					first1 = first1.GetNext();
					first2 = first2.GetNext();
				}
				result = result.GetNext();
			}
		}

		public static NativeArrayIterator<T> SetIntersection<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			NativeArrayIterator<T> result,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (first1.NotEqual(last1) && first2.NotEqual(last2))
			{
				if (comp(first1.GetCurrent(), first2.GetCurrent()))
				{
					first1 = first1.GetNext();
				}
				else if (comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					first2 = first2.GetNext();
				}
				else
				{
					result.SetCurrent(first1.GetCurrent());
					result = result.GetNext();
					first1 = first1.GetNext();
					first2 = first2.GetNext();
				}
			}
			return result;
		}

		public static NativeArrayIterator<T> SetDifference<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			NativeArrayIterator<T> result,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (first1.NotEqual(last1) && first2.NotEqual(last2))
			{
				if (comp(first1.GetCurrent(), first2.GetCurrent()))
				{
					result.SetCurrent(first1.GetCurrent());
					result = result.GetNext();
					first1 = first1.GetNext();
				}
				else if (comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					first2 = first2.GetNext();
				}
				else
				{
					first1 = first1.GetNext();
					first2 = first2.GetNext();
				}
			}
			return first1.Copy(last1, result);
		}

		public static NativeArrayIterator<T> SetSymmetricDifference<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			NativeArrayIterator<T> result,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (true)
			{
				if (first1.IsEqual(last1))
				{
					return first2.Copy(last2, result);
				}
				if (first2.IsEqual(last2))
				{
					return first1.Copy(last1, result);
				}
				if (comp(first1.GetCurrent(), first2.GetCurrent()))
				{
					result.SetCurrent(first1.GetCurrent());
					result = result.GetNext();
					first1 = first1.GetNext();
				}
				else if (comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					result.SetCurrent(first2.GetCurrent());
					result = result.GetNext();
					first2 = first2.GetNext();
				}
				else
				{
					first1 = first1.GetNext();
					first2 = first2.GetNext();
				}
			}
		}

		public static void PushHeap<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.Distance(last) < 2)
			{
				return;
			}
			last = last.GetPrev();
			var temp = last.GetCurrent();
			var parent = first.GetAdvanced((first.Distance(last) - 1) / 2);
			while (first.Distance(last) > 0 && comp(parent.GetCurrent(), temp))
			{
				last.SetCurrent(parent.GetCurrent());
				last = parent;
				parent = first.GetAdvanced((first.Distance(last) - 1) / 2);
			}
			last.SetCurrent(temp);
		}

		public static void PopHeap<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.Distance(last) < 2)
			{
				return;
			}
			last = last.GetPrev();
			Swap(first, last);
			AdjustHeap(first.Array, first.Index, first.Index, last.Index, comp);
		}

		public static void MakeHeap<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var dist = first.Distance(last);
			if (dist < 2)
			{
				return;
			}
			var parent = (dist - 2) / 2;
			do
			{
				AdjustHeap(first.Array, first.Index, first.Index + parent, last.Index, comp);
			}
			while (parent-- != 0);
		}

		public static void SortHeap<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (first.Distance(last) > 1)
			{
				last = last.GetPrev();
				Swap(first, last);
				AdjustHeap(first.Array, first.Index, first.Index, last.Index, comp);
			}
		}

		public static NativeArrayIterator<T> MinElement<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				return last;
			}
			var smallest = first;
			while ((first = first.GetNext()).NotEqual(last))
			{
				if (comp(first.GetCurrent(), smallest.GetCurrent()))
				{
					smallest = first;
				}
			}
			return smallest;
		}

		public static NativeArrayIterator<T> MaxElement<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				return last;
			}
			var largest = first;
			while ((first = first.GetNext()).NotEqual(last))
			{
				if (comp(largest.GetCurrent(), first.GetCurrent()))
				{
					largest = first;
				}
			}
			return largest;
		}

		public static void MinMaxElement<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp,
			out NativeArrayIterator<T> min,
			out NativeArrayIterator<T> max
		)
			where T : struct
		{
			if (first.IsEqual(last))
			{
				min = last;
				max = last;
			}
			min = first;
			max = first;
			while ((first = first.GetNext()).NotEqual(last))
			{
				if (comp(first.GetCurrent(), min.GetCurrent()))
				{
					min = first;
				}
				if (comp(max.GetCurrent(), first.GetCurrent()))
				{
					max = first;
				}
			}
		}

		public static bool LexicographicalCompare<T>(
			this NativeArrayIterator<T> first1,
			NativeArrayIterator<T> last1,
			NativeArrayIterator<T> first2,
			NativeArrayIterator<T> last2,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			while (first1.NotEqual(last1))
			{
				if (first2.IsEqual(last2) || comp(first2.GetCurrent(), first1.GetCurrent()))
				{
					return false;
				}
				else if (comp(first1.GetCurrent(), first2.GetCurrent()))
				{
					return true;
				}
				first1 = first1.GetNext();
				first2 = first2.GetNext();
			}
			return first2.NotEqual(last2);
		}

		public static bool NextPermutation<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var i = last;
			if (first.IsEqual(last) || first.IsEqual((i = i.GetPrev())))
			{
				return false;
			}
			while (true)
			{
				var ip1 = i;
				if (comp((i = i.GetPrev()).GetCurrent(), ip1.GetCurrent()))
				{
					var j = last;
					while (comp(i.GetCurrent(), (j = j.GetPrev()).GetCurrent()) == false)
					{
					}
					Swap(i, j);
					Reverse(ip1, last);
					return true;
				}
				if (i.IsEqual(first))
				{
					Reverse(first, last);
					return false;
				}
			}
		}

		public static bool PrevPermutation<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var i = last;
			if (first.IsEqual(last) || first.IsEqual(i = i.GetPrev()))
			{
				return false;
			}
			while (true)
			{
				var ip1 = i;
				if (comp(ip1.GetCurrent(), (i = i.GetPrev()).GetCurrent()))
				{
					var j = last;
					while (comp((j = j.GetPrev()).GetCurrent(), i.GetCurrent()) == false)
					{
					}
					Swap(i, j);
					Reverse(ip1, last);
					return true;
				}
				if (i.IsEqual(first))
				{
					Reverse(first, last);
					return false;
				}
			}
		}

		private static void AdjustHeap<T>(
			this NativeArray<T> array,
			int first,
			int position,
			int last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var tmp = array[position];
			int len = last - first;
			int holeIndex = position - first;
			int secondChild = 2 * holeIndex + 2;
			while (secondChild < len)
			{
				if (
					comp(
						array[first + secondChild],
						array[first + (secondChild - 1)]
					)
				)
				{
					--secondChild;
				}
				array[first + holeIndex] = array[first + secondChild];
				holeIndex = secondChild++;
				secondChild *= 2;
			}
			if (secondChild-- == len)
			{
				array[first + holeIndex] = array[first + secondChild];
				holeIndex = secondChild;
			}
			var parent = (holeIndex - 1) / 2;
			var topIndex = position - first;
			while (holeIndex != topIndex && comp(array[first + parent], tmp))
			{
				array[first + holeIndex] = array[first + parent];
				holeIndex = parent;
				parent = (holeIndex - 1) / 2;
			}
			array[first + holeIndex] = tmp;
		}

		public static bool IsHeap<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			return first.IsHeapUntil(last, comp).IsEqual(last);
		}

		public static NativeArrayIterator<T> IsHeapUntil<T>(
			this NativeArrayIterator<T> first,
			NativeArrayIterator<T> last,
			Func<T, T, bool> comp
		)
			where T : struct
		{
			var len = first.Distance(last);
			var p = 0;
			var c = 1;
			var pp = first;
			while (c < len)
			{
				var cp = first.GetAdvanced(c);
				if (comp(pp.GetCurrent(), cp.GetCurrent()))
				{
					return cp;
				}
				c++;
				cp = cp.GetNext();
				if (c == len)
				{
					return last;
				}
				if (comp(pp.GetCurrent(), cp.GetCurrent()))
				{
					return cp;
				}
				p++;
				pp = pp.GetNext();
				c = 2 * p + 1;
			}
			return last;
		}
	}
}

#endif