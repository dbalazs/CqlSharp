// CqlSharp - CqlSharp
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

namespace CqlSharp
{
    /// <summary>
    ///   Represents the result of a query that does not have actual result values
    /// </summary>
    public struct CqlPrepared : ICqlQueryResult
    {
        public bool FromCache { get; internal set; }

        #region ICqlQueryResult Members

        public CqlResultType ResultType
        {
            get { return CqlResultType.Prepared; }
        }

        public Guid? TracingId { get; internal set; }

        #endregion
    }
}