using System.Collections.Generic;

namespace APiConsumer.Models
{
    public class ROLES
    {
        public int id { get; set; }
        public string role1 { get; set; }

        public List<USERS> users { get; set; }
    }
}