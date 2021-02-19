﻿/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 * 
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using SafeExamBrowser.Settings.Proctoring;

namespace SafeExamBrowser.Proctoring.Contracts
{
	/// <summary>
	/// Defines the remote proctoring functionality.
	/// </summary>
	public interface IProctoringController
	{
		/// <summary>
		/// 
		/// </summary>
		void Initialize(ProctoringSettings settings);

		/// <summary>
		/// 
		/// </summary>
		void Terminate();
	}
}
