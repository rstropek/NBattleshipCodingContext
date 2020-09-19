namespace NBattleshipCodingContest.Logic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// <see cref="BattleshipBoard"/> implements a board for a battleship game.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Methods reading data are thread-safe, methods writing data are not.
    /// </para>
    /// </remarks>
    public class BoardContent : IReadOnlyBoard
    {
        private readonly SquareContent[] boardContent;

        public BoardContent()
        {
            boardContent = new SquareContent[10 * 10];
        }

        public BoardContent(IEnumerable<byte> content)
        {
            if (content.Count() != 10 * 10 || content.Any(c => c is < 0 or > (byte)SquareContent.Unknown))
            {
                throw new ArgumentOutOfRangeException(nameof(content));
            }

            boardContent = content.Cast<SquareContent>().ToArray();
        }

        /// <summary>
        /// Set all squares of the board to a given content
        /// </summary>
        /// <param name="content">Square content that should be written to all squares</param>
        public void Clear(SquareContent content)
        {
            for (var i = 0; i < 10 * 10; i++)
            {
                boardContent[i] = content;
            }
        }

        /// <inheritdoc/>
        public SquareContent this[BoardIndex ix]
        {
            get => boardContent[ix];
            set => boardContent[ix] = value;
        }

        /// <inheritdoc/>
        public int Count => 10 * 10;

        /// <inheritdoc/>
        public SquareContent this[int location]
        {
            get => this[new BoardIndex(location)];
            set => this[new BoardIndex(location)] = value;
        }

        /// <inheritdoc/>
        public IEnumerator<SquareContent> GetEnumerator()
        {
            foreach (var square in boardContent)
            {
                yield return square;
            }
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Indicates if the player of this board has lost.
        /// </summary>
        /// <remarks>
        /// Lost means that all ship squares were hit by shots.
        /// </remarks>
        /// <seealso cref="ShootAt(int, int)"/>
        public bool HasLost => this.Count(s => s == SquareContent.HitShip) == 5 + 4 + 3 + 3 + 2;

        /// <inheritdoc/>
        public override string ToString()
        {
            // Note string.Create. Read more at
            // https://docs.microsoft.com/en-us/dotnet/api/system.string.create

            static string BuildSeparator(string chars) =>
                string.Create(1 + 10 * 2 + 9 + 1 + 1, chars, (buf, sepChars) =>
                {
                    var i = 0;
                    buf[i++] = sepChars[0];
                    for (var j = 0; j < 10 - 1; j++)
                    {
                        buf[i++] = sepChars[1];
                        buf[i++] = sepChars[1];
                        buf[i++] = sepChars[2];
                    }

                    buf[i++] = sepChars[1];
                    buf[i++] = sepChars[1];
                    buf[i++] = sepChars[3];

                    buf[i++] = '\n';
                });

            var top = BuildSeparator("┏━┯┓");
            var middle = BuildSeparator("┠─┼┨");
            var bottom = BuildSeparator("┗━┷┛");

            return string.Create((1 + 10 + 9 + 1) * top.Length, (top, middle, bottom), (buf, seps) =>
            {
                var origBuf = buf;
                ((ReadOnlySpan<char>)seps.top).CopyTo(buf);
                buf = buf[seps.top.Length..];
                for (var row = 0; row < 10; row++)
                {
                    buf[0] = '┃';
                    buf = buf[1..];
                    for (var col = 0; col < 10; col++)
                    {
                        switch (boardContent[new BoardIndex(col, row)])
                        {
                            case SquareContent.HitShip:
                                buf[0] = 'X';
                                buf[1] = 'X';
                                break;
                            case SquareContent.Ship:
                                buf[0] = '█';
                                buf[1] = '█';
                                break;
                            case SquareContent.Unknown:
                                buf[0] = ' ';
                                buf[1] = ' ';
                                break;
                            case SquareContent.Water:
                                buf[0] = '~';
                                buf[1] = '~';
                                break;
                            default:
                                throw new InvalidOperationException("Invalid board state, should never happen!");
                        }

                        buf = buf[2..];

                        if (col < 9)
                        {
                            buf[0] = '│';
                            buf = buf[1..];
                        }
                    }
                    buf[0] = '┃';
                    buf[1] = '\n';
                    buf = buf[2..];

                    if (row < 9)
                    {
                        ((ReadOnlySpan<char>)seps.middle).CopyTo(buf);
                        buf = buf[seps.middle.Length..];
                    }
                }

                ((ReadOnlySpan<char>)seps.bottom).CopyTo(buf);
            });
        }
    }
}