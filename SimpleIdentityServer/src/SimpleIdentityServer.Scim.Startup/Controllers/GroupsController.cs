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

using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using SimpleIdentityServer.Scim.Core.Apis;
using System;

namespace SimpleIdentityServer.Scim.Startup.Controllers
{
    [Route(Constants.RoutePaths.GroupsController)]
    public class GroupsController : Controller
    {
        private readonly IGroupsAction _groupsAction;

        public GroupsController(IGroupsAction groupsAction)
        {
            _groupsAction = groupsAction;
        }

        [HttpPost]
        public ActionResult AddGroup([FromBody] JObject jObj)
        {
            if (jObj == null)
            {
                throw new ArgumentNullException(nameof(jObj));
            }

            return new CreatedResult("", _groupsAction.AddGroup(jObj));
        }

        [HttpGet("{id}")]
        public ActionResult GetGroup(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var representation = _groupsAction.GetGroup(id);
            if (representation == null)
            {
                return new NotFoundResult();
            }

            return new OkObjectResult(representation);
        }

        [HttpDelete("{id}")]
        public ActionResult DeleteGroup(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            var action = _groupsAction.RemoveGroup(id);
            if (action.Content != null)
            {
                var result = new ObjectResult(action.Content);
                result.StatusCode = action.StatusCode;
                return result;
            }

            return new StatusCodeResult(action.StatusCode.Value);
        }
    }
}