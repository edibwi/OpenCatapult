﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using Polyrific.Catapult.Cli.Extensions;
using Polyrific.Catapult.Shared.Dto.ExternalService;
using Polyrific.Catapult.Shared.Dto.ExternalServiceType;
using Polyrific.Catapult.Shared.Service;

namespace Polyrific.Catapult.Cli.Commands.Service
{
    [Command("update", Description = "Update an external service")]
    public class UpdateCommand : BaseCommand
    {
        private readonly IConsoleReader _consoleReader;
        private readonly IExternalServiceService _externalServiceService;
        private readonly IExternalServiceTypeService _externalServiceTypeService;

        public UpdateCommand(IConsole console, ILogger<UpdateCommand> logger, IConsoleReader consoleReader, IExternalServiceService externalServiceService, IExternalServiceTypeService externalServiceTypeService) : base(console, logger)
        {
            _consoleReader = consoleReader;
            _externalServiceService = externalServiceService;
            _externalServiceTypeService = externalServiceTypeService;
        }

        [Required]
        [Option("-n|--name <NAME>", "Name of the external service", CommandOptionType.SingleValue)]
        public string Name { get; set; }

        [Option("-d|--description <DESCRIPTION>", "Description of the external service", CommandOptionType.SingleValue)]
        public string Description { get; set; }

        [Option("-prop|--property <KEY>:<PROPERTY>", "Property of the external service", CommandOptionType.MultipleValue)]
        public (string, string)[] Property { get; set; }

        public override string Execute()
        {
            Console.WriteLine($"Trying to update external service \"{Name}\"...");

            string message;

            var service = _externalServiceService.GetExternalServiceByName(Name).Result;

            if (service != null)
            {
                Console.WriteLine("Please enter the updated service properties (leave blank if it's unchanged):");
                var serviceType = _externalServiceTypeService.GetExternalServiceType(service.ExternalServiceTypeId).Result;
                foreach (var property in serviceType.ExternalServiceProperties)
                {
                    if (CheckPropertyCondition(property.AdditionalLogic?.HideCondition, service.Config))
                    {
                        // clear the value if now it's hidden
                        service.Config.Remove(property.Name);
                        continue;
                    }

                    string input = null;

                    if (IsPropertySet(property.Name))
                    {
                        input = Property.First(p => p.Item1 == property.Name).Item2;
                    }
                    else
                    {
                        string prompt = $"{(!string.IsNullOrEmpty(property.Description) ? property.Description : property.Name)}:";

                        bool validInput;
                        do
                        {
                            if (property.IsSecret)
                                input = _consoleReader.GetPassword(prompt);
                            else
                                input = Console.GetString(prompt);

                            if (property.AllowedValues != null && property.AllowedValues.Length > 0 && !string.IsNullOrEmpty(input) && !property.AllowedValues.Contains(input))
                            {
                                Console.WriteLine($"Input is not valid. Please enter the allowed values: {string.Join(',', property.AllowedValues)}");
                                validInput = false;
                            }
                            else
                            {
                                validInput = true;
                            }

                        } while (!validInput);
                    }

                    if (!string.IsNullOrEmpty(input))
                        service.Config[property.Name] = input;
                }

                _externalServiceService.UpdateExternalService(service.Id, new UpdateExternalServiceDto
                {
                    Description = Description ?? service.Description,
                    Config = service.Config
                }).Wait();
                message = $"External Service {Name} has been updated successfully";
                Logger.LogInformation(message);
            }
            else
            {
                message = $"External Service {Name} was not found";
            }

            return message;
        }

        private bool CheckPropertyCondition(PropertyConditionDto condition, Dictionary<string, string> properties)
        {
            if (condition == null || properties == null)
                return false;

            var propertyValue = properties.GetValueOrDefault(condition.PropertyName);
            return propertyValue == condition.PropertyValue;
        }

        private bool IsPropertySet(string propertyName)
        {
            return Property?.Any(p => p.Item1 == propertyName) ?? false;
        }
    }
}
