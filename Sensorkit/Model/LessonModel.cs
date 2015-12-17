//----------------------------------------------------------------------------------------------
// <copyright file="LessonModel.cs" company="Lukas Handler">
// Copyright (c) Lukas Handler.  All rights reserved.
// </copyright>
//-------------------------------------------------------------------------------------------------
namespace Sensorkit.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class LessonModel
    {
        public string[] Arguments
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public bool RunAble
        {
            get;
            set;
        }
    }
}