using NBattleshipCodingContest.Logic;
using System;
using System.Threading.Tasks;
using Xunit;

namespace NBattleshipCodingContest.Players.Tests
{
    public class SequentialTests
    {
        [Fact]
        public void ThrowIfTooManyShots()
        {
            var player = new Sequential();
            for (var i = 0; i < 100; i++)
            {
                player.GetShot(null!, _ => Task.FromResult(SquareContent.Water));
            }

            Assert.Throws<InvalidOperationException>(() => player.GetShot(null!, _ => Task.FromResult(SquareContent.Water)));
        }

        [Fact]
        public void SequentialShots()
        {
            var player = new Sequential();
            Assert.Equal("A1", Shoot(player));
            Assert.Equal("B1", Shoot(player));
            for (var i = 0; i < 8; i++)
            {
                Shoot(player);
            }

            Assert.Equal("A2", Shoot(player));
        }

        private string Shoot(PlayerBase player)
        {
            string? location = null;
            Task<SquareContent> shoot(string l)
            {
                location = l;
                return Task.FromResult(SquareContent.Water);
            }

            player.GetShot(null!, shoot);
            Assert.NotNull(location);
            return location!;
        }
    }
}
