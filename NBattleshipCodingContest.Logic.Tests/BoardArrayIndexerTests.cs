namespace NBattleshipCodingContest.Logic.Tests
{
    // Note status using to make code shorter and better readable.

    using System;
    using Xunit;
    using static NBattleshipCodingContest.Logic.BoardArrayIndexer;

    public class BoardArrayIndexerTests
    {
        [Fact] public void Numeric_Invalid_Col() => Assert.Throws<ArgumentException>("col", () => GetIndex(10, 0));

        [Fact] public void Numeric_Invalid_Row() => Assert.Throws<ArgumentException>("row", () => GetIndex(0, 10));

        [Fact] public void Numeric_First() => Assert.Equal(1, GetIndex(1, 0));

        [Fact] public void Numeric_Last() => Assert.Equal(98, GetIndex(8, 9));

        [Fact] public void Invalid_Col() => Assert.Throws<ArgumentException>("col", () => GetIndex('K', 0));

        [Fact] public void Invalid_Row() => Assert.Throws<ArgumentException>("row", () => GetIndex('A', 11));

        [Fact] public void First() => Assert.Equal(1, GetIndex('B', 1));

        [Fact] public void Last() => Assert.Equal(98, GetIndex('I', 10));

        [Fact] public void Location_InvalidCol() => Assert.Throws<ArgumentException>("col", () => GetIndex("K1"));

        [Fact] public void Location_InvalidRow() => Assert.Throws<ArgumentException>("row", () => GetIndex("A11"));

        [Fact] public void Location_Two_Digits() => Assert.Equal(0, GetIndex("A1"));

        [Fact] public void Location_Three_Digits() => Assert.Equal(90, GetIndex("A10"));
    }
}
