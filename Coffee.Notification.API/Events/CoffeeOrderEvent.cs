﻿namespace Coffee.Notification.API.Events
{
    public class CoffeeOrderEvent
    {
        public string TrackingCode { get; set; }
        public string ContactEmail { get; set; }
        public string Description { get; set; }
    }
}
