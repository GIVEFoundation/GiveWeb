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
    
    public partial class CharityGroup
    {
        public int Id { get; set; }
        public string CharityName { get; set; }
        public string Description { get; set; }
        public string ImageLink { get; set; }
        public Nullable<int> Userid { get; set; }
    
        public virtual KidsData KidsData { get; set; }
    }
}
