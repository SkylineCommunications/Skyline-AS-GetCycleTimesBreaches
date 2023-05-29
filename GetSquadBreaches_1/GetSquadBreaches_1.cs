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
using Skyline.DataMiner.Automation;
using Skyline.DataMiner.Net.Apps.UserDefinableApis;
using Skyline.DataMiner.Net.Apps.UserDefinableApis.Actions;

namespace UserDefinableApiScripts.Examples.ExistingWithEntryPoint
{
	public class Script
	{

		[AutomationEntryPoint(AutomationEntryPointType.Types.OnApiTrigger)]
		public ApiTriggerOutput OnApiTrigger(IEngine engine, ApiTriggerInput requestData)
		{
			// Retrieve the values for the input parameters from the parsed API trigger request body.
			requestData.Parameters.TryGetValue("DMAID", out var dmaIdParam);
			requestData.Parameters.TryGetValue("ELEMENTID", out var elementIdParam);

			// Convert the string values to int.
			if (!int.TryParse(dmaIdParam, out var dmaId))
			{
				return new ApiTriggerOutput()
				{
					ResponseBody = "Could not parse 'DMAID' parameter to int.",
					ResponseCode = (int)StatusCode.BadRequest
				};
			}

			if (!int.TryParse(elementIdParam, out var elementId))
			{
				return new ApiTriggerOutput()
				{
					ResponseBody = "Could not parse 'ELEMENTID' parameter to int.",
					ResponseCode = (int)StatusCode.BadRequest
				};
			}

			// Call the InnerRun method with our input.
			int breaches = GetBreaches(engine as Engine, dmaId, elementId);

			return new ApiTriggerOutput()
			{
				ResponseBody = Convert.ToString(breaches),
				ResponseCode = (int)StatusCode.Ok
			};
		}

		public void Run(Engine engine)
		{
			var dmaId = engine.GetScriptParam("DMAID").Value;
			var elementId = engine.GetScriptParam("ELEMENTID").Value;

			if (!int.TryParse(dmaId, out var parsedDmaId))
			{
				engine.ExitFail("Failed to parse DMA ID");
				return;
			}

			if (!int.TryParse(elementId, out var parsedElementId))
			{
				engine.ExitFail("Failed to parse element ID");
				return;
			}

			int breaches = GetBreaches(engine, parsedDmaId, parsedElementId);
		}

		private int GetBreaches(Engine engine, int dmaId, int elementId)
		{
			var element = engine.FindElement(dmaId, elementId);

			return Convert.ToInt32(element.GetParameter(97));
		}
	}
}