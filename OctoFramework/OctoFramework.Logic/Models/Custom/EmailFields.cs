﻿namespace OctoFramework.Logic.Models.Custom
{
    public class EmailFields
    {
        public bool Send { get; set; }
        public string SenderName { get; set; }
        public string SenderEmail { get; set; }
        public string Subject { get; set; }
        public string ReceiverEmail { get; set; }
        public string BccEmail { get; set; }
        public string CcEmail { get; set; }
        public string Body { get; set; }
    }
}