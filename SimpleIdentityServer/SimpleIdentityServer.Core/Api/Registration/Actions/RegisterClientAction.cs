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

using SimpleIdentityServer.Core.Parameters;
using SimpleIdentityServer.Core.Validators;

namespace SimpleIdentityServer.Core.Api.Registration.Actions
{
    public interface IRegisterClientAction
    {
        void Execute(RegistrationParameter parameter);
    }

    public class RegisterClientAction : IRegisterClientAction
    {
        private readonly IRegistrationParameterValidator _registrationParameterValidator;

        public RegisterClientAction(IRegistrationParameterValidator registrationParameterValidator)
        {
            _registrationParameterValidator = registrationParameterValidator;
        }

        public void Execute(RegistrationParameter parameter)
        {
            _registrationParameterValidator.Validate(parameter);
        }
    }
}
