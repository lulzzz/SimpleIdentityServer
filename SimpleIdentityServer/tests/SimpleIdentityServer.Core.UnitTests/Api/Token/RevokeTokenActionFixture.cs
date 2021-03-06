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

using Moq;
using SimpleIdentityServer.Core.Api.Token.Actions;
using SimpleIdentityServer.Core.Authenticate;
using SimpleIdentityServer.Core.Common.Models;
using SimpleIdentityServer.Core.Common.Repositories;
using SimpleIdentityServer.Core.Errors;
using SimpleIdentityServer.Core.Exceptions;
using SimpleIdentityServer.Core.Parameters;
using SimpleIdentityServer.Store;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xunit;

namespace SimpleIdentityServer.Core.UnitTests.Api.Token
{
    public class RevokeTokenActionFixture
    {
        private Mock<IAuthenticateInstructionGenerator> _authenticateInstructionGeneratorStub;
        private Mock<IAuthenticateClient> _authenticateClientStub;
        private Mock<ITokenStore> _grantedTokenRepositoryStub;
        private Mock<IClientRepository> _clientRepositoryStub;
        private IRevokeTokenAction _revokeTokenAction;

        #region Exceptions

        [Fact]
        public async Task When_Passing_Null_Parameter_Then_Exceptions_Are_Thrown()
        {
            // ARRANGE
            InitializeFakeObjects();

            // ACTS & ASSERTS
            await Assert.ThrowsAsync<ArgumentNullException>(() => _revokeTokenAction.Execute(null, null, null, null));
            await Assert.ThrowsAsync<ArgumentNullException>(() => _revokeTokenAction.Execute(new RevokeTokenParameter(), null, null, null));
        }

        [Fact]
        public async Task When_Client_Doesnt_Exist_Then_Exception_Is_Thrown()
        {
            // ARRANGE
            InitializeFakeObjects();
            var parameter = new RevokeTokenParameter
            {
                Token = "access_token"
            };
            _authenticateInstructionGeneratorStub.Setup(a => a.GetAuthenticateInstruction(It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new AuthenticateInstruction());
            _authenticateClientStub.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticateInstruction>(), null))
                .Returns(() => Task.FromResult(new AuthenticationResult(null, null)));
            _clientRepositoryStub.Setup(c => c.GetClientByIdAsync(It.IsAny<string>()))
                .Returns(() => Task.FromResult((Core.Common.Models.Client)null));

            // ACT & ASSERTS
            var exception = await Assert.ThrowsAsync<IdentityServerException>(() => _revokeTokenAction.Execute(parameter, null, null, null));
            Assert.NotNull(exception);
            Assert.True(exception.Code == ErrorCodes.InvalidClient);
        }

        [Fact]
        public async Task When_Token_Doesnt_Exist_Then_Exception_Is_Returned()
        {
            // ARRANGE
            InitializeFakeObjects();
            var parameter = new RevokeTokenParameter
            {
                Token = "access_token"
            };
            _authenticateInstructionGeneratorStub.Setup(a => a.GetAuthenticateInstruction(It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new AuthenticateInstruction());
            _authenticateClientStub.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticateInstruction>(), null))
                .Returns(() => Task.FromResult(new AuthenticationResult(new Core.Common.Models.Client(), null)));
            _grantedTokenRepositoryStub.Setup(g => g.GetAccessToken(It.IsAny<string>()))
                .Returns(() => Task.FromResult((GrantedToken)null));
            _grantedTokenRepositoryStub.Setup(g => g.GetRefreshToken(It.IsAny<string>()))
                .Returns(() => Task.FromResult((GrantedToken)null));

            // ACT
            var result = await Assert.ThrowsAsync<IdentityServerException>(() => _revokeTokenAction.Execute(parameter, null, null, null));

            // ASSERT
            Assert.NotNull(result);
            Assert.Equal("invalid_token", result.Code);
        }

        #endregion

        #region Happy path

        [Fact]
        public async Task When_Invalidating_Refresh_Token_Then_GrantedTokenChildren_Are_Removed()
        {
            // ARRANGE
            InitializeFakeObjects();
            var parent = new GrantedToken
            {
                RefreshToken = "refresh_token"
            };
            var child = new GrantedToken
            {
                ParentTokenId = "refresh_token",
                AccessToken = "access_token_child"
            };
            var parameter = new RevokeTokenParameter
            {
                Token = "refresh_token"
            };
            _authenticateInstructionGeneratorStub.Setup(a => a.GetAuthenticateInstruction(It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new AuthenticateInstruction());
            _authenticateClientStub.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticateInstruction>(), null))
                .Returns(() => Task.FromResult(new AuthenticationResult(new Core.Common.Models.Client(), null)));
            _grantedTokenRepositoryStub.Setup(g => g.GetAccessToken(It.IsAny<string>()))
                .Returns(() => Task.FromResult((GrantedToken)null));
            _grantedTokenRepositoryStub.Setup(g => g.GetRefreshToken(It.IsAny<string>()))
                .Returns(Task.FromResult(parent));
            _grantedTokenRepositoryStub.Setup(g => g.RemoveAccessToken(It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            // ACT
            await _revokeTokenAction.Execute(parameter, null, null, null);

            // ASSERTS
            _grantedTokenRepositoryStub.Verify(g => g.RemoveRefreshToken(parent.RefreshToken));
        }

        [Fact]
        public async Task When_Invalidating_Access_Token_Then_GrantedToken_Is_Removed()
        {
            // ARRANGE
            InitializeFakeObjects();
            var grantedToken = new GrantedToken
            {
                AccessToken = "access_token"
            };
            var parameter = new RevokeTokenParameter
            {
                Token = "access_token"
            };
            _authenticateInstructionGeneratorStub.Setup(a => a.GetAuthenticateInstruction(It.IsAny<AuthenticationHeaderValue>()))
                .Returns(new AuthenticateInstruction());
            _authenticateClientStub.Setup(a => a.AuthenticateAsync(It.IsAny<AuthenticateInstruction>(), null))
                .Returns(() => Task.FromResult(new AuthenticationResult(new Core.Common.Models.Client(), null)));
            _grantedTokenRepositoryStub.Setup(g => g.GetAccessToken(It.IsAny<string>()))
                .Returns(Task.FromResult(grantedToken));
            _grantedTokenRepositoryStub.Setup(g => g.GetRefreshToken(It.IsAny<string>()))
                .Returns(() => Task.FromResult((GrantedToken)null));
            _grantedTokenRepositoryStub.Setup(g => g.RemoveAccessToken(It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            // ACT
            await _revokeTokenAction.Execute(parameter, null, null, null);

            // ASSERTS
            _grantedTokenRepositoryStub.Verify(g => g.RemoveAccessToken(grantedToken.AccessToken));
        }

        #endregion

        #region Private methods

        private void InitializeFakeObjects()
        {
            _authenticateInstructionGeneratorStub = new Mock<IAuthenticateInstructionGenerator>();
            _authenticateClientStub = new Mock<IAuthenticateClient>();
            _grantedTokenRepositoryStub = new Mock<ITokenStore>();
            _clientRepositoryStub = new Mock<IClientRepository>();
            _revokeTokenAction = new RevokeTokenAction(
                _authenticateInstructionGeneratorStub.Object,
                _authenticateClientStub.Object,
                _grantedTokenRepositoryStub.Object,
                _clientRepositoryStub.Object);
        }

        #endregion
    }
}
