//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Benchmark.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class test
    {
        public int Id { get; set; }
        public Nullable<int> ProcesorId { get; set; }
        public Nullable<int> TestSize { get; set; }
        public Nullable<double> AvarageTime { get; set; }
        public Nullable<System.DateTime> DateTimeOfTest { get; set; }
    
        public virtual processor processor { get; set; }
    }
}
