// <copyright file="PloppableRICODefinition.cs" company="algernon (K. Algernon A. Sheppard)">
// Copyright (c) algernon (K. Algernon A. Sheppard). All rights reserved.
// Licensed under the MIT license. See LICENSE.txt file in the project root for full license information.
// </copyright>

namespace PloppableRICO
{
    using System.Collections.Generic;

    /// <summary>
    /// Ploppable RICO XML file definition.
    /// </summary>
    public class PloppableRICODefinition
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PloppableRICODefinition"/> class.
        /// </summary>
        public PloppableRICODefinition()
        {
            Buildings = new List<RICOBuilding>();
        }

        /// <summary>
        /// Gets or sets the list of RICO building definitions in this file.
        /// </summary>
        public List<RICOBuilding> Buildings { get; set; }
    }
}