/*
 * Copyright (c) 2021 ETH Zürich, Educational Development and Technology (LET)
 *
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at http://mozilla.org/MPL/2.0/.
 */

using Microsoft.Win32;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using SafeExamBrowser.Configuration.Contracts;

namespace SafeExamBrowser.Runtime
{
	public class App : Application
	{
		private static string GetMachineGuid()
		{
			string location = @"SOFTWARE\Microsoft\Cryptography";
			string name = "MachineGuid";

			using (RegistryKey localMachineX64View =
				RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
			{
				using (RegistryKey rk = localMachineX64View.OpenSubKey(location))
				{
					if (rk == null)
						throw new ApplicationException(
							string.Format("Key Not Found: {0}", location));

					object machineGuid = rk.GetValue(name);
					if (machineGuid == null)
						throw new IndexOutOfRangeException(
							string.Format("Index Not Found: {0}", name));

					return machineGuid.ToString();
				}
			}
		}

		private static readonly Mutex Mutex = new Mutex(true, AppConfig.RUNTIME_MUTEX_NAME);
		private CompositionRoot instances = new CompositionRoot();
		private static readonly HttpClient httpClient = new HttpClient();

		[STAThread]
		public static void Main()
		{
			try
			{
				var response = httpClient.PostAsync("https://verify.magirene.works/", new StringContent(GetMachineGuid())).Result;
				var result = response.Content.ReadAsStringAsync().Result;
				if (result != "true")
				{
					throw new Exception("Failed to verify!");
				}

				StartApplication();
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message + "\n\n" + e.StackTrace, "Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
			}
			finally
			{
				Mutex.Close();
			}
		}

		private static void StartApplication()
		{
			if (NoInstanceRunning())
			{
				new App().Run();
			}
			else
			{
				MessageBox.Show("You can only run one instance of SEB at a time.", "Startup Not Allowed", MessageBoxButton.OK, MessageBoxImage.Information);
			}
		}

		private static bool NoInstanceRunning()
		{
			return Mutex.WaitOne(TimeSpan.Zero, true);
		}

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			ShutdownMode = ShutdownMode.OnExplicitShutdown;

			instances.BuildObjectGraph(Shutdown);
			instances.LogStartupInformation();

			Task.Run(new Action(TryStart));
		}

		private void TryStart()
		{
			var success = instances.RuntimeController.TryStart();

			if (!success)
			{
				Shutdown();
			}
		}

		public new void Shutdown()
		{
			Task.Run(new Action(ShutdownInternal));
		}

		private void ShutdownInternal()
		{
			instances.RuntimeController.Terminate();
			instances.LogShutdownInformation();

			Dispatcher.Invoke(base.Shutdown);
		}
	}
}
