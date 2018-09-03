//-----------------------------------------------------------------------
// <copyright file="GcTest.cs" company="Jackson Dunstan">
//     Copyright (c) Jackson Dunstan. See LICENSE.txt.
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Linq;
using UnityEngine;
using JacksonDunstanIterator;

/// <summary>
/// Script to test whether the library creates any garbage. Run it with the
/// Unity profiler in "deep" mode and check the "Gc Alloc" column for
/// GcTest.Start().
/// </summary>
public class GcTest : MonoBehaviour
{
	private static readonly System.Random random = new System.Random();
	private static readonly int[] defaultArr = { 1, 2, 2, 3 };
	private static readonly int[] arr = defaultArr.ToArray();
	private static readonly int[] arr2 = new int[arr.Length];
	private static readonly int[] arr3 = new int[arr.Length];
	private static readonly int[] arrLong = new int[arr.Length * 2];
	private static readonly Func<int, int, bool> AreIntsEqual = (a, b) => a == b;
	private static readonly Func<int, bool> IsIntEven = i => i % 2 == 0;
	private static readonly Func<int, bool> IsIntEqualTo2 = i => i == 2;
	private static readonly Func<int, bool> IsIntLessThanOrEqualTo2 = i => i <= 2;
	private static readonly Func<int, bool> IsIntGreaterThan0 = i => i > 0;
	private static readonly Func<int, bool> IsIntGreaterThan10 = i => i > 10;
	private static readonly Func<int, int> DoubleInt = i => i * 2;
	private static readonly Func<int, int, int> MultiplyInts = (a, b) => a * b;
	private static readonly Func<int, int, bool> IsIntGreaterThanInt = (a, b) => a > b;
	private static readonly Func<int, int, bool> IsIntLessThanInt = (a, b) => a < b;
	private static readonly Action<int> NoOp = i => { };
	private static readonly Func<int, int> RandomIntLessThan = random.Next;
	private static int[] OneThreeThreeFour = { 1, 3, 3, 4 };
	private static int[] OneThreeTwoFour = { 1, 3, 2, 4 };
	private static int[] ThreeThreeOneTwo = { 3, 3, 1, 2 };
	private static int[] ThreeTwoOneTwo = { 3, 2, 1, 2 };
	private static int[] ThreeTwoOne = { 3, 2, 1 };
	private static int[] TwoThree = { 2, 3 };
	private static int[] FourTwoThreeOne = { 4, 2, 3, 1 };
	private static int[] OneTwoThreeFour = { 1, 2, 3, 4 };

	void Start()
	{
		arr.Begin().GetAdvanced(1).GetCurrent();
		arr.Begin().Distance(arr.End());
		arr.Begin().AllOf(arr.End(), IsIntEven);
		arr.Begin().AllOf(arr.End(), IsIntGreaterThan0);
		arr.Begin().AnyOf(arr.End(), IsIntGreaterThan10);
		arr.Begin().AnyOf(arr.End(), IsIntEqualTo2);
		arr.Begin().NoneOf(arr.End(), IsIntEqualTo2);
		arr.Begin().NoneOf(arr.End(), IsIntGreaterThan10);
		arr.Begin().ForEach(arr.End(), NoOp);
		arr.Begin().Find(arr.End(), 2, AreIntsEqual);
		arr.Begin().FindIf(arr.End(), IsIntEven);
		arr.Begin().FindIfNot(arr.End(), IsIntEven);
		arr.Begin().FindEnd(
			arr.End(),
			arr.IteratorAt(1),
			arr.IteratorAt(2),
			AreIntsEqual
		);
		arr.Begin().FindFirstOf(
			arr.End(),
			arr.IteratorAt(1),
			arr.IteratorAt(2),
			AreIntsEqual
		);
		arr.Begin().AdjacentFind(arr.End(), AreIntsEqual);
		arr.Begin().Count(arr.End(), 2, AreIntsEqual);
		arr.Begin().CountIf(arr.End(), IsIntEven);
		ArrayIterator<int> mm1;
		ArrayIterator<int> mm2;
		arr.Begin().Mismatch(
			arr.End(),
			OneThreeThreeFour.Begin(),
			AreIntsEqual,
			out mm1,
			out mm2
		);
		arr.Begin().IsPermutation(arr.End(), ThreeThreeOneTwo.Begin(), AreIntsEqual);
		arr.Begin().IsPermutation(arr.End(), ThreeTwoOneTwo.Begin(), AreIntsEqual);
		arr.Begin().Search(arr.End(), TwoThree.Begin(), TwoThree.End(), AreIntsEqual);
		arr.Begin().SearchN(arr.End(), 2, 2, AreIntsEqual);
		arr.Begin().Copy(arr.IteratorAt(2), arr2.Begin());
		arr.Begin().CopyN(3, arr2.Begin());
		arr.Begin().CopyIf(arr.End(), arr2.Begin(), IsIntEven);
		arr.IteratorAt(1).CopyBackward(arr.IteratorAt(3), arr2.End());
		arr.IteratorAt(1).SwapRanges(arr.IteratorAt(3), arr2.IteratorAt(1));
		var itA = arr.IteratorAt(0);
		var itB = arr.IteratorAt(1);
		itA.Swap(itB);
		arr.Begin().Transform(arr.End(), arr.Begin(), DoubleInt);
		arr.Begin().Transform(arr.End(), arr.Begin(), arr.Begin(), MultiplyInts);
		arr.Begin().ReplaceIf(arr.End(), IsIntEqualTo2, 20);
		arr.Begin().ReplaceCopyIf(arr.End(), arr.Begin(), IsIntEven, 200);
		arr.Begin().Unique(arr.End(), AreIntsEqual);
		arr.Begin().UniqueCopy(arr.End(), arr2.Begin(), AreIntsEqual);
		arr.Begin().Reverse(arr.End());
		arr.Begin().ReverseCopy(arr.End(), arr2.Begin());
		arr.Begin().Rotate(arr.IteratorAt(2), arr.End());
		arr.Begin().RotateCopy(arr.IteratorAt(2), arr.End(), arr2.Begin());
		arr.Begin().RandomShuffle(arr.End(), RandomIntLessThan);
		arr.Begin().Copy(arr.End(), arr2.Begin());
		arr2.Begin().RandomShuffle(arr2.End(), RandomIntLessThan);
		arr.Begin().IsPartitioned(arr.End(), IsIntEven);
		arr.Begin().IsPartitioned(arr.End(), IsIntLessThanOrEqualTo2);
		arr.Begin().Partition(arr.End(), IsIntEven);
		ArrayIterator<int> outResultTrue;
		ArrayIterator<int> outResultFalse;
		arr.Begin().PartitionCopy(
			arr.End(),
			arr2.Begin(),
			arr3.Begin(),
			IsIntEven,
			out outResultTrue,
			out outResultFalse
		);
		arr.Begin().PartitionPoint(arr.End(), IsIntLessThanOrEqualTo2);
		arr.Begin().Sort(arr.End(), IsIntGreaterThanInt);
		arr.Begin().StableSort(arr.End(), IsIntGreaterThanInt);
		arr.Begin().PartialSort(arr.IteratorAt(2), arr.End(), IsIntGreaterThanInt);
		arr.Begin().IsSorted(arr.End(), IsIntLessThanInt);
		OneThreeTwoFour.Begin().IsSortedUntil(OneThreeTwoFour.End(), IsIntLessThanInt);
		OneThreeTwoFour.Begin().Copy(OneThreeTwoFour.End(), arr.Begin());
		arr.Begin().NthElement(arr.IteratorAt(2), arr.End(), IsIntLessThanInt);
		arr.Begin().LowerBound(arr.End(), 2, IsIntLessThanInt);
		arr.Begin().UpperBound(arr.End(), 2, IsIntLessThanInt);
		ArrayIterator<int> equalRangeLower;
		ArrayIterator<int> equalRangeUpper;
		arr.Begin().EqualRange(
			arr.End(),
			2,
			IsIntLessThanInt,
			out equalRangeLower,
			out equalRangeUpper
		);
		arr.Begin().BinarySearch(arr.End(), 2, IsIntLessThanInt);
		arr.Begin().BinarySearch(arr.End(), 9, IsIntLessThanInt);
		arr.Begin().Merge(
			arr.End(),
			OneThreeThreeFour.Begin(),
			OneThreeThreeFour.End(),
			arrLong.Begin(),
			IsIntLessThanInt
		);
		var copyResult = arr.Begin().Copy(arr.End(), arrLong.Begin());
		OneThreeThreeFour.Begin().Copy(OneThreeThreeFour.End(), copyResult);
		arrLong.Begin().InplaceMerge(copyResult, arrLong.End(), IsIntLessThanInt);
		arr.Begin().Includes(arr.End(), TwoThree.Begin(), TwoThree.End(), IsIntLessThanInt);
		arr.Begin().SetUnion(
			arr.End(),
			OneThreeThreeFour.Begin(),
			OneThreeThreeFour.End(),
			arrLong.Begin(),
			IsIntLessThanInt
		);
		arr.Begin().SetIntersection(
			arr.End(),
			OneThreeThreeFour.Begin(),
			OneThreeThreeFour.End(),
			arrLong.Begin(),
			IsIntLessThanInt
		);
		arr.Begin().SetDifference(
			arr.End(),
			OneThreeThreeFour.Begin(),
			OneThreeThreeFour.End(),
			arrLong.Begin(),
			IsIntLessThanInt
		);
		arr.Begin().SetSymmetricDifference(
			arr.End(),
			OneThreeThreeFour.Begin(),
			OneThreeThreeFour.End(),
			arrLong.Begin(),
			IsIntLessThanInt
		);
		var pushHeapIt = FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arrLong.Begin());
		pushHeapIt.SetCurrent(5);
		pushHeapIt = pushHeapIt.GetNext();
		arrLong.Begin().PushHeap(pushHeapIt, IsIntLessThanInt);
		FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arr2.Begin());
		arr2.Begin().PopHeap(arr2.End(), IsIntLessThanInt);
		OneTwoThreeFour.Begin().Copy(OneTwoThreeFour.End(), arr2.Begin());
		arr2.Begin().MakeHeap(arr2.End(), IsIntLessThanInt);
		FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arr2.Begin());
		arr2.Begin().SortHeap(arr2.End(), IsIntLessThanInt);
		FourTwoThreeOne.Begin().IsHeap(FourTwoThreeOne.End(), IsIntLessThanInt);
		FourTwoThreeOne.Begin().IsHeapUntil(FourTwoThreeOne.End(), IsIntLessThanInt);
		arr.Begin().MinElement(arr.End(), IsIntLessThanInt);
		arr.Begin().MaxElement(arr.End(), IsIntLessThanInt);
		ArrayIterator<int> minIt;
		ArrayIterator<int> maxIt;
		arr.Begin().MinMaxElement(arr.End(), IsIntLessThanInt, out minIt, out maxIt);
		arr.Begin().LexicographicalCompare(
			arr.End(),
			arr2.Begin(),
			arr2.End(),
			IsIntLessThanInt
		);
		arr2.Begin().LexicographicalCompare(
			arr2.End(),
			arr.Begin(),
			arr.End(),
			IsIntLessThanInt
		);
		while (arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt))
		{
		}
		while (ThreeTwoOne.Begin().PrevPermutation(ThreeTwoOne.End(), IsIntLessThanInt))
		{
		}
	}
}