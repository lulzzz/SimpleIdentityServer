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
using SimpleIdentityServer.Configuration.Core.Api.Setting.Actions;
using SimpleIdentityServer.Configuration.Core.Parameters;
using SimpleIdentityServer.Configuration.Core.Repositories;
using System;
using System.Collections.Generic;
using Xunit;

namespace SimpleIdentityServer.Configuration.Core.Tests.Api.Setting
{
    public class BulkUpdateSettingsOperationFixture
    {
        private Mock<ISettingRepository> _settingRepositoryStub;

        private IBulkUpdateSettingsOperation _bulkUpdateSettingsOperation;

        [Fact]
        public void When_Passing_Null_Parameter_Then_Exception_Is_Thrown()
        {
            // ARRANGE
            InitializeFakeObjects();

            // ACT & ASSERT
            Assert.Throws<ArgumentNullException>(() => _bulkUpdateSettingsOperation.Execute(null));
        }

        [Fact]
        public void When_GetSettings_Then_Operation_Is_Called()
        {
            // ARRANGE
            var parameter = new List<UpdateSettingParameter>
            {
                new UpdateSettingParameter
                {
                    Key = "key",
                    Value = "value"
                }
            };
            InitializeFakeObjects();

            // ACT
            _bulkUpdateSettingsOperation.Execute(parameter);

            // ASSERT
            _settingRepositoryStub.Verify(s => s.Update(It.IsAny<IEnumerable<Models.Setting>>()));
        }

        private void InitializeFakeObjects()
        {
            _settingRepositoryStub = new Mock<ISettingRepository>();
            _bulkUpdateSettingsOperation = new BulkUpdateSettingsOperation(_settingRepositoryStub.Object);
        }
    }
}