﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Polyrific.Catapult.Cli.Extensions;
using Polyrific.Catapult.Shared.Service;

namespace Polyrific.Catapult.Cli.Commands.Service
{
    [Command("get", Description = "Display a single external service")]
    public class GetCommand : BaseCommand
    {
        private readonly IExternalServiceService _externalServiceService;
        private readonly IExternalServiceTypeService _externalServiceTypeService;

        public GetCommand(IConsole console, ILogger<GetCommand> logger, IExternalServiceService externalServiceService, IExternalServiceTypeService externalServiceTypeService) : base(console, logger)
        {
            _externalServiceService = externalServiceService;
            _externalServiceTypeService = externalServiceTypeService;
        }

        [Required]
        [Option("-n|--name <NAME>", "Name of the external service", CommandOptionType.SingleValue)]
        public string Name { get; set; }
        
        public override string Execute()
        {
            Console.WriteLine($"Trying to get external service {Name}...");

            string message;

            var service = _externalServiceService.GetExternalServiceByName(Name).Result;
            
            if (service != null)
            {
                var serviceType = _externalServiceTypeService.GetExternalServiceType(service.ExternalServiceTypeId).Result;
                message = service.ToCliString($"External Service {Name}:", serviceType?.ExternalServiceProperties?.Where(x => x.IsSecret).Select(x => x.Name).ToArray(), excludedFields: new string[]
                {
                    "ExternalServiceTypeId"
                }, nameDictionary: new Dictionary<string, string>
                {
                    {"Config", "Properties"}
                });
            }
            else
            {
                message = $"External Service {Name} was not found";
            }

            return message;
        }
    }
}
