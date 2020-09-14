﻿namespace NBattleshipCodingContest.Logic.Tests
{
    using System;
    using Xunit;

    public class BoardIndexTests
    {
        [Fact] public void SingleIndex_Invalid() => Assert.Throws<ArgumentOutOfRangeException>("index", () => new BoardIndex(100));

        [Fact] public void SingleIndex_First() => Assert.Equal(0, new BoardIndex(0));

        [Fact] public void SingleIndex_Last() => Assert.Equal(99, new BoardIndex(99));

        [Fact] public void Numeric_Invalid_Col() => Assert.Throws<ArgumentOutOfRangeException>("col", () => new BoardIndex(10, 0));

        [Fact] public void Numeric_Invalid_Row() => Assert.Throws<ArgumentOutOfRangeException>("row", () => new BoardIndex(0, 10));

        [Fact] public void Numeric_First() => Assert.Equal(1, new BoardIndex(1, 0));

        [Fact] public void Numeric_Last() => Assert.Equal(98, new BoardIndex(8, 9));

        [Fact] public void Invalid_Col() => Assert.Throws<ArgumentOutOfRangeException>("col", () => new BoardIndex('K', 0));

        [Fact] public void Invalid_Row() => Assert.Throws<ArgumentOutOfRangeException>("row", () => new BoardIndex('A', 11));

        [Fact] public void First() => Assert.Equal(1, new BoardIndex('B', 1));

        [Fact] public void Last() => Assert.Equal(98, new BoardIndex('I', 10));

        [Fact] public void Location_InvalidCol() => Assert.Throws<ArgumentOutOfRangeException>("location", () => new BoardIndex("K1"));

        [Fact] public void Location_InvalidRow() => Assert.Throws<ArgumentOutOfRangeException>("location", () => new BoardIndex("A11"));

        [Fact] public void Location_Two_Digits() => Assert.Equal(0, new BoardIndex("A1"));

        [Fact] public void Location_Three_Digits() => Assert.Equal(90, new BoardIndex("A10"));

        [Fact] public void String_Conversion() => Assert.Equal("A1", new BoardIndex(0));

        [Fact]
        public void Deconstruct()
        {
            var (col, row) = new BoardIndex(0);
            Assert.Equal(0, col);
            Assert.Equal(0, row);
        }

        [Fact] public void Equals_Index() => Assert.Equal(new BoardIndex(0), new BoardIndex(0));

        [Fact] public void Equals_Operator() => Assert.True(new BoardIndex(0) == new BoardIndex(0));

        [Fact] public void Not_Equals_Operator() => Assert.True(new BoardIndex(0) != new BoardIndex(1));

        [Fact] public void HashCode() => Assert.True(new BoardIndex(0).GetHashCode() == new BoardIndex(0).GetHashCode());

        [Fact] public void NextColumn() => Assert.Equal(1, new BoardIndex(0).NextColumn());

        [Fact] public void NextColumn_Invalid() => Assert.Throws<InvalidOperationException>(() => new BoardIndex(9).NextColumn());

        [Fact] public void NextRow() => Assert.Equal(10, new BoardIndex(0).NextRow());

        [Fact] public void NextRow_Invalid() => Assert.Throws<InvalidOperationException>(() => new BoardIndex(90).NextRow());
    }
}
