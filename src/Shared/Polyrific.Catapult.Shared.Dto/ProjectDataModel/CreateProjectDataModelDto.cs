﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

namespace Polyrific.Catapult.Shared.Dto.ProjectDataModel
{
    public class CreateProjectDataModelDto
    {
        /// <summary>
        /// Name of the data model
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Description of the data model
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Label of the data model
        /// </summary>
        public string Label { get; set; }
    }
}