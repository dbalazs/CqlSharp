﻿// CqlSharp - CqlSharp
// Copyright (c) 2013 Joost Reuzel
//   
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
// http://www.apache.org/licenses/LICENSE-2.0
//  
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace CqlSharp.Protocol
{
    internal class AuthResponseFrame : Frame
    {
        public AuthResponseFrame(byte[] saslResponse, FrameVersion version)
        {
            Debug.Assert((version & FrameVersion.ProtocolVersionMask) != FrameVersion.ProtocolVersion1, "Version 1 of the protocol does not support AuthResponse Frames");
            
            Version = FrameVersion.Request | version;
            Flags = FrameFlags.None;
            Stream = 0;
            OpCode = FrameOpcode.AuthResponse;

            SaslResponse = saslResponse;
        }

        public byte[] SaslResponse { get; set; }

        protected override void WriteData(Stream buffer)
        {
            buffer.WriteByteArray(SaslResponse);
        }

        protected override Task InitializeAsync()
        {
            throw new NotSupportedException();
        }
    }
}