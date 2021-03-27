//-----------------------------------------------------------------------
// <copyright file="TestNativeArrayIterator.cs" company="Jackson Dunstan">
//     Copyright (c) Jackson Dunstan. See LICENSE.txt.
// </copyright>
//-----------------------------------------------------------------------

// NativeArray<T> was introduced in Unity 2018.1
#if UNITY_2018_1_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using JacksonDunstanIterator;
using NUnit.Framework;

public class TestNativeArrayIterator
{
	private List<NativeArray<int>> nativeArrays;
	private MonotonicRandom random;
	private NativeArray<int> defaultArr;
	private NativeArray<int> arr;
	private NativeArray<int> arr2;
	private NativeArray<int> arr3;
	private NativeArray<int> arrLong;
	private Func<int, int, bool> AreIntsEqual;
	private Func<int, bool> IsIntEven;
	private Func<int, bool> IsIntEqualTo2;
	private Func<int, bool> IsIntLessThanOrEqualTo2;
	private Func<int, bool> IsIntGreaterThan0;
	private Func<int, bool> IsIntGreaterThan10;
	private Func<int, int> DoubleInt;
	private Func<int, int, int> MultiplyInts;
	private Func<int, int, bool> IsIntGreaterThanInt;
	private Func<int, int, bool> IsIntLessThanInt;
	private Func<int, int> RandomIntLessThan;
	private NativeArray<int> OneThreeThreeFour;
	private NativeArray<int> OneThreeTwoFour;
	private NativeArray<int> ThreeTwoOne;
	private NativeArray<int> TwoThree;
	private NativeArray<int> FourTwoThreeOne;
	private NativeArray<int> OneTwoThreeFour;

	private NativeArray<int> CreateNativeArray(params int[] values)
	{
		NativeArray<int> array = new NativeArray<int>(
			values.Length,
			Allocator.Temp);
		for (int i = 0; i < values.Length; ++i)
		{
			array[i] = values[i];
		}
		nativeArrays.Add(array);
		return array;
	}

	private NativeArray<int> CreateEmptyNativeArray(int length)
	{
		NativeArray<int> array = new NativeArray<int>(
			length,
			Allocator.Temp);
		nativeArrays.Add(array);
		return array;
	}

	private NativeArray<int> CopyNativeArray(NativeArray<int> values)
	{
		NativeArray<int> array = new NativeArray<int>(
			values.Length,
			Allocator.Temp);
		for (int i = 0; i < values.Length; ++i)
		{
			array[i] = values[i];
		}
		nativeArrays.Add(array);
		return array;
	}

	[SetUp]
	public void SetUp()
	{
		nativeArrays = new List<NativeArray<int>>();
		random = new MonotonicRandom();
		defaultArr = CreateNativeArray(1, 2, 2, 3);
		arr = CopyNativeArray(defaultArr);
		arr2 = CreateEmptyNativeArray(arr.Length);
		arr3 = CreateEmptyNativeArray(arr.Length);
		arrLong = CreateEmptyNativeArray(arr.Length * 2);
		AreIntsEqual = (a, b) => a == b;
		IsIntEven = i => i % 2 == 0;
		IsIntEqualTo2 = i => i == 2;
		IsIntLessThanOrEqualTo2 = i => i <= 2;
		IsIntGreaterThan0 = i => i > 0;
		IsIntGreaterThan10 = i => i > 10;
		DoubleInt = i => i * 2;
		MultiplyInts = (a, b) => a * b;
		IsIntGreaterThanInt = (a, b) => a > b;
		IsIntLessThanInt = (a, b) => a < b;
		RandomIntLessThan = random.Next;
		OneThreeThreeFour = CreateNativeArray(1, 3, 3, 4);
		OneThreeTwoFour = CreateNativeArray(1, 3, 2, 4);
		ThreeTwoOne = CreateNativeArray(3, 2, 1);
		TwoThree = CreateNativeArray(2, 3);
		FourTwoThreeOne = CreateNativeArray(4, 2, 3, 1);
		OneTwoThreeFour = CreateNativeArray(1, 2, 3, 4);
	}

	[TearDown]
	public void TearDown()
	{
		foreach (NativeArray<int> nativeArray in nativeArrays)
		{
			nativeArray.Dispose();
		}
	}

	[Test]
	public void GetAdvanced()
	{
		Assert.That(arr.Begin().GetAdvanced(1).GetCurrent(), Is.EqualTo(2));
	}

	[Test]
	public void Distance()
	{
		Assert.That(arr.Begin().Distance(arr.End()), Is.EqualTo(4));
	}

	[Test]
	public void AllOf()
	{
		Assert.That(arr.Begin().AllOf(arr.End(), IsIntEven), Is.False);
		Assert.That(arr.Begin().AllOf(arr.End(), IsIntGreaterThan0), Is.True);
	}

	[Test]
	public void AnyOf()
	{
		Assert.That(arr.Begin().AnyOf(arr.End(), IsIntGreaterThan10), Is.False);
		Assert.That(arr.Begin().AnyOf(arr.End(), IsIntEqualTo2), Is.True);
	}

	[Test]
	public void NoneOf()
	{
		Assert.That(arr.Begin().NoneOf(arr.End(), IsIntEqualTo2), Is.False);
		Assert.That(arr.Begin().NoneOf(arr.End(), IsIntGreaterThan10), Is.True);
	}

	[Test]
	public void ForEach()
	{
		int index = 0;
		arr.Begin().ForEach(
			arr.End(),
			i => Assert.That(i, Is.EqualTo(arr[index++]))
		);
	}

	[Test]
	public void Find()
	{
		Assert.That(
			arr.Begin().Find(
				arr.End(),
				2,
				AreIntsEqual).Index,
			Is.EqualTo(1));
	}

	[Test]
	public void FindIf()
	{
		Assert.That(
			arr.Begin().FindIf(
				arr.End(),
				IsIntEven).Index,
			Is.EqualTo(1));
	}

	[Test]
	public void FindIfNot()
	{
		Assert.That(
			arr.Begin().FindIfNot(
				arr.End(),
				IsIntEven).Index,
			Is.EqualTo(0));
	}

	[Test]
	public void FindEnd()
	{
		Assert.That(
			arr.Begin().FindEnd(
				arr.End(),
				arr.IteratorAt(1),
				arr.IteratorAt(2),
				AreIntsEqual
			).Index,
			Is.EqualTo(2)
		);
	}

	[Test]
	public void FindFirstOf()
	{
		Assert.That(
			arr.Begin().FindFirstOf(
				arr.End(),
				arr.IteratorAt(1),
				arr.IteratorAt(2),
				AreIntsEqual
			).Index,
			Is.EqualTo(1)
		);
	}

	[Test]
	public void AdjacentFind()
	{
		Assert.That(
			arr.Begin().AdjacentFind(arr.End(), AreIntsEqual).Index,
			Is.EqualTo(1));
	}

	[Test]
	public void Count()
	{
		Assert.That(
			arr.Begin().Count(arr.End(), 2, AreIntsEqual),
			Is.EqualTo(2));
	}

	[Test]
	public void CountIf()
	{
		Assert.That(arr.Begin().CountIf(arr.End(), IsIntEven), Is.EqualTo(2));
	}

	[Test]
	public void Mismatch()
	{
		NativeArrayIterator<int> mm1;
		NativeArrayIterator<int> mm2;
		arr.Begin().Mismatch(
			arr.End(),
			CreateNativeArray(1, 3, 3, 4).Begin(),
			AreIntsEqual,
			out mm1,
			out mm2
		);
		Assert.That(mm1.GetCurrent(), Is.EqualTo(2));
		Assert.That(mm2.GetCurrent(), Is.EqualTo(3));
	}

	[Test]
	public void Equal()
	{
		Assert.That(
			arr.Begin().Equal(
				arr.End(),
				CreateNativeArray(1, 3, 2, 3).Begin(),
				AreIntsEqual),
			Is.False);
		Assert.That(
			arr.Begin().Equal(
				arr.End(),
				CreateNativeArray(1, 2, 2, 3).Begin(),
				AreIntsEqual),
			Is.True);
	}

	[Test]
	public void IsPermutation()
	{
		Assert.That(
			arr.Begin().IsPermutation(
				arr.End(),
				CreateNativeArray(3, 3, 1, 2).Begin(),
				AreIntsEqual),
			Is.False);
		Assert.That(
			arr.Begin().IsPermutation(
				arr.End(),
				CreateNativeArray(3, 2, 1, 2).Begin(),
				AreIntsEqual),
			Is.True);
	}

	[Test]
	public void Search()
	{
		var sub = CreateNativeArray(2, 3);
		Assert.That(
			arr.Begin().Search(
				arr.End(),
				sub.Begin(),
				sub.End(),
				AreIntsEqual).Index,
			Is.EqualTo(2));
	}

	[Test]
	public void SearchN()
	{
		Assert.That(
			arr.Begin().SearchN(arr.End(), 2, 2, AreIntsEqual).Index,
			Is.EqualTo(1));
	}

	[Test]
	public void Copy()
	{
		Assert.That(
			arr.Begin().Copy(arr.IteratorAt(2), arr2.Begin()).Index,
			Is.EqualTo(2));
		Assert.That(arr2, Is.EqualTo(new[] { 1, 2, 0, 0 }));
	}

	[Test]
	public void CopyN()
	{
		Assert.That(arr.Begin().CopyN(3, arr2.Begin()).Index, Is.EqualTo(3));
		Assert.That(arr2, Is.EqualTo(new[] { 1, 2, 2, 0 }));
	}

	[Test]
	public void CopyIf()
	{
		Assert.That(
			arr.Begin().CopyIf(arr.End(), arr2.Begin(), IsIntEven).Index,
			Is.EqualTo(2));
		Assert.That(arr2, Is.EqualTo(new[] { 2, 2, 0, 0 }));
	}

	[Test]
	public void CopyBackward()
	{
		Assert.That(
			arr.IteratorAt(1).CopyBackward(arr.IteratorAt(3), arr2.End()).Index,
			Is.EqualTo(2));
		Assert.That(arr2, Is.EqualTo(new[] { 0, 0, 2, 2 }));
	}

	[Test]
	public void SwapRanges()
	{
		Assert.That(
			arr.IteratorAt(1).SwapRanges(
				arr.IteratorAt(3),
				arr2.IteratorAt(1)).Index,
			Is.EqualTo(3));
		Assert.That(arr, Is.EqualTo(new[] { 1, 0, 0, 3 }));
		Assert.That(arr2, Is.EqualTo(new[] { 0, 2, 2, 0 }));
	}

	[Test]
	public void Swap()
	{
		var itA = arr.IteratorAt(0);
		var itB = arr.IteratorAt(1);
		itA.Swap(itB);
		Assert.That(arr, Is.EqualTo(new[] { 2, 1, 2, 3 }));
	}

	[Test]
	public void TransformDouble()
	{
		Assert.That(
			arr.Begin().Transform(arr.End(), arr.Begin(), DoubleInt).Index,
			Is.EqualTo(4));
		Assert.That(arr, Is.EqualTo(new[] { 2, 4, 4, 6 }));
	}

	[Test]
	public void TransformMultiply()
	{
		Assert.That(
			arr.Begin().Transform(
				arr.End(),
				arr.Begin(),
				arr.Begin(),
				MultiplyInts).Index,
			Is.EqualTo(4));
		Assert.That(arr, Is.EqualTo(new[] { 1, 4, 4, 9 }));
	}

	[Test]
	public void ReplaceIf()
	{
		arr.Begin().ReplaceIf(arr.End(), IsIntEqualTo2, 20);
		Assert.That(arr, Is.EqualTo(new[] { 1, 20, 20, 3 }));
	}

	[Test]
	public void ReplaceCopyIf()
	{
		Assert.That(
			arr.Begin().ReplaceCopyIf(
				arr.End(),
				arr.Begin(),
				IsIntEven,
				200).Index,
			Is.EqualTo(4));
		Assert.That(arr, Is.EqualTo(new[] { 1, 200, 200, 3 }));
	}

	[Test]
	public void Unique()
	{
		Assert.That(
			arr.Begin().Unique(arr.End(), AreIntsEqual).Index,
			Is.EqualTo(3));
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 3, 3 }));
	}

	[Test]
	public void UniqueCopy()
	{
		Assert.That(
			arr.Begin().UniqueCopy(arr.End(), arr2.Begin(), AreIntsEqual).Index,
			Is.EqualTo(3));
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 2, 3 }));
		Assert.That(arr2, Is.EqualTo(new[] { 1, 2, 3, 0 }));
	}

	[Test]
	public void Reverse()
	{
		arr.Begin().Reverse(arr.End());
		Assert.That(arr, Is.EqualTo(new[] { 3, 2, 2, 1 }));
	}

	[Test]
	public void ReverseCopy()
	{
		Assert.That(
			arr.Begin().ReverseCopy(arr.End(), arr2.Begin()).Index,
			Is.EqualTo(4));
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 2, 3 }));
		Assert.That(arr2, Is.EqualTo(new[] { 3, 2, 2, 1 }));
	}

	[Test]
	public void Rotate()
	{
		arr.Begin().Rotate(arr.IteratorAt(2), arr.End());
		Assert.That(arr, Is.EqualTo(new[] { 2, 3, 1, 2 }));
	}

	[Test]
	public void RotateCopy()
	{
		Assert.That(
			arr.Begin().RotateCopy(
				arr.IteratorAt(2),
				arr.End(),
				arr2.Begin()).Index,
			Is.EqualTo(4));
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 2, 3 }));
		Assert.That(arr2, Is.EqualTo(new[] { 2, 3, 1, 2 }));
	}

	[Test]
	public void RandomShuffle()
	{
		arr.Begin().RandomShuffle(arr.End(), RandomIntLessThan);
		Assert.That(arr, Is.EqualTo(new[] { 2, 3, 2, 1 }));
		arr.Begin().RandomShuffle(arr.End(), RandomIntLessThan);
		Assert.That(arr, Is.EqualTo(new[] { 2, 2, 3, 1 }));
	}

	[Test]
	public void IsPartitioned()
	{
		Assert.That(arr.Begin().IsPartitioned(arr.End(), IsIntEven), Is.False);
		Assert.That(
			arr.Begin().IsPartitioned(arr.End(), IsIntLessThanOrEqualTo2),
			Is.True);
	}

	[Test]
	public void Partition()
	{
		Assert.That(
			arr.Begin().Partition(arr.End(), IsIntEven).Index,
			Is.EqualTo(2));
		Assert.That(arr, Is.EqualTo(new[] { 2, 2, 1, 3 }));
	}

	[Test]
	public void PartitionCopy()
	{
		NativeArrayIterator<int> outResultTrue;
		NativeArrayIterator<int> outResultFalse;
		arr.Begin().PartitionCopy(
			arr.End(),
			arr2.Begin(),
			arr3.Begin(),
			IsIntEven,
			out outResultTrue,
			out outResultFalse
		);
		Assert.That(outResultTrue.Index, Is.EqualTo(2));
		Assert.That(outResultFalse.Index, Is.EqualTo(2));
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 2, 3 }));
		Assert.That(arr2, Is.EqualTo(new[] { 2, 2, 0, 0 }));
		Assert.That(arr3, Is.EqualTo(new[] { 1, 3, 0, 0 }));
	}

	[Test]
	public void PartitionPoint()
	{
		Assert.That(
			arr.Begin().PartitionPoint(
				arr.End(),
				IsIntLessThanOrEqualTo2).Index,
			Is.EqualTo(2));
	}

	[Test]
	public void Sort()
	{
		arr.Begin().Sort(arr.End(), IsIntGreaterThanInt);
		Assert.That(arr, Is.EqualTo(new[] { 3, 2, 2, 1 }));
	}

	[Test]
	public void StableSort()
	{
		arr.Begin().StableSort(arr.End(), IsIntGreaterThanInt);
		Assert.That(arr, Is.EqualTo(new[] { 3, 2, 2, 1 }));
	}

	[Test]
	public void PartialSort()
	{
		arr.Begin().PartialSort(
			arr.IteratorAt(2),
			arr.End(),
			IsIntGreaterThanInt);
		Assert.That(arr, Is.EqualTo(new[] { 3, 2, 2, 1 }));
	}

	[Test]
	public void IsSorted()
	{
		Assert.That(arr.Begin().IsSorted(arr.End(), IsIntLessThanInt), Is.True);
	}

	[Test]
	public void IsSortedUtil()
	{
		Assert.That(
			OneThreeTwoFour.Begin().IsSortedUntil(
				OneThreeTwoFour.End(),
				IsIntLessThanInt).Index,
			Is.EqualTo(2));
	}

	[Test]
	public void NthElement()
	{
		OneThreeTwoFour.Begin().Copy(OneThreeTwoFour.End(), arr.Begin());
		arr.Begin().NthElement(arr.IteratorAt(2), arr.End(), IsIntLessThanInt);
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 3, 4 }));
	}

	[Test]
	public void LowerBound()
	{
		Assert.That(
			arr.Begin().LowerBound(arr.End(), 2, IsIntLessThanInt).Index,
			Is.EqualTo(1));
	}

	[Test]
	public void UpperBound()
	{
		Assert.That(
			arr.Begin().UpperBound(arr.End(), 2, IsIntLessThanInt).Index,
			Is.EqualTo(3));
	}

	[Test]
	public void EqualRange()
	{
		NativeArrayIterator<int> equalRangeLower;
		NativeArrayIterator<int> equalRangeUpper;
		arr.Begin().EqualRange(
			arr.End(),
			2,
			IsIntLessThanInt,
			out equalRangeLower,
			out equalRangeUpper
		);
		Assert.That(equalRangeLower.Index, Is.EqualTo(1));
		Assert.That(equalRangeUpper.Index, Is.EqualTo(3));
	}

	[Test]
	public void BinarySearch()
	{
		Assert.That(
			arr.Begin().BinarySearch(arr.End(), 2, IsIntLessThanInt),
			Is.True);
		Assert.That(
			arr.Begin().BinarySearch(arr.End(), 9, IsIntLessThanInt),
			Is.False);
	}

	[Test]
	public void Merge()
	{
		Assert.That(
			arr.Begin().Merge(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index,
			Is.EqualTo(8));
		Assert.That(arrLong, Is.EqualTo(new[] { 1, 1, 2, 2, 3, 3, 3, 4 }));
	}

	[Test]
	public void InplaceMerge()
	{
		NativeArrayIterator<int> copyResult = arr.Begin().Copy(
			arr.End(),
			arrLong.Begin());
		OneThreeThreeFour.Begin().Copy(OneThreeThreeFour.End(), copyResult);
		arrLong.Begin().InplaceMerge(
			copyResult,
			arrLong.End(),
			IsIntLessThanInt);
		Assert.That(arrLong, Is.EqualTo(new[] { 1, 1, 2, 2, 3, 3, 3, 4 }));
	}

	[Test]
	public void Includes()
	{
		Assert.That(
			arr.Begin().Includes(
				arr.End(),
				TwoThree.Begin(),
				TwoThree.End(),
				IsIntLessThanInt),
			Is.True);
	}

	[Test]
	public void SetUnion()
	{
		Assert.That(
			arr.Begin().SetUnion(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index,
			Is.EqualTo(6));
		Assert.That(arrLong, Is.EqualTo(new[] { 1, 2, 2, 3, 3, 4, 0, 0 }));
	}

	[Test]
	public void SetIntersection()
	{
		Assert.That(
			arr.Begin().SetIntersection(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index,
			Is.EqualTo(2));
		Assert.That(arrLong, Is.EqualTo(new[] { 1, 3, 0, 0, 0, 0, 0, 0 }));
	}

	[Test]
	public void SetDifference()
	{
		Assert.That(
			arr.Begin().SetDifference(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index,
			Is.EqualTo(2));
		Assert.That(arrLong, Is.EqualTo(new[] { 2, 2, 0, 0, 0, 0, 0, 0 }));
	}

	[Test]
	public void SetSymmetricDifference()
	{
		Assert.That(
			arr.Begin().SetSymmetricDifference(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index,
			Is.EqualTo(4));
		Assert.That(arrLong, Is.EqualTo(new[] { 2, 2, 3, 4, 0, 0, 0, 0 }));
	}

	[Test]
	public void PushHeap()
	{
		NativeArrayIterator<int> pushHeapIt = FourTwoThreeOne.Begin().Copy(
			FourTwoThreeOne.End(),
			arrLong.Begin());
		pushHeapIt.SetCurrent(5);
		pushHeapIt = pushHeapIt.GetNext();
		arrLong.Begin().PushHeap(pushHeapIt, IsIntLessThanInt);
		Assert.That(arrLong, Is.EqualTo(new[] { 5, 4, 3, 1, 2, 0, 0, 0 }));
	}

	[Test]
	public void PopHeap()
	{
		FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arr2.Begin());
		arr2.Begin().PopHeap(arr2.End(), IsIntLessThanInt);
		Assert.That(arr2, Is.EqualTo(new[] { 3, 2, 1, 4 }));
	}

	[Test]
	public void MakeHeap()
	{
		OneTwoThreeFour.Begin().Copy(OneTwoThreeFour.End(), arr2.Begin());
		arr2.Begin().MakeHeap(arr2.End(), IsIntLessThanInt);
		Assert.That(arr2, Is.EqualTo(new[] { 4, 2, 3, 1 }));
	}

	[Test]
	public void SortHeap()
	{
		FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arr2.Begin());
		arr2.Begin().SortHeap(arr2.End(), IsIntLessThanInt);
		Assert.That(arr2, Is.EqualTo(new[] { 1, 2, 3, 4 }));
	}

	[Test]
	public void IsHeap()
	{
		Assert.That(
			FourTwoThreeOne.Begin().IsHeap(
				FourTwoThreeOne.End(),
				IsIntLessThanInt), Is.True);
	}

	[Test]
	public void IsHeapUntil()
	{
		Assert.That(
			FourTwoThreeOne.Begin().IsHeapUntil(
				FourTwoThreeOne.End(),
				IsIntLessThanInt).Index,
			Is.EqualTo(4));
	}

	[Test]
	public void MinElement()
	{
		Assert.That(
			arr.Begin().MinElement(arr.End(), IsIntLessThanInt).Index,
			Is.EqualTo(0));
	}

	[Test]
	public void MaxElement()
	{
		Assert.That(
			arr.Begin().MaxElement(arr.End(), IsIntLessThanInt).Index,
			Is.EqualTo(3));
	}

	[Test]
	public void MinMaxElement()
	{
		NativeArrayIterator<int> minIt;
		NativeArrayIterator<int> maxIt;
		arr.Begin().MinMaxElement(
			arr.End(),
			IsIntLessThanInt,
			out minIt,
			out maxIt);
		Assert.That(minIt.Index, Is.EqualTo(0));
		Assert.That(maxIt.Index, Is.EqualTo(3));
	}

	[Test]
	public void LexicographicCompare()
	{
		Assert.That(
			arr.Begin().LexicographicalCompare(
				arr.End(),
				arr2.Begin(),
				arr2.End(),
				IsIntLessThanInt),
			Is.False);
		Assert.That(
			arr2.Begin().LexicographicalCompare(
				arr2.End(),
				arr.Begin(),
				arr.End(),
				IsIntLessThanInt),
			Is.True);
	}

	[Test]
	public void NextPermutation()
	{
		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 3, 2 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 1, 3, 2, 2 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 2, 1, 2, 3 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 2, 1, 3, 2 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 2, 2, 1, 3 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 2, 2, 3, 1 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 2, 3, 1, 2 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 2, 3, 2, 1 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 3, 1, 2, 2 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 3, 2, 1, 2 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.True);
		Assert.That(arr, Is.EqualTo(new[] { 3, 2, 2, 1 }));

		Assert.That(
			arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt),
			Is.False);
		Assert.That(arr, Is.EqualTo(new[] { 1, 2, 2, 3 }));
	}

	[Test]
	public void PrevPermutation()
	{
		Assert.That(
			ThreeTwoOne.Begin().PrevPermutation(
				ThreeTwoOne.End(),
				IsIntLessThanInt),
			Is.True);
		Assert.That(ThreeTwoOne, Is.EqualTo(new[] { 3, 1, 2 }));


		Assert.That(
			ThreeTwoOne.Begin().PrevPermutation(
				ThreeTwoOne.End(),
				IsIntLessThanInt),
			Is.True);
		Assert.That(ThreeTwoOne, Is.EqualTo(new[] { 2, 3, 1 }));

		Assert.That(
			ThreeTwoOne.Begin().PrevPermutation(
				ThreeTwoOne.End(),
				IsIntLessThanInt),
			Is.True);
		Assert.That(ThreeTwoOne, Is.EqualTo(new[] { 2, 1, 3 }));


		Assert.That(
			ThreeTwoOne.Begin().PrevPermutation(
				ThreeTwoOne.End(),
				IsIntLessThanInt),
			Is.True);
		Assert.That(ThreeTwoOne, Is.EqualTo(new[] { 1, 3, 2 }));


		Assert.That(
			ThreeTwoOne.Begin().PrevPermutation(
				ThreeTwoOne.End(),
				IsIntLessThanInt),
			Is.True);
		Assert.That(ThreeTwoOne, Is.EqualTo(new[] { 1, 2, 3 }));

		Assert.That(
			ThreeTwoOne.Begin().PrevPermutation(
				ThreeTwoOne.End(),
				IsIntLessThanInt),
			Is.False);
		Assert.That(ThreeTwoOne, Is.EqualTo(new[] { 3, 2, 1 }));
	}
}

#endif