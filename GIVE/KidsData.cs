//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GIVE
{
    using System;
    using System.Collections.Generic;
    
    public partial class KidsData
    {
        public KidsData()
        {
            this.CharityGroups = new HashSet<CharityGroup>();
        }
    
        public int Id { get; set; }
        public string Name { get; set; }
        public string PrimaryID { get; set; }
        public string Password { get; set; }
        public Nullable<int> Account { get; set; }
    
        public virtual ICollection<CharityGroup> CharityGroups { get; set; }
        public virtual LoginType LoginType { get; set; }
    }
}
