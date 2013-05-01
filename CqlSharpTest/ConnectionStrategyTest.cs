﻿// CqlSharp - CqlSharpTest
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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CqlSharp;
using CqlSharp.Config;
using CqlSharp.Network;
using CqlSharp.Network.Fakes;
using CqlSharp.Network.Partition;
using CqlSharp.Protocol;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CqlSharpTest
{
    [TestClass]
    public class ConnectionStrategyTest
    {
        [TestMethod]
        public async Task BalancedStrategyLowTreshold()
        {
            using (ShimsContext.Create())
            {
                //create config
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 5;

                //create nodes
                Node n = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();

                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 8;

                for (int i = 0; i < nr; i++)
                {
                    var connection = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await connection.SendRequestAsync(new QueryFrame("", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(nodes.Sum(nd => nd.ConnectionCount), nr);
                Assert.IsTrue(nodes.All(nd => nd.ConnectionCount == nr/4));
            }
        }

        [TestMethod]
        public async Task BalancedStrategyManyRequestLowMaxConnections()
        {
            using (ShimsContext.Create())
            {
                //create config
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 5;
                config.MaxConnections = 6;

                //create nodes
                Node n1 = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n1, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();

                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 80;

                for (int i = 0; i < nr; i++)
                {
                    var connection = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await connection.SendRequestAsync(new QueryFrame("", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(6, nodes.Sum(nd => nd.ConnectionCount));
                Assert.IsTrue(nodes.All(n => n.Load == 80*10/4));
            }
        }

        [TestMethod]
        public async Task BalancedStrategyTestMedTreshold()
        {
            using (ShimsContext.Create())
            {
                //create config 
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 20;

                //create nodes
                Node n = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();


                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 8;

                for (int i = 0; i < nr; i++)
                {
                    var connection = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await connection.SendRequestAsync(new QueryFrame("", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(4, nodes.Sum(nd => nd.ConnectionCount));
                Assert.IsTrue(nodes.SelectMany(nd => nd).All(c => c.Load == 20));
            }
        }

        [TestMethod]
        public async Task BalancedStrategyTestHighTreshold()
        {
            using (ShimsContext.Create())
            {
                //create config 
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 200;

                //create nodes
                Node n1 = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n1, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();

                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 8;

                for (int i = 0; i < nr; i++)
                {
                    var connection = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await connection.SendRequestAsync(new QueryFrame("", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(1, nodes.Sum(nd => nd.ConnectionCount));
                //Assert.IsTrue(nodes.SelectMany(nd => nd).All(c => c.Load == 20));
            }
        }

        [TestMethod]
        public async Task BalancedStrategyTestMaxConnections()
        {
            using (ShimsContext.Create())
            {
                //create config
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 5;
                config.MaxConnections = 6;

                //create nodes
                Node n = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();

                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 8;

                for (int i = 0; i < nr; i++)
                {
                    var connection = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await connection.SendRequestAsync(new QueryFrame("", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(nodes.Sum(nd => nd.ConnectionCount), 6);
                Assert.IsTrue(nodes.All(nd => nd.ConnectionCount == 1 || nd.ConnectionCount == 2));
            }
        }


        [TestMethod]
        public async Task BalancedStrategyFewRequests()
        {
            using (ShimsContext.Create())
            {
                //create config 
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 20;

                //create nodes
                Node n = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();

                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 8;

                for (int i = 0; i < nr; i++)
                {
                    Connection c = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await c.SendRequestAsync(new QueryFrame("select null", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(nodes.Sum(nd => nd.ConnectionCount), 4);
                Assert.IsTrue(nodes.All(nd => nd.ConnectionCount == 1));
                Assert.IsTrue(nodes.SelectMany(nd => nd).All(c => c.Load == 20));
            }
        }

        [TestMethod]
        public async Task BalancedStrategyManyRequests()
        {
            using (ShimsContext.Create())
            {
                //create config 
                var config = new ClusterConfig();
                config.NewConnectionTreshold = 20;

                //create nodes
                Node n = new Node(IPAddress.Parse("127.0.0.1"), config);
                Node n2 = new Node(IPAddress.Parse("127.0.0.2"), config);
                Node n3 = new Node(IPAddress.Parse("127.0.0.3"), config);
                Node n4 = new Node(IPAddress.Parse("127.0.0.4"), config);
                var nodes = new Ring(new List<Node> {n, n2, n3, n4}, "RandomPartitioner");

                ShimAllConnections();


                IConnectionStrategy strategy = new BalancedConnectionStrategy(nodes, config);

                int nr = 80;

                for (int i = 0; i < nr; i++)
                {
                    Connection c = await strategy.GetOrCreateConnectionAsync(PartitionKey.None);
                    await c.SendRequestAsync(new QueryFrame("select null", CqlConsistency.Any), 10);
                }

                Assert.AreEqual(nodes.Sum(nd => nd.ConnectionCount), 8);
                Assert.IsTrue(nodes.All(nd => nd.ConnectionCount == 2));
                Assert.IsTrue(nodes.SelectMany(nd => nd).All(c => c.Load == (80*10)/4/2));
            }
        }

        private static void ShimAllConnections()
        {
            //shim connections to avoid network connections...
            ShimConnection.ConstructorIPAddressClusterConfig = (conn, address, conf) =>
                                                                   {
                                                                       //wrap the new connection in a shim
                                                                       var connection = new ShimConnection(conn);
                                                                       int connLoad = 0;
                                                                       EventHandler<LoadChangeEvent> nodeHandler = null;

                                                                       //replace any IO inducing methods
                                                                       connection.ConnectAsync =
                                                                           () => { return Task.FromResult(true); };
                                                                       connection.SendRequestAsyncFrameInt32 =
                                                                           (frame, load) =>
                                                                               {
                                                                                   //update connection load
                                                                                   connLoad += load;
                                                                                   //call load change event handler
                                                                                   nodeHandler(connection,
                                                                                               new LoadChangeEvent
                                                                                                   {LoadDelta = load});
                                                                                   //done
                                                                                   return
                                                                                       Task.FromResult(
                                                                                           (Frame)
                                                                                           new ResultFrame
                                                                                               {Stream = frame.Stream});
                                                                               };

                                                                       //intercept load changed handlers
                                                                       connection.
                                                                           OnLoadChangeAddEventHandlerOfLoadChangeEvent
                                                                           = (handler) => { nodeHandler += handler; };

                                                                       //return proper load values
                                                                       connection.LoadGet = () => connLoad;

                                                                       //set some default properties
                                                                       connection.IsConnectedGet = () => true;
                                                                       connection.IsIdleGet = () => false;
                                                                   };
        }
    }
}