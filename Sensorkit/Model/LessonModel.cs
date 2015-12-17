//----------------------------------------------------------------------------------------------
// <copyright file="LessonModel.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
// <summary>
// This class represents a Model of our lessons.
// </summary>
//-------------------------------------------------------------------------------------------------
namespace Sensorkit.Model
{
    /// <summary>
    /// This lesson have all the elements necessary to use our lessons.
    /// </summary>
    public class LessonModel
    {
        /// <summary>
        /// Gets or sets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        public string[] Arguments
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public int Id
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name of the lesson.
        /// </value>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the lesson is execute-able or not.
        /// </summary>
        /// <value>
        ///   <c>true</c> if lessons is execute-able; otherwise, <c>false</c>.
        /// </value>
        public bool ExecuteAble
        {
            get;
            set;
        }
    }
}