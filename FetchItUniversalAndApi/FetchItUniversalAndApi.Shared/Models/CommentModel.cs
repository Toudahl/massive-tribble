﻿using System;
using System.Collections.Generic;
using System.Text;

namespace FetchItUniversalAndApi.Models
{
    public partial class CommentModel
    {
        public int CommentId { get; set; }
        public string FK_CommentNotificationReferenceId { get; set; }
        public string CommentText { get; set; }
        public System.DateTime CommentTimeCreated { get; set; }
        public int FK_CommentCreator { get; set; }
        public int FK_CommentTask { get; set; }

        public virtual ProfileModel Profile { get; set; }
        public virtual TaskModel Task { get; set; }
    }
}