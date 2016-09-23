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
using SimpleIdentityServer.Core.Models;
using SimpleIdentityServer.Core.Repositories;
using SimpleIdentityServer.Manager.Core.Api.Scopes.Actions;
using SimpleIdentityServer.Manager.Core.Errors;
using SimpleIdentityServer.Manager.Core.Exceptions;
using System;
using Xunit;

namespace SimpleIdentityServer.Manager.Core.Tests.Api.Scopes.Actions
{
    public class UpdateScopeOperationFixture
    {
        private Mock<IScopeRepository> _scopeRepositoryStub;

        private IUpdateScopeOperation _updateScopeOperation;

        #region Exceptions

        [Fact]
        public void When_Passing_Null_Parameter_Then_Exception_Is_Thrown()
        {
            // ARRANGE
            InitializeFakeObjects();

            // ACT & ASSERT
            Assert.Throws<ArgumentNullException>(() => _updateScopeOperation.Execute(null));
        }

        [Fact]
        public void When_Scope_Doesnt_Exist_Then_Exception_Is_Thrown()
        {
            // ARRANGE
            const string name = "scope_name";
            Scope scope = null;
            InitializeFakeObjects();
            _scopeRepositoryStub.Setup(s => s.GetScopeByName(It.IsAny<string>()))
                .Returns(scope);

            // ACT & ASSERTS
            var ex = Assert.Throws<IdentityServerManagerException>(() => _updateScopeOperation.Execute(new Scope
            {
                Name = name
            }));
            Assert.NotNull(ex);
            Assert.True(ex.Code == ErrorCodes.InvalidParameterCode);
            Assert.True(ex.Message == string.Format(ErrorDescriptions.TheScopeDoesntExist, name));
        }

        #endregion

        #region Happy paths

        [Fact]
        public void When_Updating_Then_Operation_Is_Called()
        {
            // ARRANGE
            var parameter = new Scope
            {
                Name = "scope_name"
            };
            InitializeFakeObjects();
            _scopeRepositoryStub.Setup(s => s.GetScopeByName(It.IsAny<string>()))
                .Returns(parameter);

            // ACT
            _updateScopeOperation.Execute(parameter);

            // ASSERT
            _scopeRepositoryStub.Verify(s => s.UpdateScope(parameter));
        }

        #endregion

        #region Private methods

        private void InitializeFakeObjects()
        {
            _scopeRepositoryStub = new Mock<IScopeRepository>();
            _updateScopeOperation = new UpdateScopeOperation(_scopeRepositoryStub.Object);
        }

        #endregion
    }
}