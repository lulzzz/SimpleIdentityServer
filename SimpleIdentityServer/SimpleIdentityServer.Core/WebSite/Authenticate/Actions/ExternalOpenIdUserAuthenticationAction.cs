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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using SimpleIdentityServer.Core.Parameters;
using SimpleIdentityServer.Core.Results;
using SimpleIdentityServer.Core.WebSite.Authenticate.Common;
using SimpleIdentityServer.Core.Exceptions;
using SimpleIdentityServer.Core.Errors;
using SimpleIdentityServer.Core.Repositories;
using SimpleIdentityServer.Core.Models;

namespace SimpleIdentityServer.Core.WebSite.Authenticate.Actions
{
    public interface IExternalOpenIdUserAuthenticationAction
    {
        ActionResult Execute(
            List<Claim> claims,
            AuthorizationParameter authorizationParameter,
            string code,
            string providerType);
    }

    public sealed class ExternalOpenIdUserAuthenticationAction : IExternalOpenIdUserAuthenticationAction
    {
        private class MappingRule
        {
            public string Type { get; set; }

            public string OpenIdType { get; set; }
        }

        private readonly Dictionary<string, List<MappingRule>> _mappingRulesToOpenIdClaims;

        private readonly IAuthenticateHelper _authenticateHelper;

        private readonly IResourceOwnerRepository _resourceOwnerRepository;

        #region Constructors

        public ExternalOpenIdUserAuthenticationAction(
            IAuthenticateHelper authenticateHelper,
            IResourceOwnerRepository resourceOwnerRepository)
        {
            _authenticateHelper = authenticateHelper;
            _resourceOwnerRepository = resourceOwnerRepository;
            _mappingRulesToOpenIdClaims = new Dictionary<string, List<MappingRule>>
            {
                {
                    Constants.ProviderTypeNames.Microsoft,
                    new List<MappingRule>
                    {
                        new MappingRule
                        {
                            Type = Constants.MicrosoftClaimNames.Id,
                            OpenIdType = Core.Jwt.Constants.StandardResourceOwnerClaimNames.Subject
                        },
                        new MappingRule
                        {
                            Type = Constants.MicrosoftClaimNames.Name,
                            OpenIdType = Core.Jwt.Constants.StandardResourceOwnerClaimNames.Name
                        },
                        new MappingRule
                        {
                            Type = Constants.MicrosoftClaimNames.FirstName,
                            OpenIdType = Core.Jwt.Constants.StandardResourceOwnerClaimNames.GivenName
                        },
                        new MappingRule
                        {
                            Type = Constants.MicrosoftClaimNames.LastName,
                            OpenIdType = Core.Jwt.Constants.StandardResourceOwnerClaimNames.FamilyName
                        }
                    }
                }
            };
        }

        #endregion

        #region Public methods

        public ActionResult Execute(
            List<Claim> claims,
            AuthorizationParameter authorizationParameter,
            string code,
            string providerType)
        {
            if (claims == null || !claims.Any())
            {
                throw new ArgumentNullException("claims");
            }

            if (authorizationParameter == null)
            {
                throw new ArgumentNullException("authorizationParameter");
            }

            if (string.IsNullOrWhiteSpace(code))
            {
                throw new ArgumentNullException("code");
            }

            if (string.IsNullOrWhiteSpace(providerType))
            {
                throw new ArgumentNullException("providerType");
            }

            if (!Constants.SupportedProviderTypes.Contains(providerType))
            {
                throw new IdentityServerException(ErrorCodes.UnhandledExceptionCode,
                    string.Format(ErrorDescriptions.TheExternalProviderIsNotSupported, providerType));
            }

            var result = new List<Claim>();
            switch(providerType)
            {
                case Constants.ProviderTypeNames.Microsoft:
                    result = ConvertMicrosoftClaims(claims);
                    break;
            }

            var subjectClaim = result.FirstOrDefault(r => r.Type == Core.Jwt.Constants.StandardResourceOwnerClaimNames.Subject);
            var nameClaim = result.FirstOrDefault(r => r.Type == Core.Jwt.Constants.StandardResourceOwnerClaimNames.Name);
            var givenNameClaim = result.FirstOrDefault(r => r.Type == Core.Jwt.Constants.StandardResourceOwnerClaimNames.GivenName);
            var familyNameClaim = result.FirstOrDefault(r => r.Type == Core.Jwt.Constants.StandardResourceOwnerClaimNames.FamilyName);
            if (subjectClaim == null)
            {
                throw new IdentityServerException(ErrorCodes.UnhandledExceptionCode,
                    ErrorDescriptions.NoSubjectCanBeExtracted);
            }

            var resourceOwner = _resourceOwnerRepository.GetBySubject(subjectClaim.Value);
            if (resourceOwner == null)
            {
                resourceOwner = new ResourceOwner
                {
                    Id = subjectClaim.Value,
                    Name = nameClaim == null ? string.Empty : nameClaim.Value,
                    GivenName = givenNameClaim == null ? string.Empty : givenNameClaim.Value,
                    FamilyName = familyNameClaim == null ? string.Empty : familyNameClaim.Value,
                };

                _resourceOwnerRepository.Insert(resourceOwner);
            }

            return _authenticateHelper.ProcessRedirection(authorizationParameter,
                code,
                "subject",
                result);
        }

        #endregion

        #region Private methods

        private List<Claim> ConvertMicrosoftClaims(List<Claim> claims)
        {
            var result = new List<Claim>();
            var mappingRules = _mappingRulesToOpenIdClaims[Constants.ProviderTypeNames.Microsoft];
            claims.ForEach(c =>
            {
                var mappingRule = mappingRules.FirstOrDefault(m => m.Type == c.Type);
                if (mappingRule != null)
                {
                    result.Add(new Claim(mappingRule.OpenIdType, c.Value));
                }
            });

            return result;
        }

        #endregion
    }
}
