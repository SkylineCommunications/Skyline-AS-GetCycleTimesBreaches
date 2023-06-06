/*
****************************************************************************
*  Copyright (c) 2023,  Skyline Communications NV  All Rights Reserved.    *
****************************************************************************

By using this script, you expressly agree with the usage terms and
conditions set out below.
This script and all related materials are protected by copyrights and
other intellectual property rights that exclusively belong
to Skyline Communications.

A user license granted for this script is strictly for personal use only.
This script may not be used in any way by anyone without the prior
written consent of Skyline Communications. Any sublicensing of this
script is forbidden.

Any modifications to this script by the user are only allowed for
personal use and within the intended purpose of the script,
and will remain the sole responsibility of the user.
Skyline Communications will not be responsible for any damages or
malfunctions whatsoever of the script resulting from a modification
or adaptation by the user.

The content of this script is confidential information.
The user hereby agrees to keep this confidential information strictly
secret and confidential and not to disclose or reveal it, in whole
or in part, directly or indirectly to any person, entity, organization
or administration without the prior written consent of
Skyline Communications.

Any inquiries can be addressed to:

	Skyline Communications NV
	Ambachtenstraat 33
	B-8870 Izegem
	Belgium
	Tel.	: +32 51 31 35 69
	Fax.	: +32 51 31 01 29
	E-mail	: info@skyline.be
	Web		: www.skyline.be
	Contact	: Ben Vandenberghe

****************************************************************************
Revision History:

DATE		VERSION		AUTHOR			COMMENTS

29/05/2023	1.0.0.1		ACA, Skyline	Initial version
****************************************************************************
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.Apps.UserDefinableApis;
using Skyline.DataMiner.Net.Apps.UserDefinableApis.Actions;

namespace UserDefinableApiScripts.Examples.ExistingWithEntryPoint
{
	public class Script
	{

		public enum ECSParameters
		{
			Breaches = 97,
			OpenBreaches = 99,
			TasksProcessed = 66,
			TasksAssignedMembers = 68,
		}

		[AutomationEntryPoint(AutomationEntryPointType.Types.OnApiTrigger)]
		public ApiTriggerOutput OnApiTrigger(IEngine engine, ApiTriggerInput requestData)
		{
			// Find Elements
			Element[] ecsElements = engine.FindElementsByProtocol("Skyline ECS Agile Metrics");
			ecsElements = ecsElements.Where(x => x.ProtocolVersion == "Production").ToArray();

			List<SquadMetrics> squadMetrics = new List<SquadMetrics>();

			foreach (Element ecsElement in ecsElements)
			{
				SquadMetrics metrics = new SquadMetrics
				{
					Squad = ecsElement.ElementName.Split(' ').LastOrDefault(),
					Breaches = GetBreaches(engine, ecsElement),
					TasksProcessed = GetTasksProcessed(engine, ecsElement),
					OpenBreaches = GetOpenBreaches(engine, ecsElement),
					TasksAssignedMembers = GetTasksAssignedMembers(engine, ecsElement),
				};

				squadMetrics.Add(metrics);
			}

			string results = JsonConvert.SerializeObject(squadMetrics);

			return new ApiTriggerOutput()
			{
				ResponseBody = Convert.ToString(results),
				ResponseCode = (int)StatusCode.Ok,
			};
		}


		public void Run(Engine engine)
		{ }

		private int GetBreaches(IEngine engine, Element element)
		{
			try
			{
				return (int)Convert.ToDouble(element.GetParameter((int)ECSParameters.Breaches));
			}
			catch (Exception e)
			{
				engine.GenerateInformation("Exception on Method GetBreaches => " + e);
				return -1;
			}
		}

		private int GetOpenBreaches(IEngine engine, Element element)
		{
			try
			{
				return (int)Convert.ToDouble(element.GetParameter((int)ECSParameters.OpenBreaches));
			}
			catch (Exception e)
			{
				engine.GenerateInformation("Exception on Method GetOpenBreaches => " + e);
				return -1;
			}
		}

		private int GetTasksAssignedMembers(IEngine engine, Element element)
		{
			try
			{
				return (int)Convert.ToDouble(element.GetParameter((int)ECSParameters.TasksAssignedMembers));
			}
			catch (Exception e)
			{
				engine.GenerateInformation("Exception on Method GetOpenBreaches => " + e);
				return -1;
			}
		}

		private int GetTasksProcessed(IEngine engine, Element element)
		{
			try
			{
				return (int)Convert.ToDouble(element.GetParameter((int)ECSParameters.TasksProcessed));
			}
			catch (Exception e)
			{
				engine.GenerateInformation("Exception on Method GetTasksProcessed => " + e);
				return -1;
			}
		}
	}

	public class SquadMetrics
	{
		public string Squad { get; set; }
		public int Breaches { get; set; }
		public int TasksProcessed { get; set; }
		public int TasksAssignedMembers { get; set; }
		public int OpenBreaches { get; set; }
	}
}