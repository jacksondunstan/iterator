using System;
using System.Linq;

using UnityEngine;

public class TestScript : MonoBehaviour
{
	private static readonly System.Random random = new System.Random();
	private static readonly int[] defaultArr = { 1, 2, 2, 3 };
	private static readonly int[] arr = defaultArr.ToArray();
	private static readonly int[] arr2 = new int[arr.Length];
	private static readonly int[] arr3 = new int[arr.Length];
	private static readonly int[] arrLong = new int[arr.Length*2];
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
	private static readonly Func<int, int> RandomIntLessThan = max => random.Next(max);
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
		GcTest();
		Debug.Log(Test());
	}

	string Test()
	{
		var log = "";
		Action<string> Log = msg => log += msg + "\n";
		Func<int[], string> ArrayToString = a => "[" + string.Join(
			", ",
			a.Select(i => i.ToString()).ToArray()
		) + "]";
		Action ResetArrays = () => {
			for (var i = 0; i < arr.Length; ++i)
			{
				arr[i] = defaultArr[i];
			}
			for (var i = 0; i < arr2.Length; ++i)
			{
				arr2[i] = 0;
			}
			for (var i = 0; i < arr3.Length; ++i)
			{
				arr3[i] = 0;
			}
			for (var i = 0; i < arrLong.Length; ++i)
			{
				arrLong[i] = 0;
			}
		};

		// GetAdvanced
		Log("advance 1 from begin: " + arr.Begin().GetAdvanced(1).GetCurrent());

		// Distance
		Log("distance from first to last: " + arr.Begin().Distance(arr.End()));

		// AllOf
		Log("all even?: " + arr.Begin().AllOf(arr.End(), IsIntEven));
		Log("all positive?: " + arr.Begin().AllOf(arr.End(), IsIntGreaterThan0));

		// AnyOf
		Log("any > 10?: " + arr.Begin().AnyOf(arr.End(), IsIntGreaterThan10));
		Log("any == 2?: " + arr.Begin().AnyOf(arr.End(), IsIntEqualTo2));

		// NoneOf
		Log("none == 2?: " + arr.Begin().NoneOf(arr.End(), IsIntEqualTo2));
		Log("none > 10?: " + arr.Begin().NoneOf(arr.End(), IsIntGreaterThan10));

		// ForEach
		log += "foreach: ";
		arr.Begin().ForEach(arr.End(), i => log += i + " ");
		log += "\n";

		// Find
		Log("find 2 at index: " + arr.Begin().Find(arr.End(), 2, AreIntsEqual).Index);

		// FindIf
		Log("first even at index: " + arr.Begin().FindIf(arr.End(), IsIntEven).Index);

		// FindIfNot
		Log("first not even at index: " + arr.Begin().FindIfNot(arr.End(), IsIntEven).Index);

		// FindEnd
		Log(
			"end of [2,3] at index: "
			+ arr.Begin().FindEnd(
				arr.End(),
				arr.IteratorAt(1),
				arr.IteratorAt(2),
				AreIntsEqual
			).Index
		);

		// FindFirstOf
		Log(
			"first of [2,3] at index: "
			+ arr.Begin().FindFirstOf(
				arr.End(),
				arr.IteratorAt(1),
				arr.IteratorAt(2),
				AreIntsEqual
			).Index
		);

		// AdjacentFind
		Log("adjacents at index: " + arr.Begin().AdjacentFind(arr.End(), AreIntsEqual).Index);

		// Count
		Log("count 2s: " + arr.Begin().Count(arr.End(), 2, AreIntsEqual));

		// CountIf
		Log("count evens: " + arr.Begin().CountIf(arr.End(), IsIntEven));

		// Mismatch
		ArrayIterator<int> mm1;
		ArrayIterator<int> mm2;
		arr.Begin().Mismatch(
			arr.End(),
			new[] { 1, 3, 3, 4 }.Begin(),
			AreIntsEqual,
			out mm1,
			out mm2
		);
		Log("mismatch with [1, 3, 3, 4] at: " + mm1.GetCurrent() + ", " + mm2.GetCurrent());

		// Equal
		Log(
			"equal [1, 3, 2, 3]? "
			+ arr.Begin().Equal(arr.End(), new[] { 1, 3, 2, 3 }.Begin(), AreIntsEqual)
		);
		Log(
			"equal [1, 2, 2, 3]? "
			+ arr.Begin().Equal(arr.End(), new[] { 1, 2, 2, 3 }.Begin(), AreIntsEqual)
		);

		// IsPermutation
		Log(
			"is permutation of [3, 3, 1, 2]? "
			+ arr.Begin().IsPermutation(arr.End(), new[] { 3, 3, 1, 2 }.Begin(), AreIntsEqual)
		);
		Log(
			"is permutation of [3, 2, 1, 2]? "
			+ arr.Begin().IsPermutation(arr.End(), new[] { 3, 2, 1, 2 }.Begin(), AreIntsEqual)
		);

		// Search
		var sub = new[] { 2, 3 };
		Log(
			"search found [2, 3] at index: "
			+ arr.Begin().Search(arr.End(), sub.Begin(), sub.End(), AreIntsEqual).Index
		);

		// SearchN
		Log(
			"search 2 2s found at index: "
			+ arr.Begin().SearchN(arr.End(), 2, 2, AreIntsEqual).Index
		);

		// Copy
		ResetArrays();
		Log(
			"copy first two returns iterator at index "
			+ arr.Begin().Copy(arr.IteratorAt(2), arr2.Begin()).Index
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// CopyN
		ResetArrays();
		Log(
			"copyN first three returns iterator at index "
			+ arr.Begin().CopyN(3, arr2.Begin()).Index
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// CopyIf
		ResetArrays();
		Log(
			"copy evens returns iterator at index "
			+ arr.Begin().CopyIf(arr.End(), arr2.Begin(), IsIntEven).Index
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// CopyBackward
		ResetArrays();
		Log(
			"copy backward middle two returns iterator at index "
			+ arr.IteratorAt(1).CopyBackward(arr.IteratorAt(3), arr2.End()).Index
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// SwapRanges
		ResetArrays();
		Log(
			"swap middle two returns iterator at index "
			+ arr.IteratorAt(1).SwapRanges(arr.IteratorAt(3), arr2.IteratorAt(1)).Index
			+ ", arr now: "
			+ ArrayToString(arr)
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// Swap
		ResetArrays();
		var itA = arr.IteratorAt(0);
		var itB = arr.IteratorAt(1);
		itA.Swap(itB);
		Log("swap iterator at index 0 with iterator at index 1, arr now: " + ArrayToString(arr));

		// Transform
		ResetArrays();
		Log(
			"transform by doubling returns index "
			+ arr.Begin().Transform(arr.End(), arr.Begin(), DoubleInt).Index
			+ ", arr now: "
			+ ArrayToString(arr)
		);

		// Transform
		ResetArrays();
		Log(
			"transform by multiplying with self returns index "
			+ arr.Begin().Transform(arr.End(), arr.Begin(), arr.Begin(), MultiplyInts).Index
			+ ", arr now: "
			+ ArrayToString(arr)
		);

		// ReplaceIf
		ResetArrays();
		arr.Begin().ReplaceIf(arr.End(), IsIntEqualTo2, 20);
		Log("replace 2 with 20 " + ArrayToString(arr));

		// ReplaceCopyIf
		ResetArrays();
		Log(
			"replace evens with 200 returns iterator at index "
			+ arr.Begin().ReplaceCopyIf(arr.End(), arr.Begin(), IsIntEven, 200).Index
			+ ", arr now: "
			+ ArrayToString(arr)
		);

		// Unique
		ResetArrays();
		Log(
			"unique returns index "
			+ arr.Begin().Unique(arr.End(), AreIntsEqual).Index
			+ ", arr now: "
			+ ArrayToString(arr)
		);

		// UniqueCopy
		ResetArrays();
		Log(
			"unique copy returns index "
			+ arr.Begin().UniqueCopy(arr.End(), arr2.Begin(), AreIntsEqual).Index
			+ ", arr now: "
			+ ArrayToString(arr)
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// Reverse
		ResetArrays();
		arr.Begin().Reverse(arr.End());
		Log("reverse: " + ArrayToString(arr));

		// ReverseCopy
		ResetArrays();
		Log(
			"reverse copy returns index: "
			+ arr.Begin().ReverseCopy(arr.End(), arr2.Begin()).Index
			+ ", arr now: "
			+ ArrayToString(arr)
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// Rotate
		ResetArrays();
		arr.Begin().Rotate(arr.IteratorAt(2), arr.End());
		Log("rotate around second 2, arr now: " + ArrayToString(arr));

		// RotateCopy
		ResetArrays();
		Log(
			"rotate copy around second 2 returns index: "
			+ arr.Begin().RotateCopy(arr.IteratorAt(2), arr.End(), arr2.Begin()).Index
			+ ", arr now: "
			+ ArrayToString(arr)
			+ ", arr2 now: "
			+ ArrayToString(arr2)
		);

		// RandomShuffle
		ResetArrays();
		arr.Begin().RandomShuffle(arr.End(), RandomIntLessThan);
		arr.Begin().Copy(arr.End(), arr2.Begin());
		arr2.Begin().RandomShuffle(arr2.End(), RandomIntLessThan);
		Log("some random shuffles: " + ArrayToString(arr) + ", " + ArrayToString(arr2));

		// IsPartitioned
		ResetArrays();
		Log(
			"arr is partitioned by odd/even? "
			+ arr.Begin().IsPartitioned(arr.End(), IsIntEven)
			+ ", by <= 2? "
			+ arr.Begin().IsPartitioned(arr.End(), IsIntLessThanOrEqualTo2)
		);

		// Partition
		ResetArrays();
		Log(
			"partition by odd/even returns index: "
			+ arr.Begin().Partition(arr.End(), IsIntEven).Index
			+ ", arr now: "
			+ ArrayToString(arr)
		);

		// PartitionCopy
		ResetArrays();
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
		Log(
			"partition copy by odd/even returns indexes: "
			+ outResultTrue.Index
			+ ", "
			+ outResultFalse.Index
			+ ", arr now: "
			+ ArrayToString(arr)
			+ ", arr2 now: "
			+ ArrayToString(arr2)
			+ ", arr3 now: "
			+ ArrayToString(arr3)
		);

		// PartitionPoint
		ResetArrays();
		Log(
			"partition point for <= 2 returns index: "
			+ arr.Begin().PartitionPoint(arr.End(), IsIntLessThanOrEqualTo2).Index
		);

		// Sort
		ResetArrays();
		arr.Begin().Sort(arr.End(), IsIntGreaterThanInt);
		Log("sorted backwards: " + ArrayToString(arr));

		// StableSort
		ResetArrays();
		arr.Begin().StableSort(arr.End(), IsIntGreaterThanInt);
		Log("stable sorted backwards: " + ArrayToString(arr));

		// PartialSort
		ResetArrays();
		arr.Begin().PartialSort(arr.IteratorAt(2), arr.End(), IsIntGreaterThanInt);
		Log("partial sorted backwards up to index 2: " + ArrayToString(arr));

		// IsSorted
		ResetArrays();
		Log("is array sorted: " + arr.Begin().IsSorted(arr.End(), IsIntLessThanInt));

		// IsSorted
		ResetArrays();
		Log(
			ArrayToString(OneThreeTwoFour)
			+ " is sorted until: "
			+ OneThreeTwoFour.Begin().IsSortedUntil(OneThreeTwoFour.End(), IsIntLessThanInt).Index
		);

		// NthElement
		ResetArrays();
		OneThreeTwoFour.Begin().Copy(OneThreeTwoFour.End(), arr.Begin());
		arr.Begin().NthElement(arr.IteratorAt(2), arr.End(), IsIntLessThanInt);
		Log(
			"NthElement 2 on "
			+ ArrayToString(OneThreeTwoFour)
			+ ": "
			+ ArrayToString(arr)
		);

		// LowerBound
		ResetArrays();
		Log(
			"lower bound of 2 at index: "
			+ arr.Begin().LowerBound(arr.End(), 2, IsIntLessThanInt).Index
		);

		// UpperBound
		ResetArrays();
		Log(
			"upper bound of 2 at index: "
			+ arr.Begin().UpperBound(arr.End(), 2, IsIntLessThanInt).Index
		);

		// EqualRange
		ResetArrays();
		ArrayIterator<int> equalRangeLower;
		ArrayIterator<int> equalRangeUpper;
		arr.Begin().EqualRange(
			arr.End(),
			2,
			IsIntLessThanInt,
			out equalRangeLower,
			out equalRangeUpper
		);
		Log(
			"equal range of 2 from index "
			+ equalRangeLower.Index
			+ " to "
			+ equalRangeUpper.Index
		);

		// BinarySearch
		ResetArrays();
		Log("binary search finds 2? " + arr.Begin().BinarySearch(arr.End(), 2, IsIntLessThanInt));
		Log("binary search finds 9? " + arr.Begin().BinarySearch(arr.End(), 9, IsIntLessThanInt));

		// Merge
		ResetArrays();
		Log(
			ArrayToString(arr)
			+ " merged with "
			+ ArrayToString(OneThreeThreeFour)
			+ " returns " +
			arr.Begin().Merge(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index
			+ " result: "
			+ ArrayToString(arrLong)
		);

		// InplaceMerge
		ResetArrays();
		var copyResult = arr.Begin().Copy(arr.End(), arrLong.Begin());
		OneThreeThreeFour.Begin().Copy(OneThreeThreeFour.End(), copyResult);
		var inplaceMergeMsg = ArrayToString(arrLong) + " in-place merged: ";
		arrLong.Begin().InplaceMerge(copyResult, arrLong.End(), IsIntLessThanInt);
		Log(inplaceMergeMsg + ArrayToString(arrLong));

		// Includes
		ResetArrays();
		Log(
			ArrayToString(arr)
			+ " includes "
			+ ArrayToString(TwoThree)
			+ "? " +
			arr.Begin().Includes(arr.End(), TwoThree.Begin(), TwoThree.End(), IsIntLessThanInt)
		);

		// SetUnion
		ResetArrays();
		Log(
			"union of "
			+ ArrayToString(arr)
			+ " and "
			+ ArrayToString(OneThreeThreeFour)
			+ " returns index "
			+ arr.Begin().SetUnion(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index
			+ " result: "
			+ ArrayToString(arrLong)
		);

		// SetUnion
		ResetArrays();
		Log(
			"intersection of "
			+ ArrayToString(arr)
			+ " and "
			+ ArrayToString(OneThreeThreeFour)
			+ " returns index "
			+ arr.Begin().SetIntersection(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index
			+ " result: "
			+ ArrayToString(arrLong)
		);

		// SetDifference
		ResetArrays();
		Log(
			"difference of "
			+ ArrayToString(arr)
			+ " and "
			+ ArrayToString(OneThreeThreeFour)
			+ " returns index "
			+ arr.Begin().SetDifference(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index
			+ " result: "
			+ ArrayToString(arrLong)
		);

		// SetSymmetricDifference
		ResetArrays();
		Log(
			"symmetric difference of "
			+ ArrayToString(arr)
			+ " and "
			+ ArrayToString(OneThreeThreeFour)
			+ " returns index "
			+ arr.Begin().SetSymmetricDifference(
				arr.End(),
				OneThreeThreeFour.Begin(),
				OneThreeThreeFour.End(),
				arrLong.Begin(),
				IsIntLessThanInt
			).Index
			+ " result: "
			+ ArrayToString(arrLong)
		);

		// PushHeap
		ResetArrays();
		var pushHeapIt = FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arrLong.Begin());
		pushHeapIt.SetCurrent(5);
		pushHeapIt = pushHeapIt.GetNext();
		var pushHeapMsg = "push heap of " + ArrayToString(arrLong);
		arrLong.Begin().PushHeap(pushHeapIt, IsIntLessThanInt);
		Log(pushHeapMsg + ": " + ArrayToString(arrLong));

		// PopHeap
		ResetArrays();
		FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arr2.Begin());
		var popHeapMsg = "pop heap of " + ArrayToString(arr2);
		arr2.Begin().PopHeap(arr2.End(), IsIntLessThanInt);
		Log(popHeapMsg + ": " + ArrayToString(arr2));

		// MakeHeap
		ResetArrays();
		OneTwoThreeFour.Begin().Copy(OneTwoThreeFour.End(), arr2.Begin());
		var makeHeapMsg = "make heap of " + ArrayToString(arr2);
		arr2.Begin().MakeHeap(arr2.End(), IsIntLessThanInt);
		Log(makeHeapMsg + ": " + ArrayToString(arr2));

		// SortHeap
		ResetArrays();
		FourTwoThreeOne.Begin().Copy(FourTwoThreeOne.End(), arr2.Begin());
		var sortHeapMsg = "sort heap of " + ArrayToString(arr2);
		arr2.Begin().SortHeap(arr2.End(), IsIntLessThanInt);
		Log(sortHeapMsg + ": " + ArrayToString(arr2));

		// IsHeap
		ResetArrays();
		Log(
			ArrayToString(FourTwoThreeOne)
			+ " is heap? "
			+ FourTwoThreeOne.Begin().IsHeap(FourTwoThreeOne.End(), IsIntLessThanInt)
		);

		// IsHeapUntil
		ResetArrays();
		Log(
			ArrayToString(FourTwoThreeOne)
			+ " is heap until "
			+ FourTwoThreeOne.Begin().IsHeapUntil(FourTwoThreeOne.End(), IsIntLessThanInt).Index
		);

		// MinElement
		ResetArrays();
		Log(
			"min element: "
			+ arr.Begin().MinElement(arr.End(), IsIntLessThanInt).Index
		);

		// MaxElement
		ResetArrays();
		Log(
			"max element: "
			+ arr.Begin().MaxElement(arr.End(), IsIntLessThanInt).Index
		);

		// MinMaxElement
		ArrayIterator<int> minIt;
		ArrayIterator<int> maxIt;
		arr.Begin().MinMaxElement(arr.End(), IsIntLessThanInt, out minIt, out maxIt);
		ResetArrays();
		Log(
			"min/max elements: "
			+ minIt.Index
			+ ", "
			+ maxIt.Index
		);

		// LexicographicCompare
		ResetArrays();
		Log(
			"lexicographical compare of "
			+ ArrayToString(arr)
			+ " and "
			+ ArrayToString(arr2)
			+ ": "
			+ arr.Begin().LexicographicalCompare(
				arr.End(),
				arr2.Begin(),
				arr2.End(),
				IsIntLessThanInt
			)
			+ ", reverse: "
			+ arr2.Begin().LexicographicalCompare(
				arr2.End(),
				arr.Begin(),
				arr.End(),
				IsIntLessThanInt
			)
		);

		// NextPermutation
		ResetArrays();
		var nextPermutationMsg = "permutations of " + ArrayToString(arr) + ":";
		while (arr.Begin().NextPermutation(arr.End(), IsIntLessThanInt))
		{
			nextPermutationMsg += "\n    " + ArrayToString(arr);
		}
		Log(nextPermutationMsg);

		// PrevPermutation
		ResetArrays();
		var prevPermutationMsg = "prev permutations of " + ArrayToString(ThreeTwoOne) + ":";
		while (ThreeTwoOne.Begin().PrevPermutation(ThreeTwoOne.End(), IsIntLessThanInt))
		{
			prevPermutationMsg += "\n    " + ArrayToString(ThreeTwoOne);
		}
		Log(prevPermutationMsg);

		return log;
	}

	void GcTest()
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