﻿// Copyright (c) Polyrific, Inc 2018. All rights reserved.

namespace Polyrific.Catapult.Shared.Dto.ProjectDataModel
{
    public class CreateProjectDataModelPropertyDto
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Label of the property
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Data type of the property
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// Control type of the property
        /// </summary>
        public string ControlType { get; set; }

        /// <summary>
        /// Id of the related project data model
        /// </summary>
        public int? RelatedProjectDataModelId { get; set; }

        /// <summary>
        /// Type of the relation with the related project data model
        /// </summary>
        public string RelationalType { get; set; }

        /// <summary>
        /// Is property required?
        /// </summary>
        public bool IsRequired { get; set; }
    }
}