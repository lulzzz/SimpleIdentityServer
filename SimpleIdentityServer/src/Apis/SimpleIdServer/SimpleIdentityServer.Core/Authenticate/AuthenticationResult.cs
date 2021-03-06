﻿#region copyright
// Copyright 2015 Habart Thierry
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

namespace SimpleIdentityServer.Core.Authenticate
{
    public class AuthenticationResult
    {
        public AuthenticationResult(Core.Common.Models.Client client, string errorMessage)
        {
            Client = client;
            ErrorMessage = errorMessage;
        }

        public Core.Common.Models.Client Client { get; set; }
        public string ErrorMessage { get; set; }
    }
}
