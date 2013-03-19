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

using System.Threading.Tasks;
using CqlSharp.Network.Partition;

namespace CqlSharp.Network
{
    /// <summary>
    ///   Provides access to connections to cassandra node(s)
    /// </summary>
    internal interface IConnectionProvider
    {
        /// <summary>
        ///   Gets or creates a network connection to a cassandra node.
        /// </summary>
        /// <param name="partitionKey"> </param>
        /// <returns> Connection that is ready to use </returns>
        Task<Connection> GetOrCreateConnectionAsync(PartitionKey partitionKey);

        /// <summary>
        ///   Returns the connection to the provider.
        /// </summary>
        /// <param name="connection"> The connection. </param>
        void ReturnConnection(Connection connection);
    }
}