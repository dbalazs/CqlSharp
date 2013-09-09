using System;
using System.IO;
using System.Threading.Tasks;

namespace CqlSharp.Protocol
{
    /// <summary>
    /// Authenticate Challenge Frame
    /// </summary>
    internal class AuthChallengeFrame : Frame
    {
        public byte[] SaslChallenge { get; set; }

        protected override void WriteData(Stream buffer)
        {
            throw new NotSupportedException();
        }

        protected override async Task InitializeAsync()
        {
            FrameReader reader = Reader;
            SaslChallenge = await reader.ReadBytesAsync().ConfigureAwait(false);
        }
    }
}