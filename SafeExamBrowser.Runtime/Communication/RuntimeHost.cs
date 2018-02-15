﻿/*
 * Copyright (c) 2018 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using System;
using SafeExamBrowser.Contracts.Communication;
using SafeExamBrowser.Contracts.Communication.Messages;
using SafeExamBrowser.Contracts.Communication.Responses;
using SafeExamBrowser.Contracts.Configuration;
using SafeExamBrowser.Contracts.Logging;
using SafeExamBrowser.Core.Communication;
using SafeExamBrowser.Runtime.Communication.Responses;

namespace SafeExamBrowser.Runtime.Communication
{
	internal class RuntimeHost : BaseHost, IRuntimeHost
	{
		private IConfigurationRepository configuration;

		public Guid StartupToken { private get; set; }

		public event CommunicationEventHandler ClientReady;

		public RuntimeHost(string address, IConfigurationRepository configuration, ILogger logger) : base(address, logger)
		{
			this.configuration = configuration;
		}

		protected override bool OnConnect(Guid? token = null)
		{
			return StartupToken == token;
		}

		protected override void OnDisconnect()
		{
			// TODO
		}

		protected override IResponse OnReceive(IMessage message)
		{
			// TODO
			return null;
		}

		protected override IResponse OnReceive(Message message)
		{
			switch (message)
			{
				case Message.ClientIsReady:
					ClientReady?.Invoke();
					break;
				case Message.ConfigurationNeeded:
					return new ConfigurationResponse { Configuration = configuration.BuildClientConfiguration() };
			}

			return null;
		}
	}
}