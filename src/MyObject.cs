using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace MyService 
{
    public class Provider
    {
        
        public Guid ProviderID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
        public bool IsActive { get; set; }

        public static Provider DeserializeProvider(string myObjectJSON)
        {
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore };
            Provider myObject = JsonConvert.DeserializeObject<Provider>(myObjectJSON, settings);

            return myObject;
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}