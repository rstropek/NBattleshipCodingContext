namespace NBattleshipCodingContest.Logic
{
    using System;
    using System.Runtime.Serialization;

    public class BoardTooOccupiedException : ApplicationException
    {
        public BoardTooOccupiedException()
        {
        }

        public BoardTooOccupiedException(string? message) : base(message)
        {
        }

        public BoardTooOccupiedException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected BoardTooOccupiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
