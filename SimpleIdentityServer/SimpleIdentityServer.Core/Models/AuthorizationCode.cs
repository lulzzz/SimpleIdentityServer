﻿using SimpleIdentityServer.Core.Jwt;
using System;
using System.Collections.Generic;

namespace SimpleIdentityServer.Core.Models
{
    public class AuthorizationCode
    {
        /// <summary>
        /// Gets or sets the authorization code.
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the redirection uri.
        /// </summary>
        public string RedirectUri { get; set; }

        /// <summary>
        /// Gets or sets the creation date time.
        /// </summary>
        public DateTime CreateDateTime { get; set; }

        /// <summary>
        /// Gets or sets the client id.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets or sets the id token.
        /// </summary>
        public string IdToken { get; set; }

        /// <summary>
        /// Gets or sets the user information payload.
        /// </summary>
        public JwsPayload UserInfoPayLoad { get; set; }

        /// <summary>
        /// Gets or sets the concatenated list of scopes.
        /// </summary>
        public string Scopes { get; set; }
    }
}
